using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static nietras.SeparatedValues.SepReader;

namespace nietras.SeparatedValues;

// Cannot be nested due to CS0146: Circular base type dependency
public class SepReaderRowState : IDisposable
{
    // To avoid `call     CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE`,
    // promote cache to member here.
    readonly string[] _singleCharToString = SepStringCache.SingleCharToString;

    protected internal SepHeader _header = null!;
    readonly char _fastFloatDecimalSeparatorOrZero;
    readonly CultureInfo? _cultureInfo;

    protected internal char[] _chars = Array.Empty<char>();
    protected internal int _charsDataStart = 0;
    protected internal int _charsDataEnd = 0;
    protected internal int _charsParseStart = 0;
    protected internal int _charsRowStart = 0;

    protected internal const int _colEndsMaximumLength = 1024;
    // [0] = Previous row/col end e.g. one before row/first col start
    // [1...] = Col ends e.g. [1] = first col end
    // Length = colCount + 1
    protected internal int[] _colEnds = Array.Empty<int>();
    protected internal int _colCountExpected = -1;
    protected internal int _colCount = 0;

    protected internal int _rowIndex = -1;
    protected internal int _rowLineNumberFrom = 0;
    protected internal int _lineNumber = 1;

    internal readonly SepArrayPoolAccessIndexed _arrayPool = new();
    protected internal (string colName, int colIndex)[] _colNameCache = Array.Empty<(string colName, int colIndex)>();
    protected internal int _cacheIndex = 0;
    protected internal SepToString[] _colToStrings = Array.Empty<SepToString>();

    public bool HasHeader { get; internal init; }
    public SepHeader Header => _header;

    internal SepReaderRowState() { }

    internal SepReaderRowState(SepReader other)
    {
        _header = other._header;
        _fastFloatDecimalSeparatorOrZero = other._fastFloatDecimalSeparatorOrZero;
        _cultureInfo = other._cultureInfo;

        // TODO: Consider if length should be less for copy or if wait on initialize to copy
        //_chars = ArrayPool<char>.Shared.Rent(other._chars.Length);
        _colEnds = ArrayPool<int>.Shared.Rent(other._colEnds.Length);
        _colNameCache = new (string colName, int colIndex)[other._colNameCache.Length];
        _colToStrings = new SepToString[other._colToStrings.Length];
        for (var colIndex = 0; colIndex < _colToStrings.Length; colIndex++)
        {
            _colToStrings[colIndex] = other._options.CreateToString(_header, colIndex);
        }
        _colCountExpected = other._colCountExpected;
    }

    internal void CopyNewRowTo(SepReaderRowState other)
    {
        other._cacheIndex = 0;
        other._arrayPool.Reset();

        other._colCount = _colCount;

        other._rowIndex = _rowIndex;
        other._rowLineNumberFrom = _rowLineNumberFrom;
        other._lineNumber = _lineNumber;

        var rowSpan = RowSpan();
        ref var otherChars = ref other._chars;
        if (rowSpan.Length > otherChars.Length)
        {
            if (otherChars.Length > 0)
            { ArrayPool<char>.Shared.Return(otherChars); }
            otherChars = ArrayPool<char>.Shared.Rent(rowSpan.Length);
        }
        rowSpan.CopyTo(otherChars);
        other._charsDataEnd = rowSpan.Length;

        _colEnds.AsSpan().Slice(0, _colCount + 1).CopyTo(other._colEnds);
    }

    #region Row
    public ReadOnlySpan<char> RowSpan()
    {
        if (_colCount > 0)
        {
            var colEnds = _colEnds;
            var start = colEnds[0] + 1; // +1 since previous end
            var end = colEnds[_colCount];
            return new(_chars, start, end - start);
        }
        else
        {
            return default;
        }
    }

    internal int GetCachedColIndex(string colName)
    {
        var colNameCache = _colNameCache;
        var currentCacheIndex = _cacheIndex;
        var cacheable = (uint)currentCacheIndex < (uint)colNameCache.Length;
        ref (string colName, int colIndex) colNameCacheRef = ref MemoryMarshal.GetArrayDataReference(colNameCache);
        if (cacheable)
        {
            colNameCacheRef = ref Unsafe.Add(ref colNameCacheRef, currentCacheIndex);
            var (cacheColumnName, cacheColumnIndex) = colNameCacheRef;
            ++_cacheIndex;
            if (ReferenceEquals(colName, cacheColumnName)) { return cacheColumnIndex; }
        }
        var columnIndex = _header.IndexOf(colName);
        if (cacheable)
        {
            colNameCacheRef = (colName, columnIndex);
        }
        return columnIndex;
    }
    #endregion

    #region Col
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<char> GetColSpan(int index)
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

    internal T Parse<T>(int index) where T : ISpanParsable<T>
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

    internal T? TryParse<T>(int index) where T : struct, ISpanParsable<T> =>
        TryParse<T>(index, out var value) ? value : null;

    internal bool TryParse<T>(int index, out T value) where T : ISpanParsable<T>
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
    #endregion

    #region Cols

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
            span[i] = ToString(colIndices[i]);
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
        SepCheck.LengthSameAsCols(colIndices.Length, nameof(span), span.Length);
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
        SepCheck.LengthSameAsCols(colIndices.Length, nameof(span), span.Length);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = TryParse<T>(colIndices[i]);
        }
    }

    internal Span<T> Select<T>(ReadOnlySpan<int> colIndices, ColFunc<T> selector)
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

    internal unsafe Span<T> Select<T>(ReadOnlySpan<int> colIndices, delegate*<Col, T> selector)
    {
        var length = colIndices.Length;
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(length);
        for (var i = 0; i < length; i++)
        {
            span[i] = selector(new(this, colIndices[i]));
        }
        return span;
    }
    #endregion

    protected virtual void DisposeManaged()
    {
        ArrayPool<char>.Shared.Return(_chars);
        ArrayPool<int>.Shared.Return(_colEnds);
        _arrayPool.Dispose();
        foreach (var toString in _colToStrings)
        {
            toString.Dispose();
        }
    }

    #region Dispose
    bool _disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DisposeManaged();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion Dispose
}
