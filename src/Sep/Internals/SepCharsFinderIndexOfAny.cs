using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

sealed class SepCharsFinderIndexOfAny : ISepCharsFinder
{
    readonly char[] _specialChars;

    public unsafe SepCharsFinderIndexOfAny(Sep sep)
    {
        _specialChars = new[] { sep.Separator, CarriageReturn, LineFeed, Quote };
    }

    public int PaddingLength => 0;
    public int RequestedPositionsFreeLength => 1024;

    [SkipLocalsInit]
    public int Find(char[] _chars, int charsStart, int charsEnd,
                    Pos[] positions, int positionsStart, ref int positionsEnd)
    {
        var chars = _chars;
        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        // Absolute minimum, prefer RequestedPositionsFreeLength for free
        positions.CheckPadding(positionsEnd, PaddingLength);

        A.Assert(charsStart <= charsEnd);
        A.Assert(charsEnd <= (_chars.Length - PaddingLength));
        var dataStart = charsStart;
        var dataEnd = charsEnd;
        ref var charsRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars), dataStart);

        ref var positionsRef = ref Unsafe.As<Pos, int>(ref MemoryMarshal.GetArrayDataReference(positions));
        ref var positionsRefCurrent = ref Unsafe.Add(ref positionsRef, positionsEnd);
        ref var positionsRefStop = ref Unsafe.Add(ref positionsRef, positions.Length - 1);

        var span = _chars.AsSpan(0, charsEnd);
        var specialCharsSpan = _specialChars.AsSpan();
        var index = charsStart;
        while ((uint)index < (uint)charsEnd)
        {
            // https://github.com/dotnet/runtime/blob/942ce9af6e4858b74cc3a1429e9a64065ffb207a/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.T.cs#L1926-L2045
            var foundIndex = span.Slice(index).IndexOfAny(specialCharsSpan);
            if (foundIndex >= 0)
            {
                index += foundIndex;
                A.Assert(index < charsEnd, $"{nameof(index)} >= {nameof(charsEnd)}");
                A.Assert(index < (1 << SepCharPosition.CharShift - Vector<byte>.Count), $"index must be within pack limits");
                var foundChar = span[index];
                positionsRefCurrent = foundChar << SepCharPosition.CharShift | index;
                positionsRefCurrent = ref Unsafe.Add(ref positionsRefCurrent, 1);
                ++index;

                // If current is greater than or equal than "stop", then break.
                // There is no longer guaranteed space enough for next.
                if (Unsafe.IsAddressLessThan(ref positionsRefStop, ref positionsRefCurrent))
                {
                    break;
                }
            }
            else
            {
                index = charsEnd;
            }
        }
        positionsEnd = (int)(Unsafe.ByteOffset(ref positionsRef, ref positionsRefCurrent) >> 2); // / sizeof(int)); // CQ: Weird with div sizeof
        return Math.Min(index, charsEnd);
    }
}
