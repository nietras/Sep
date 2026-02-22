using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#if NET9_0_OR_GREATER
using System.IO;
#endif
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static nietras.SeparatedValues.SepReader;

namespace nietras.SeparatedValues;

internal readonly record struct SepRowInfo(int LineNumberTo, int ColCount);

// Cannot be nested due to CS0146: Circular base type dependency
public class SepReaderState : SepReaderStateBase<char, SepCharInfoUtf16>
{
    // To avoid `call     CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE`,
    // promote cache to member here.
    readonly string[] _singleCharToString = SepStringCache.SingleCharToString;

    internal char _fastFloatDecimalSeparatorOrZero;
    internal SepCreateToString _createToString = default!;

#pragma warning disable CA2213 // Disposable fields should be disposed
    internal SepToString _toString = null!;
#pragma warning restore CA2213 // Disposable fields should be disposed

    internal SepReaderState(bool colUnquoteUnescape = false, SepTrim trim = SepTrim.None)
        : base(colUnquoteUnescape, trim)
    {
        // Delegate must point to this instance's method
        UnsafeToStringDelegate = ToStringDefault;
    }

    internal SepReaderState(SepReader other) : base(other._colSpanFlags)
    {
        // Delegate must point to this instance's method
        UnsafeToStringDelegate = ToStringDefault;

        _header = other._header;
        _fastFloatDecimalSeparatorOrZero = other._fastFloatDecimalSeparatorOrZero;
        System.Diagnostics.Debug.Assert(_fastFloatDecimalSeparatorOrZero != '\0');
        _cultureInfo = other._cultureInfo;
        _createToString = other._createToString;

        _colEndsOrColInfos = ArrayPool<int>.Shared.Rent(other._colEndsOrColInfos.Length);
        _colCountExpected = other._colCountExpected;
        _arrayPool = new();
        _colNameCache = new (string colName, int colIndex)[other._colNameCache.Length];
        // Only duplicate toString if not thread safe
        _toString = other._toString.IsThreadSafe ? other._toString : _createToString(_header, _colCountExpected);
    }

    internal Func<int, string> UnsafeToStringDelegate { get; }

    internal void SwapParsedRowsTo(SepReaderState other)
    {
        SwapParsedRowsBuffersTo(other);
        // Try swap tostring instance to avoid allocations
        (other._toString, _toString) = (_toString, other._toString);
    }

    internal override string CreateRowString(int rowStart, int rowEnd)
    {
        return new string(_chars, rowStart, rowEnd - rowStart);
    }

    internal string ToStringDefault(int index)
    {
        var span = GetColSpan(index);
        var s = TryGetStaticallyCachedString(span);
        if (s is not null)
        {
            return s;
        }
        return _toString.ToString(span, index);
    }

    internal string ToStringDirect(int index)
    {
        var span = GetColSpan(index);
        var s = TryGetStaticallyCachedString(span);
        if (s is not null)
        {
            return s;
        }
        return new(span);
    }

    internal T Parse<T>(int index) where T : ISpanParsable<T>
    {
        // To ensure SepToString and potential string pooling used for generic
        // case, check if type is T and use normal ToString
        if (typeof(T) == typeof(string))
        {
            var s = ToStringDefault(index);
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

    internal T? TryParse<T>(int index) where T : struct, ISpanParsable<T> =>
        TryParse<T>(index, out var value) ? value : null;

    internal bool TryParse<T>(int index, out T value) where T : ISpanParsable<T>
    {
        // To ensure SepToString and potential string pooling used for generic
        // case, check if type is T and use normal ToString
        if (typeof(T) == typeof(string))
        {
            var s = ToStringDefault(index);
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
    internal string? TryGetStaticallyCachedString(ReadOnlySpan<char> span)
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

    internal T[] ParseToArray<T>(ReadOnlySpan<int> colIndices) where T : ISpanParsable<T>
    {
        var values = new T[colIndices.Length];
        Parse<T>(colIndices, values);
        return values;
    }

    internal Span<T> Parse<T>(ReadOnlySpan<int> colIndices) where T : ISpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(colIndices.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(colIndices[i]);
        }
        return span;
    }

    internal void Parse<T>(ReadOnlySpan<int> colIndices, Span<T> span) where T : ISpanParsable<T>
    {
        SepCheck.CountOrLengthSameAsCols(colIndices.Length, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(colIndices[i]);
        }
    }

    internal Span<T?> TryParse<T>(ReadOnlySpan<int> colIndices) where T : struct, ISpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T?>(colIndices.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(colIndices[i]);
        }
        return span;
    }

    internal void TryParse<T>(ReadOnlySpan<int> colIndices, Span<T?> span) where T : struct, ISpanParsable<T>
    {
        SepCheck.CountOrLengthSameAsCols(colIndices.Length, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(colIndices[i]);
        }
    }

    internal Span<T> ColsSelect<T>(ReadOnlySpan<int> colIndices, ColFunc<T> selector)
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

    internal unsafe Span<T> ColsSelect<T>(ReadOnlySpan<int> colIndices, delegate*<Col, T> selector)
    {
        var length = colIndices.Length;
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(length);
        for (var i = 0; i < length; i++)
        {
            span[i] = selector(new(this, colIndices[i]));
        }
        return span;
    }

    [SkipLocalsInit]
    internal ReadOnlySpan<char> Join(ReadOnlySpan<int> colIndices, scoped ReadOnlySpan<char> separator)
    {
        var length = colIndices.Length;
        if (length == 0) { return []; }
        if (length == 1) { return GetColSpan(colIndices[0]); }
        // Assume col count never so high stackalloc is not possible
        Span<SepRange> colRanges = stackalloc SepRange[colIndices.Length];
        GetColRanges(colIndices, colRanges);
        return Join(colRanges, separator);
    }
    [SkipLocalsInit]
    internal string JoinToString(ReadOnlySpan<int> colIndices, scoped ReadOnlySpan<char> separator)
    {
        var length = colIndices.Length;
        if (length == 0) { return string.Empty; }
        if (length == 1) { return ToStringDefault(colIndices[0]); }
        // Assume col count never so high stackalloc is not possible
        Span<SepRange> colRanges = stackalloc SepRange[colIndices.Length];
        GetColRanges(colIndices, colRanges);
        return JoinToString(colRanges, separator);
    }

#if NET9_0_OR_GREATER
    [SkipLocalsInit]
    internal string JoinPathsToString(ReadOnlySpan<int> colIndices)
    {
        var length = colIndices.Length;
        if (length == 0) { return string.Empty; }
        if (length == 1) { return ToStringDefault(colIndices[0]); }
        if (length <= 4)
        {
            Span<SepRange> colRanges = stackalloc SepRange[colIndices.Length];
            GetColRanges(colIndices, colRanges);
            return JoinPathsToString(colRanges);
        }
        else
        {
            var paths = ToStrings(colIndices);
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            return Path.Join(paths);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        }
    }

    internal string CombinePathsToString(ReadOnlySpan<int> colIndices)
    {
        var length = colIndices.Length;
        if (length == 0) { return string.Empty; }
        if (length == 1) { return ToStringDefault(colIndices[0]); }
        var paths = ToStrings(colIndices);
        return Path.Combine(paths);
    }
#endif

    void GetColRanges(ReadOnlySpan<int> colIndices, Span<SepRange> colRanges)
    {
        A.Assert(colIndices.Length == colRanges.Length);
        for (var i = 0; i < colIndices.Length; i++)
        {
            colRanges[i] = GetColRange(colIndices[i]);
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

    internal T[] ParseToArray<T>(int colStart, int colCount) where T : ISpanParsable<T>
    {
        var values = new T[colCount];
        Parse<T>(colStart, colCount, values);
        return values;
    }

    internal Span<T> Parse<T>(int colStart, int colCount) where T : ISpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(colCount);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(i + colStart);
        }
        return span;
    }

    internal void Parse<T>(int colStart, int colCount, Span<T> span) where T : ISpanParsable<T>
    {
        SepCheck.CountOrLengthSameAsCols(colCount, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = Parse<T>(i + colStart);
        }
    }

    internal Span<T?> TryParse<T>(int colStart, int colCount) where T : struct, ISpanParsable<T>
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T?>(colCount);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(i + colStart);
        }
        return span;
    }

    internal void TryParse<T>(int colStart, int colCount, Span<T?> span) where T : struct, ISpanParsable<T>
    {
        SepCheck.CountOrLengthSameAsCols(colCount, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(i + colStart);
        }
    }

    internal Span<T> ColsSelect<T>(int colStart, int colCount, ColFunc<T> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(colCount);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = selector(new(this, i + colStart));
        }
        return span;
    }

    internal unsafe Span<T> ColsSelect<T>(int colStart, int colCount, delegate*<Col, T> selector)
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(colCount);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = selector(new(this, i + colStart));
        }
        return span;
    }

    [SkipLocalsInit]
    internal ReadOnlySpan<char> Join(int colStart, int colCount, scoped ReadOnlySpan<char> separator)
    {
        if (colCount == 0) { return []; }
        if (colCount == 1) { return GetColSpan(colStart); }
        // Assume col count never so high stackalloc is not possible
        Span<SepRange> colRanges = stackalloc SepRange[colCount];
        GetColRanges(colStart, colRanges);
        return Join(colRanges, separator);
    }
    [SkipLocalsInit]
    internal string JoinToString(int colStart, int colCount, scoped ReadOnlySpan<char> separator)
    {
        if (colCount == 0) { return string.Empty; }
        if (colCount == 1) { return ToStringDefault(colStart); }
        // Assume col count never so high stackalloc is not possible
        Span<SepRange> colRanges = stackalloc SepRange[colCount];
        GetColRanges(colStart, colRanges);
        return JoinToString(colRanges, separator);
    }


#if NET9_0_OR_GREATER
    [SkipLocalsInit]
    internal string JoinPathsToString(int colStart, int colCount)
    {
        if (colCount == 0) { return string.Empty; }
        if (colCount == 1) { return ToStringDefault(colStart); }
        if (colCount <= 4)
        {
            Span<SepRange> colRanges = stackalloc SepRange[colCount];
            GetColRanges(colStart, colRanges);
            return JoinPathsToString(colRanges);
        }
        else
        {
            var paths = ToStrings(colStart, colCount);
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            return Path.Join(paths);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        }
    }
    internal string CombinePathsToString(int colStart, int colCount)
    {
        if (colCount == 0) { return string.Empty; }
        if (colCount == 1) { return ToStringDefault(colStart); }
        var paths = ToStrings(colStart, colCount);
        return Path.Combine(paths);
    }
#endif

    void GetColRanges(int colStart, Span<SepRange> colRanges)
    {
        for (var i = 0; i < colRanges.Length; i++)
        {
            colRanges[i] = GetColRange(colStart + i);
        }
    }
    #endregion

    #region Join
    ReadOnlySpan<char> Join(scoped Span<SepRange> colRanges, scoped ReadOnlySpan<char> separator)
    {
        var totalLength = JoinTotalLength(colRanges, separator.Length);
        var join = _arrayPool.RentUniqueArrayAsSpan<char>(totalLength);
        Join(_chars.AsSpan(), colRanges, separator, join);
        return join;
    }

    readonly ref struct JoinToStringState(ReadOnlySpan<SepRange> colRanges, ReadOnlySpan<char> separator)
    {
        public ReadOnlySpan<SepRange> ColRanges { get; } = colRanges;
        public ReadOnlySpan<char> Separator { get; } = separator;
    }

    string JoinToString(scoped ReadOnlySpan<SepRange> colRanges, scoped ReadOnlySpan<char> separator)
    {
        var totalLength = JoinTotalLength(colRanges, separator.Length);
#if NET9_0_OR_GREATER
        var state = new JoinToStringState(colRanges, separator);
        return string.Create(totalLength, state, (join, state) =>
        {
            Join(_chars.AsSpan(), state.ColRanges, state.Separator, join);
        });
#else
        // Before .NET 9 no `allows ref struct`, so create uninitialized string,
        // and get mutable span for that and join into that.
        var s = new string('\0', totalLength);
        var join = MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference<char>(s), s.Length);
        Join(_chars.AsSpan(), colRanges, separator, join);
        return s;
#endif
    }

#if NET9_0_OR_GREATER
    string JoinPathsToString(scoped ReadOnlySpan<SepRange> colRanges)
    {
        var length = colRanges.Length;
        A.Assert(1 < length && length <= 4);

        var chars = _chars.AsSpan();
        var colSpan0 = GetSpan(chars, colRanges[0]);
        var colSpan1 = GetSpan(chars, colRanges[1]);
        if (length == 2) { return Path.Join(colSpan0, colSpan1); }
        var colSpan2 = GetSpan(chars, colRanges[2]);
        if (length == 3) { return Path.Join(colSpan0, colSpan1, colSpan2); }
        var colSpan3 = GetSpan(chars, colRanges[3]);
        return Path.Join(colSpan0, colSpan1, colSpan2, colSpan3);

        static ReadOnlySpan<char> GetSpan(ReadOnlySpan<char> chars, SepRange range) =>
            chars.Slice(range.Start, range.Length);
    }
#endif

    static void Join(ReadOnlySpan<char> chars,
        ReadOnlySpan<SepRange> colRanges, ReadOnlySpan<char> separator,
        Span<char> join)
    {
        var separatorLength = separator.Length;
        var spanIndex = 0;
        for (var i = 0; i < colRanges.Length; i++)
        {
            var colRange = colRanges[i];
            var colSpan = chars.Slice(colRange.Start, colRange.Length);
            colSpan.CopyTo(join.Slice(spanIndex));
            spanIndex += colSpan.Length;
            if (i < colRanges.Length - 1)
            {
                separator.CopyTo(join.Slice(spanIndex));
                spanIndex += separatorLength;
            }
        }
        A.Assert(spanIndex == join.Length);
    }

    static int JoinTotalLength(ReadOnlySpan<SepRange> colRanges, int separatorLength)
    {
        var totalLength = 0;
        for (var i = 0; i < colRanges.Length; i++)
        {
            totalLength += colRanges[i].Length;
        }
        totalLength += separatorLength * (colRanges.Length - 1);
        return totalLength;
    }
    #endregion

    internal override void DisposeManaged()
    {
        _toString?.Dispose();
        base.DisposeManaged();
    }
}
