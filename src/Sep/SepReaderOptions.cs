﻿using System.Collections.Generic;
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
        Unescape = false;
        Trim = SepTrim.None;
    }

    /// <summary>
    /// Specifies the separator used, if `null` then automatic detection 
    /// is used based on first row in source.
    /// </summary>
    public Sep? Sep { get; init; } = null;
    /// <summary>
    /// Specifies initial internal `char` buffer length.
    /// </summary>
    /// <remarks>
    /// The length will likely be rounded up to the nearest power of 2. A
    /// smaller buffer may end up being used if the underlying source for <see
    /// cref="System.IO.TextReader"/> is known to be smaller. Prefer to keep the
    /// default length as that has been tuned for performance and cache sizes.
    /// Avoid making this unnecessarily large as that will likely not improve
    /// performance and may waste memory.
    /// </remarks>
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
    /// <summary>
    /// Option for trimming spaces (` ` - ASCII 32) on column access.
    /// </summary>
    /// <remarks>
    /// By default no trimming is done. See <see cref="SepTrim"/> for options.
    /// Note that trimming may happen in-place e.g. if also unescaping, which
    /// means the <see cref="SepReader.Row.Span" /> will be modified and contain
    /// "garbage" state for trimmed/unescaped cols. This is for efficiency to
    /// avoid allocating secondary memory for trimmed/unescaped columns. Header
    /// columns/names will also be trimmed. Note that only the space ` ` (ASCII
    /// 32) character is trimmed, not any whitespace character.
    /// </remarks>
    public SepTrim Trim { get; init; } = SepTrim.None;
    /// <summary>
    /// Forwarded to <see
    /// cref="System.Threading.Tasks.ValueTask.ConfigureAwait(bool)"/> or
    /// similar when async methods are called.
    /// </summary>
    public bool AsyncContinueOnCapturedContext { get; init; } = false;
}
