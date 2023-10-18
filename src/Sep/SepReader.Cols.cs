using System;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

public partial class SepReader
{
    // Problem here is Col is a ref struct so can't use Func<Col,T>
    public delegate T ColFunc<T>(Col col);
    public delegate void ColAction(Col col);
    public delegate void ColsAction(Cols col);

    public readonly ref struct Cols
    {
        readonly SepReaderState _state;
        readonly ReadOnlySpan<int> _colIndices;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Cols(SepReaderState state, ReadOnlySpan<int> indices)
        {
            _state = state;
            _colIndices = indices;
        }

        public int Length => _colIndices.Length;

        public Col this[int index] => new(_state, _colIndices[index]);

        /// <summary>
        /// Get all selected cols as strings in an array.
        /// </summary>
        /// <remarks>
        /// Convenience method since <see cref="ParseToArray{T}()" /> only works for
        /// <see cref="string"/> in .NET 8+ where <see cref="string"/>  is <see
        /// cref="ISpanParsable{TSelf}"/>.
        /// </remarks>
        /// <returns>Newly allocated array of each col as a string.</returns>
        public string[] ToStringsArray() => _state.ToStringsArray(_colIndices);
        /// <summary>
        /// Get all selected cols as strings in a span.
        /// </summary>
        /// <remarks>
        /// Convenience method since <see cref="Parse{T}()" /> only works for
        /// <see cref="string"/> in .NET 8+ where <see cref="string"/>  is <see
        /// cref="ISpanParsable{TSelf}"/>.
        /// </remarks>
        /// <returns>Span with each col as a string.</returns>
        public Span<string> ToStrings() => _state.ToStrings(_colIndices);

        public T[] ParseToArray<T>() where T : ISpanParsable<T> =>
            _state.ParseToArray<T>(_colIndices);

        public Span<T> Parse<T>() where T : ISpanParsable<T> =>
            _state.Parse<T>(_colIndices);

        public void Parse<T>(Span<T> span) where T : ISpanParsable<T> =>
            _state.Parse<T>(_colIndices, span);

        public Span<T?> TryParse<T>() where T : struct, ISpanParsable<T> =>
            _state.TryParse<T>(_colIndices);

        public void TryParse<T>(Span<T?> span) where T : struct, ISpanParsable<T> =>
            _state.TryParse<T>(_colIndices, span);

        public Span<T> Select<T>(ColFunc<T> selector) =>
            _state.Select<T>(_colIndices, selector);

        public unsafe Span<T> Select<T>(delegate*<Col, T> selector) =>
            _state.Select<T>(_colIndices, selector);
    }
}
