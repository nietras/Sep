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
        DisableColCountCheck = false;
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
    /// Disables checking if column count is the same for all rows.
    /// </summary>
    /// <remarks>
    /// When true, any row written will contain only columns that have been set
    /// for that row regardless of header. If any columns are missing, then
    /// columns of a row may, therefore, be out of sync with column names. That
    /// is, there may be gaps. When header is written it is not possible to
    /// write more columns than in the header column. If a header is not
    /// written, then any number of columns can be written as long as done
    /// sequentially.
    /// </remarks>
    public bool DisableColCountCheck { get; init; } = false;
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
}
