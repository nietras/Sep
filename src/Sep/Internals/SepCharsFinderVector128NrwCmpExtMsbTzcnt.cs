using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using static nietras.SeparatedValues.SepCharsFinderHelper;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

sealed class SepCharsFinderVector128NrwCmpExtMsbTzcnt : ISepCharsFinder
{
    readonly char _separator;
    readonly Vector128<ushort> _max = Vector128.Create((ushort)(Sep.Max.Separator + 1));
    readonly Vector128<byte> _nls = Vector128.Create(LineFeedByte);
    readonly Vector128<byte> _crs = Vector128.Create(CarriageReturnByte);
    readonly Vector128<byte> _qts = Vector128.Create(QuoteByte);
    readonly Vector128<byte> _sps;

    public unsafe SepCharsFinderVector128NrwCmpExtMsbTzcnt(Sep sep)
    {
        _separator = sep.Separator;
        _sps = Vector128.Create((byte)_separator);
    }

    public int PaddingLength => Vector128<byte>.Count;
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

        var max = _max;
        var nls = _nls;
        var crs = _crs;
        var qts = _qts;
        var sps = _sps;

        var separatorShifted = _separator << SepCharPosition.CharShift;

        var dataIndex = dataStart;
        for (; dataIndex < dataEnd; dataIndex += Vector128<byte>.Count,
             charsRef = ref Unsafe.Add(ref charsRef, Vector128<byte>.Count))
        {
            var vector0 = Unsafe.ReadUnaligned<Vector128<ushort>>(
                ref Unsafe.As<char, byte>(ref charsRef));
            var vector1 = Unsafe.ReadUnaligned<Vector128<ushort>>(
                ref Unsafe.As<char, byte>(ref Unsafe.Add(ref charsRef, Vector128<ushort>.Count)));

            var limit0 = Vector128.Min(vector0, max);
            var limit1 = Vector128.Min(vector1, max);
            var vector = Vector128.Narrow(limit0, limit1);

            var nlsEq = Vector128.Equals(vector, nls);
            var crsEq = Vector128.Equals(vector, crs);
            var qtsEq = Vector128.Equals(vector, qts);
            var spsEq = Vector128.Equals(vector, sps);

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
                    SepAssert.AssertMaxPosition(dataIndex, Vector128<byte>.Count);
                    positionsRefCurrent = ref PackSeparatorPositions((int)sepsMask,
                        separatorShifted, dataIndex, ref positionsRefCurrent);
                }
                else
                {
                    positionsRefCurrent = ref PackSpecialCharPositions((int)specialCharMask,
                        ref charsRef, dataIndex, ref positionsRefCurrent);
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
