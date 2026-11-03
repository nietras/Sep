namespace nietras.SeparatedValues;

interface ISepParser
{
    int PaddingLength { get; }
    void ParseColEnds(SepReaderState s);
    void ParseColInfos(SepReaderState s);
}
