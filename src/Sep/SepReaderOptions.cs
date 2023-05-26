using System.Globalization;

namespace nietras.SeparatedValues;

public readonly record struct SepReaderOptions
{
    public SepReaderOptions() : this(null) { }

    public SepReaderOptions(Sep? sep)
    {
        Sep = sep;
        CultureInfo = SepDefaults.CultureInfo;
        HasHeader = true;
        CreateToString = SepToString.Direct;
        UseFastFloat = true;
    }

    public Sep? Sep { get; init; }
    public CultureInfo? CultureInfo { get; init; }
    public bool HasHeader { get; init; }
    public SepCreateToString CreateToString { get; init; }
    public bool UseFastFloat { get; init; }
}
