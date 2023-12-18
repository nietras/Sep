using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static nietras.SeparatedValues.SepReader;

namespace nietras.SeparatedValues;

internal readonly record struct SepRowInfo(int LineNumberTo, int ColCount);

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

    internal int _parsingLineNumber = 1;
    internal int _parsingRowCharsStartIndex = 0;
    internal int _parsingRowColEndsOrInfosStartIndex = 0;
    internal int _parsingRowColCount = 0;

    readonly internal uint _colUnquoteUnescape = 0;
    internal int _colCountExpected = -1;
#if DEBUG
    internal const int ColEndsInitialLength = 128;
#else
    internal const int ColEndsInitialLength = 8 * 1024;
#endif
    // Multiple rows of format
    // [0] = Previous row/col end e.g. one before row/first col start
    // [1..ColCount] = Col ends/infos e.g. [1] = first col end/info
    // Length = Sum of All Rows (ColCount + 1) and Current Row
    internal int[] _colEndsOrColInfos = Array.Empty<int>();

#if DEBUG
    internal const int ParsedRowsLength = 3;
#else
    internal const int ParsedRowsLength = 512;
#endif
    internal SepRowInfo[] _parsedRows = ArrayPool<SepRowInfo>.Shared.Rent(ParsedRowsLength);
    internal int _parsedRowsCount = 0;
    internal int _parsedRowIndex = 0;

    internal int _currentRowColCount = -1; // Must be minus one for start
    internal int _currentRowColEndsOrInfosOffset = 0;
    internal int _currentRowLineNumberFrom = 1;
    internal int _currentRowLineNumberTo = 1;

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

    internal void CopyParsedRowsTo(SepReaderState other)
    {
        A.Assert(_parsedRowIndex == 0);
        A.Assert(_parsedRowIndex <= _parsedRowsCount);

        other._parsedRowIndex = _parsedRowIndex;
        other._parsedRowsCount = _parsedRowsCount;
        // Below is perhaps not needed
        other._currentRowColCount = _currentRowColCount;
        other._currentRowColEndsOrInfosOffset = _currentRowColEndsOrInfosOffset;
        other._currentRowLineNumberFrom = _currentRowLineNumberFrom;
        other._currentRowLineNumberTo = _currentRowLineNumberTo;

        other._parsingLineNumber = _parsingLineNumber;

        other._charsDataStart = _charsDataStart;
        other._charsDataEnd = _charsDataEnd;
        other._charsParseStart = _charsParseStart;

        other._parsingRowCharsStartIndex = _parsingRowCharsStartIndex;
        other._parsingRowColEndsOrInfosStartIndex = _parsingRowColEndsOrInfosStartIndex;
        other._parsingRowColCount = _parsingRowColCount;

        // Swap buffers
        (other._chars, _chars) = (_chars, other._chars);
        (other._colEndsOrColInfos, _colEndsOrColInfos) = (_colEndsOrColInfos, other._colEndsOrColInfos);
        (other._parsedRows, _parsedRows) = (_parsedRows, other._parsedRows);
        // Try swap tostring instance to avoid allocations
        (other._toString, _toString) = (_toString, other._toString);

        // Ensure buffers on this are still allocated correctly after swap
        EnsureArrayFromPoolHasMinimumLength(ref _chars, other._chars.Length);
        EnsureArrayFromPoolHasMinimumLength(ref _colEndsOrColInfos, other._colEndsOrColInfos.Length);
        EnsureArrayFromPoolHasMinimumLength(ref _parsedRows, other._parsedRows.Length);

        // Copy data for perhaps incomplete row after parsed rows
        Array.Copy(other._chars, _parsingRowCharsStartIndex, _chars, _parsingRowCharsStartIndex, _charsDataEnd - _parsingRowCharsStartIndex);
        var intsPerColInfo = GetIntegersPerColInfo();
        var colInfosStart = _parsingRowColEndsOrInfosStartIndex * intsPerColInfo;
        var colInfosCount = (_parsingRowColCount + 1) * intsPerColInfo;
        Array.Copy(other._colEndsOrColInfos, colInfosStart, _colEndsOrColInfos, colInfosStart, colInfosCount);

        // Ensure state
        _rowIndex += _parsedRowsCount;
        _parsedRowIndex = 0;
        _parsedRowsCount = 0;
    }

    static void EnsureArrayFromPoolHasMinimumLength<T>(ref T[] array, int minimumLength)
    {
        if (minimumLength > array.Length)
        {
            if (array.Length > 0) { ArrayPool<T>.Shared.Return(array); }
            array = ArrayPool<T>.Shared.Rent(minimumLength);
        }
#if DEBUG
        Array.Clear(array);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool MoveNextAlreadyParsed()
    {
        if (_parsedRowIndex < _parsedRowsCount)
        {
            _cacheIndex = 0;
            _arrayPool.Reset();
            ++_rowIndex;

            ref readonly var info = ref _parsedRows[_parsedRowIndex];
            var colCount = info.ColCount;
            _currentRowColEndsOrInfosOffset += _currentRowColCount + 1; // +1 since one more for start col
            _currentRowColCount = colCount;
            _currentRowLineNumberFrom = _currentRowLineNumberTo;
            _currentRowLineNumberTo = info.LineNumberTo;

            ++_parsedRowIndex;
            if (_colCountExpected >= 0 && colCount != _colCountExpected)
            {
                ThrowInvalidDataExceptionColCountMismatch(_colCountExpected);
            }
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void ThrowInvalidDataExceptionColCountMismatch(int colCountExpected)
    {
        var (rowStart, rowEnd) = RowStartEnd();
        ThrowInvalidDataExceptionColCountMismatch(colCountExpected, rowStart, rowEnd);
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    void ThrowInvalidDataExceptionColCountMismatch(int colCountExpected, int rowStart, int rowEnd)
    {
        AssertState(rowStart, rowEnd);
        var row = new string(_chars, rowStart, rowEnd - rowStart);
        SepThrow.InvalidDataException_ColCountMismatch(_currentRowColCount, _rowIndex, _currentRowLineNumberFrom, _currentRowLineNumberTo, row,
            colCountExpected, _header.ToString());

        [ExcludeFromCodeCoverage]
        void AssertState(int rowStart, int rowEnd)
        {
            A.Assert(_charsDataStart <= rowStart && rowEnd <= _charsDataEnd, $"Row not within data range {_charsDataStart} <= {rowStart} && {rowEnd} <= {_charsDataEnd}");
            A.Assert(rowEnd >= rowStart, $"Row end before row start {rowEnd} >= {rowStart}");
        }
    }


    #region Row
    internal ReadOnlySpan<char> RowSpan()
    {
        var (start, end) = RowStartEnd();
        return new(_chars, start, end - start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal (int start, int end) RowStartEnd()
    {
        if (_currentRowColCount > 0)
        {
            if (_colUnquoteUnescape == 0)
            {
                var colEnds = _colEndsOrColInfos;
                var start = colEnds[_currentRowColEndsOrInfosOffset] + 1; // +1 since previous end
                var end = colEnds[_currentRowColEndsOrInfosOffset + _currentRowColCount];
                return new(start, end);
            }
            else
            {
                ref var colInfos = ref Unsafe.As<int, SepColInfo>(ref MemoryMarshal.GetArrayDataReference(_colEndsOrColInfos));
                colInfos = ref Unsafe.Add(ref colInfos, _currentRowColEndsOrInfosOffset);
                var start = colInfos.ColEnd + 1; // +1 since previous end
                var end = Unsafe.Add(ref colInfos, _currentRowColCount).ColEnd;
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
        if ((uint)index >= (uint)_currentRowColCount) { SepThrow.IndexOutOfRangeException(); }
        A.Assert(_currentRowColEndsOrInfosOffset >= 0);
        index += _currentRowColEndsOrInfosOffset;
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

            A.Assert(colStart >= 0);
            A.Assert(colEnd < _chars.Length);
            A.Assert(colEnd >= colStart);

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

            A.Assert(colStart >= 0);
            A.Assert(colEnd < _chars.Length);
            A.Assert(colEnd >= colStart);

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
        ArrayPool<SepRowInfo>.Shared.Return(_parsedRows);
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
