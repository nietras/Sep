namespace nietras.SeparatedValues;

interface ISepParser
{
    int PaddingLength { get; }
    int QuoteCount { get; }
    void ParseColEnds(SepReaderState s);
    void ParseColInfos(SepReaderState s);
}
