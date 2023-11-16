namespace nietras.SeparatedValues;

record struct SepColInfo(int ColEnd, int QuoteCount)
{
    public override string ToString() => $"({ColEnd}, {QuoteCount})";
}

interface ISepColInfoMethods<TColInfo>
{
    static abstract TColInfo Create(int colEnd, int quoteCount);
    static abstract int GetColEnd(TColInfo colInfo);
}

abstract class SepColInfoMethods : ISepColInfoMethods<SepColInfo>
{
    public static SepColInfo Create(int colEnd, int quoteCount) => new(colEnd, quoteCount);
    public static int GetColEnd(SepColInfo colInfo) => colInfo.ColEnd;
}

abstract class SepColEndMethods : ISepColInfoMethods<int>
{
    public static int Create(int colEnd, int quoteCount) => colEnd;
    public static int GetColEnd(int colEnd) => colEnd;
}
