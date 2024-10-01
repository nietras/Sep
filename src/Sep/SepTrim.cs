using System;

namespace nietras.SeparatedValues;

[Flags]
public enum SepTrim : byte
{
    None = 0b000,
    Outer = 0b001,
    AfterUnescape = 0b010,
    All = Outer | AfterUnescape,
}
