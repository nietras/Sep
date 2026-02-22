using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace nietras.SeparatedValues;

public partial class SepUtf8Reader
{
    public delegate void RowAction(Row row);
    public delegate T RowFunc<T>(Row row)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;
    public delegate bool RowTryFunc<T>(Row row, out T value)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
        ;

    [DebuggerDisplay("{DebuggerDisplayPrefix,nq}{Span}")]
    public readonly ref struct Row
    {
        internal readonly SepUtf8ReaderState _state;

        internal Row(SepUtf8ReaderState state) => _state = state;

        public int RowIndex => _state._rowIndex;

        public int LineNumberFrom => _state._currentRowLineNumberFrom;
        public int LineNumberToExcl => _state._currentRowLineNumberTo;

        public int ColCount => _state._currentRowColCount;

        public ReadOnlySpan<byte> Span => _state.RowSpan();

        public override string ToString() => Encoding.UTF8.GetString(Span);

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

        public Cols this[[UnscopedRef] params ReadOnlySpan<int> indices] => new(_state, indices);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(string colName, out Col col)
        {
            if (_state.TryGetCachedColIndex(colName, out var colIndex))
            {
                col = new(_state, colIndex);
                return true;
            }
            col = default;
            return false;
        }

        internal string DebuggerDisplayPrefix => $"{RowIndex,3}:[{LineNumberFrom}..{LineNumberToExcl}] = ";
    }
}
