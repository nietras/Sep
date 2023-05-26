using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

sealed class SepCharsFinderSse2PackCmpOrMoveMaskTzcnt : ISepCharsFinder
{
    readonly byte _separator;
    readonly Vector128<byte> _nls = Vector128.Create(LineFeedByte);
    readonly Vector128<byte> _crs = Vector128.Create(CarriageReturnByte);
    readonly Vector128<byte> _qts = Vector128.Create(QuoteByte);
    readonly Vector128<byte> _sps;

    public unsafe SepCharsFinderSse2PackCmpOrMoveMaskTzcnt(Sep sep)
    {
        _separator = (byte)sep.Separator;
        _sps = Vector128.Create(_separator);
    }

    public int PaddingLength => Vector128<byte>.Count; // Parses 2 x char vectors e.g. 1 byte vector
    public int RequestedPositionsFreeLength => PaddingLength * 32;

    [SkipLocalsInit]
    public int Find(char[] _chars, int charsStart, int charsEnd,
                    Pos[] positions, int positionsStart, ref int positionsEnd)
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        var chars = _chars;
        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        // Absolute minimum, prefer RequestedPositionsFreeLength for free
        positions.CheckPadding(positionsEnd, PaddingLength);

        A.Assert(charsStart <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - PaddingLength));
        var dataStart = charsStart;
        var dataEnd = charsEnd;
        ref var charsRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars), dataStart);

        ref var positionsRef = ref Unsafe.As<Pos, int>(ref MemoryMarshal.GetArrayDataReference(positions));
        ref var positionsRefCurrent = ref Unsafe.Add(ref positionsRef, positionsEnd);
        ref var positionsRefStop = ref Unsafe.Add(ref positionsRef, positions.Length - Vector128<byte>.Count);

        var nls = _nls; //Vector128.Create(LineFeedByte);
        var crs = _crs; //Vector128.Create(CarriageReturnByte);
        var qts = _qts; //Vector128.Create(QuoteByte);
        var sps = _sps; //Vector128.Create(_separator);

        var separatorShifted = _separator << SepCharPosition.CharShift;

        var dataIndex = dataStart;
        for (; dataIndex < dataEnd; dataIndex += Vector128<byte>.Count,
             charsRef = ref Unsafe.Add(ref charsRef, Vector128<byte>.Count))
        {
            var vector0 = Unsafe.ReadUnaligned<Vector128<short>>(
                ref Unsafe.As<char, byte>(ref charsRef));
            var vector1 = Unsafe.ReadUnaligned<Vector128<short>>(
                ref Unsafe.As<char, byte>(ref Unsafe.Add(ref charsRef, Vector128<short>.Count)));

            var bytes = Sse2.PackUnsignedSaturate(vector0, vector1);

            var nlsEq = Vector128.Equals(bytes, nls);
            var crsEq = Vector128.Equals(bytes, crs);
            var qtsEq = Vector128.Equals(bytes, qts);
            var spsEq = Vector128.Equals(bytes, sps);

            var lineEndings = nlsEq | crsEq;
            var endingsAndQuotes = lineEndings | qtsEq;
            var specialChars = endingsAndQuotes | spsEq;

            // Optimize for the case of no special character
            var specialCharMask = Sse2.MoveMask(specialChars);
            if (specialCharMask != 0)
            {
                var sepsMask = Sse2.MoveMask(spsEq);
                // Optimize for case of only separators i.e. no endings or quotes
                if (sepsMask == specialCharMask)
                {
                    var mask = sepsMask;
                    SepAssert.AssertMaxPosition(dataIndex, Vector128<byte>.Count);
                    var dataIndexWithSeparatorShifted = separatorShifted | dataIndex;
                    do
                    {
                        var sepRelativeIndex = BitOperations.TrailingZeroCount((int)mask);
                        mask &= (mask - 1); // Or Bmi1.ResetLowestSetBit/JIT seems to do it fine

                        // Accumulate index + character found, bit pack char with index
                        // Using ctor and Pos type is too slow due to bad code gen
                        var sepIndex = dataIndexWithSeparatorShifted + sepRelativeIndex;
                        positionsRefCurrent = sepIndex;
                        positionsRefCurrent = ref Unsafe.Add(ref positionsRefCurrent, 1);
                    }
                    while (mask != 0);
                }
                else
                {
                    var mask = specialCharMask;
                    do
                    {
                        var sepRelativeIndex = BitOperations.TrailingZeroCount(mask);
                        mask &= (mask - 1); // Or Bmi1.ResetLowestSetBit/JIT seems to do it fine

                        // Accumulate index + character found, bit pack char with index
                        // Getting from Vector is slow and incurs bounds check
                        // Instead code generation better if just get as char from buffer
                        var charFound = Unsafe.Add(ref charsRef, sepRelativeIndex);
                        var sepIndex = dataIndex + sepRelativeIndex;
                        positionsRefCurrent = SepCharPosition.PackRaw(charFound, sepIndex);
                        positionsRefCurrent = ref Unsafe.Add(ref positionsRefCurrent, 1);
                    }
                    while (mask != 0);
                }
                // If current is greater than or equal than "stop", then break.
                // There is no longer guaranteed space enough for next Vector128<byte>.Count.
                if (Unsafe.IsAddressLessThan(ref positionsRefStop, ref positionsRefCurrent))
                {
                    // Move data index so next find starts correctly
                    dataIndex += Vector128<byte>.Count;
                    break;
                }
            }
        }
        positionsEnd = (int)(Unsafe.ByteOffset(ref positionsRef, ref positionsRefCurrent) >> 2); // / sizeof(int)); // CQ: Weird with div sizeof
        // Step is Vector128<byte>.Count so may go past end, ensure limited
        dataIndex = Math.Min(charsEnd, dataIndex);
        return dataIndex;
    }
}
