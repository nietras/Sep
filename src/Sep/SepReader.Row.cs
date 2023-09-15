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

    [DebuggerDisplay("{DebuggerDisplayPrefix,nq}{Span}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public readonly ref struct Row
    {
        internal readonly SepReaderRowState _rowState;

        internal Row(SepReaderRowState rowState) => _rowState = rowState;

        public int RowIndex => _rowState._rowIndex;

        public int LineNumberFrom => _rowState._rowLineNumberFrom;
        public int LineNumberToExcl => _rowState._lineNumber;

        public int ColCount => _rowState._colCount;

        public ReadOnlySpan<char> Span => _rowState.RowSpan();

        public override string ToString() => new(Span);

        public Col this[int index] => new(_rowState, index);

        public Col this[Index index] => new(_rowState, index.GetOffset(_rowState._colCount));

        public Col this[string colName]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var colIndex = _rowState.GetCachedColIndex(colName);
                return new(_rowState, colIndex);
            }
        }

        public Cols this[Range range]
        {
            get
            {
                var (offset, length) = range.GetOffsetAndLength(_rowState._colCount);
                var colIndices = _rowState._arrayPool.RentUniqueArrayAsSpan<int>(length);
                for (var i = 0; i < colIndices.Length; i++)
                {
                    colIndices[i] = i + offset;
                }
                return new(_rowState, colIndices);
            }
        }

        public Cols this[ReadOnlySpan<int> indices] => new(_rowState, indices);

        public Cols this[IReadOnlyList<int> indices]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(indices);
                var colIndices = _rowState._arrayPool.RentUniqueArrayAsSpan<int>(indices.Count);
                for (var i = 0; i < indices.Count; i++)
                {
                    colIndices[i] = indices[i];
                }
                return new(_rowState, colIndices);
            }
        }

        // Overload needed since otherwise ambiguous call for array between
        // ReadOnlySpan<> and IReadOnlyList<>
        public Cols this[int[] indices] => new(_rowState, indices);

        public Cols this[ReadOnlySpan<string> colNames]
        {
            get
            {
                var colIndices = _rowState._arrayPool.RentUniqueArrayAsSpan<int>(colNames.Length);
                for (var i = 0; i < colNames.Length; i++)
                {
                    var name = colNames[i];
                    colIndices[i] = _rowState.GetCachedColIndex(name);
                }
                return new(_rowState, colIndices);
            }
        }

        public Cols this[IReadOnlyList<string> colNames]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(colNames);
                var colIndices = _rowState._arrayPool.RentUniqueArrayAsSpan<int>(colNames.Count);
                for (var i = 0; i < colNames.Count; i++)
                {
                    var name = colNames[i];
                    colIndices[i] = _rowState.GetCachedColIndex(name);
                }
                return new(_rowState, colIndices);
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
            readonly SepReaderRowState _rowState;

            internal DebugView(Row row) => _rowState = row._rowState;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            internal ColDebugView[] Cols => GetCols();

            ColDebugView[] GetCols()
            {
                var row = new Row(_rowState);
                var cols = new ColDebugView[_rowState._colCount];
                var maybeHeader = _rowState.HasHeader ? _rowState._header : null;
                for (var colIndex = 0; colIndex < cols.Length; colIndex++)
                {
                    var colValue = row[colIndex].ToStringRaw();
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
