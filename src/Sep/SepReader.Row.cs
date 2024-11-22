using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

public partial class SepReader
{
    // Problem here is Row is a ref struct so can't use Action<Row>
    public delegate void RowAction(Row row);
    public delegate T RowFunc<T>(Row row);
    public delegate bool RowTryFunc<T>(Row row, out T value);

    [DebuggerDisplay("{DebuggerDisplayPrefix,nq}{Span}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public readonly ref struct Row
    {
        internal readonly SepReaderState _state;

        internal Row(SepReaderState state) => _state = state;

        public int RowIndex => _state._rowIndex;

        public int LineNumberFrom => _state._currentRowLineNumberFrom;
        public int LineNumberToExcl => _state._currentRowLineNumberTo;

        public int ColCount => _state._currentRowColCount;

        public ReadOnlySpan<char> Span => _state.RowSpan();

        public override string ToString() => new(Span);

        /// <summary>
        /// Delegate to get column string at a given index.
        /// </summary>
        /// <remarks>
        /// Named "unsafe" since this refers to internal state and should not be
        /// used outside the scope of <see cref="SepReader.Row"/>. This is,
        /// however, needed to integrate with external benchmarks like NCsvPerf
        /// that require such a delegate. Hence, in order to avoid an allocation
        /// per row this property is provided.
        /// </remarks>
        public Func<int, string> UnsafeToStringDelegate => _state.UnsafeToStringDelegate;

        public Col this[int index] => new(_state, index);

        public Col this[Index index] => new(_state, index.GetOffset(_state._currentRowColCount));

        public Col this[string colName]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var colIndex = _state.GetCachedColIndex(colName);
                return new(_state, colIndex);
            }
        }

        public Cols this[Range range]
        {
            get
            {
                var (offset, length) = range.GetOffsetAndLength(_state._currentRowColCount);
                return new(_state, offset, length);
            }
        }

        public Cols this[ReadOnlySpan<int> indices] => new(_state, indices);

        public Cols this[IReadOnlyList<int> indices]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(indices);
                if (indices.Count == 0) { return new(_state, default); }

                var colIndices = _state._arrayPool.RentUniqueArrayAsSpan<int>(indices.Count);
                for (var i = 0; i < indices.Count; i++)
                {
                    colIndices[i] = indices[i];
                }
                return new(_state, colIndices);
            }
        }

        // Overload needed since otherwise ambiguous call for array between
        // ReadOnlySpan<> and IReadOnlyList<>
        public Cols this[int[] indices] => new(_state, indices);

        public Cols this[params ReadOnlySpan<string> colNames]
        {
            get
            {
                var colCount = colNames.Length;
                if (colCount == 0) { return new(_state, default); }

                // Optimize for a continuous range of columns by checking if
                // indices are consecutive. Otherwise, fall back to array of
                // indices.

                var colStart = _state.GetCachedColIndex(colNames[0]);
                var i = 1;
                for (; i < colCount; i++)
                {
                    var name = colNames[i];
                    var colIndex = _state.GetCachedColIndex(name);
                    if (colIndex != (colStart + i))
                    {
                        break;
                    }
                }
                if (i == colCount)
                {
                    return new(_state, colStart, colCount);
                }
                else
                {

                    var colIndices = _state._arrayPool.RentUniqueArrayAsSpan<int>(colNames.Length);
                    for (var j = 0; j < i; j++)
                    {
                        colIndices[j] = colStart + j;
                    }
                    for (; i < colNames.Length; i++)
                    {
                        var name = colNames[i];
                        colIndices[i] = _state.GetCachedColIndex(name);
                    }
                    return new(_state, colIndices);
                }
            }
        }

        public Cols this[IReadOnlyList<string> colNames]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(colNames);
                var colCount = colNames.Count;
                if (colCount == 0) { return new(_state, default); }

                // Optimize for a continuous range of columns by checking if
                // indices are consecutive. Otherwise, fall back to array of
                // indices.

                var colStart = _state.GetCachedColIndex(colNames[0]);
                var i = 1;
                for (; i < colCount; i++)
                {
                    var name = colNames[i];
                    var colIndex = _state.GetCachedColIndex(name);
                    if (colIndex != (colStart + i))
                    {
                        break;
                    }
                }
                if (i == colCount)
                {
                    return new(_state, colStart, colCount);
                }
                else
                {

                    var colIndices = _state._arrayPool.RentUniqueArrayAsSpan<int>(colNames.Count);
                    for (var j = 0; j < i; j++)
                    {
                        colIndices[j] = colStart + j;
                    }
                    for (; i < colNames.Count; i++)
                    {
                        var name = colNames[i];
                        colIndices[i] = _state.GetCachedColIndex(name);
                    }
                    return new(_state, colIndices);
                }
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

        internal string DebuggerDisplayPrefix => $"{RowIndex,3}:[{LineNumberFrom}..{LineNumberToExcl}] = ";

        internal class DebugView
        {
            readonly SepReaderState _state;

            internal DebugView(Row row) => _state = row._state;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            internal ColDebugView[] Cols => GetCols();

            ColDebugView[] GetCols()
            {
                var row = new Row(_state);
                var cols = new ColDebugView[_state._currentRowColCount];
                var maybeHeader = _state._hasHeader ? _state._header : null;
                for (var colIndex = 0; colIndex < cols.Length; colIndex++)
                {
                    var colValue = row[colIndex].ToStringDirect();
                    cols[colIndex] = new(colIndex, maybeHeader?.ColNames[colIndex], colValue);
                }
                return cols;
            }
        }

        [DebuggerDisplay("{ColValue}", Name = "{ColIndexName,nq}")]
        internal readonly struct ColDebugView
        {
            internal ColDebugView(int colIndex, string? colName, string colValue)
            {
                ColIndex = colIndex;
                ColName = colName;
                ColValue = colValue;
            }

            internal int ColIndex { get; }
            internal string? ColName { get; }
            internal string ColValue { get; }

            internal string ColIndexName => ColName is not null
                ? $"{ColIndex:D2}:'{ColName}'" : ColIndex.ToString("D2");
        }
    }
}
