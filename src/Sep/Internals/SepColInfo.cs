using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

record struct SepColInfo(int ColEnd, int QuoteCount)
{
    public override string ToString() => $"({ColEnd}, {QuoteCount})";
}

interface ISepColInfoMethods<TColInfo>
{
    static abstract TColInfo Create(int colEnd, int quoteCount);
    static abstract int GetColEnd(TColInfo colInfo);
    static abstract int CountOffset(ref TColInfo origin, ref TColInfo target);
}

abstract class SepColInfoMethods : ISepColInfoMethods<SepColInfo>
{
    public static SepColInfo Create(int colEnd, int quoteCount) => new(colEnd, quoteCount);
    public static int GetColEnd(SepColInfo colInfo) => colInfo.ColEnd;
    public static int CountOffset(ref SepColInfo origin, ref SepColInfo target) =>
        // ">> 3" instead of "/ Unsafe.SizeOf<SepColInfo>()" // CQ: Weird with div sizeof
        (int)Unsafe.ByteOffset(ref origin, ref target) >> 3;
}

abstract class SepColEndMethods : ISepColInfoMethods<int>
{
    public static int Create(int colEnd, int quoteCount) => colEnd;
    public static int GetColEnd(int colEnd) => colEnd;
    public static int CountOffset(ref int origin, ref int target) =>
        // ">> 2" instead of "/ sizeof(int)" // CQ: Weird with div sizeof
        (int)Unsafe.ByteOffset(ref origin, ref target) >> 2;
}
