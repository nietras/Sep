using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

sealed class SepCharsFinderVector64NrwCmpExtMsbTzcnt : ISepCharsFinder
{
    readonly char _separator;
    readonly Vector64<ushort> _max = Vector64.Create((ushort)(Sep.Max.Separator + 1));
    readonly Vector64<byte> _nls = Vector64.Create(LineFeedByte);
    readonly Vector64<byte> _crs = Vector64.Create(CarriageReturnByte);
    readonly Vector64<byte> _qts = Vector64.Create(QuoteByte);
    readonly Vector64<byte> _sps;

    public unsafe SepCharsFinderVector64NrwCmpExtMsbTzcnt(Sep sep)
    {
        _separator = sep.Separator;
        _sps = Vector64.Create((byte)_separator);
    }

    public int PaddingLength => Vector64<byte>.Count;
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
        ref var positionsRefStop = ref Unsafe.Add(ref positionsRef, positions.Length - Vector64<byte>.Count);

        var max = _max;
        var nls = _nls;
        var crs = _crs;
        var qts = _qts;
        var sps = _sps;

        var separatorShifted = _separator << SepCharPosition.CharShift;

        var dataIndex = dataStart;
        for (; dataIndex < dataEnd; dataIndex += Vector64<byte>.Count,
             charsRef = ref Unsafe.Add(ref charsRef, Vector64<byte>.Count))
        {
            var vector0 = Unsafe.ReadUnaligned<Vector64<ushort>>(
                ref Unsafe.As<char, byte>(ref charsRef));
            var vector1 = Unsafe.ReadUnaligned<Vector64<ushort>>(
                ref Unsafe.As<char, byte>(ref Unsafe.Add(ref charsRef, Vector64<ushort>.Count)));

            var limit0 = Vector64.Min(vector0, max);
            var limit1 = Vector64.Min(vector1, max);
            var vector = Vector64.Narrow(limit0, limit1);

            var nlsEq = Vector64.Equals(vector, nls);
            var crsEq = Vector64.Equals(vector, crs);
            var qtsEq = Vector64.Equals(vector, qts);
            var spsEq = Vector64.Equals(vector, sps);

            var lineEndings = nlsEq | crsEq;
            var endingsAndQuotes = lineEndings | qtsEq;
            var specialChars = endingsAndQuotes | spsEq;

            // Optimize for the case of no special character
            var specialCharMask = specialChars.ExtractMostSignificantBits();
            if (specialCharMask != 0)
            {
                var sepsMask = spsEq.ExtractMostSignificantBits();
                // Optimize for case of only separators i.e. no endings or quotes
                if (sepsMask == specialCharMask)
                {
                    var mask = sepsMask;
                    SepAssert.AssertMaxPosition(dataIndex, Vector64<byte>.Count);
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
                // There is no longer guaranteed space enough for next Vector64<byte>.Count.
                if (Unsafe.IsAddressLessThan(ref positionsRefStop, ref positionsRefCurrent))
                {
                    // Move data index so next find starts correctly
                    dataIndex += Vector64<byte>.Count;
                    break;
                }
            }
        }
        positionsEnd = (int)(Unsafe.ByteOffset(ref positionsRef, ref positionsRefCurrent) >> 2); // / sizeof(int)); // CQ: Weird with div sizeof
        // Step is Vector64<byte>.Count so may go past end, ensure limited
        dataIndex = Math.Min(charsEnd, dataIndex);
        return dataIndex;
    }
}
