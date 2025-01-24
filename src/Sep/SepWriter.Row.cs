using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nietras.SeparatedValues;

public partial class SepWriter
{
    public delegate void RowAction(Row row);

    public ref struct Row
#if NET9_0_OR_GREATER
        : IDisposable
        , IAsyncDisposable
#endif
    {
        SepWriter? _writer;

        internal Row(SepWriter writer) => _writer = writer;

        public Col this[int colIndex] => new(_writer!.GetOrAddCol(colIndex));

        public Col this[string colName] => new(_writer!.GetOrAddCol(colName));

        public Cols this[params ReadOnlySpan<int> indices]
        {
            get
            {
                var cols = _writer!._arrayPool.RentUniqueArrayAsSpan<ColImpl>(indices.Length);
                for (var i = 0; i < indices.Length; i++)
                {
                    cols[i] = _writer!.GetOrAddCol(indices[i]);
                }
                return new Cols(cols);
            }
        }

        public Cols this[params ReadOnlySpan<string> colNames]
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
            _writer?.EndRow();
            _writer = null;
        }

        public ValueTask DisposeAsync()
        {
            if (_writer is not null)
            {
                return _writer.EndRowAsync();
            }
            _writer = null;
            return ValueTask.CompletedTask;
        }
    }

    internal ColImpl GetOrAddCol(int colIndex)
    {
        var cols = _cols;
        if (colIndex == cols.Count && (!_writeHeader || _disableColCountCheck))
        {
            var col = new ColImpl(this, colIndex, string.Empty);
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

    internal ColImpl AddCol(string colName)
    {
        var colIndex = _colNameToCol.Count;
        var col = new ColImpl(this, colIndex, colName);
        _colNameToCol.Add(colName, col);
        _cols.Add(col);
        _colNameCache.Add((colName, colIndex));
        // Is it really necessary to increment cache colIndex for first row,
        // we won't hit cache here any way.
        ++_cacheIndex;
        return col;
    }
}
