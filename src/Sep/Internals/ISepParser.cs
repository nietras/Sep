namespace nietras.SeparatedValues;

interface ISepParserOld
{
    int PaddingLength { get; }
    int Parse(char[] chars, int charsIndex, int charsEnd,
              int[] colEnds, ref int colEndsEnd,
              scoped ref int rowLineEndingOffset, scoped ref int lineNumber);
}
