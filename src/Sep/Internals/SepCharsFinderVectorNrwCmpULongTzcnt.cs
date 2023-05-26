using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

sealed class SepCharsFinderVectorNrwCmpULongTzcnt : ISepCharsFinder
{
    readonly char _separator;
    readonly Vector<ushort> _max = new((ushort)(Sep.Max.Separator + 1));
    readonly Vector<byte> _nls = new(LineFeedByte);
    readonly Vector<byte> _crs = new(CarriageReturnByte);
    readonly Vector<byte> _qts = new(QuoteByte);
    readonly Vector<byte> _sps;

    public unsafe SepCharsFinderVectorNrwCmpULongTzcnt(Sep sep)
    {
        _separator = sep.Separator;
        _sps = new((byte)_separator);
    }

    public int PaddingLength => Vector<byte>.Count;
    public int RequestedPositionsFreeLength => PaddingLength * 32;

    [SkipLocalsInit]
    public unsafe int Find(char[] _chars, int charsStart, int charsEnd,
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
        ref var positionsRefStop = ref Unsafe.Add(ref positionsRef, positions.Length - Vector<byte>.Count);

        var max = _max;
        var nls = _nls;
        var crs = _crs;
        var qts = _qts;
        var sps = _sps;

        var separatorShifted = _separator << SepCharPosition.CharShift;

        var dataIndex = dataStart;
        for (; dataIndex < dataEnd; dataIndex += Vector<byte>.Count,
             charsRef = ref Unsafe.Add(ref charsRef, Vector<byte>.Count))
        {
            var vector0 = Unsafe.ReadUnaligned<Vector<ushort>>(
                ref Unsafe.As<char, byte>(ref charsRef));
            var vector1 = Unsafe.ReadUnaligned<Vector<ushort>>(
                ref Unsafe.As<char, byte>(ref Unsafe.Add(ref charsRef, Vector<ushort>.Count)));

            var limit0 = Vector.Min(vector0, max);
            var limit1 = Vector.Min(vector1, max);
            var vector = Vector.Narrow(limit0, limit1);

            var nlsEq = Vector.Equals(vector, nls);
            var crsEq = Vector.Equals(vector, crs);
            var qtsEq = Vector.Equals(vector, qts);
            var spsEq = Vector.Equals(vector, sps);

            var lineEndings = nlsEq | crsEq;
            var endingsAndQuotes = lineEndings | qtsEq;
            var specialChars = endingsAndQuotes | spsEq;

            // There does not appear to be any equivalent to
            // ExtractMostSignificantBits() for Vector<T>. Instead, check 64-bit
            // at a time find non-zero using tzcnt.
            // Code-gen is really poor for below and bench really slow
            var specialCharsULong = Vector.As<byte, ulong>(specialChars);
            for (var i = 0; i < Vector<ulong>.Count; i++)
            {
                var l = specialCharsULong[i];
                while (l != 0)
                {
                    var nonZeroIndex = BitOperations.TrailingZeroCount(l) >> 3;
                    ((byte*)&l)[nonZeroIndex] = 0; // Reset
                    var sepRelativeIndex = nonZeroIndex + i * sizeof(ulong);
                    // Accumulate index + character found, bit pack char with index
                    // Getting from Vector is slow and incurs bounds check
                    // Instead code generation better if just get as char from buffer
                    var charFound = Unsafe.Add(ref charsRef, sepRelativeIndex);
                    var sepIndex = dataIndex + sepRelativeIndex;
                    positionsRefCurrent = SepCharPosition.PackRaw(charFound, sepIndex);
                    positionsRefCurrent = ref Unsafe.Add(ref positionsRefCurrent, 1);
                }
            }
            // If current is greater than or equal than "stop", then break.
            // There is no longer guaranteed space enough for next Vector<byte>.Count.
            if (Unsafe.IsAddressLessThan(ref positionsRefStop, ref positionsRefCurrent))
            {
                // Move data index so next find starts correctly
                dataIndex += Vector<byte>.Count;
                break;
            }
        }
        positionsEnd = (int)(Unsafe.ByteOffset(ref positionsRef, ref positionsRefCurrent) >> 2); // / sizeof(int)); // CQ: Weird with div sizeof
        // Step is Vector<byte>.Count so may go past end, ensure limited
        dataIndex = Math.Min(charsEnd, dataIndex);
        return dataIndex;
    }
}
