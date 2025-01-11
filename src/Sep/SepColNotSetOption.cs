namespace nietras.SeparatedValues;

/// <summary>
/// Specifies how to handle columns that are not set when writing.
/// </summary>
public enum SepColNotSetOption : byte
{
    /// <summary>
    /// Write empty column if it is not set.
    /// </summary>
    Empty,
    /// <summary>
    /// Skip column if it is not set.
    /// </summary>
    Skip,
}
