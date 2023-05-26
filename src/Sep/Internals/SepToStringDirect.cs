using System;

namespace nietras.SeparatedValues;

sealed class SepToStringDirect : SepToString
{
    public static SepToStringDirect Instance { get; } = new();
    public override string ToString(ReadOnlySpan<char> chars) => new(chars);
}
