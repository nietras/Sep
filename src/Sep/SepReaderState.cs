﻿using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static nietras.SeparatedValues.SepReader;

namespace nietras.SeparatedValues;

internal readonly record struct RowInfo(int LineNumberFrom, int LineNumberTo, int ColCount);

// Cannot be nested due to CS0146: Circular base type dependency
public class SepReaderState : IDisposable
{
    // To avoid `call     CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE`,
    // promote cache to member here.
    readonly string[] _singleCharToString = SepStringCache.SingleCharToString;

    internal SepHeader _header = null!;
    internal bool _hasHeader;
    internal char _fastFloatDecimalSeparatorOrZero;
    internal CultureInfo? _cultureInfo;
    internal SepCreateToString _createToString = default!;

    internal char[] _chars = Array.Empty<char>();
    internal int _charsDataStart = 0;
    internal int _charsDataEnd = 0;

    internal int _charsParseStart = 0;

    internal int _parseLineNumber = 1;

    internal int _currentRowCharsStartIndex = 0;
    internal int _currentRowColEndsOrInfosStartIndex = 0;
    internal int _currentRowColCount = 0;
    internal int _currentRowLineNumberFrom = 1;

    readonly internal uint _colUnquoteUnescape = 0;
    internal int _colCountExpected = -1;
    internal const int ColEndsInitialLength = 128;
    // Multiple rows of format
    // [0] = Previous row/col end e.g. one before row/first col start
    // [1..ColCount] = Col ends/infos e.g. [1] = first col end/info
    // Length = Sum of All Rows (ColCount + 1) and Current Row
    internal int[] _colEndsOrColInfos = Array.Empty<int>();
    internal int _colEndsOrColInfosTotalCount = 0;

    internal const int ParsedRowsInitialLength = 128;
    internal RowInfo[] _parsedRows = ArrayPool<RowInfo>.Shared.Rent(ParsedRowsInitialLength);
    internal int _parsedRowsCount = 0;

    internal int _parsedRowIndex = 0;

    internal int _parsedRowColCount = -1; // Must be minus one for start
    internal int _parsedRowColEndsOrInfosOffset = 0;
    internal int _parsedRowLineNumberFrom = 1;
    internal int _parsedRowLineNumberTo = 1;

    internal int _rowIndex = -1;

#pragma warning disable CA2213 // Disposable fields should be disposed
    internal SepArrayPoolAccessIndexed _arrayPool = null!;
    internal SepToString _toString = null!;
#pragma warning restore CA2213 // Disposable fields should be disposed
    internal (string colName, int colIndex)[] _colNameCache = Array.Empty<(string colName, int colIndex)>();
    internal int _cacheIndex = 0;

    internal SepReaderState(bool colUnquoteUnescape = false) { _colUnquoteUnescape = colUnquoteUnescape ? 1u : 0u; }

    internal SepReaderState(SepReader other)
        : this(other._colUnquoteUnescape != 0)
    {
        InitializeFrom(other);
        InitializeFromWithNewCacheState(other);
    }

    internal SepReaderState CloneWithSharedCache()
    {
        var clone = new SepReaderState();
        clone.InitializeFrom(this);
        clone._arrayPool = _arrayPool;
        clone._colNameCache = _colNameCache;
        clone._toString = _toString;
        return clone;
    }

    void InitializeFrom(SepReaderState other)
    {
        _header = other._header;
        _fastFloatDecimalSeparatorOrZero = other._fastFloatDecimalSeparatorOrZero;
        System.Diagnostics.Debug.Assert(_fastFloatDecimalSeparatorOrZero != '\0');
        _cultureInfo = other._cultureInfo;
        _createToString = other._createToString;

        _colEndsOrColInfos = ArrayPool<int>.Shared.Rent(other._colEndsOrColInfos.Length);
        _colCountExpected = other._colCountExpected;
    }

    void InitializeFromWithNewCacheState(SepReaderState other)
    {
        _arrayPool = new();
        _colNameCache = new (string colName, int colIndex)[other._colNameCache.Length];
        _toString = _createToString(_header, _colCountExpected);
    }

    internal void CopyNewRowTo(SepReaderState other)
    {
        other._cacheIndex = 0;
        other._arrayPool.Reset();

        other._parsedRowColCount = _parsedRowColCount;
        other._parsedRowColEndsOrInfosOffset = _parsedRowColEndsOrInfosOffset;
        other._parsedRowLineNumberFrom = _parsedRowLineNumberFrom;
        other._parsedRowLineNumberTo = _parsedRowLineNumberTo;

        other._rowIndex = _rowIndex;
        other._parseLineNumber = _parseLineNumber;

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

        if (_colUnquoteUnescape == 0)
        {
            // Copy starts at 0 index so need to fix up col ends
            var colCount = _parsedRowColCount;
            if (colCount > 0)
            {
                var thisColEnds = _colEndsOrColInfos;
                var start = thisColEnds[0] + 1; // +1 since previous end
                var otherColEnds = other._colEndsOrColInfos;
                for (var col = 0; col <= colCount; col++)
                {
                    otherColEnds[col] = thisColEnds[col] - start;
                }
                // Assume colEnds length is divisible by Vector128<int>.Length
                // and vectorize col ends fix up if need be
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    internal void ResetSharedCache() => _arrayPool.Reset();

    #region Row
    internal ReadOnlySpan<char> RowSpan()
    {
        var (start, end) = RowStartEnd();
        return new(_chars, start, end - start);
        //if (_parsedRowColCount > 0)
        //{
        //    //if (_colUnquoteUnescape == 0)
        //    //{
        //    //    var colEnds = _colEndsOrColInfos;
        //    //    var start = colEnds[_parsedRowColEndsOrInfosOffset] + 1; // +1 since previous end
        //    //    var end = colEnds[_parsedRowColEndsOrInfosOffset + _parsedRowColCount];
        //    //    return new(_chars, start, end - start);
        //    //}
        //    //else
        //    //{
        //    //    ref var colInfos = ref Unsafe.As<int, SepColInfo>(ref MemoryMarshal.GetArrayDataReference(_colEndsOrColInfos));
        //    //    colInfos = ref Unsafe.Add(ref colInfos, _parsedRowColEndsOrInfosOffset);
        //    //    var start = colInfos.ColEnd + 1; // +1 since previous end
        //    //    var end = Unsafe.Add(ref colInfos, _parsedRowColCount).ColEnd;
        //    //    return new(_chars, start, end - start);
        //    //}
        //}
        //else
        //{
        //    return default;
        //}
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal (int start, int end) RowStartEnd()
    {
        if (_parsedRowColCount > 0)
        {
            if (_colUnquoteUnescape == 0)
            {
                var colEnds = _colEndsOrColInfos;
                var start = colEnds[_parsedRowColEndsOrInfosOffset] + 1; // +1 since previous end
                var end = colEnds[_parsedRowColEndsOrInfosOffset + _parsedRowColCount];
                return new(start, end);
            }
            else
            {
                ref var colInfos = ref Unsafe.As<int, SepColInfo>(ref MemoryMarshal.GetArrayDataReference(_colEndsOrColInfos));
                colInfos = ref Unsafe.Add(ref colInfos, _parsedRowColEndsOrInfosOffset);
                var start = colInfos.ColEnd + 1; // +1 since previous end
                var end = Unsafe.Add(ref colInfos, _parsedRowColCount).ColEnd;
                return new(start, end);
            }
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
        if ((uint)index >= (uint)_parsedRowColCount) { SepThrow.IndexOutOfRangeException(); }
        index += _parsedRowColEndsOrInfosOffset;
        if (_colUnquoteUnescape == 0)
        {
            // Using array indexing is slightly faster despite more code 🤔
            var colEnds = _colEndsOrColInfos;
            var colStart = colEnds[index] + 1; // +1 since previous end
            var colEnd = colEnds[index + 1];
            // Above bounds checked is faster than below 🤔
            //ref var colEndsRef = ref MemoryMarshal.GetArrayDataReference(_colEndsOrColInfos);
            //var colStart = Unsafe.Add(ref colEndsRef, index) + 1; // +1 since previous end
            //var colEnd = Unsafe.Add(ref colEndsRef, index + 1);

            var colLength = colEnd - colStart;
            // Much better code generation given col span always inside buffer
            ref var colRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_chars), colStart);
            var col = MemoryMarshal.CreateReadOnlySpan(ref colRef, colLength);
            return col;
        }
        else // Unquote/Unescape
        {
            ref var colInfos = ref Unsafe.As<int, SepColInfo>(ref MemoryMarshal.GetArrayDataReference(_colEndsOrColInfos));
            var colStart = Unsafe.Add(ref colInfos, index).ColEnd + 1; // +1 since previous end
            ref var colInfo = ref Unsafe.Add(ref colInfos, index + 1);
            var (colEnd, quoteCountOrNegativeUnescapedLength) = colInfo;
            var colLength = colEnd - colStart;
            ref var colRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_chars), colStart);
            // Unescape if quotes found, negative and col has already been
            // unescaped and the count is instead the new col length.
            if (quoteCountOrNegativeUnescapedLength == 0 ||
                (quoteCountOrNegativeUnescapedLength > 0 && colRef != SepDefaults.Quote))
            {
                return MemoryMarshal.CreateReadOnlySpan(ref colRef, colLength);
            }
            // From now on it is known the first char in col is a quote if not
            // already escaped. Optimize for common case of outermost quotes.
            else if (quoteCountOrNegativeUnescapedLength == 2 &&
                     Unsafe.Add(ref colRef, colLength - 1) == SepDefaults.Quote)
            {
                return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref colRef, 1), colLength - 2);
            }
            else if (quoteCountOrNegativeUnescapedLength < 0)
            {
                var unescapedLength = -quoteCountOrNegativeUnescapedLength;
                return MemoryMarshal.CreateReadOnlySpan(ref colRef, unescapedLength);
            }
            else
            {
                // Unescape fully and in-place
                var unescapedLength = SepUnescape.UnescapeInPlace(ref colRef, colLength);
                colInfo.QuoteCount = -unescapedLength;
                return MemoryMarshal.CreateReadOnlySpan(ref colRef, unescapedLength);
            }
        }
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
    #endregion

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

    internal Span<T> Select<T>(int colStart, int colCount, ColFunc<T> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(colCount);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = selector(new(this, i + colStart));
        }
        return span;
    }

    internal unsafe Span<T> Select<T>(int colStart, int colCount, delegate*<Col, T> selector)
    {
        var span = _arrayPool.RentUniqueArrayAsSpan<T>(colCount);
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = selector(new(this, i + colStart));
        }
        return span;
    }
    #endregion

    [ExcludeFromCodeCoverage]
    internal Span<T> GetColsEntireSpanAs<T>() where T : unmanaged =>
        MemoryMarshal.CreateSpan(ref GetColsRefAs<T>(), GetColInfosLength<T>());
    [ExcludeFromCodeCoverage]
    internal int GetColInfosLength<T>() where T : unmanaged =>
        _colEndsOrColInfos.Length / (Unsafe.SizeOf<T>() / sizeof(int));

    internal int GetColInfosLength() =>
        _colEndsOrColInfos.Length / GetIntegersPerColInfo();

    internal int GetIntegersPerColInfo() =>
        _colUnquoteUnescape == 0 ? 1 : Unsafe.SizeOf<SepColInfo>() / sizeof(int);

    internal ref T GetColsRefAs<T>() where T : unmanaged
    {
        A.Assert(Unsafe.SizeOf<T>() % sizeof(int) == 0);
        ref var colEndsOrColInfosRef = ref MemoryMarshal.GetArrayDataReference(_colEndsOrColInfos);
        return ref Unsafe.As<int, T>(ref colEndsOrColInfosRef);
    }

    internal virtual void DisposeManaged()
    {
        ArrayPool<char>.Shared.Return(_chars);
        ArrayPool<int>.Shared.Return(_colEndsOrColInfos);
        _arrayPool.Dispose();
        _toString?.Dispose();
    }

    #region Dispose
    bool _disposed;
#pragma warning disable CA1063 // Implement IDisposable Correctly
    void Dispose(bool disposing)
#pragma warning restore CA1063 // Implement IDisposable Correctly
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
