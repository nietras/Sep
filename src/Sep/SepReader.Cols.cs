using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        readonly int _colStartIfRange;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Cols(SepReaderState state, ReadOnlySpan<int> indices)
        {
            _state = state;
            _colIndices = indices;
            _colStartIfRange = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Cols(SepReaderState state, int colStart, int colCount)
        {
            _state = state;
            _colIndices = MemoryMarshal.CreateSpan(ref Unsafe.NullRef<int>(), colCount);
            _colStartIfRange = colStart;
        }

        public int Length => _colIndices.Length;

        public Col this[int index]
        {
            get => new(_state, IsIndices() ? _colIndices[index] : GetRangeIndex(index));
        }

        /// <summary>
        /// Get all selected cols as strings in an array.
        /// </summary>
        /// <remarks>
        /// Convenience method since <see cref="ParseToArray{T}()" /> only works for
        /// <see cref="string"/> in .NET 8+ where <see cref="string"/>  is <see
        /// cref="ISpanParsable{TSelf}"/>.
        /// </remarks>
        /// <returns>Newly allocated array of each col as a string.</returns>
        public string[] ToStringsArray() => IsIndices()
            ? _state.ToStringsArray(_colIndices)
            : _state.ToStringsArray(_colStartIfRange, _colIndices.Length);
        /// <summary>
        /// Get all selected cols as strings in a span.
        /// </summary>
        /// <remarks>
        /// Convenience method since <see cref="Parse{T}()" /> only works for
        /// <see cref="string"/> in .NET 8+ where <see cref="string"/>  is <see
        /// cref="ISpanParsable{TSelf}"/>.
        /// </remarks>
        /// <returns>Span with each col as a string.</returns>
        public Span<string> ToStrings() => IsIndices()
            ? _state.ToStrings(_colIndices)
            : _state.ToStrings(_colStartIfRange, _colIndices.Length);

        public T[] ParseToArray<T>() where T : ISpanParsable<T> => IsIndices()
            ? _state.ParseToArray<T>(_colIndices)
            : _state.ParseToArray<T>(_colStartIfRange, _colIndices.Length);

        public Span<T> Parse<T>() where T : ISpanParsable<T> => IsIndices()
            ? _state.Parse<T>(_colIndices)
            : _state.Parse<T>(_colStartIfRange, _colIndices.Length);

        public void Parse<T>(Span<T> span) where T : ISpanParsable<T>
        {
            if (IsIndices()) { _state.Parse<T>(_colIndices, span); }
            else { _state.Parse<T>(_colStartIfRange, _colIndices.Length, span); }
        }

        public Span<T?> TryParse<T>() where T : struct, ISpanParsable<T> => IsIndices()
            ? _state.TryParse<T>(_colIndices)
            : _state.TryParse<T>(_colStartIfRange, _colIndices.Length);

        public void TryParse<T>(Span<T?> span) where T : struct, ISpanParsable<T>
        {
            if (IsIndices()) { _state.TryParse<T>(_colIndices, span); }
            else { _state.TryParse<T>(_colStartIfRange, _colIndices.Length, span); }
        }

        public Span<T> Select<T>(ColFunc<T> selector) => IsIndices()
            ? _state.Select<T>(_colIndices, selector)
            : _state.Select<T>(_colStartIfRange, _colIndices.Length, selector);

        public unsafe Span<T> Select<T>(delegate*<Col, T> selector) => IsIndices()
            ? _state.Select<T>(_colIndices, selector)
            : _state.Select<T>(_colStartIfRange, _colIndices.Length, selector);

        bool IsIndices() => _colStartIfRange < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetRangeIndex(int index)
        {
            Debug.Assert(_colStartIfRange >= 0);
            if ((uint)index >= (uint)_colIndices.Length)
            {
                SepThrow.IndexOutOfRangeException();
            }
            return index + _colStartIfRange;
        }
    }
}
