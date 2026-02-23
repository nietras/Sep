using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace nietras.SeparatedValues;

class SepParserIndexOfAny<TChar, TCharInfo> : ISepParser<TChar, TCharInfo>
    where TChar : unmanaged, IEquatable<TChar>
    where TCharInfo : struct, ISepCharInfo<TChar>
{
    readonly TChar _separator;
    readonly SearchValues<TChar> _specialChars;
    nuint _quoteCount = 0;

    public SepParserIndexOfAny(TChar separator, TChar quotesOrSeparatorIfDisabled)
    {
        _separator = separator;
        _specialChars = TCharInfo.CreateSearchValues(
            [separator, TCharInfo.CarriageReturn, TCharInfo.LineFeed, quotesOrSeparatorIfDisabled]);
    }

    internal static SepParserIndexOfAny<TChar, TCharInfo> Create(Sep sep, bool disableQuotesParsing)
    {
        var separator = TCharInfo.FromChar(sep.Separator);
        var quote = disableQuotesParsing ? separator : TCharInfo.Quote;
        return new(separator, quote);
    }

    public int PaddingLength => 4;
    public int QuoteCount => (int)_quoteCount;

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColEnds(SepReaderStateBase<TChar, TCharInfo> s)
    {
        Parse<int, SepColEndMethods>(s);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColInfos(SepReaderStateBase<TChar, TCharInfo> s)
    {
        Parse<SepColInfo, SepColInfoMethods>(s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Parse<TColInfo, TColInfoMethods>(SepReaderStateBase<TChar, TCharInfo> s)
        where TColInfo : unmanaged
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        // Unpack instance fields
        var separator = _separator;
        var quoteCount = _quoteCount;

        // Unpack state fields
        var chars = s._chars;
        var charsIndex = s._charsParseStart;
        var charsEnd = s._charsDataEnd;
        var lineNumber = s._parsingLineNumber;
        var colInfos = s._colEndsOrColInfos;

        var colInfosLength = TColInfoMethods.IntsLengthToColInfosLength(colInfos.Length);

        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        SepArrayExtensions.CheckPadding(colInfosLength, s._parsingRowColCount + s._parsingRowColEndsOrInfosStartIndex, PaddingLength);
        A.Assert(charsIndex <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - PaddingLength));
        ref var charsRef = ref MemoryMarshal.GetArrayDataReference(chars);

        ref var colInfosRefOrigin = ref As<int, TColInfo>(ref MemoryMarshal.GetArrayDataReference(colInfos));
        ref var colInfosRef = ref Add(ref colInfosRefOrigin, s._parsingRowColEndsOrInfosStartIndex);
        ref var colInfosRefCurrent = ref Add(ref colInfosRefOrigin, s._parsingRowColCount + s._parsingRowColEndsOrInfosStartIndex);
        ref var colInfosRefEnd = ref Add(ref colInfosRefOrigin, colInfosLength);
        var colInfosStopLength = colInfosLength - 3 - SepReaderState.ColEndsOrInfosExtraEndCount;
        ref var colInfosRefStop = ref Add(ref colInfosRefOrigin, colInfosStopLength);

        var span = chars.AsSpan(0, charsEnd);
        var specialChars = _specialChars;
        while ((uint)charsIndex < (uint)charsEnd &&
               !IsAddressLessThan(ref colInfosRefStop, ref colInfosRefCurrent))
        {
            var relativeIndex = span.Slice(charsIndex).IndexOfAny(specialChars);
            if (relativeIndex >= 0)
            {
                A.Assert(charsIndex < charsEnd, $"{nameof(charsIndex)} >= {nameof(charsEnd)}");

                ref var charsCurrentRef = ref Add(ref charsRef, charsIndex);
                var rowLineEndingOffset = 0;
                colInfosRefCurrent = ref SepParseMask.ParseAnyChar<TChar, TCharInfo, TColInfo, TColInfoMethods>(
                    ref charsCurrentRef, charsIndex, relativeIndex,
                    separator, ref rowLineEndingOffset, ref quoteCount, ref colInfosRefCurrent, ref lineNumber);
                charsIndex += relativeIndex + 1;

                // Used both to indicate row ended and if need to step +2 due to '\r\n'
                if (rowLineEndingOffset != 0)
                {
                    // Must be a col end and last is then dataIndex
                    charsIndex = TColInfoMethods.GetColEnd(colInfosRefCurrent) + rowLineEndingOffset;

                    var colCount = TColInfoMethods.CountOffset(ref colInfosRef, ref colInfosRefCurrent);
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
        // Update instance state from enregistered
        _quoteCount = quoteCount;
        s._parsingRowColCount = TColInfoMethods.CountOffset(ref colInfosRef, ref colInfosRefCurrent);
        s._parsingLineNumber = lineNumber;
        // Step is VecUI8.Count so may go past end, ensure limited
        s._charsParseStart = Math.Min(charsEnd, charsIndex);
    }
}

sealed class SepParserIndexOfAny : SepParserIndexOfAny<char, SepCharInfoUtf16>, ISepParser
{
    public SepParserIndexOfAny(SepParserOptions options)
        : base(options.Separator, options.QuotesOrSeparatorIfDisabled) { }
}
