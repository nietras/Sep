// Copyright (c) 2023, Olaf Kober <olaf.kober@outlook.com>

using System;
using System.Numerics; // For BitOperations
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using nietras.SeparatedValues;


namespace Sep.Internals;

internal sealed class SepParserSve : ISepParser
{
    private readonly char _separator;
    private readonly char _quote;
    private nuint _quoteCount = 0; // Total quotes found

    public SepParserSve(SepParserOptions options)
    {
        _separator = options.Separator;
        // If quotes are disabled, options.Quote is (char)0.
        // Treat (char)0 as "no quote character" to simplify logic later,
        // effectively disabling quote processing if options.Quote is NUL.
        _quote = options.Quote != default ? options.Quote : SepDefaults.NoQuote;
    }

    // SVE vector length can vary at runtime, but for padding purposes,
    // we usually refer to the specific length being used by the compiled code,
    // or a minimum if it's truly scalable in a way that impacts buffer padding.
    // For .NET, Sve.VectorLength gives the size of Vector<T> in bytes.
    public int PaddingLength => Sve.IsSupported ? Sve.VectorLength : 0;

    public int QuoteCount => (int)_quoteCount;


    public unsafe void ParseColEnds(SepReaderState s)
    {
        if (!Sve.IsSupported)
        {
            // Fallback or error if SVE is not supported, though factory should prevent this.
            SepParserIndexOfAny.Instance.ParseColEnds(s);
            return;
        }

        var separator = s.Sep.Separator;
        var chars = s._chars;
        var colEndsOrInfos = s._colEndsOrInfos;
        var stateColCount = s._parsingRowColCount;

        fixed (char* charsOrigin = chars)
        fixed (int* colEndsOrInfosOrigin = colEndsOrInfos)
        {
            char* ptr = charsOrigin + s._charsParseStart;
            char* endPtr = charsOrigin + s._charsDataEnd; // Points one past the last valid char

            int* colEndsPtr = colEndsOrInfosOrigin + s._parsingRowColEndsOrInfosStartIndex + stateColCount;

            var vectorLength = Sve.VectorLength / sizeof(char); // Sve.VectorLength is in bytes
            var sepVec = Sve.Create(separator);

            // Main loop for processing full vectors
            // Ensure ptr does not go past endPtr when loading a full vector.
            // endPtr points one beyond the last valid character.
            // So, ptr must be less than or equal to endPtr - vectorLength.
            while (ptr <= endPtr - vectorLength)
            {
                var dataVec = Sve.LoadVector(ptr);
                var predicate = Sve.CompareEqual(dataVec, sepVec);
                ulong matches = Sve.ExtractTrueElementMask(predicate);

                while (matches != 0)
                {
                    var indexInVector = BitOperations.TrailingZeroCount(matches);
                    *colEndsPtr = (int)(ptr - charsOrigin) + indexInVector;
                    colEndsPtr++;
                    stateColCount++;
                    matches = BitOperations.ResetLowestSetBit(matches);
                }
                ptr += vectorLength;
            }

            // Handle remaining characters (less than a full vector)
            while (ptr < endPtr)
            {
                if (*ptr == separator)
                {
                    *colEndsPtr = (int)(ptr - charsOrigin);
                    colEndsPtr++;
                    stateColCount++;
                }
                ptr++;
            }

            s._parsingRowColCount = stateColCount;
            // s._charsParseStart will be updated by the reader based on newline detection or end of data
            // For ParseColEnds, we effectively report how many we've scanned up to.
            // The caller (SepReader) is responsible for advancing s._charsParseStart after handling newlines.
            // However, we should indicate how far we've actually processed the chars array.
            s._charsProcessedNeqNewline = (int)(ptr - charsOrigin);
        }
    }

    public unsafe void ParseColInfos(SepReaderState s)
    {
        if (!Sve.IsSupported || _quote == SepDefaults.NoQuote)
        {
            // Fallback if SVE not supported or quotes are disabled for this parser instance
            // If quotes are disabled, ParseColEnds logic is sufficient but needs to populate ColInfos.
            // For simplicity, using IndexOfAny for disabled quotes scenario.
            // A dedicated SVE ColEnds-to-ColInfos adapter could be faster.
            SepParserIndexOfAny.Instance.ParseColInfos(s);
            _quoteCount = SepParserIndexOfAny.Instance.QuoteCount; // Keep quote count consistent
            return;
        }

        var separatorChar = _separator;
        var quoteChar = _quote;

        var chars = s._chars;
        var colInfos = s._colEndsOrInfos; // This is an int array, cast to SepColInfo* later

        // Local copy of quote count, sync back at the end
        nuint currentTotalQuotes = _quoteCount;
        // State carried from previous buffers or vector iterations
        bool currentlyInQuote = s._parsingRowIsUnfinishedQuoted;
        bool currentColumnIsQuoted = s._parsingRowCurrentColumnIsQuoted; // If the current column being built is quoted
        int currentColumnStartAbsoluteIndex = s._parsingRowCharsStartIndex;

        fixed (char* charsOrigin = chars)
        fixed (int* colInfosOriginInts = colInfos)
        {
            SepColInfo* colInfosPtr = (SepColInfo*)colInfosOriginInts + s._parsingRowColEndsOrInfosStartIndex + s._parsingRowColCount;

            char* ptr = charsOrigin + s._charsParseStart;
            char* endPtr = charsOrigin + s._charsDataEnd;

            var vectorLengthChars = Sve.VectorLength / sizeof(char);
            var sepVec = Sve.Create(separatorChar);
            var quoteVec = Sve.Create(quoteChar);

            // Main loop for processing full vectors
            while (ptr <= endPtr - vectorLengthChars)
            {
                var dataVec = Sve.LoadVector(ptr);
                var sepPredicate = Sve.CompareEqual(dataVec, sepVec);
                var quotePredicate = Sve.CompareEqual(dataVec, quoteVec);

                ulong sepBitmask = Sve.ExtractTrueElementMask(sepPredicate);
                ulong quoteBitmask = Sve.ExtractTrueElementMask(quotePredicate);
                currentTotalQuotes += (uint)BitOperations.PopCount(quoteBitmask);

                ulong combinedBitmask = sepBitmask | quoteBitmask;

                // Process characters within the vector based on combinedBitmask
                while (combinedBitmask != 0)
                {
                    var indexInVector = BitOperations.TrailingZeroCount(combinedBitmask);
                    var charAbsoluteIndex = (int)(ptr - charsOrigin) + indexInVector;

                    if (((quoteBitmask >> indexInVector) & 1) != 0) // Is it a quote?
                    {
                        currentlyInQuote = !currentlyInQuote;
                        currentColumnIsQuoted = true;
                    }
                    else // Is it a separator? (since it's in combined and not a quote)
                    {
                        if (!currentlyInQuote)
                        {
                            *colInfosPtr = new SepColInfo(currentColumnStartAbsoluteIndex, charAbsoluteIndex - currentColumnStartAbsoluteIndex, currentColumnIsQuoted);
                            colInfosPtr++;
                            s._parsingRowColCount++;
                            currentColumnIsQuoted = false; // Reset for next column
                            currentColumnStartAbsoluteIndex = charAbsoluteIndex + 1;
                        }
                    }
                    combinedBitmask = BitOperations.ResetLowestSetBit(combinedBitmask);
                }
                ptr += vectorLengthChars;
            }

            // Scalar loop for remaining characters
            while (ptr < endPtr)
            {
                var currentChar = *ptr;
                var charAbsoluteIndex = (int)(ptr - charsOrigin);

                if (currentChar == quoteChar)
                {
                    currentlyInQuote = !currentlyInQuote;
                    currentColumnIsQuoted = true;
                    currentTotalQuotes++;
                }
                else if (currentChar == separatorChar)
                {
                    if (!currentlyInQuote)
                    {
                        *colInfosPtr = new SepColInfo(currentColumnStartAbsoluteIndex, charAbsoluteIndex - currentColumnStartAbsoluteIndex, currentColumnIsQuoted);
                        colInfosPtr++;
                        s._parsingRowColCount++;
                        currentColumnIsQuoted = false;
                        currentColumnStartAbsoluteIndex = charAbsoluteIndex + 1;
                    }
                }
                ptr++;
            }

            // Update state that persists across calls
            s._parsingRowIsUnfinishedQuoted = currentlyInQuote;
            s._parsingRowCurrentColumnIsQuoted = currentColumnIsQuoted;
            s._charsProcessedNeqNewline = (int)(ptr - charsOrigin); 
            // The SepReader is responsible for finalizing the last column on newline/EOF
            // and setting s._parsingRowCharsStartIndex for the next segment.
            _quoteCount = currentTotalQuotes;
        }
    }
}
