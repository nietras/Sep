using System.Globalization;

namespace nietras.SeparatedValues;

public readonly record struct SepUtf8WriterOptions
{
    public SepUtf8WriterOptions() : this(Sep.Default) { }

    public SepUtf8WriterOptions(Sep sep)
    {
        Sep = sep;
        CultureInfo = SepDefaults.CultureInfo;
        WriteHeader = true;
        DisableColCountCheck = false;
        Escape = false;
    }

    /// <summary>
    /// Specifies the separator used.
    /// </summary>
    public Sep Sep { get; init; }
    /// <summary>
    /// Specifies the culture used for formatting. 
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
    /// Disables checking if column count is the 
    /// same for all rows.
    /// </summary>
    /// <remarks>
    /// When true, the <see cref="ColNotSetOption"/>
    /// will define how columns that are not set
    /// are handled. For example, whether to skip
    /// or write an empty column if a column has
    /// not been set for a given row.
    /// <para>
    /// If any columns are skipped, then columns of
    /// a row may, therefore, be out of sync with
    /// column names if <see cref="WriteHeader"/>
    /// is true.
    /// </para>
    /// As such, any number of columns can be
    /// written as long as done sequentially.
    /// </remarks>
    public bool DisableColCountCheck { get; init; } = false;
    /// <summary>
    /// Specifies how to handle columns that are 
    /// not set.
    /// </summary>
    public SepColNotSetOption ColNotSetOption { get; init; } = SepColNotSetOption.Throw;
    /// <summary>
    /// Specifies whether to escape column names 
    /// and values when writing.
    /// </summary>
    /// <remarks>
    /// When true, if a column contains a separator 
    /// (e.g. `;`), carriage return (`\r`), line 
    /// feed (`\n` or quote (`"`) then the column 
    /// is prefixed and suffixed with quotes `"` 
    /// and any quote in the column is escaped by
    /// adding an extra quote so it becomes `""`.
    /// Note that escape applies to column names 
    /// too, but only the written name.
    /// </remarks>
    public bool Escape { get; init; } = false;
    /// <summary>
    /// Forwarded to <see
    /// cref="System.Threading.Tasks.ValueTask.ConfigureAwait(bool)"/> or
    /// similar when async methods are called.
    /// </summary>
    public bool AsyncContinueOnCapturedContext { get; init; } = false;
}
