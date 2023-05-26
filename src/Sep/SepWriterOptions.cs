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

    public Sep Sep { get; init; }
    public CultureInfo? CultureInfo { get; init; }
}
