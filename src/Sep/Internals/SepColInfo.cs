using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace nietras.SeparatedValues;

interface ISepColInfoMethods
{
    static abstract void Write(ref int colEnd, int end, int quoteCount, int colInfosOffset);
    static abstract void Clear(ref int colEnd, int count, int colInfosOffset);
}

struct SepColInfoMethods : ISepColInfoMethods
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(ref int colEnd, int end, int quoteCount, int colInfosOffset)
    {
        colEnd = end;
        Add(ref colEnd, colInfosOffset) = quoteCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear(ref int colEnd, int count, int colInfosOffset) =>
        MemoryMarshal.CreateSpan(ref Add(ref colEnd, colInfosOffset + 1), count).Clear();
}

struct SepColEndMethods : ISepColInfoMethods
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(ref int colEnd, int end, int quoteCount, int colInfosOffset) => colEnd = end;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear(ref int colEnd, int count, int colInfosOffset) { }
}
