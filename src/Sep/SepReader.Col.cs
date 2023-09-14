using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nietras.SeparatedValues;

public partial class SepReader
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public readonly ref struct Col
    {
        readonly SepReader _reader;
        readonly int _colIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Col(SepReader reader, int colIndex)
        {
            _reader = reader;
            _colIndex = colIndex;
        }

        public ReadOnlySpan<char> Span => _reader.GetColSpan(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _reader.ToString(_colIndex);

        // Allow opt out of pooling and don't add yet another configuration option
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToStringRaw() => _reader.ToStringRaw(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Parse<T>() where T : ISpanParsable<T> => _reader.Parse<T>(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? TryParse<T>() where T : struct, ISpanParsable<T> => _reader.TryParse<T>(_colIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse<T>(out T value) where T : ISpanParsable<T> => _reader.TryParse<T>(_colIndex, out value);

        internal string DebuggerDisplay => new(Span);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ReadOnlySpan<char> GetColSpan(int index)
    {
        if ((uint)index >= (uint)_colCount) { SepThrow.IndexOutOfRangeException(); }

        // Using array indexing is slightly faster despite more code 🤔
        var colEnds = _colEnds;
        var colStart = colEnds[index] + 1; // +1 since previous end
        var colEnd = colEnds[index + 1];
        // Above bounds checked is faster than below 🤔
        //ref var colEndsRef = ref MemoryMarshal.GetArrayDataReference(_colEnds);
        //var colStart = Unsafe.Add(ref colEndsRef, index) + 1; // +1 since previous end
        //var colEnd = Unsafe.Add(ref colEndsRef, index + 1);

        var colLength = colEnd - colStart;
        // Much better code generation given col span always inside buffer
        ref var colRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_chars), colStart);
        var col = MemoryMarshal.CreateReadOnlySpan(ref colRef, colLength);
        return col;
    }

    public string ToString(int index)
    {
        var span = GetColSpan(index);
        var s = TryGetStaticallyCachedString(span);
        if (s is not null)
        {
            return s;
        }
        if (index < _colToStrings.Length)
        {
            var spanToString = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_colToStrings), index);
            return spanToString.ToString(span);
        }
        else
        {
            return new(span);
        }
    }

    public string ToStringRaw(int index)
    {
        var span = GetColSpan(index);
        var s = TryGetStaticallyCachedString(span);
        if (s is not null)
        {
            return s;
        }
        return new(span);
    }

    T Parse<T>(int index) where T : ISpanParsable<T>
    {
        // To ensure SepToString and potential string pooling used for generic
        // case, check if type is T and use normal ToString
        if (typeof(T) == typeof(string))
        {
            var s = ToString(index);
            return Unsafe.As<string, T>(ref s);
        }
        else
        {
            var span = GetColSpan(index);
            var decimalSeparator = _fastFloatDecimalSeparatorOrZero;
            if (decimalSeparator != '\0')
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
    }

    T? TryParse<T>(int index) where T : struct, ISpanParsable<T> =>
        TryParse<T>(index, out var value) ? value : null;

    bool TryParse<T>(int index, out T value) where T : ISpanParsable<T>
    {
        // To ensure SepToString and potential string pooling used for generic
        // case, check if type is T and use normal ToString
        if (typeof(T) == typeof(string))
        {
            var s = ToString(index);
            value = Unsafe.As<string, T>(ref s);
            return true;
        }
        else
        {
            var span = GetColSpan(index);
            var decimalSeparator = _fastFloatDecimalSeparatorOrZero;
            if (decimalSeparator != '\0')
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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string? TryGetStaticallyCachedString(ReadOnlySpan<char> span)
    {
        if (span.Length == 0) { return string.Empty; }
        if (span.Length == 1)
        {
            var c = MemoryMarshal.GetReference(span);
            if (c < _singleCharToString.Length)
            {
                return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_singleCharToString), c);
            }
        }
        return null;
    }
}
