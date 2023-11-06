namespace nietras.SeparatedValues;

interface ISepParser
{
    int PaddingLength { get; }
    int Parse(SepReaderState s);
}

interface ISepParserOld
{
    int PaddingLength { get; }
    int Parse(char[] chars, int charsIndex, int charsEnd,
              int[] colEnds, ref int colEndsEnd,
              scoped ref int rowLineEndingOffset, scoped ref int lineNumber);
}
