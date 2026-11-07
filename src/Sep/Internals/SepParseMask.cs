using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.CompilerServices.Unsafe;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

static partial class SepParseMask
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsMask(nuint mask, int charsIndex, ref int colEndsRef)
        => ref ParseSeparatorsMask<SepColEndMethods>(
            mask, charsIndex, ref colEndsRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseAnyCharsMask(nuint mask, char separator,
        scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref int colEndsRef, scoped ref int lineNumber)
        => ref ParseAnyCharsMask<SepColEndMethods>(
            mask, separator, ref charsRef, charsIndex,
            ref rowLineEndingOffset, ref quoteCount,
            ref colEndsRef, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsLineEndingsMasks(nuint separatorsMask, nuint separatorsLineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex, char separator,
        ref int colEndsRefCurrent, scoped ref int lineNumber)
        => ref ParseSeparatorsLineEndingsMasks<SepColEndMethods>(
            separatorsMask, separatorsLineEndingsMask, ref charsRef, ref charsIndex, separator,
            ref colEndsRefCurrent, ref lineNumber);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsMask<TColInfoMethods>(
        nuint mask, int charsIndex, ref int colInfosRef)
        where TColInfoMethods : ISepColInfoMethods
    {
#if NET10_0_OR_GREATER
        // Avoid 512-bit setup for a single separator or a mask confined to 16 positions.
        if ((typeof(TColInfoMethods) == typeof(SepColEndMethods) ||
             typeof(TColInfoMethods) == typeof(SepColInfoMethods)) &&
            Avx512Vbmi2.IsSupported && Avx512Vbmi.IsSupported &&
            mask > ushort.MaxValue && (mask & (mask - 1)) != 0)
        {
            var count = BitOperations.PopCount(mask);
            return ref ParseSeparatorsMaskVbmi2(mask, count,
                charsIndex + (typeof(TColInfoMethods) == typeof(SepColInfoMethods) ? 1 : 0),
                ref colInfosRef);
        }
#endif
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

#if NET10_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe ref int ParseSeparatorsMaskVbmi2(nuint mask, int count, int charsIndex, ref int colEndsRef)
    {
        // Broadcast each mask byte to its eight position lanes, then test each lane's bit.
        var byteIndices = Vector512.Create(0x0000000000000000UL, 0x0101010101010101UL,
            0x0202020202020202UL, 0x0303030303030303UL, 0x0404040404040404UL,
            0x0505050505050505UL, 0x0606060606060606UL, 0x0707070707070707UL).AsByte();
        var bits = Vector512.Shuffle(Vector512.Create((ulong)mask).AsByte(), byteIndices);
        var selected = ~Vector512.Equals(bits & Vector512.Create(0x8040201008040201UL).AsByte(), Vector512<byte>.Zero);
        var packed = Avx512Vbmi2.Compress(Vector512<byte>.Zero, selected, Vector512<byte>.Indices);
        var origin = Vector512.Create(charsIndex);
        fixed (int* destination = &Add(ref colEndsRef, 1))
        {
            StoreSeparatorPositions(destination, packed.GetLower().GetLower(), origin, count);
            if (count > 16)
            {
                StoreSeparatorPositions(destination + 16, Avx512F.ExtractVector128(packed, 1), origin, count - 16);
                if (count > 32)
                {
                    StoreSeparatorPositions(destination + 32, Avx512F.ExtractVector128(packed, 2), origin, count - 32);
                    if (count > 48)
                    {
                        StoreSeparatorPositions(destination + 48, Avx512F.ExtractVector128(packed, 3), origin, count - 48);
                    }
                }
            }
        }
        return ref Add(ref colEndsRef, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe void StoreSeparatorPositions(int* destination, Vector128<byte> positions, Vector512<int> origin, int count)
    {
        var entries = Avx512F.ConvertToVector512Int32(positions) + origin;
        var valid = Vector512.LessThan(Vector512<int>.Indices, Vector512.Create(count));
        Avx512F.MaskStore(destination, valid, entries);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static nuint PrefixXor(nuint mask)
    {
        if (Pclmulqdq.IsSupported)
        {
            return (nuint)Pclmulqdq.CarrylessMultiply(
                Vector128.CreateScalar((ulong)mask), Vector128.CreateScalar(ulong.MaxValue), 0).ToScalar();
        }
        mask ^= mask << 1;
        mask ^= mask << 2;
        mask ^= mask << 4;
        mask ^= mask << 8;
        mask ^= mask << 16;
        if (SizeOf<nuint>() == sizeof(ulong)) { mask ^= mask << 32; }
        return mask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseQuotedMask<TColInfoMethods>(
        nuint separatorsMask, nuint separatorLineEndingsMask, nuint specialCharMask,
        char separator, scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref int colInfosRef, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods
    {
        var quotesMask = specialCharMask ^ separatorLineEndingsMask;
        // Inclusive quote parity masks separators inside quotes, including a carried opening quote.
        var quotedMask = PrefixXor(quotesMask) ^ (0 - (quoteCount & 1));
        var lineEndingsMask = separatorLineEndingsMask ^ separatorsMask;
        if ((lineEndingsMask & quotedMask) != 0)
        {
            // Keep the scalar line counting for embedded CR, LF and CRLF, including block boundaries.
            return ref ParseAnyCharsMask<TColInfoMethods>(specialCharMask,
                separator, ref charsRef, charsIndex,
                ref rowLineEndingOffset, ref quoteCount,
                ref colInfosRef, ref lineNumber);
        }

        var mask = separatorLineEndingsMask & ~quotedMask;
        while (mask != 0)
        {
            var relativeIndex = BitOperations.TrailingZeroCount(mask);
            var bit = (nuint)1 << relativeIndex;
            var before = bit - 1;
            quoteCount += (nuint)BitOperations.PopCount(quotesMask & before);
            quotesMask &= ~before;
            colInfosRef = ref Add(ref colInfosRef, 1);
            colInfosRef = TColInfoMethods.Create(charsIndex + relativeIndex, (int)quoteCount);
            quoteCount = 0;

            if ((lineEndingsMask & bit) != 0)
            {
                rowLineEndingOffset = 1;
                if (Add(ref charsRef, relativeIndex) == CarriageReturn &&
                    Add(ref charsRef, relativeIndex + 1) == LineFeed)
                {
                    ++rowLineEndingOffset;
                }
                ++lineNumber;
                return ref colInfosRef;
            }
            mask &= mask - 1;
        }
        quoteCount += (nuint)BitOperations.PopCount(quotesMask);
        return ref colInfosRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseAnyCharsMask<TColInfoMethods>(
        nuint mask, char separator,
        scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref int colInfosRef, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods
    {
        do
        {
            var relativeIndex = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);

            colInfosRef = ref ParseAnyChar<TColInfoMethods>(ref charsRef,
                charsIndex, relativeIndex, separator,
                ref rowLineEndingOffset, ref quoteCount,
                ref colInfosRef, ref lineNumber);
        }
        while (mask != 0 && (rowLineEndingOffset == 0));
        return ref colInfosRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseAnyChar<TColInfoMethods>(
        scoped ref char charsRef, int charsIndex, int relativeIndex, char separator,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref int colInfosRef, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods
    {
        var c = Add(ref charsRef, relativeIndex);
        if ((quoteCount & 1) != 0)
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
    internal static ref int ParseSeparatorsLineEndingsMasks<TColInfoMethods>(
        nuint separatorsMask, nuint separatorsLineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex, char separator,
        ref int colInfosRefCurrent, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods
    {
        var rowLineEndingOffset = 0;
        if (separatorsMask == 0)
        {
            colInfosRefCurrent = ref ParseLineEndingMask<TColInfoMethods>(
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
                colInfosRefCurrent = ref ParseSeparatorsMask<TColInfoMethods>(
                    separatorsMask, charsIndex, ref colInfosRefCurrent);

                var c = Add(ref charsRef, lineEndingIndex);
                ++rowLineEndingOffset;
                // Pre-increment colInfosRef since [0] reserved for row start
                colInfosRefCurrent = ref Add(ref colInfosRefCurrent, 1);
                charsIndex += lineEndingIndex;
                colInfosRefCurrent = TColInfoMethods.Create(charsIndex, 0);
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
                colInfosRefCurrent = ref ParseSeparatorsLineEndingsMask<TColInfoMethods>(
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
    internal static ref int ParseLineEndingMask<TColInfoMethods>(
        nuint lineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex,
        ref int colInfosRefCurrent, scoped ref int rowLineEndingOffset, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods
    {
        var lineEndingIndex = BitOperations.TrailingZeroCount(lineEndingsMask);
        var c = Add(ref charsRef, lineEndingIndex);
        ++rowLineEndingOffset;
        // Pre-increment colInfosRef since [0] reserved for row start
        colInfosRefCurrent = ref Add(ref colInfosRefCurrent, 1);
        charsIndex += lineEndingIndex;
        colInfosRefCurrent = TColInfoMethods.Create(charsIndex, 0);
        if (c == CarriageReturn)
        {
            // If \r=CR, we should always be able to look 1 ahead, and if char not valid should not be \n=LF
            var oneCharAhead = Add(ref charsRef, lineEndingIndex + 1);
            if (oneCharAhead == LineFeed) { ++rowLineEndingOffset; }
        }
        ++lineNumber;
        return ref colInfosRefCurrent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ref int ParseSeparatorsLineEndingsMask<TColInfoMethods>(
        nuint mask, char separator,
        scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset,
        ref int colInfosRef, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods
    {
        do
        {
            var relativeIndex = BitOperations.TrailingZeroCount(mask);
            mask &= (mask - 1);

            colInfosRef = ref ParseSeparatorLineEndingChar<TColInfoMethods>(
                ref charsRef, charsIndex, relativeIndex, separator,
                ref rowLineEndingOffset, ref colInfosRef, ref lineNumber);
        }
        while (mask != 0 && (rowLineEndingOffset == 0));
        return ref colInfosRef;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ref int ParseSeparatorLineEndingChar<TColInfoMethods>(
        scoped ref char charsRef, int charsIndex, int relativeIndex, char separator,
        scoped ref int rowLineEndingOffset,
        ref int colInfosRef, scoped ref int lineNumber)
        where TColInfoMethods : ISepColInfoMethods
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
        // Pre-increment colInfosRef since [0] reserved for row start
        colInfosRef = ref Add(ref colInfosRef, 1);
        colInfosRef = TColInfoMethods.Create(charsIndex + relativeIndex, 0);
        return ref colInfosRef;
    }
}
