using System.Globalization;

namespace nietras.SeparatedValues;

public readonly record struct SepWriterOptions
{
    public SepWriterOptions() : this(Sep.Default) { }

    public SepWriterOptions(Sep sep)
    {
        Sep = sep;
        CultureInfo = SepDefaults.CultureInfo;
        WriteHeader = true;
        Escape = false;
    }

    /// <summary>
    /// Specifies the separator used.
    /// </summary>
    public Sep Sep { get; init; }
    /// <summary>
    /// Specifies the culture used for parsing. 
    /// May be `null` for default culture.
    /// </summary>
    public CultureInfo? CultureInfo { get; init; }
    /// <summary>
    /// Specifies whether to write a header row 
    /// before data rows. Requires all columns 
    /// to have a name. Otherwise, columns can be
    /// added by indexing alone.
    /// </summary>
    public bool WriteHeader { get; init; } = true;
    /// <summary>
    /// Specifies whether to escape column values
    /// when writing. Default is false.
    /// </summary>
    public bool Escape { get; init; } = false;
}
