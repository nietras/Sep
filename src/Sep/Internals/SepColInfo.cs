using System;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

static class SepColInfo
{
    // Low 24 bits: end + 1; bits 24-25: quote count capped at 3.
    // Bit 31 marks a decoded length in the array's second half.
    const int EndMask = (1 << 24) - 1;
    const int StateShift = 24;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Create(int end, int quoteCount)
    {
        A.Assert((uint)(end + 1) <= EndMask);
        A.Assert(quoteCount >= 0);
        return (end + 1) | (Math.Min(quoteCount, 3) << StateShift);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetColEnd(int info) => (info & EndMask) - 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetState(int info) => info >> StateShift;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int SetUnescaped(int info) => info | int.MinValue;
}

interface ISepColInfoMethods
{
    static abstract int Create(int end, int quoteCount);
    static abstract int GetColEnd(int info);
}

struct SepColInfoMethods : ISepColInfoMethods
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Create(int end, int quoteCount) => SepColInfo.Create(end, quoteCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetColEnd(int info) => SepColInfo.GetColEnd(info);
}

struct SepColEndMethods : ISepColInfoMethods
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Create(int end, int quoteCount) => end;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetColEnd(int info) => info;
}
