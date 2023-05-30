using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nietras.SeparatedValues;

public partial class SepReader
{
    // Problem here is Row is a ref struct so can't use Action<Row>
    public delegate void RowAction(Row row);
    public delegate T RowFunc<T>(Row row);

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly ref struct Row
    {
        internal readonly SepReader _reader;

        internal Row(SepReader reader) => _reader = reader;

        public int RowIndex => _reader._rowIndex;

        public int ColCount => _reader._colCount;

        public ReadOnlySpan<char> Span => _reader.RowSpan();

        public override string ToString() => new(Span);

        public Col this[int index] => new(_reader, index);

        public Col this[Index index] => new(_reader, index.GetOffset(_reader._colCount));

        public Col this[string colName]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var colIndex = _reader.GetCachedColIndex(colName);
                return new(_reader, colIndex);
            }
        }

        public Cols this[Range range]
        {
            get
            {
                var (offset, length) = range.GetOffsetAndLength(_reader._colCount);
                var colIndices = _reader._arrayPool.RentUniqueArrayAsSpan<int>(length);
                for (var i = 0; i < colIndices.Length; i++)
                {
                    colIndices[i] = i + offset;
                }
                return new(_reader, colIndices);
            }
        }

        public Cols this[ReadOnlySpan<int> indices] => new(_reader, indices);

        public Cols this[IReadOnlyList<int> indices]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(indices);
                var colIndices = _reader._arrayPool.RentUniqueArrayAsSpan<int>(indices.Count);
                for (var i = 0; i < indices.Count; i++)
                {
                    colIndices[i] = indices[i];
                }
                return new(_reader, colIndices);
            }
        }

        // Overload needed since otherwise ambiguous call for array between
        // ReadOnlySpan<> and IReadOnlyList<>
        public Cols this[int[] indices] => new(_reader, indices);

        public Cols this[ReadOnlySpan<string> colNames]
        {
            get
            {
                var colIndices = _reader._arrayPool.RentUniqueArrayAsSpan<int>(colNames.Length);
                for (var i = 0; i < colNames.Length; i++)
                {
                    var name = colNames[i];
                    colIndices[i] = _reader.GetCachedColIndex(name);
                }
                return new(_reader, colIndices);
            }
        }

        public Cols this[IReadOnlyList<string> colNames]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(colNames);
                var colIndices = _reader._arrayPool.RentUniqueArrayAsSpan<int>(colNames.Count);
                for (var i = 0; i < colNames.Count; i++)
                {
                    var name = colNames[i];
                    colIndices[i] = _reader.GetCachedColIndex(name);
                }
                return new(_reader, colIndices);
            }
        }

        // Overload needed since otherwise ambiguous call for array between
        // ReadOnlySpan<> and IReadOnlyList<>
        public Cols this[string[] colNames] => this[colNames.AsSpan()];

        //IEnumerator<ColDebug> IEnumerable<ColDebug>.GetEnumerator()
        //{
        //    var names = _header.ColNames;
        //    for (var i = 0; i < names.Count; ++i)
        //    {
        //        yield return new(i, names[i], this[i].ToString());
        //    }
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        internal string DebuggerDisplay => $"{RowIndex,3} = '{Span}'";
    }

    public ReadOnlySpan<char> RowSpan()
    {
        if (_colCount > 0)
        {
            var colEnds = _colEnds;
            var start = colEnds[0] + 1; // +1 since previous end
            var end = colEnds[_colCount];
            return new(_chars, start, end - start);
        }
        else
        {
            return default;
        }
    }

    int GetCachedColIndex(string colName)
    {
        var colNameCache = _colNameCache;
        var currentCacheIndex = _cacheIndex;
        var cacheable = (uint)currentCacheIndex < (uint)colNameCache.Length;
        ref (string colName, int colIndex) colNameCacheRef = ref MemoryMarshal.GetArrayDataReference(colNameCache);
        if (cacheable)
        {
            colNameCacheRef = ref Unsafe.Add(ref colNameCacheRef, currentCacheIndex);
            var (cacheColumnName, cacheColumnIndex) = colNameCacheRef;
            ++_cacheIndex;
            if (ReferenceEquals(colName, cacheColumnName)) { return cacheColumnIndex; }
        }
        var columnIndex = _header.IndexOf(colName);
        if (cacheable)
        {
            colNameCacheRef = (colName, columnIndex);
        }
        return columnIndex;
    }
}
