using System;

namespace nietras.SeparatedValues;

// Cannot be nested due to CS0146: Circular base type dependency
public class SepReaderRowState
{
    protected internal char[] _chars = Array.Empty<char>();
    protected internal int _charsDataStart = 0;
    protected internal int _charsDataEnd = 0;
    protected internal int _charsParseStart = 0;
    protected internal int _charsRowStart = 0;

    protected internal const int _colEndsMaximumLength = 1024;
    // [0] = Previous row/col end e.g. one before row/first col start
    // [1...] = Col ends e.g. [1] = first col end
    // Length = colCount + 1
    protected internal int[] _colEnds = Array.Empty<int>();
    protected internal int _colCountExpected = -1;
    protected internal int _colCount = 0;
}
