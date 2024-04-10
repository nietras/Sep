using System;
using System.Collections.Generic;

namespace nietras.SeparatedValues;

public partial class SepWriter
{
    public delegate void RowAction(Row row);

    public ref struct Row
    {
        internal SepWriter? _writer;

        internal Row(SepWriter writer) => _writer = writer;

        public Col this[int colIndex] => new(_writer!.GetOrAddCol(colIndex));

        public Col this[string colName] => new(_writer!.GetOrAddCol(colName));

        public Cols this[ReadOnlySpan<int> indices]
        {
            get
            {
                var cols = _writer!._arrayPool.RentUniqueArrayAsSpan<ColImpl>(indices.Length);
                for (var i = 0; i < indices.Length; i++)
                {
                    cols[i] = _writer!.GetOrAddCol(i);
                }
                return new Cols(cols);
            }
        }

        public Cols this[ReadOnlySpan<string> colNames]
        {
            get
            {
                var cols = _writer!._arrayPool.RentUniqueArrayAsSpan<ColImpl>(colNames.Length);
                for (var i = 0; i < colNames.Length; i++)
                {
                    var name = colNames[i];
                    cols[i] = _writer!.GetOrAddCol(name);
                }
                return new Cols(cols);
            }
        }

        public Cols this[IReadOnlyList<string> colNames]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(colNames);
                var cols = _writer!._arrayPool.RentUniqueArrayAsSpan<ColImpl>(colNames.Count);
                for (var i = 0; i < colNames.Count; i++)
                {
                    var name = colNames[i];
                    cols[i] = _writer!.GetOrAddCol(name);
                }
                return new Cols(cols);
            }
        }

        // Overload needed since otherwise ambiguous call for array between
        // ReadOnlySpan<> and IReadOnlyList<>
        public Cols this[string[] colNames]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(colNames);
                var cols = _writer!._arrayPool.RentUniqueArrayAsSpan<ColImpl>(colNames.Length);
                for (var i = 0; i < colNames.Length; i++)
                {
                    var name = colNames[i];
                    cols[i] = _writer!.GetOrAddCol(name);
                }
                return new Cols(cols);
            }
        }

        public void Dispose()
        {
            _writer?.EndRow(this);
            _writer = null;
        }
    }

    internal ColImpl GetOrAddCol(int colIndex)
    {
        var cols = _cols;
        if (colIndex == cols.Count && !_writeHeader)
        {
            var col = new ColImpl(this, colIndex, string.Empty, SepStringBuilderPool.Take());
            _cols.Add(col);
        }
        return cols[colIndex];
    }

    internal ColImpl GetOrAddCol(string colName)
    {
        if ((uint)_cacheIndex < (uint)_colNameCache.Count)
        {
            var (cacheColumnName, cacheColumnIndex) = _colNameCache[_cacheIndex];
            ++_cacheIndex;
            if (ReferenceEquals(colName, cacheColumnName))
            {
                var c = _cols[cacheColumnIndex];
                return c;
            }
        }

        if (!_colNameToCol.TryGetValue(colName, out var col))
        {
            col = !_headerWrittenOrSkipped ? AddCol(colName) : throw new KeyNotFoundException(colName);
        }
        // Should we cache on else

        return col;
    }

    ColImpl AddCol(string colName)
    {
        var colIndex = _colNameToCol.Count;
        var col = new ColImpl(this, colIndex, colName, SepStringBuilderPool.Take());
        _colNameToCol.Add(colName, col);
        _cols.Add(col);
        _colNameCache.Add((colName, colIndex));
        // Is it really necessary to increment cache colIndex for first row,
        // we won't hit cache here any way.
        ++_cacheIndex;
        return col;
    }
}
