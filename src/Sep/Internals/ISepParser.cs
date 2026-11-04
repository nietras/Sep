namespace nietras.SeparatedValues;

interface ISepParser
{
    int PaddingLength { get; }
    void ParseColEnds(SepReaderState s, SepParserOptions options);
    void ParseColInfos(SepReaderState s, SepParserOptions options);
}
