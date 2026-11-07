using System;

namespace nietras.SeparatedValues;

static class SepColInfo
{
    // Bits 0-23: end + 1; bits 24-25: quote count capped at 3.
    // Bit 31 indicates that the unescaped length is cached.
    // Reader buffers are limited to RowLengthMax and reserve padding and lookahead,
    // so the end plus one fits in 24 bits.
    const int EndMask = (1 << 24) - 1;
    const int StateShift = 24;
    internal const int NoQuotes = 0;
    internal const int OneQuote = 1;
    internal const int TwoQuotes = 2;
    internal const int ManyQuotes = 3;

    // The end is stored plus one so that -1 is representable without a branch.
    internal static int Create(int colEnd, int quoteCount)
    {
        A.Assert((uint)(colEnd + 1) <= EndMask);
        A.Assert(quoteCount >= 0);
        return (colEnd + 1) | (Math.Min(quoteCount, ManyQuotes) << StateShift);
    }
    internal static int GetColEnd(int info) => (info & EndMask) - 1;
    internal static int GetState(int info) => info >> StateShift;
    internal static int SetUnescaped(int info) => (info & EndMask) | int.MinValue;
}

interface ISepColInfoMethods<TColInfo>
{
    static abstract TColInfo Create(int colEnd, int quoteCount);
    static abstract int GetColEnd(TColInfo colInfo);
}

struct SepColInfoMethods : ISepColInfoMethods<int>
{
    public static int Create(int colEnd, int quoteCount) => SepColInfo.Create(colEnd, quoteCount);
    public static int GetColEnd(int colInfo) => SepColInfo.GetColEnd(colInfo);
}

struct SepColEndMethods : ISepColInfoMethods<int>
{
    public static int Create(int colEnd, int quoteCount) => colEnd;
    public static int GetColEnd(int colEnd) => colEnd;
}
