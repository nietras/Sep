using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

sealed class SepParserIndexOfAny : ISepParser
{
    readonly char _separator;
    readonly char[] _specialChars;
    int _quoting = 0;

    public unsafe SepParserIndexOfAny(Sep sep)
    {
        _separator = sep.Separator;
        _specialChars = new[] { sep.Separator, CarriageReturn, LineFeed, Quote };
    }

    public int PaddingLength => 0;

    [SkipLocalsInit]
    public int Parse(char[] chars, int charsIndex, int charsEnd,
                     int[] colEnds, ref int colEndsEnd,
                     scoped ref int _rowLineEndingOffset, scoped ref int _lineNumber)
    {
        var separator = _separator;

        var quoting = _quoting;
        var rowLineEndingOffset = _rowLineEndingOffset;
        var lineNumber = _lineNumber;

        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        colEnds.CheckPadding(colEndsEnd, PaddingLength);

        A.Assert(charsIndex <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - PaddingLength));
        ref var charsRef = ref MemoryMarshal.GetArrayDataReference(chars);

        ref var colEndsRef = ref MemoryMarshal.GetArrayDataReference(colEnds);
        ref var colEndsRefCurrent = ref Add(ref colEndsRef, colEndsEnd);
        ref var colEndsRefStop = ref Add(ref colEndsRef, colEnds.Length - 2);

        var span = chars.AsSpan(0, charsEnd);
        var specialCharsSpan = _specialChars.AsSpan();
        while ((uint)charsIndex < (uint)charsEnd)
        {
            // https://github.com/dotnet/runtime/blob/942ce9af6e4858b74cc3a1429e9a64065ffb207a/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.T.cs#L1926-L2045
            var relativeIndex = span.Slice(charsIndex).IndexOfAny(specialCharsSpan);
            if (relativeIndex >= 0)
            {
                A.Assert(charsIndex < charsEnd, $"{nameof(charsIndex)} >= {nameof(charsEnd)}");

                ref var charsCurrentRef = ref Add(ref charsRef, charsIndex);
                colEndsRefCurrent = ref SepParseMask.ParseAnyChar(ref charsCurrentRef, charsIndex, relativeIndex,
                    separator, ref rowLineEndingOffset, ref quoting, ref colEndsRefCurrent, ref lineNumber);
                charsIndex += relativeIndex + 1;

                // Used both to indicate row ended and if need to step +2 due to '\r\n'
                if (rowLineEndingOffset != 0)
                {
                    // Must be a col end and last is then dataIndex
                    charsIndex = colEndsRefCurrent + rowLineEndingOffset;
                    break;
                }

                // If current is greater than or equal than "stop", then break.
                // There is no longer guaranteed space enough for next.
                if (IsAddressLessThan(ref colEndsRefStop, ref colEndsRefCurrent))
                {
                    break;
                }
            }
            else
            {
                charsIndex = charsEnd;
            }
        }
        // ">> 2" instead of "/ sizeof(int))" // CQ: Weird with div sizeof
        colEndsEnd = (int)(ByteOffset(ref colEndsRef, ref colEndsRefCurrent) >> 2);
        // Step is VecUI8.Count so may go past end, ensure limited
        charsIndex = Math.Min(charsEnd, charsIndex);

        _quoting = quoting;
        _rowLineEndingOffset = rowLineEndingOffset;
        _lineNumber = lineNumber;

        return charsIndex;
    }
}
