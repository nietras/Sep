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
        DisableFastFloat = false;
        DisableColCountCheck = false;
    }

    public Sep? Sep { get; init; }
    public CultureInfo? CultureInfo { get; init; }
    public bool HasHeader { get; init; }
    public SepCreateToString CreateToString { get; init; }
    public bool DisableFastFloat { get; init; }
    /// <summary>
    /// Disable checking if column count is the same for all rows.
    /// </summary>
    public bool DisableColCountCheck { get; init; }
}
