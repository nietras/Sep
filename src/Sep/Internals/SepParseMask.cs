using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

static partial class SepParseMask
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsMask(nuint mask, int charsIndex, ref int colEndsRef)
        => ref ParseSeparatorsMask<int, SepColEndMethods>(
            mask, charsIndex, ref colEndsRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseAnyCharsMask(nuint mask, char separator,
        scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref int colEndsRef, scoped ref int lineNumber)
        => ref ParseAnyCharsMask<char, SepCharInfoUtf16, int, SepColEndMethods>(
            mask, separator, ref charsRef, charsIndex,
            ref rowLineEndingOffset, ref quoteCount,
            ref colEndsRef, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsLineEndingsMasks(nuint separatorsMask, nuint separatorsLineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex, char separator,
        ref int colEndsRefCurrent, scoped ref int lineNumber)
        => ref ParseSeparatorsLineEndingsMasks<char, SepCharInfoUtf16, int, SepColEndMethods>(
            separatorsMask, separatorsLineEndingsMask, ref charsRef, ref charsIndex, separator,
            ref colEndsRefCurrent, ref lineNumber);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TColInfo ParseSeparatorsMask<TColInfo, TColInfoMethods>(
        nuint mask, int charsIndex, ref TColInfo colInfosRef)
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        do
        {
            var relativeIndex = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);
            // Pre-increment colInfosRef since [0] reserved for row start
            colInfosRef = ref Add(ref colInfosRef, 1);
            colInfosRef = TColInfoMethods.Create(charsIndex + relativeIndex, 0);
        }
        while (mask != 0);
        return ref colInfosRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TColInfo ParseAnyCharsMask<TColInfo, TColInfoMethods>(
        nuint mask, char separator,
        scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref TColInfo colInfosRef, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
        => ref ParseAnyCharsMask<char, SepCharInfoUtf16, TColInfo, TColInfoMethods>(
            mask, separator, ref charsRef, charsIndex,
            ref rowLineEndingOffset, ref quoteCount,
            ref colInfosRef, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TColInfo ParseAnyCharsMask<TChar, TCharInfo, TColInfo, TColInfoMethods>(
        nuint mask, TChar separator,
        scoped ref TChar charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref TColInfo colInfosRef, scoped ref int lineNumber)
        where TChar : unmanaged, IEquatable<TChar>
        where TCharInfo : struct, ISepCharInfo<TChar>
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        do
        {
            var relativeIndex = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);

            colInfosRef = ref ParseAnyChar<TChar, TCharInfo, TColInfo, TColInfoMethods>(ref charsRef,
                charsIndex, relativeIndex, separator,
                ref rowLineEndingOffset, ref quoteCount,
                ref colInfosRef, ref lineNumber);
        }
        while (mask != 0 && (rowLineEndingOffset == 0));
        return ref colInfosRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TColInfo ParseAnyChar<TColInfo, TColInfoMethods>(
        scoped ref char charsRef, int charsIndex, int relativeIndex, char separator,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref TColInfo colInfosRef, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
        => ref ParseAnyChar<char, SepCharInfoUtf16, TColInfo, TColInfoMethods>(
            ref charsRef, charsIndex, relativeIndex, separator,
            ref rowLineEndingOffset, ref quoteCount,
            ref colInfosRef, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TColInfo ParseAnyChar<TChar, TCharInfo, TColInfo, TColInfoMethods>(
        scoped ref TChar charsRef, int charsIndex, int relativeIndex, TChar separator,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref TColInfo colInfosRef, scoped ref int lineNumber)
        where TChar : unmanaged, IEquatable<TChar>
        where TCharInfo : struct, ISepCharInfo<TChar>
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        var c = Add(ref charsRef, relativeIndex);
        if ((quoteCount & 1) != 0)
        {
            if (c.Equals(TCharInfo.CarriageReturn))
            {
                // If next char is a line feed, don't count it in line number,
                // but let that happen on the line feed when handled next.
                var oneCharAhead = Add(ref charsRef, relativeIndex + 1);
                if (!oneCharAhead.Equals(TCharInfo.LineFeed))
                {
                    ++lineNumber;
                }
                goto RETURN;
            }
            if (c.Equals(TCharInfo.LineFeed))
            {
                ++lineNumber;
                goto RETURN;
            }
            if (!c.Equals(TCharInfo.Quote)) goto RETURN;
        }
        if (c.Equals(separator)) goto ADDCOLEND;
        if (c.Equals(TCharInfo.CarriageReturn))
        {
            // If \r=CR, we should always be able to look 1 ahead, and if char not valid should not be \n=LF
            var oneCharAhead = Add(ref charsRef, relativeIndex + 1);
            if (oneCharAhead.Equals(TCharInfo.LineFeed))
            { ++rowLineEndingOffset; }
            goto NEWLINE;
        }
        if (c.Equals(TCharInfo.LineFeed)) { goto NEWLINE; }
        if (c.Equals(TCharInfo.Quote))
        {
            ++quoteCount;
            goto RETURN;
        }
    NEWLINE:
        ++lineNumber;
        ++rowLineEndingOffset;
    ADDCOLEND:
        // Pre-increment colInfosRef since [0] reserved for row start
        colInfosRef = ref Add(ref colInfosRef, 1);
        colInfosRef = TColInfoMethods.Create(charsIndex + relativeIndex, (int)quoteCount);
        quoteCount = 0;
    RETURN:
        return ref colInfosRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TColInfo ParseSeparatorsLineEndingsMasks<TColInfo, TColInfoMethods>(
        nuint separatorsMask, nuint separatorsLineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex, char separator,
        ref TColInfo colInfosRefCurrent, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
        => ref ParseSeparatorsLineEndingsMasks<char, SepCharInfoUtf16, TColInfo, TColInfoMethods>(
            separatorsMask, separatorsLineEndingsMask, ref charsRef, ref charsIndex, separator,
            ref colInfosRefCurrent, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TColInfo ParseSeparatorsLineEndingsMasks<TChar, TCharInfo, TColInfo, TColInfoMethods>(
        nuint separatorsMask, nuint separatorsLineEndingsMask,
        scoped ref TChar charsRef, scoped ref int charsIndex, TChar separator,
        ref TColInfo colInfosRefCurrent, scoped ref int lineNumber)
        where TChar : unmanaged, IEquatable<TChar>
        where TCharInfo : struct, ISepCharInfo<TChar>
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        var rowLineEndingOffset = 0;
        if (separatorsMask == 0)
        {
            colInfosRefCurrent = ref ParseLineEndingMask<TChar, TCharInfo, TColInfo, TColInfoMethods>(
                separatorsLineEndingsMask, ref charsRef, ref charsIndex, ref colInfosRefCurrent,
                ref rowLineEndingOffset, ref lineNumber);
            charsIndex += rowLineEndingOffset;
        }
        else
        {
            var endingsMask = separatorsLineEndingsMask & ~separatorsMask;
            var lineEndingIndex = BitOperations.TrailingZeroCount(endingsMask);
            if (((SizeOf<nuint>() * 8 - 1) - BitOperations.LeadingZeroCount(separatorsMask)) < lineEndingIndex)
            {
                colInfosRefCurrent = ref ParseSeparatorsMask<TColInfo, TColInfoMethods>(
                    separatorsMask, charsIndex, ref colInfosRefCurrent);

                var c = Add(ref charsRef, lineEndingIndex);
                ++rowLineEndingOffset;
                // Pre-increment colInfosRef since [0] reserved for row start
                colInfosRefCurrent = ref Add(ref colInfosRefCurrent, 1);
                charsIndex += lineEndingIndex;
                colInfosRefCurrent = TColInfoMethods.Create(charsIndex, 0);
                if (c.Equals(TCharInfo.CarriageReturn))
                {
                    // If \r=CR, we should always be able to look 1 ahead, and if char not valid should not be \n=LF
                    var oneCharAhead = Add(ref charsRef, lineEndingIndex + 1);
                    if (oneCharAhead.Equals(TCharInfo.LineFeed)) { ++rowLineEndingOffset; }
                }
                ++lineNumber;
                charsIndex += rowLineEndingOffset;
            }
            else
            {
                // Used both to indicate row ended and if need to step +2 due to '\r\n'
                colInfosRefCurrent = ref ParseSeparatorsLineEndingsMask<TChar, TCharInfo, TColInfo, TColInfoMethods>(
                    separatorsLineEndingsMask, separator, ref charsRef, charsIndex, ref rowLineEndingOffset,
                    ref colInfosRefCurrent, ref lineNumber);
                // We know line has ended and RowEnded set so no need to check
                // Must be a col end and last is then dataIndex, +1 to start at next
                charsIndex = TColInfoMethods.GetColEnd(colInfosRefCurrent) + rowLineEndingOffset;
            }
        }
        return ref colInfosRefCurrent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TColInfo ParseLineEndingMask<TColInfo, TColInfoMethods>(
        nuint lineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex,
        ref TColInfo colInfosRefCurrent, scoped ref int rowLineEndingOffset, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
        => ref ParseLineEndingMask<char, SepCharInfoUtf16, TColInfo, TColInfoMethods>(
            lineEndingsMask, ref charsRef, ref charsIndex,
            ref colInfosRefCurrent, ref rowLineEndingOffset, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TColInfo ParseLineEndingMask<TChar, TCharInfo, TColInfo, TColInfoMethods>(
        nuint lineEndingsMask,
        scoped ref TChar charsRef, scoped ref int charsIndex,
        ref TColInfo colInfosRefCurrent, scoped ref int rowLineEndingOffset, scoped ref int lineNumber)
        where TChar : unmanaged, IEquatable<TChar>
        where TCharInfo : struct, ISepCharInfo<TChar>
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        var lineEndingIndex = BitOperations.TrailingZeroCount(lineEndingsMask);
        var c = Add(ref charsRef, lineEndingIndex);
        ++rowLineEndingOffset;
        // Pre-increment colInfosRef since [0] reserved for row start
        colInfosRefCurrent = ref Add(ref colInfosRefCurrent, 1);
        charsIndex += lineEndingIndex;
        colInfosRefCurrent = TColInfoMethods.Create(charsIndex, 0);
        if (c.Equals(TCharInfo.CarriageReturn))
        {
            // If \r=CR, we should always be able to look 1 ahead, and if char not valid should not be \n=LF
            var oneCharAhead = Add(ref charsRef, lineEndingIndex + 1);
            if (oneCharAhead.Equals(TCharInfo.LineFeed)) { ++rowLineEndingOffset; }
        }
        ++lineNumber;
        return ref colInfosRefCurrent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ref TColInfo ParseSeparatorsLineEndingsMask<TColInfo, TColInfoMethods>(
        nuint mask, char separator,
        scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset,
        ref TColInfo colInfosRef, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
        => ref ParseSeparatorsLineEndingsMask<char, SepCharInfoUtf16, TColInfo, TColInfoMethods>(
            mask, separator, ref charsRef, charsIndex, ref rowLineEndingOffset,
            ref colInfosRef, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ref TColInfo ParseSeparatorsLineEndingsMask<TChar, TCharInfo, TColInfo, TColInfoMethods>(
        nuint mask, TChar separator,
        scoped ref TChar charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset,
        ref TColInfo colInfosRef, scoped ref int lineNumber)
        where TChar : unmanaged, IEquatable<TChar>
        where TCharInfo : struct, ISepCharInfo<TChar>
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        do
        {
            var relativeIndex = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);

            colInfosRef = ref ParseSeparatorLineEndingChar<TChar, TCharInfo, TColInfo, TColInfoMethods>(
                ref charsRef, charsIndex, relativeIndex, separator,
                ref rowLineEndingOffset, ref colInfosRef, ref lineNumber);
        }
        while (mask != 0 && (rowLineEndingOffset == 0));
        return ref colInfosRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ref TColInfo ParseSeparatorLineEndingChar<TColInfo, TColInfoMethods>(
        scoped ref char charsRef, int charsIndex, int relativeIndex, char separator,
        scoped ref int rowLineEndingOffset,
        ref TColInfo colInfosRef, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
        => ref ParseSeparatorLineEndingChar<char, SepCharInfoUtf16, TColInfo, TColInfoMethods>(
            ref charsRef, charsIndex, relativeIndex, separator,
            ref rowLineEndingOffset, ref colInfosRef, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ref TColInfo ParseSeparatorLineEndingChar<TChar, TCharInfo, TColInfo, TColInfoMethods>(
        scoped ref TChar charsRef, int charsIndex, int relativeIndex, TChar separator,
        scoped ref int rowLineEndingOffset,
        ref TColInfo colInfosRef, scoped ref int lineNumber)
        where TChar : unmanaged, IEquatable<TChar>
        where TCharInfo : struct, ISepCharInfo<TChar>
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        var c = Add(ref charsRef, relativeIndex);
        if (c.Equals(separator)) goto ADDCOLEND;
        if (c.Equals(TCharInfo.CarriageReturn))
        {
            // If \r=CR, we should always be able to look 1 ahead, and if char not valid should not be \n=LF
            var oneCharAhead = Add(ref charsRef, relativeIndex + 1);
            if (oneCharAhead.Equals(TCharInfo.LineFeed))
            {
                ++rowLineEndingOffset;
            }
            goto NEWLINE;
        }
        if (c.Equals(TCharInfo.LineFeed)) { goto NEWLINE; }
    NEWLINE:
        ++lineNumber;
        ++rowLineEndingOffset;
    ADDCOLEND:
        // Pre-increment colInfosRef since [0] reserved for row start
        colInfosRef = ref Add(ref colInfosRef, 1);
        colInfosRef = TColInfoMethods.Create(charsIndex + relativeIndex, 0);
        return ref colInfosRef;
    }
}
