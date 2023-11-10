namespace nietras.SeparatedValues;

record struct SepColInfo(int ColEnd, int QuoteCount);

interface ISepColInfoCreate<TColInfo>
{
    TColInfo Create(int colEnd, int quoteCount);
}

readonly struct SepColInfoCreate : ISepColInfoCreate<SepColInfo>
{
    public readonly SepColInfo Create(int colEnd, int quoteCount) => new(colEnd, quoteCount);
}

readonly struct SepColEndCreate : ISepColInfoCreate<int>
{
    public readonly int Create(int colEnd, int quoteCount) => colEnd;
}
