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
        readonly SepReader _reader;
        readonly ReadOnlySpan<int> _colIndices;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Cols(SepReader reader, ReadOnlySpan<int> indices)
        {
            _reader = reader;
            _colIndices = indices;
        }

        public int Length => _colIndices.Length;

        public Col this[int index] => new(_reader, _colIndices[index]);

        /// <summary>
        /// Get all selected cols as strings in an array.
        /// </summary>
        /// <remarks>
        /// Convenience method since <see cref="Parse{T}()" /> only works for
        /// <see cref="string"/> in .NET 8+ where <see cref="string"/>  is <see
        /// cref="ISpanParsable{TSelf}"/>.
        /// </remarks>
        /// <returns>Newly allocated array of each col as a string.</returns>
        public string[] ToStringsArray() => _reader.ToStringsArray(_colIndices);
        /// <summary>
        /// Get all selected cols as strings in a span.
        /// </summary>
        /// <remarks>
        /// Convenience method since <see cref="Parse{T}()" /> only works for
        /// <see cref="string"/> in .NET 8+ where <see cref="string"/>  is <see
        /// cref="ISpanParsable{TSelf}"/>.
        /// </remarks>
        /// <returns>Span with each col as a string.</returns>
        public Span<string> ToStrings() => _reader.ToStrings(_colIndices);

        public T[] ParseToArray<T>() where T : ISpanParsable<T> =>
            _reader.ParseToArray<T>(_colIndices);

        public Span<T> Parse<T>() where T : ISpanParsable<T> =>
            _reader.Parse<T>(_colIndices);

        public void Parse<T>(Span<T> span) where T : ISpanParsable<T> =>
            _reader.Parse<T>(_colIndices, span);

        public Span<T?> TryParse<T>() where T : struct, ISpanParsable<T> =>
            _reader.TryParse<T>(_colIndices);

        public void TryParse<T>(Span<T?> span) where T : struct, ISpanParsable<T> =>
            _reader.TryParse<T>(_colIndices, span);

        public Span<T> Select<T>(ColFunc<T> selector) =>
            _reader.Select<T>(_colIndices, selector);

        public unsafe Span<T> Select<T>(delegate*<Col, T> selector) =>
            _reader.Select<T>(_colIndices, selector);
    }

    string[] ToStringsArray(ReadOnlySpan<int> colIndices)
    {
        var values = new string[colIndices.Length];
        ToStrings(colIndices, values);
        return values;
    }

    Span<string> ToStrings(ReadOnlySpan<int> colIndices)
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<string>(colIndices.Length);
        ToStrings(colIndices, span);
        return span;
    }

    void ToStrings(ReadOnlySpan<int> colIndices, Span<string> span)
    {
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = ToString(colIndices[i]);
        }
    }

    T[] ParseToArray<T>(ReadOnlySpan<int> colIndices) where T : ISpanParsable<T>
    {
        var values = new T[colIndices.Length];
        Parse<T>(colIndices, values);
        return values;
    }

    Span<T> Parse<T>(ReadOnlySpan<int> colIndices) where T : ISpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(colIndices.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(colIndices[i]);
        }
        return span;
    }

    void Parse<T>(ReadOnlySpan<int> colIndices, Span<T> span) where T : ISpanParsable<T>
    {
        SepCheck.LengthSameAsCols(colIndices.Length, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(colIndices[i]);
        }
    }

    Span<T?> TryParse<T>(ReadOnlySpan<int> colIndices) where T : struct, ISpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T?>(colIndices.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(colIndices[i]);
        }
        return span;
    }

    void TryParse<T>(ReadOnlySpan<int> colIndices, Span<T?> span) where T : struct, ISpanParsable<T>
    {
        SepCheck.LengthSameAsCols(colIndices.Length, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(colIndices[i]);
        }
    }

    Span<T> Select<T>(ReadOnlySpan<int> colIndices, ColFunc<T> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        var length = colIndices.Length;
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(length);
        for (var i = 0; i < length; i++)
        {
            span[i] = selector(new(this, colIndices[i]));
        }
        return span;
    }

    unsafe Span<T> Select<T>(ReadOnlySpan<int> colIndices, delegate*<Col, T> selector)
    {
        var length = colIndices.Length;
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(length);
        for (var i = 0; i < length; i++)
        {
            span[i] = selector(new(this, colIndices[i]));
        }
        return span;
    }
}
