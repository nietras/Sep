using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

sealed class SepParserIndexOfAny : ISepParser
{
    public int PaddingLength => 4;

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColEnds(SepReaderState s, SepParserOptions options)
    {
        Parse<SepColEndMethods>(s, options);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColInfos(SepReaderState s, SepParserOptions options)
    {
        Parse<SepColInfoMethods>(s, options);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    static void Parse<TColInfoMethods>(SepReaderState s, SepParserOptions options)
        where TColInfoMethods : ISepColInfoMethods
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        var separator = (char)options.Separator;
        var quotesOrSeparatorIfDisabled = (char)options.QuotesOrSeparatorIfDisabled;

        // Unpack state fields
        var quoteCount = s._parsingRowQuoteCount;
        var chars = s._chars;
        var charsIndex = s._charsParseStart;
        var charsEnd = s._charsDataEnd;
        var lineNumber = s._parsingLineNumber;
        var colInfos = s._colEndsOrColInfos;

        var colInfosLength = colInfos.Length;
        if (typeof(TColInfoMethods) == typeof(SepColInfoMethods)) { colInfosLength >>= 1; }

        chars.CheckPaddingAndIsZero(charsEnd, 4);
        SepArrayExtensions.CheckPadding(colInfosLength, s._parsingRowColCount + s._parsingRowColEndsOrInfosStartIndex, 4);
        A.Assert(charsIndex <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - 4));
        ref var charsRef = ref MemoryMarshal.GetArrayDataReference(chars);

        ref var colInfosRefOrigin = ref MemoryMarshal.GetArrayDataReference(colInfos);
        ref var colInfosRef = ref Add(ref colInfosRefOrigin, s._parsingRowColEndsOrInfosStartIndex);
        ref var colInfosRefCurrent = ref Add(ref colInfosRefOrigin, s._parsingRowColCount + s._parsingRowColEndsOrInfosStartIndex);
        ref var colInfosRefEnd = ref Add(ref colInfosRefOrigin, colInfosLength);
        var colInfosStopLength = colInfosLength - 3 - SepReaderState.ColEndsOrInfosExtraEndCount;
        ref var colInfosRefStop = ref Add(ref colInfosRefOrigin, colInfosStopLength);

        var span = chars.AsSpan(0, charsEnd);
        ReadOnlySpan<char> specialCharsSpan = stackalloc char[]
            { separator, (char)options.CarriageReturn, (char)options.LineFeed, quotesOrSeparatorIfDisabled };
        if (separator == quotesOrSeparatorIfDisabled)
        {
            specialCharsSpan = specialCharsSpan[..3];
        }
        while ((uint)charsIndex < (uint)charsEnd &&
               !IsAddressLessThan(ref colInfosRefStop, ref colInfosRefCurrent))
        {
            // https://github.com/dotnet/runtime/blob/942ce9af6e4858b74cc3a1429e9a64065ffb207a/src/libraries/System.Private.CoreLib/src/System/SpanHelpers.T.cs#L1926-L2045
            var relativeIndex = span.Slice(charsIndex).IndexOfAny(specialCharsSpan);
            if (relativeIndex >= 0)
            {
                A.Assert(charsIndex < charsEnd, $"{nameof(charsIndex)} >= {nameof(charsEnd)}");

                ref var charsCurrentRef = ref Add(ref charsRef, charsIndex);
                var rowLineEndingOffset = 0;
                colInfosRefCurrent = ref SepParseMask.ParseAnyChar<TColInfoMethods>(ref charsCurrentRef, charsIndex, relativeIndex,
                    separator, ref rowLineEndingOffset, ref quoteCount,
                    ref colInfosRefCurrent, ref lineNumber);
                charsIndex += relativeIndex + 1;

                // Used both to indicate row ended and if need to step +2 due to '\r\n'
                if (rowLineEndingOffset != 0)
                {
                    // Must be a col end and last is then dataIndex
                    charsIndex = TColInfoMethods.GetColEnd(colInfosRefCurrent) + rowLineEndingOffset;

                    var colCount = (int)(ByteOffset(ref colInfosRef, ref colInfosRefCurrent) / sizeof(int));
                    // Add new parsed row
                    ref var parsedRowRef = ref MemoryMarshal.GetArrayDataReference(s._parsedRows);
                    Add(ref parsedRowRef, s._parsedRowsCount) = new(lineNumber, colCount);
                    ++s._parsedRowsCount;
                    // Next row start (one before)
                    colInfosRefCurrent = ref Add(ref colInfosRefCurrent, 1);
                    A.Assert(IsAddressLessThan(ref colInfosRefCurrent, ref colInfosRefEnd));
                    colInfosRefCurrent = TColInfoMethods.Create(charsIndex - 1, 0);
                    // Update for next row
                    colInfosRef = ref colInfosRefCurrent;
                    s._parsingRowColEndsOrInfosStartIndex += colCount + 1;
                    s._parsingRowCharsStartIndex = charsIndex;
                    // Space for more rows?
                    if (s._parsedRowsCount >= s._parsedRows.Length)
                    {
                        break;
                    }
                }
            }
            else
            {
                charsIndex = charsEnd;
            }
        }
        // Update reader state from enregistered
        s._parsingRowQuoteCount = quoteCount;
        s._parsingRowColCount = (int)(ByteOffset(ref colInfosRef, ref colInfosRefCurrent) / sizeof(int));
        s._parsingLineNumber = lineNumber;
        // Step is VecUI8.Count so may go past end, ensure limited
        s._charsParseStart = Math.Min(charsEnd, charsIndex);
    }
}
