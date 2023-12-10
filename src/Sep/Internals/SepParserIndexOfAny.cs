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
    nuint _quoteCount = 0;

    public unsafe SepParserIndexOfAny(Sep sep)
    {
        _separator = sep.Separator;
        _specialChars = new[] { sep.Separator, CarriageReturn, LineFeed, Quote };
    }

    public int PaddingLength => 0;
    public int QuoteCount => (int)_quoteCount;

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColEnds(SepReaderState s)
    {
        Parse<int, SepColEndMethods>(s);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void ParseColInfos(SepReaderState s)
    {
        Parse<SepColInfo, SepColInfoMethods>(s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int Parse<TColInfo, TColInfoMethods>(SepReaderState s)
        where TColInfo : unmanaged
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        var separator = (char)_separator;
        var quoteCount = _quoteCount;

        var chars = s._chars;
        var charsIndex = s._charsParseStart;
        var charsEnd = s._charsDataEnd;
        var colInfos = s._colEndsOrColInfos;
        var colCount = s._parsingRowColCount;
        var lineNumber = s._parsingLineNumber;

        var rowLineEndingOffset = 0;

        var colInfosLength = colInfos.Length / (SizeOf<TColInfo>() / SizeOf<int>());

        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        SepArrayExtensions.CheckPadding(colInfosLength, colCount, PaddingLength);

        A.Assert(charsIndex <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - PaddingLength));
        ref var charsRef = ref MemoryMarshal.GetArrayDataReference(chars);

        ref var colInfosRef = ref As<int, TColInfo>(ref MemoryMarshal.GetArrayDataReference(colInfos));
        ref var colInfosRefCurrent = ref Add(ref colInfosRef, colCount);
        ref var colInfosRefStop = ref Add(ref colInfosRef, colInfosLength - 2);

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
                colInfosRefCurrent = ref SepParseMask.ParseAnyChar<TColInfo, TColInfoMethods>(ref charsCurrentRef, charsIndex, relativeIndex,
                    separator, ref rowLineEndingOffset, ref quoteCount, ref colInfosRefCurrent, ref lineNumber);
                charsIndex += relativeIndex + 1;

                // Used both to indicate row ended and if need to step +2 due to '\r\n'
                if (rowLineEndingOffset != 0)
                {
                    // Must be a col end and last is then dataIndex
                    charsIndex = TColInfoMethods.GetColEnd(colInfosRefCurrent) + rowLineEndingOffset;
                    break;
                }

                // If current is greater than or equal than "stop", then break.
                // There is no longer guaranteed space enough for next.
                if (IsAddressLessThan(ref colInfosRefStop, ref colInfosRefCurrent))
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
        colCount = (int)(ByteOffset(ref colInfosRef, ref colInfosRefCurrent) / SizeOf<TColInfo>());
        // Step is VecUI8.Count so may go past end, ensure limited
        charsIndex = Math.Min(charsEnd, charsIndex);

        _quoteCount = quoteCount;
        s._parsingRowColCount = colCount;
        s._parsingLineNumber = lineNumber;
        s._charsParseStart = charsIndex;

        return rowLineEndingOffset;
    }
}
