using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace nietras.SeparatedValues;

public class SepUtf8ReaderState : SepReaderStateBase<byte, SepCharInfoUtf8>
{
    internal byte _fastFloatDecimalSeparatorOrZero;
    internal SepCreateToString _createToString = default!;
    internal SepCreateToBytes _createToBytes = default!;

#pragma warning disable CA2213 // Disposable fields should be disposed
    internal SepToString _toString = null!;
    internal SepToBytes _toBytes = null!;
#pragma warning restore CA2213 // Disposable fields should be disposed

    internal SepUtf8ReaderState(bool colUnquoteUnescape = false, SepTrim trim = SepTrim.None)
        : base(colUnquoteUnescape, trim)
    {
    }

    internal override string CreateRowString(int rowStart, int rowEnd)
    {
        return Encoding.UTF8.GetString(_chars, rowStart, rowEnd - rowStart);
    }

    internal string ToStringDefault(int index)
    {
        var span = GetColSpan(index);
        if (span.Length == 0) { return string.Empty; }
        // SepToString works on char spans, decode UTF-8 first
        // TODO: Consider a SepToString variant that takes byte spans directly
        return Encoding.UTF8.GetString(span);
    }

    internal ReadOnlyMemory<byte> ToBytesDefault(int index)
    {
        var span = GetColSpan(index);
        if (span.Length == 0) { return ReadOnlyMemory<byte>.Empty; }
        return _toBytes.ToBytes(span, index);
    }

    internal string ToStringDirect(int index)
    {
        var span = GetColSpan(index);
        if (span.Length == 0) { return string.Empty; }
        return Encoding.UTF8.GetString(span);
    }

    internal T Parse<T>(int index) where T : IUtf8SpanParsable<T>
    {
        var span = GetColSpan(index);
        var decimalSeparator = _fastFloatDecimalSeparatorOrZero;
        if (decimalSeparator != 0)
        {
            if (typeof(T) == typeof(float))
            {
                var v = csFastFloat.FastFloatParser.ParseFloat(span,
                    decimal_separator: decimalSeparator);
                return Unsafe.As<float, T>(ref v);
            }
            else if (typeof(T) == typeof(double))
            {
                var v = csFastFloat.FastDoubleParser.ParseDouble(span,
                    decimal_separator: decimalSeparator);
                return Unsafe.As<double, T>(ref v);
            }
        }
        return T.Parse(span, _cultureInfo);
    }

    internal T? TryParse<T>(int index) where T : struct, IUtf8SpanParsable<T> =>
        TryParse<T>(index, out var value) ? value : null;

    internal bool TryParse<T>(int index, out T value) where T : IUtf8SpanParsable<T>
    {
        var span = GetColSpan(index);
        var decimalSeparator = _fastFloatDecimalSeparatorOrZero;
        if (decimalSeparator != 0)
        {
            if (typeof(T) == typeof(float))
            {
                if (csFastFloat.FastFloatParser.TryParseFloat(span, out var v,
                    decimal_separator: decimalSeparator))
                {
                    value = Unsafe.As<float, T>(ref v);
                    return true;
                }
                value = default!;
                return false;
            }
            else if (typeof(T) == typeof(double))
            {
                if (csFastFloat.FastDoubleParser.TryParseDouble(span, out var v,
                    decimal_separator: decimalSeparator))
                {
                    value = Unsafe.As<double, T>(ref v);
                    return true;
                }
                value = default!;
                return false;
            }
        }
        return T.TryParse(span, _cultureInfo, out value!);
    }

    #region Cols Indices
    internal string[] ToStringsArray(ReadOnlySpan<int> colIndices)
    {
        var values = new string[colIndices.Length];
        ToStrings(colIndices, values);
        return values;
    }

    internal Span<string> ToStrings(ReadOnlySpan<int> colIndices)
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<string>(colIndices.Length);
        ToStrings(colIndices, span);
        return span;
    }

    internal void ToStrings(ReadOnlySpan<int> colIndices, Span<string> span)
    {
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = ToStringDefault(colIndices[i]);
        }
    }

    internal T[] ParseToArray<T>(ReadOnlySpan<int> colIndices) where T : IUtf8SpanParsable<T>
    {
        var values = new T[colIndices.Length];
        Parse<T>(colIndices, values);
        return values;
    }

    internal Span<T> Parse<T>(ReadOnlySpan<int> colIndices) where T : IUtf8SpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(colIndices.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(colIndices[i]);
        }
        return span;
    }

    internal void Parse<T>(ReadOnlySpan<int> colIndices, Span<T> span) where T : IUtf8SpanParsable<T>
    {
        SepCheck.CountOrLengthSameAsCols(colIndices.Length, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(colIndices[i]);
        }
    }

    internal Span<T?> TryParse<T>(ReadOnlySpan<int> colIndices) where T : struct, IUtf8SpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T?>(colIndices.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(colIndices[i]);
        }
        return span;
    }

    internal void TryParse<T>(ReadOnlySpan<int> colIndices, Span<T?> span) where T : struct, IUtf8SpanParsable<T>
    {
        SepCheck.CountOrLengthSameAsCols(colIndices.Length, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(colIndices[i]);
        }
    }
    #endregion

    #region Cols Range
    internal string[] ToStringsArray(int colStart, int colCount)
    {
        var values = new string[colCount];
        ToStrings(colStart, values);
        return values;
    }

    internal Span<string> ToStrings(int colStart, int colCount)
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<string>(colCount);
        ToStrings(colStart, span);
        return span;
    }

    internal void ToStrings(int colStart, Span<string> span)
    {
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = ToStringDefault(i + colStart);
        }
    }

    internal T[] ParseToArray<T>(int colStart, int colCount) where T : IUtf8SpanParsable<T>
    {
        var values = new T[colCount];
        Parse<T>(colStart, colCount, values);
        return values;
    }

    internal Span<T> Parse<T>(int colStart, int colCount) where T : IUtf8SpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(colCount);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(i + colStart);
        }
        return span;
    }

    internal void Parse<T>(int colStart, int colCount, Span<T> span) where T : IUtf8SpanParsable<T>
    {
        SepCheck.CountOrLengthSameAsCols(colCount, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(i + colStart);
        }
    }

    internal Span<T?> TryParse<T>(int colStart, int colCount) where T : struct, IUtf8SpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T?>(colCount);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(i + colStart);
        }
        return span;
    }

    internal void TryParse<T>(int colStart, int colCount, Span<T?> span) where T : struct, IUtf8SpanParsable<T>
    {
        SepCheck.CountOrLengthSameAsCols(colCount, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(i + colStart);
        }
    }
    #endregion

    internal override void DisposeManaged()
    {
        _toString?.Dispose();
        _toBytes?.Dispose();
        base.DisposeManaged();
    }
}
