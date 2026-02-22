namespace nietras.SeparatedValues;

interface ISepUtf8Parser
{
    int PaddingLength { get; }
    int QuoteCount { get; }
    void ParseColEnds(SepReaderStateBase<byte, SepCharInfoUtf8> s);
    void ParseColInfos(SepReaderStateBase<byte, SepCharInfoUtf8> s);
}
