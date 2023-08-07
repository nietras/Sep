using System.Globalization;

namespace nietras.SeparatedValues;

public readonly record struct SepWriterOptions
{
    public SepWriterOptions() : this(Sep.Default) { }

    public SepWriterOptions(Sep sep)
    {
        Sep = sep;
        CultureInfo = SepDefaults.CultureInfo;
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
}
