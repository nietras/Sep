using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class SepUtf8Reader : SepUtf8ReaderState
{
    internal readonly record struct Info(object Source, Func<Info, string> DebuggerDisplay);
    internal string DebuggerDisplay => _info.DebuggerDisplay(_info);

    readonly Info _info;
    byte _separator;
    readonly bool _disableQuotesParsing;
    readonly Stream _stream;
    ISepParser<byte, SepCharInfoUtf8>? _parser;

    readonly int _charsMinimumFreeLength;
    int _charsPaddingLength;
    readonly bool _skipBom;
    bool _bomChecked;

    internal SepUtf8Reader(Info info, in SepUtf8ReaderOptions options, Stream stream)
        : base(colUnquoteUnescape: options.Unescape, trim: options.Trim)
    {
        _info = info;
        _stream = stream;
        _cultureInfo = options.CultureInfo;
        _createToString = options.CreateToString;
        _createToBytes = options.CreateToBytes;
        _disableQuotesParsing = options.DisableQuotesParsing;
        _skipBom = options.SkipBom;
        _arrayPool = new();

        var decimalSeparator = _cultureInfo?.NumberFormat.CurrencyDecimalSeparator ??
            System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator;
        _fastFloatDecimalSeparatorOrZero =
            decimalSeparator.Length == 1 && !options.DisableFastFloat
            ? (byte)decimalSeparator[0]
            : (byte)0;

        long? maybeStreamLength = null;
        try { if (_stream.CanSeek) { maybeStreamLength = _stream.Length; } }
        catch { /* ignore */ }

        var sep = options.Sep;
        if (sep.HasValue)
        {
            _parser = SepUtf8ParserFactory.Create(new(sep.Value, _disableQuotesParsing));
            _charsPaddingLength = _parser.PaddingLength;
            _separator = (byte)sep.Value.Separator;
        }
        else
        {
            _charsPaddingLength = 128; // SepParserFactory.MaxPaddingLength
        }

        var initialBufferLength = Math.Max(options.InitialBufferLength, _charsPaddingLength);
        var bufferLength = maybeStreamLength.HasValue
            ? ((maybeStreamLength.Value < initialBufferLength)
               ? ((int)maybeStreamLength.Value + _charsPaddingLength) : initialBufferLength)
            : initialBufferLength;

        _chars = ArrayPool<byte>.Shared.Rent(bufferLength);
        _charsMinimumFreeLength = Math.Max(_chars.Length / 2, _charsPaddingLength);
        _colEndsOrColInfos = ArrayPool<int>.Shared.Rent(Math.Max(ColEndsInitialLength, _charsPaddingLength * 2));
    }

    public bool IsEmpty { get; private set; }
    public SepSpec Spec => new(new((char)_separator), _cultureInfo, false);
    public bool HasHeader { get => _hasHeader; private set => _hasHeader = value; }
    public bool HasRows { get; private set; }
    public SepReaderHeader Header => _header;

    public Row Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    public SepUtf8Reader GetEnumerator() => this;

    public string ToString(int index) => ToStringDefault(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasParsedRows() => _parsedRowIndex < _parsedRowsCount;

    internal void Initialize(in SepUtf8ReaderOptions options)
    {
        // Parse first row/header
        if (MoveNext())
        {
            A.Assert(_parsedRowsCount > 0);

            IsEmpty = false;
            var firstRowColCount = _parsedRows[0].ColCount;
            _colCountExpected = firstRowColCount;
            if (options.HasHeader)
            {
                var headerRow = Encoding.UTF8.GetString(RowSpan());
                var colNameComparer = options.ColNameComparer;
                var colNameToIndex = new System.Collections.Generic.Dictionary<string, int>(
                    firstRowColCount, colNameComparer);
                for (var colIndex = 0; colIndex < firstRowColCount; colIndex++)
                {
                    var colName = ToStringDirect(colIndex);
                    if (!colNameToIndex.TryAdd(colName, colIndex))
                    { SepThrow.ArgumentException_DuplicateColNamesFound(this, colNameToIndex, colName, firstRowColCount, colNameComparer, headerRow); }
                }
                _header = new(headerRow, colNameToIndex);

                HasHeader = true;

                HasRows = _parsedRowsCount > 1 || _charsDataEnd > _charsParseStart || _parsingRowColCount > 0;
                if (!HasRows)
                {
                    HasRows = !FillAndMaybeDoubleBuffer(_charsPaddingLength);
                }
            }
            else
            {
                --_rowIndex;
                --_parsedRowIndex;
                _currentRowColEndsOrInfosOffset = 0;
                _currentRowColCount = -1;
                A.Assert(_rowIndex == -1);
                A.Assert(_parsedRowIndex == 0);
                HasHeader = false;
                HasRows = true;
            }
            A.Assert(_separator != 0);
        }
        else
        {
            IsEmpty = true;
            HasHeader = false;
            HasRows = false;
            _colCountExpected = 0;
            _separator = (byte)Sep.Default.Separator;
        }

        _colNameCache = new (string colName, int colIndex)[_colCountExpected];

        _toString = options.CreateToString(_header, _colCountExpected);
        _toBytes = options.CreateToBytes(_header, _colCountExpected);

        _header ??= SepReaderHeader.Empty;

        _colCountExpected = options.DisableColCountCheck ? -1 : _colCountExpected;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        do
        {
            if (MoveNextAlreadyParsed()) { return true; }
        } while (ParseNewRows());
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal bool ParseNewRows()
    {
        _parsedRowsCount = 0;
        _parsedRowIndex = 0;
        _currentRowColCount = -1;
        _currentRowColEndsOrInfosOffset = 0;

        // Move data to start
        if (_parsingRowCharsStartIndex > 0)
        {
            A.Assert(_parser != null);
            var offset = _chars.MoveDataToStart(ref _parsingRowCharsStartIndex, ref _charsDataEnd, _parser.PaddingLength);

            _charsParseStart -= offset;

            A.Assert((_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount + 1) * GetIntegersPerColInfo() <= _colEndsOrColInfos.Length);

            if (_colUnquoteUnescape == 0)
            {
                ref var colEndsRef = ref GetColsRefAs<int>();
                for (var i = 0; i <= _parsingRowColCount; i++)
                {
                    ref var colEnd = ref Unsafe.Add(ref colEndsRef, i + _parsingRowColEndsOrInfosStartIndex);
                    colEnd -= offset;
                }
            }
            else
            {
                ref var colInfosRef = ref GetColsRefAs<SepColInfo>();
                for (var i = 0; i <= _parsingRowColCount; i++)
                {
                    ref var colInfo = ref Unsafe.Add(ref colInfosRef, i + _parsingRowColEndsOrInfosStartIndex);
                    colInfo.ColEnd -= offset;
                }
            }
        }
        if (_parsingRowColEndsOrInfosStartIndex > 0)
        {
            var intsPerColInfo = GetIntegersPerColInfo();
            var colInfosSpan = _colEndsOrColInfos.AsSpan();
            var length = (_parsingRowColCount + 1) * intsPerColInfo;
            var source = colInfosSpan.Slice(_parsingRowColEndsOrInfosStartIndex * intsPerColInfo, length);
            var destination = colInfosSpan.Slice(0, length);
            source.CopyTo(destination);
            _parsingRowColEndsOrInfosStartIndex = 0;
        }

        _colEndsOrColInfos[0] = -1;
        if (_colUnquoteUnescape != 0)
        {
            _colEndsOrColInfos[1] = 0;
        }

        _charsDataStart = 0;

#if DEBUG
        _colEndsOrColInfos.AsSpan().Slice((_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount) * GetIntegersPerColInfo() + GetIntegersPerColInfo()).Fill(-42);
#endif

        var endOfFile = false;
    LOOP:
        if (_parser is not null)
        {
            if (_colUnquoteUnescape == 0) { _parser.ParseColEnds(this); }
            else { _parser.ParseColInfos(this); }
        }

        if (endOfFile) { goto RETURN; }

        if (_parsedRowsCount > 0) { return true; }

        endOfFile = EnsureInitializeAndReadData(endOfFile);
        if (endOfFile && _parsingRowCharsStartIndex < _charsDataEnd && _charsParseStart == _charsDataEnd)
        {
            ++_parsingRowColCount;
            var colInfoIndex = _parsingRowColEndsOrInfosStartIndex + _parsingRowColCount;
            if (_colUnquoteUnescape == 0)
            {
                _colEndsOrColInfos[colInfoIndex] = _charsDataEnd;
            }
            else
            {
                Debug.Assert(_parser is not null);
                var quoteCount = _parser.QuoteCount;
                Unsafe.Add(ref Unsafe.As<int, SepColInfo>(ref MemoryMarshal.GetArrayDataReference(_colEndsOrColInfos)), colInfoIndex) =
                    new(_charsDataEnd, quoteCount);
            }
            ++_parsingLineNumber;
            _parsedRows[_parsedRowsCount] = new(_parsingLineNumber, _parsingRowColCount);
            ++_parsedRowsCount;

            _parsingRowColCount = 0;
            _parsingRowColEndsOrInfosStartIndex = colInfoIndex + 1;
            _parsingRowCharsStartIndex = _charsDataEnd;
            goto RETURN;
        }
        if (_parsedRowsCount <= 0) { goto LOOP; }
    RETURN:
        return _parsedRowsCount > 0;
    }

    bool EnsureInitializeAndReadData(bool endOfFile)
    {
        var nothingLeftToRead = FillAndMaybeDoubleBuffer(_charsPaddingLength);
        if (_parser == null)
        {
            TryDetectSeparatorInitializeParser(nothingLeftToRead);
        }

        if (_parser == null || _charsParseStart >= _charsDataEnd)
        {
            if (nothingLeftToRead)
            {
                CheckColInfosCapacityMaybeDouble(paddingLength: 0);
                endOfFile = true;
            }
        }
        else
        {
            CheckColInfosCapacityMaybeDouble(_parser.PaddingLength);
        }
        return endOfFile;
    }

    void CheckColInfosCapacityMaybeDouble(int paddingLength)
    {
        var parsingRowColInfosEnd = _parsingRowColEndsOrInfosStartIndex + _parsingRowColCount;
        var colInfosPotentialEnd = parsingRowColInfosEnd + paddingLength + ColEndsOrInfosExtraEndCount;
        var colInfosLength = GetColInfosLength();
        if (colInfosLength < colInfosPotentialEnd)
        {
            DoubleColInfosCapacityCopyState();
        }
    }

    void DoubleColInfosCapacityCopyState()
    {
        var previousColEnds = _colEndsOrColInfos;
        _colEndsOrColInfos = ArrayPool<int>.Shared.Rent(_colEndsOrColInfos.Length * 2);

        var factor = GetIntegersPerColInfo();
        A.Assert(factor > 0);
        var lengthInIntegers = (_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount + 1) * factor;

        var previousColEndsSpan = previousColEnds.AsSpan().Slice(0, lengthInIntegers);
        var newColEndsSpan = _colEndsOrColInfos.AsSpan().Slice(0, lengthInIntegers);
        previousColEndsSpan.CopyTo(newColEndsSpan);
        ArrayPool<int>.Shared.Return(previousColEnds);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void TryDetectSeparatorInitializeParser(bool nothingLeftToRead)
    {
        var validBytes = _chars.AsSpan(_charsDataStart.._charsDataEnd);
        // Convert to chars for sep detection (reuse existing logic)
        Span<char> charBuffer = stackalloc char[Math.Min(validBytes.Length, 1024)];
        var charCount = Encoding.UTF8.GetChars(validBytes.Slice(0, charBuffer.Length), charBuffer);
        var maybeSep = SepReaderExtensions.DetectSep(charBuffer.Slice(0, charCount), nothingLeftToRead, _disableQuotesParsing);
        if (maybeSep.HasValue)
        {
            var sep = maybeSep.Value;
            _separator = (byte)sep.Separator;
            _parser = SepUtf8ParserFactory.Create(new(sep, _disableQuotesParsing));
            _charsPaddingLength = _parser.PaddingLength;
        }
    }

    bool FillAndMaybeDoubleBuffer(int paddingLength)
    {
        A.Assert(_charsDataStart == 0);

        var offset = SepArrayExtensions.CheckFreeMaybeDoubleLength(
            ref _chars, ref _charsDataStart, ref _charsDataEnd,
            _charsMinimumFreeLength, paddingLength);
        if (_chars.Length > RowLengthMax)
        { SepThrow.NotSupportedException_BufferOrRowLengthExceedsMaximumSupported(RowLengthMax); }
        A.Assert(offset == 0);

        var freeLength = _chars.Length - _charsDataEnd - paddingLength;
        freeLength -= 1;

        var freeBytes = new Span<byte>(_chars, _charsDataEnd, freeLength);
        A.Assert(freeLength > 0, $"Free span at end of buffer length {freeLength} not greater than 0");

        var totalReadCount = 0;
        var readCount = 0;

        if (_trailingCarriageReturn)
        {
            freeBytes[0] = CarriageReturnByte;
            _trailingCarriageReturn = false;
            ++totalReadCount;
            ++_charsDataEnd;
        }

        while (totalReadCount < freeLength &&
               ((readCount = _stream.Read(freeBytes.Slice(totalReadCount))) > 0))
        {
            _charsDataEnd += readCount;
            totalReadCount += readCount;
            if (_chars[_charsDataEnd - 1] == CarriageReturnByte)
            {
                var extraReadBytes = _chars.AsSpan(_charsDataEnd, length: 1);
                var extraReadCount = _stream.Read(extraReadBytes);
                if (extraReadCount > 0)
                {
                    A.Assert(extraReadCount == 1);
                    var readByte = extraReadBytes[0];
                    if (readByte != CarriageReturnByte)
                    {
                        ++_charsDataEnd;
                        ++totalReadCount;
                    }
                    else
                    {
                        _chars[_charsDataEnd] = 0;
                        _trailingCarriageReturn = true;
                    }
                }
            }
        }

        // Skip BOM if present (first read only)
        if (!_bomChecked && _skipBom && totalReadCount > 0)
        {
            _bomChecked = true;
            if (_charsDataEnd >= 3 &&
                _chars[0] == 0xEF && _chars[1] == 0xBB && _chars[2] == 0xBF)
            {
                // Skip UTF-8 BOM
                _charsDataStart = 3;
                _charsParseStart = 3;
                _parsingRowCharsStartIndex = 3;
                // Adjust initial sentinel so first col starts after BOM
                _colEndsOrColInfos[0] = 2; // BOM length - 1
                if (_colUnquoteUnescape != 0)
                {
                    Unsafe.As<int, SepColInfo>(ref MemoryMarshal.GetArrayDataReference(_colEndsOrColInfos)) =
                        new(2, 0);
                }
            }
        }

        if (paddingLength > 0)
        {
            _chars.ClearPaddingAfterData(_charsDataEnd, paddingLength);
        }
        return totalReadCount == 0 && !_trailingCarriageReturn;
    }

    internal override void DisposeManaged()
    {
        _stream.Dispose();
        base.DisposeManaged();
    }
}
