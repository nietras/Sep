using System;
using System.Collections.Generic;
using System.Threading;

namespace nietras.SeparatedValues;

public sealed class SepReaderHeader
{
    readonly string _row;
    internal readonly Dictionary<string, int> _colNameToIndex;
    string[]? _colNames;

    internal SepReaderHeader(string row, Dictionary<string, int> colNameToIndex)
    {
        _row = row;
        _colNameToIndex = colNameToIndex;
    }

    public static SepReaderHeader Empty { get; } = new(string.Empty, []);

    public bool IsEmpty => _colNameToIndex.Count == 0;

    internal int ColCount => _colNameToIndex.Count;

    public IReadOnlyList<string> ColNames => Volatile.Read(ref _colNames) ?? CreateColNames();

    public int IndexOf(string colName) => _colNameToIndex[colName];

    public bool TryIndexOf(string colName, out int colIndex) => _colNameToIndex.TryGetValue(colName, out colIndex);

#if NET9_0_OR_GREATER
    public int IndexOf(ReadOnlySpan<char> colName) =>
        _colNameToIndex.GetAlternateLookup<ReadOnlySpan<char>>()[colName];

    public bool TryIndexOf(ReadOnlySpan<char> colName, out int colIndex) =>
        _colNameToIndex.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(colName, out colIndex);
#endif

    public IReadOnlyList<string> NamesStartingWith(string prefix, StringComparison comparison = StringComparison.Ordinal)
    {
        var colNames = new List<string>();
        foreach (var (colName, _) in _colNameToIndex)
        {
            if (colName.StartsWith(prefix, comparison))
            {
                colNames.Add(colName);
            }
        }
        return colNames;
    }

    public int[] IndicesOf(IReadOnlyList<string> colNames)
    {
        ArgumentNullException.ThrowIfNull(colNames);
        var colIndices = new int[colNames.Count];
        for (var i = 0; i < colNames.Count; i++)
        {
            colIndices[i] = IndexOf(colNames[i]);
        }
        return colIndices;
    }

    public int[] IndicesOf(params string[] colNames) => IndicesOf(colNames.AsSpan());

    public int[] IndicesOf(params ReadOnlySpan<string> colNames)
    {
        var colIndices = new int[colNames.Length];
        IndicesOf(colNames, colIndices);
        return colIndices;
    }

    public void IndicesOf(ReadOnlySpan<string> colNames, Span<int> colIndices)
    {
        if (colNames.Length != colIndices.Length)
        {
            SepThrow.ArgumentException_LengthsMustBeSame(nameof(colNames), colNames.Length, nameof(colIndices), colIndices.Length);
        }
        for (var i = 0; i < colNames.Length; i++)
        {
            colIndices[i] = IndexOf(colNames[i]);
        }
    }

    public override string ToString() => _row;

    string[] CreateColNames()
    {
        var colNames = ColCount > 0 ? new string[ColCount] : [];
        var index = 0;
        foreach (var (colName, _) in _colNameToIndex)
        {
            colNames[index++] = colName;
        }
        // Headers are shared by parallel reader states.
        return Interlocked.CompareExchange(ref _colNames, colNames, null) ?? colNames;
    }
}
