namespace nietras.SeparatedValues;

interface ISepParser
{
    int PaddingLength { get; }
    int Parse(SepReaderState s);
}
