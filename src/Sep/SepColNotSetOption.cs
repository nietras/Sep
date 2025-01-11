namespace nietras.SeparatedValues;

/// <summary>
/// Specifies how to handle columns that are not set when writing.
/// </summary>
public enum SepColNotSetOption : byte
{
    /// <summary>
    /// Throw exception if a column is not set.
    /// </summary>
    Throw,
    /// <summary>
    /// Write empty column if it is not set.
    /// </summary>
    Empty,
    /// <summary>
    /// Skip column if it is not set.
    /// </summary>
    Skip,
}
