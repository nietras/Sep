using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

public partial class SepReader
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public readonly ref struct Col
    {
        readonly SepReaderState _state;
        readonly int _colIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Col(SepReaderState state, int colIndex)
        {
            _state = state;
            _colIndex = colIndex;
        }

        public ReadOnlySpan<char> Span => _state.GetColSpan(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _state.ToString(_colIndex);

        // Allow opt out of pooling and don't add yet another configuration option
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToStringRaw() => _state.ToStringRaw(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Parse<T>() where T : ISpanParsable<T> => _state.Parse<T>(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? TryParse<T>() where T : struct, ISpanParsable<T> => _state.TryParse<T>(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse<T>(out T value) where T : ISpanParsable<T> => _state.TryParse<T>(_colIndex, out value);

        internal string DebuggerDisplay => new(Span);
    }


}
