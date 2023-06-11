using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static nietras.SeparatedValues.SepDefaults;
using static nietras.SeparatedValues.SepCharsFinderHelper;

namespace nietras.SeparatedValues;

sealed class SepCharsFinderAvx2PackCmpOrMoveMaskTzcnt : ISepCharsFinder
{
    readonly byte _separator;
    readonly Vector256<byte> _nls = Vector256.Create(LineFeedByte);
    readonly Vector256<byte> _crs = Vector256.Create(CarriageReturnByte);
    readonly Vector256<byte> _qts = Vector256.Create(QuoteByte);
    readonly Vector256<byte> _sps;

    public unsafe SepCharsFinderAvx2PackCmpOrMoveMaskTzcnt(Sep sep)
    {
        _separator = (byte)sep.Separator;
        _sps = Vector256.Create(_separator);
    }

    public int PaddingLength => Vector256<byte>.Count; // Parses 2 x char vectors e.g. 1 byte vector
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
        ref var positionsRefStop = ref Unsafe.Add(ref positionsRef, positions.Length - Vector256<byte>.Count);

        var nls = _nls; //Vector256.Create(LineFeedByte);
        var crs = _crs; //Vector256.Create(CarriageReturnByte);
        var qts = _qts; //Vector256.Create(QuoteByte);
        var sps = _sps; //Vector256.Create(_separator);

        var separatorShifted = _separator << SepCharPosition.CharShift;

        var dataIndex = dataStart;
        for (; dataIndex < dataEnd; dataIndex += Vector256<byte>.Count,
             charsRef = ref Unsafe.Add(ref charsRef, Vector256<byte>.Count))
        {
            var vector0 = Unsafe.ReadUnaligned<Vector256<short>>(
                ref Unsafe.As<char, byte>(ref charsRef));
            var vector1 = Unsafe.ReadUnaligned<Vector256<short>>(
                ref Unsafe.As<char, byte>(ref Unsafe.Add(ref charsRef, Vector256<short>.Count)));

            var packed = Avx2.PackUnsignedSaturate(vector0, vector1);
            // Pack interleaves the two vectors need to permute them back
            var bytes = Avx2.Permute4x64(packed.AsInt64(), 0b_11_01_10_00).AsByte();

            var nlsEq = Vector256.Equals(bytes, nls);
            var crsEq = Vector256.Equals(bytes, crs);
            var qtsEq = Vector256.Equals(bytes, qts);
            var spsEq = Vector256.Equals(bytes, sps);

            var lineEndings = nlsEq | crsEq;
            var endingsAndQuotes = lineEndings | qtsEq;
            var specialChars = endingsAndQuotes | spsEq;

            // Optimize for the case of no special character
            var specialCharMask = Avx2.MoveMask(specialChars);
            if (specialCharMask != 0)
            {
                var sepsMask = Avx2.MoveMask(spsEq);
                // Optimize for case of only separators i.e. no endings or quotes
                if (sepsMask == specialCharMask)
                {
                    SepAssert.AssertMaxPosition(dataIndex, Vector256<byte>.Count);
                    positionsRefCurrent = ref PackSeparatorPositions(sepsMask,
                        separatorShifted, dataIndex, ref positionsRefCurrent);
                }
                else
                {
                    positionsRefCurrent = ref PackSpecialCharPositions(specialCharMask,
                        ref charsRef, dataIndex, ref positionsRefCurrent);
                }
                // If current is greater than or equal than "stop", then break.
                // There is no longer guaranteed space enough for next Vector256<byte>.Count.
                if (Unsafe.IsAddressLessThan(ref positionsRefStop, ref positionsRefCurrent))
                {
                    // Move data index so next find starts correctly
                    dataIndex += Vector256<byte>.Count;
                    break;
                }
            }
        }
        positionsEnd = (int)(Unsafe.ByteOffset(ref positionsRef, ref positionsRefCurrent) >> 2); // / sizeof(int)); // CQ: Weird with div sizeof
        // Step is Vector256<byte>.Count so may go past end, ensure limited
        dataIndex = Math.Min(charsEnd, dataIndex);
        return dataIndex;
    }
}
