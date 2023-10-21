using System;

namespace nietras.SeparatedValues;

sealed class SepToStringDirect : SepToString
{
    SepToStringDirect() { }
    public static SepToStringDirect Instance { get; } = new();
    public override string ToString(ReadOnlySpan<char> colSpan, int colIndex) => new(colSpan);
}
