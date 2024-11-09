using System;

namespace nietras.SeparatedValues;

/// <summary>Specifies the column trimming options.</summary>
[Flags]
public enum SepTrim : byte
{
    /// <summary>No trimming is applied.</summary>
    None = 0b000,
    /// <summary>Trim outermost before any unescaping.</summary>
    Outer = 0b001,
    /// <summary>Trim after any unescaping.</summary>
    AfterUnescape = 0b010,
    /// <summary>Trim all.</summary>
    All = Outer | AfterUnescape,
}
