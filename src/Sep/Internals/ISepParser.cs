namespace nietras.SeparatedValues;

interface ISepParser
{
    int PaddingLength { get; }
    int QuoteCount { get; }
    int ParseColEnds(SepReaderState s);
    int ParseColInfos(SepReaderState s);
}
