using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

public partial class SepReader
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public readonly ref struct Col
    {
        readonly SepReaderRowState _rowState;
        readonly int _colIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Col(SepReaderRowState rowState, int colIndex)
        {
            _rowState = rowState;
            _colIndex = colIndex;
        }

        public ReadOnlySpan<char> Span => _rowState.GetColSpan(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _rowState.ToString(_colIndex);

        // Allow opt out of pooling and don't add yet another configuration option
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToStringRaw() => _rowState.ToStringRaw(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Parse<T>() where T : ISpanParsable<T> => _rowState.Parse<T>(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? TryParse<T>() where T : struct, ISpanParsable<T> => _rowState.TryParse<T>(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse<T>(out T value) where T : ISpanParsable<T> => _rowState.TryParse<T>(_colIndex, out value);

        internal string DebuggerDisplay => new(Span);
    }


}
