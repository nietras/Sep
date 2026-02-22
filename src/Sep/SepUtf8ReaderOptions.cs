using System.Collections.Generic;
using System.Globalization;

namespace nietras.SeparatedValues;

public readonly record struct SepUtf8ReaderOptions
{
    public SepUtf8ReaderOptions() : this(null) { }

    public SepUtf8ReaderOptions(Sep? sep)
    {
        Sep = sep;
        CultureInfo = SepDefaults.CultureInfo;
        HasHeader = true;
        ColNameComparer = SepDefaults.ColNameComparer;
        CreateToString = SepToString.Direct;
        CreateToBytes = SepToBytes.Direct;
        DisableFastFloat = false;
        DisableColCountCheck = false;
        Unescape = false;
        Trim = SepTrim.None;
    }

    /// <summary>
    /// Specifies the separator used, if `null` then automatic detection 
    /// is used based on first row in source.
    /// </summary>
    public Sep? Sep { get; init; } = null;
    /// <summary>
    /// Specifies initial internal `byte` buffer length.
    /// </summary>
    public int InitialBufferLength { get; init; } = SepDefaults.InitialBufferLength;
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
    /// of `byte`s to a `string` (decoding from UTF-8).
    /// </summary>
    public SepCreateToString CreateToString { get; init; } = SepToString.Direct;
    /// <summary>
    /// Specifies the method factory used to convert a column span 
    /// of `byte`s to a <see cref="System.ReadOnlyMemory{T}"/> of `byte`.
    /// </summary>
    public SepCreateToBytes CreateToBytes { get; init; } = SepToBytes.Direct;
    /// <summary>
    /// Disables using csFastFloat for parsing `float` and `double`.
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
    public bool Unescape { get; init; } = false;
    /// <summary>
    /// Option for trimming spaces on column access.
    /// </summary>
    public SepTrim Trim { get; init; } = SepTrim.None;
    /// <summary>
    /// Whether to skip a UTF-8 BOM (byte order mark) at the start of input.
    /// Defaults to true.
    /// </summary>
    public bool SkipBom { get; init; } = true;
}
