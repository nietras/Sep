namespace nietras.SeparatedValues;

interface ISepParser
{
    int PaddingLength { get; }
    int ParseColEnds(SepReaderState s);
    int ParseColInfos(SepReaderState s);
}
