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

    /// <summary>
    /// Specifies the separator used, if `null` then automatic detection is used.
    /// </summary>
    public Sep? Sep { get; init; }
    /// <summary>
    /// Specifies the culture used for parsing.
    /// </summary>
    public CultureInfo? CultureInfo { get; init; }
    /// <summary>
    /// Indicates whether the first row is a header row.
    /// </summary>
    public bool HasHeader { get; init; }
    /// <summary>
    /// Specifies the method used to convert a column span of `char`s to a `string`.
    /// </summary>
    public SepCreateToString CreateToString { get; init; }
    /// <summary>
    /// Disables using [csFastFloat](https://github.com/CarlVerret/csFastFloat) for parsing `float` and `double`.
    /// </summary>
    public bool DisableFastFloat { get; init; }
    /// <summary>
    /// Disables checking if column count is the same for all rows.
    /// </summary>
    public bool DisableColCountCheck { get; init; }
}
