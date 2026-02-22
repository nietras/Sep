using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace nietras.SeparatedValues;

public partial class SepUtf8Reader
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public readonly ref struct Col
    {
        readonly SepUtf8ReaderState _state;
        readonly int _colIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Col(SepUtf8ReaderState state, int colIndex)
        {
            _state = state;
            _colIndex = colIndex;
        }

        public ReadOnlySpan<byte> Span => _state.GetColSpan(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _state.ToStringDefault(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string ToStringDirect() => _state.ToStringDirect(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<byte> ToBytes() => _state.ToBytesDefault(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Parse<T>() where T : IUtf8SpanParsable<T> => _state.Parse<T>(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? TryParse<T>() where T : struct, IUtf8SpanParsable<T> => _state.TryParse<T>(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse<T>(out T value) where T : IUtf8SpanParsable<T> => _state.TryParse<T>(_colIndex, out value);

        internal string DebuggerDisplay => Encoding.UTF8.GetString(Span);
    }
}
