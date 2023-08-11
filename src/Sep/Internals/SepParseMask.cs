using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

static class SepParseMask
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsMask(nuint mask, int charsIndex, ref int colEndsRef)
    {
        do
        {
            var relativeIndex = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);
            // Pre-increment colEndsRef since [0] reserved for row start
            colEndsRef = ref Add(ref colEndsRef, 1);
            colEndsRef = charsIndex + relativeIndex;
        }
        while (mask != 0);
        return ref colEndsRef;
    }

    // Not faster
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsMaskLong(nuint mask, int charsIndex, ref int positionsRefCurrent)
    {
        var count = BitOperations.PopCount((uint)mask);
        ref var positionsRefCurrentEnd = ref Add(ref positionsRefCurrent, count);
        var charsIndexLong = (long)((ulong)charsIndex + ((ulong)charsIndex << 32));
        do
        {
            // Pre-increment colEndsRef since [0] reserved for row start
            positionsRefCurrent = ref Add(ref positionsRefCurrent, 1);

            long p0 = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);
            long p1 = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);
            // Assume endianness
            var packed = (p0 | (p1 << 32)) + charsIndexLong;
            As<int, long>(ref positionsRefCurrent) = packed;

            positionsRefCurrent = ref Add(ref positionsRefCurrent, 1);
        }
        while (mask != 0);
        return ref positionsRefCurrentEnd;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseAnyCharsMask(nuint mask, char separator,
        scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoting,
        ref int colEndsRef, scoped ref int lineNumber)
    {
        do
        {
            var relativeIndex = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);

            colEndsRef = ref ParseAnyChar(ref charsRef,
                charsIndex, relativeIndex, separator,
                ref rowLineEndingOffset, ref quoting,
                ref colEndsRef, ref lineNumber);
        }
        while (mask != 0 && (rowLineEndingOffset == 0));
        return ref colEndsRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseAnyChar(
        scoped ref char charsRef, int charsIndex, int relativeIndex, char separator,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoting,
        ref int colEndsRef, scoped ref int lineNumber)
    {
        var c = Add(ref charsRef, relativeIndex);
        if (quoting != 0)
        {
            if (c == CarriageReturn)
            {
                // If next char is a line feed, don't count it in line number,
                // but let that happen on the line feed when handled next.
                var oneCharAhead = Add(ref charsRef, relativeIndex + 1);
                if (oneCharAhead != LineFeed)
                {
                    ++lineNumber;
                }
                goto RETURN;
            }
            if (c == LineFeed)
            {
                ++lineNumber;
                goto RETURN;
            }
            if (c != Quote) goto RETURN;
        }
        if (c == separator) goto ADDCOLEND;
        if (c == CarriageReturn)
        {
            // If \r=CR, we should always be able to look 1 ahead, and if char not valid should not be \n=LF
            var oneCharAhead = Add(ref charsRef, relativeIndex + 1);
            if (oneCharAhead == LineFeed)
            { ++rowLineEndingOffset; }
            goto NEWLINE;
        }
        if (c == LineFeed) { goto NEWLINE; }
        if (c == Quote)
        {
            // Flip quoting flag
            quoting ^= 1;
            goto RETURN;
        }
    NEWLINE:
        ++lineNumber;
        ++rowLineEndingOffset;
    ADDCOLEND:
        // Pre-increment colEndsRef since [0] reserved for row start
        colEndsRef = ref Add(ref colEndsRef, 1);
        colEndsRef = charsIndex + relativeIndex;
    RETURN:
        return ref colEndsRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsLineEndingsMasks(nuint separatorsMask, nuint separatorsLineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex, char separator,
        ref int colEndsRefCurrent, scoped ref int rowLineEndingOffset, scoped ref int lineNumber)
    {
        if (separatorsMask == 0)
        {
            colEndsRefCurrent = ref ParseLineEndingMask(separatorsLineEndingsMask,
                ref charsRef, ref charsIndex, ref colEndsRefCurrent,
                ref rowLineEndingOffset, ref lineNumber);
            charsIndex += rowLineEndingOffset;
        }
        else
        {
            var endingsMask = separatorsLineEndingsMask & ~separatorsMask;
            var lineEndingIndex = BitOperations.TrailingZeroCount(endingsMask);
            if ((31 - BitOperations.LeadingZeroCount((uint)separatorsMask)) < lineEndingIndex)
            {
                colEndsRefCurrent = ref ParseSeparatorsMask(separatorsMask, charsIndex,
                    ref colEndsRefCurrent);

                var c = Add(ref charsRef, lineEndingIndex);
                ++rowLineEndingOffset;
                // Pre-increment colEndsRef since [0] reserved for row start
                colEndsRefCurrent = ref Add(ref colEndsRefCurrent, 1);
                charsIndex += lineEndingIndex;
                colEndsRefCurrent = charsIndex;
                if (c == CarriageReturn)
                {
                    // If \r=CR, we should always be able to look 1 ahead, and if char not valid should not be \n=LF
                    var oneCharAhead = Add(ref charsRef, lineEndingIndex + 1);
                    if (oneCharAhead == LineFeed) { ++rowLineEndingOffset; }
                }
                ++lineNumber;
                charsIndex += rowLineEndingOffset;
            }
            else
            {
                // Used both to indicate row ended and if need to step +2 due to '\r\n'
                colEndsRefCurrent = ref ParseSeparatorsLineEndingsMask(separatorsLineEndingsMask,
                    separator, ref charsRef, charsIndex, ref rowLineEndingOffset,
                    ref colEndsRefCurrent, ref lineNumber);
                // We know line has ended and RowEnded set so no need to check
                // Must be a col end and last is then dataIndex, +1 to start at next
                charsIndex = colEndsRefCurrent + rowLineEndingOffset;
            }
        }
        return ref colEndsRefCurrent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseLineEndingMask(nuint lineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex,
        ref int colEndsRefCurrent, scoped ref int rowLineEndingOffset, scoped ref int lineNumber)
    {
        var lineEndingIndex = BitOperations.TrailingZeroCount(lineEndingsMask);
        var c = Add(ref charsRef, lineEndingIndex);
        ++rowLineEndingOffset;
        // Pre-increment colEndsRef since [0] reserved for row start
        colEndsRefCurrent = ref Add(ref colEndsRefCurrent, 1);
        charsIndex += lineEndingIndex;
        colEndsRefCurrent = charsIndex;
        if (c == CarriageReturn)
        {
            // If \r=CR, we should always be able to look 1 ahead, and if char not valid should not be \n=LF
            var oneCharAhead = Add(ref charsRef, lineEndingIndex + 1);
            if (oneCharAhead == LineFeed) { ++rowLineEndingOffset; }
        }
        ++lineNumber;
        return ref colEndsRefCurrent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ref int ParseSeparatorsLineEndingsMask(nuint mask, char separator,
        scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset,
        ref int colEndsRef, scoped ref int lineNumber)
    {
        do
        {
            var relativeIndex = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);

            colEndsRef = ref ParseSeparatorLineEndingChar(ref charsRef,
                charsIndex, relativeIndex, separator,
                ref rowLineEndingOffset, ref colEndsRef, ref lineNumber);
        }
        while (mask != 0 && (rowLineEndingOffset == 0));
        return ref colEndsRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ref int ParseSeparatorLineEndingChar(
        scoped ref char charsRef, int charsIndex, int relativeIndex, char separator,
        scoped ref int rowLineEndingOffset,
        ref int colEndsRef, scoped ref int lineNumber)
    {
        var c = Add(ref charsRef, relativeIndex);
        if (c == separator) goto ADDCOLEND;
        if (c == CarriageReturn)
        {
            // If \r=CR, we should always be able to look 1 ahead, and if char not valid should not be \n=LF
            var oneCharAhead = Add(ref charsRef, relativeIndex + 1);
            if (oneCharAhead == LineFeed)
            {
                ++rowLineEndingOffset;
            }
            goto NEWLINE;
        }
        if (c == LineFeed) { goto NEWLINE; }
    NEWLINE:
        ++lineNumber;
        ++rowLineEndingOffset;
    ADDCOLEND:
        // Pre-increment colEndsRef since [0] reserved for row start
        colEndsRef = ref Add(ref colEndsRef, 1);
        colEndsRef = charsIndex + relativeIndex;
        return ref colEndsRef;
    }
}
