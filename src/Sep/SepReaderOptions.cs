using System.Collections.Generic;
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
        ColNameComparer = SepDefaults.ColNameComparer;
        CreateToString = SepToString.Direct;
        DisableFastFloat = false;
        DisableColCountCheck = false;
    }

    /// <summary>
    /// Specifies the separator used, if `null` then automatic detection 
    /// is used based on first row in source.
    /// </summary>
    public Sep? Sep { get; init; } = null;
    /// <summary>
    /// Specifies the culture used for parsing. 
    /// May be `null` for default culture.
    /// </summary>
    public CultureInfo? CultureInfo { get; init; } = SepDefaults.CultureInfo;
    /// <summary>
    /// Indicates whether the first row is a header row.
    /// </summary>
    public bool HasHeader { get; init; } = true;
    /// <summary>
    /// Specifies <see cref="IEqualityComparer{T}" /> to use 
    /// for comparing header column names and looking up index.
    /// </summary>
    public IEqualityComparer<string> ColNameComparer { get; init; } = SepDefaults.ColNameComparer;
    /// <summary>
    /// Specifies the method factory used to convert a column span 
    /// of `char`s to a `string`.
    /// </summary>
    public SepCreateToString CreateToString { get; init; } = SepToString.Direct;
    /// <summary>
    /// Disables using [csFastFloat](https://github.com/CarlVerret/csFastFloat)
    /// for parsing `float` and `double`.
    /// </summary>
    public bool DisableFastFloat { get; init; } = false;
    /// <summary>
    /// Disables checking if column count is the same for all rows.
    /// </summary>
    public bool DisableColCountCheck { get; init; } = false;
    /// <summary>
    /// Disables detecting and parsing quotes.
    /// </summary>
    public bool DisableQuotesParsing { get; init; } = false;
    /// <summary>
    /// Unescape quotes on column access.
    /// </summary>
    /// <remarks>
    /// When true, if a column starts with a quote then the two outermost quotes
    /// are removed and every second inner quote is removed. Note that
    /// unquote/unescape happens in-place, which means the <see
    /// cref="SepReader.Row.Span" /> will be modified and contain "garbage"
    /// state after unescaped cols before next col. This is for efficiency to
    /// avoid allocating secondary memory for unescaped columns. Header
    /// columns/names will also be unescaped.
    /// Requires <see cref="DisableQuotesParsing"/> to be false.
    /// </remarks>
    public bool Unescape { get; init; } = false;
}
