using System;
using System.Collections.Generic;
using System.Numerics;

namespace nietras.SeparatedValues;

public sealed class SepReaderHeader
{
    readonly string _row;
    internal readonly string[] _colNames;
    readonly int[] _slots;
    readonly IEqualityComparer<string> _comparer;

    internal SepReaderHeader(string row, string[] colNames, IEqualityComparer<string> colNameComparer)
    {
        _row = row;
        _colNames = colNames;
        _comparer = colNameComparer;
        if (colNames.Length == 0)
        {
            _slots = [];
            return;
        }

        // Leave empty slots so unsuccessful probes always terminate.
        var capacity = ((long)colNames.Length * 3 + 1) / 2;
        var slotsLength = BitOperations.RoundUpToPowerOf2((uint)capacity);
        _slots = new int[(int)slotsLength];
        var mask = _slots.Length - 1;
        for (var colIndex = 0; colIndex < colNames.Length; ++colIndex)
        {
            var colName = colNames[colIndex];
            var slot = _comparer.GetHashCode(colName) & mask;
            while (_slots[slot] != 0)
            {
                if (_comparer.Equals(colNames[_slots[slot] - 1], colName))
                { SepThrow.ArgumentException_DuplicateColNamesFound(colNames, colName, _comparer, row); }
                slot = (slot + 1) & mask;
            }
            // Zero marks an empty slot; stored indices are offset by one.
            _slots[slot] = colIndex + 1;
        }
    }

    public static SepReaderHeader Empty { get; } = new(string.Empty, [], SepDefaults.ColNameComparer);

    public bool IsEmpty => _colNames.Length == 0;

    internal int ColCount => _colNames.Length;

    /// <summary>Column names in header order, exposed as a read-only list.</summary>
    public IReadOnlyList<string> ColNames => _colNames;

    public int IndexOf(string colName)
    {
        if (!TryIndexOf(colName, out var colIndex))
        { SepThrow.KeyNotFoundException_HeaderColNameNotFound(colName); }
        return colIndex;
    }

    public bool TryIndexOf(string colName, out int colIndex)
    {
        ArgumentNullException.ThrowIfNull(colName);
        colIndex = 0;
        if (_slots.Length == 0) { return false; }

        var mask = _slots.Length - 1;
        var slotIndex = _comparer.GetHashCode(colName) & mask;
        int colNameCandidateSlot;
        while ((colNameCandidateSlot = _slots[slotIndex]) != 0)
        {
            var candidate = colNameCandidateSlot - 1;
            var colNameCandidate = _colNames[candidate];
            if (_comparer.Equals(colNameCandidate, colName))
            {
                colIndex = candidate;
                return true;
            }
            slotIndex = (slotIndex + 1) & mask;
        }
        return false;
    }

#if NET9_0_OR_GREATER
    public int IndexOf(ReadOnlySpan<char> colName)
    {
        if (!TryIndexOf(colName, out var colIndex))
        { SepThrow.KeyNotFoundException_HeaderColNameNotFound(colName.ToString()); }
        return colIndex;
    }

    public bool TryIndexOf(ReadOnlySpan<char> colName, out int colIndex)
    {
        var comparer = _comparer as IAlternateEqualityComparer<ReadOnlySpan<char>, string>;
        if (comparer is null) { SepThrow.InvalidOperationException_AlternateKeyComparerNotSupported(); }
        colIndex = 0;
        if (_slots.Length == 0) { return false; }

        var mask = _slots.Length - 1;
        var slotIndex = comparer.GetHashCode(colName) & mask;
        int colNameCandidateSlot;
        while ((colNameCandidateSlot = _slots[slotIndex]) != 0)
        {
            var candidate = colNameCandidateSlot - 1;
            var colNameCandidate = _colNames[candidate];
            if (comparer.Equals(colName, colNameCandidate))
            {
                colIndex = candidate;
                return true;
            }
            slotIndex = (slotIndex + 1) & mask;
        }
        return false;
    }
#endif

    public IReadOnlyList<string> NamesStartingWith(string prefix, StringComparison comparison = StringComparison.Ordinal)
    {
        var colNames = new List<string>();
        foreach (var colName in _colNames)
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
        { SepThrow.ArgumentException_LengthsMustBeSame(nameof(colNames), colNames.Length, nameof(colIndices), colIndices.Length); }
        for (var i = 0; i < colNames.Length; i++)
        {
            colIndices[i] = IndexOf(colNames[i]);
        }
    }

    public override string ToString() => _row;
}
