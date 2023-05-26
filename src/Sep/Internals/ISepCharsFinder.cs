namespace nietras.SeparatedValues;

public interface ISepCharsFinder
{
    int PaddingLength { get; }
    int RequestedPositionsFreeLength { get; }
    int Find(char[] _chars, int charsStart, int charsEnd,
             Pos[] positions, int positionsStart, ref int positionsEnd);
}
