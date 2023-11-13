using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

static partial class SepParseMask
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsMask(nuint mask, int charsIndex, ref int colEndsRef)
        => ref SepParseMask.ParseSeparatorsMask<int, SepColEndMethods>(
            mask, charsIndex, ref colEndsRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseAnyCharsMask(nuint mask, char separator,
        scoped ref char charsRef, int charsIndex,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref int colEndsRef, scoped ref int lineNumber)
        => ref SepParseMask.ParseAnyCharsMask<int, SepColEndMethods>(
            mask, separator, ref charsRef, charsIndex,
            ref rowLineEndingOffset, ref quoteCount,
            ref colEndsRef, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseAnyChar(
        scoped ref char charsRef, int charsIndex, int relativeIndex, char separator,
        scoped ref int rowLineEndingOffset, scoped ref nuint quoteCount,
        ref int colEndsRef, scoped ref int lineNumber)
        => ref SepParseMask.ParseAnyChar<int, SepColEndMethods>(
            ref charsRef, charsIndex, relativeIndex, separator,
            ref rowLineEndingOffset, ref quoteCount,
            ref colEndsRef, ref lineNumber);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseSeparatorsLineEndingsMasks(nuint separatorsMask, nuint separatorsLineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex, char separator,
        ref int colEndsRefCurrent, scoped ref int rowLineEndingOffset, scoped ref int lineNumber)
        => ref SepParseMask.ParseSeparatorsLineEndingsMasks<int, SepColEndMethods>(
            separatorsMask, separatorsLineEndingsMask, ref charsRef, ref charsIndex, separator,
            ref colEndsRefCurrent, ref rowLineEndingOffset, ref lineNumber);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int ParseLineEndingMask(nuint lineEndingsMask,
        scoped ref char charsRef, scoped ref int charsIndex,
        ref int colEndsRefCurrent, scoped ref int rowLineEndingOffset, scoped ref int lineNumber)
        => ref SepParseMask.ParseLineEndingMask<int, SepColEndMethods>(
            lineEndingsMask, ref charsRef, ref charsIndex,
            ref colEndsRefCurrent, ref rowLineEndingOffset, ref lineNumber);
}
