using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class SepReader : SepReaderState
{
    internal readonly record struct Info(object Source, Func<Info, string> DebuggerDisplay);
    internal string DebuggerDisplay => _info.DebuggerDisplay(_info);

    const string TraceCondition = "SEPREADERTRACE";
    const string AssertCondition = "SEPREADERASSERT";

    readonly Info _info;
    char _separator;
    readonly TextReader _reader;
    ISepParser? _parser;

#if DEBUG
    // To increase probability of detecting bugs start with short length to
    // force buffer management paths to be used.
    internal const int CharsMinimumLength = 64;
#else
    // Based on L1d typically being 32KB-48KB, so aiming for 16K-24K x sizeof(char).
    // Benchmarks show below to be a good minimum length.
    // Currently rented on ArrayPool and will be rounded up to nearest power of 2.
    internal const int CharsMinimumLength = 24 * 1024;
#endif
    readonly int _charsMinimumFreeLength;
    int _charsPaddingLength;

    internal SepReader(Info info, SepReaderOptions options, TextReader reader)
        : base(colUnquoteUnescape: options.Unescape)
    {
        _info = info;
        _reader = reader;
        _cultureInfo = options.CultureInfo;
        _createToString = options.CreateToString;
        _arrayPool = new();

        var decimalSeparator = _cultureInfo?.NumberFormat.CurrencyDecimalSeparator ??
            System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator;
        _fastFloatDecimalSeparatorOrZero =
            decimalSeparator.Length == 1 && !options.DisableFastFloat
            ? decimalSeparator[0]
            : '\0';

        var guessPaddingLength = 128;
        int? maybeReaderLengthEstimate = TryGetTextReaderLength(_reader, out var longReaderLength)
            // TextReader length can be greater than int.MaxValue so have to
            // constrain it to avoid overflow.
            ? (int)Math.Min(longReaderLength, int.MaxValue) : null;
        var bufferLength = maybeReaderLengthEstimate.HasValue
            ? ((maybeReaderLengthEstimate.Value < CharsMinimumLength)
               ? (maybeReaderLengthEstimate.Value + guessPaddingLength) : CharsMinimumLength)
            : CharsMinimumLength;

        _chars = ArrayPool<char>.Shared.Rent(bufferLength);

        var sep = options.Sep;
        if (sep.HasValue)
        {
            _parser = SepParserFactory.CreateBest(sep.Value);
            _charsPaddingLength = _parser.PaddingLength;
            _charsMinimumFreeLength = Math.Max(_chars.Length / 2, _charsPaddingLength);
            _separator = sep.Value.Separator;
        }
        else
        {
            _charsPaddingLength = 64;
            _charsMinimumFreeLength = Math.Max(_chars.Length / 2, _charsPaddingLength);
        }

        var paddingLength = _parser?.PaddingLength ?? 64;

        _colEndsOrColInfos = ArrayPool<int>.Shared.Rent(Math.Max(ColEndsInitialLength, paddingLength * 2));
    }

    public bool IsEmpty { get; private set; }
    public SepSpec Spec => new(new(_separator), _cultureInfo);
    public bool HasHeader { get => _hasHeader; private set => _hasHeader = value; }
    public bool HasRows { get; private set; }
    public SepHeader Header => _header;

    internal int CharsLength => _chars.Length;

    public Row Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    internal void Initialize(SepReaderOptions options)
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
                var colNameToIndex = new Dictionary<string, int>(firstRowColCount);
                for (var colIndex = 0; colIndex < firstRowColCount; colIndex++)
                {
                    var colName = ToStringDirect(colIndex);
                    colNameToIndex.Add(colName, colIndex);
                }
                var headerRow = new string(RowSpan());
                _header = new(headerRow, colNameToIndex);

                HasHeader = true;

                // Check if more data available and hence minimum 1 row after header
                // What if \n after \r after header only? Where \n lingering after MoveNext?
                HasRows = _parsedRowsCount > 1 || _charsDataEnd > _charsParseStart || _parsingRowColCount > 0;
                if (!HasRows)
                {
                    HasRows = !FillAndMaybeDoubleCharsBuffer(_charsPaddingLength);
                }
            }
            else
            {
                // Move back one as no header (since MoveNext called twice then)
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
            // Nothing in file
            IsEmpty = true;
            HasHeader = false;
            HasRows = false;
            _colCountExpected = 0;
            _separator = Sep.Default.Separator;
        }

        _colNameCache = new (string colName, int colIndex)[_colCountExpected];

        // Header may be null here
        _toString = options.CreateToString(_header, _colCountExpected);

        // Use empty header if no header
        _header ??= SepHeader.Empty;

        _colCountExpected = options.DisableColCountCheck ? -1 : _colCountExpected;
    }

    public SepReader GetEnumerator() => this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        do
        {
            if (MoveNextAlreadyParsed())
            {
                return true;
            }
        } while (ParseNewRows());
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasParsedRows() => _parsedRowIndex < _parsedRowsCount;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal bool ParseNewRows()
    {
        _parsedRowsCount = 0;
        _parsedRowIndex = 0;
        _currentRowColCount = -1;
        _currentRowColEndsOrInfosOffset = 0;

        CheckPoint($"{nameof(ParseNewRows)} BEGINNING");

        // Move data to start
        if (_parsingRowCharsStartIndex > 0)
        {
            A.Assert(_parser != null);
            var offset = _chars.MoveDataToStart(ref _parsingRowCharsStartIndex, ref _charsDataEnd, _parser.PaddingLength);

            // Adjust parse start
            A.Assert(_charsParseStart >= offset);
            _charsParseStart -= offset;

            A.Assert((_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount + 1) * GetIntegersPerColInfo() <= _colEndsOrColInfos.Length);

            // Adjust found current row col infos, note includes col count since +1
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

        // Ensure start conditions
        _colEndsOrColInfos[0] = -1;
        if (_colUnquoteUnescape != 0)
        {
            // QuoteCount initialize hack
            _colEndsOrColInfos[1] = 0;
        }

        _charsDataStart = 0;

        // Reset
#if DEBUG
        _colEndsOrColInfos.AsSpan().Slice((_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount) * GetIntegersPerColInfo() + GetIntegersPerColInfo()).Fill(-42);
#endif

        var endOfFile = false;
    LOOP:
        CheckPoint($"{nameof(_parser)} BEFORE");

        if (_parser is not null)
        {
            if (_colUnquoteUnescape == 0) { _parser.ParseColEnds(this); }
            else { _parser.ParseColInfos(this); }
        }

        CheckPoint($"{nameof(_parser)} AFTER");
        if (endOfFile)
        {
            CheckPoint($"{nameof(_parser)} AFTER - ENDOFFILE");
            goto RETURN;
        }

        CheckPoint($"{nameof(_parser)} AFTER");

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
                Unsafe.Add(ref Unsafe.As<int, SepColInfo>(ref MemoryMarshal.GetArrayDataReference(_colEndsOrColInfos)), colInfoIndex) =
                    new(_charsDataEnd, _parser?.QuoteCount ?? 0);
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

    public string ToString(int index) => ToStringDefault(index);

    bool EnsureInitializeAndReadData(bool endOfFile)
    {
        var nothingLeftToRead = FillAndMaybeDoubleCharsBuffer(_charsPaddingLength);
        CheckPoint($"{nameof(FillAndMaybeDoubleCharsBuffer)} AFTER");

        if (_parser == null)
        {
            TryDetectSeparatorInitializeParser(nothingLeftToRead);

            CheckPoint($"{nameof(TryDetectSeparatorInitializeParser)} AFTER");
        }

        if (_parser != null && _charsParseStart < _charsDataEnd)
        {
            if ((_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount + ColEndsOrInfosExtraEndCount) >= (GetColInfosLength() - _parser.PaddingLength))
            {
                DoubleColInfosCapacityCopyState();
            }
        }
        else
        {
            if (nothingLeftToRead)
            {
                if ((_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount + ColEndsOrInfosExtraEndCount) >= GetColInfosLength())
                {
                    DoubleColInfosCapacityCopyState();
                }
                // If nothing has been read, then at end of file.
                endOfFile = true;
            }
        }
        return endOfFile;
    }

    void DoubleColInfosCapacityCopyState()
    {
        var previousColEnds = _colEndsOrColInfos;
        _colEndsOrColInfos = ArrayPool<int>.Shared.Rent(_colEndsOrColInfos.Length * 2);

        var factor = GetIntegersPerColInfo();
        A.Assert(factor > 0);
        // + 1 since one more for start col
        var lengthInIntegers = (_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount + 1) * factor;

        var previousColEndsSpan = previousColEnds.AsSpan().Slice(0, lengthInIntegers);
        var newColEndsSpan = _colEndsOrColInfos.AsSpan().Slice(0, lengthInIntegers);
        previousColEndsSpan.CopyTo(newColEndsSpan);
        ArrayPool<int>.Shared.Return(previousColEnds);
        //Console.WriteLine($"CurrentRowColInfosStartIndex = {_currentRowColEndsOrInfosStartIndex} CurrentRowColCount = {_currentRowColCount} New ColInfos Length = {_colEndsOrColInfos.Length}");
    }

    bool FillAndMaybeDoubleCharsBuffer(int paddingLength)
    {
        A.Assert(_charsDataStart == 0);

        var offset = SepArrayExtensions.CheckFreeMaybeDoubleLength(
            ref _chars, ref _charsDataStart, ref _charsDataEnd,
            _charsMinimumFreeLength, paddingLength);
        if (_chars.Length > RowLengthMax)
        {
            SepThrow.NotSupportedException_BufferOrRowLengthExceedsMaximumSupported(RowLengthMax);
        }
        A.Assert(offset == 0);

        // Read to free buffer area
        var freeLength = _chars.Length - _charsDataEnd - paddingLength;
        // Read 1 less than free length to ensure we always read \n after \r,
        // and hence always ensure we have the two combined in buffer.
        freeLength -= 1;

        if (freeLength > 0)
        {
            var freeSpan = new Span<char>(_chars, _charsDataEnd, freeLength);
            A.Assert(freeLength > 0, $"Free span at end of buffer length {freeLength} not greater than 0");

            // Read until full or no more data
            var totalBytesRead = 0;
            var readCount = 0;
            while (totalBytesRead < freeLength &&
                   ((readCount = _reader.Read(freeSpan.Slice(totalBytesRead))) > 0))
            {
                _charsDataEnd += readCount;
                // Ensure carriage return always followed by line feed
                if (_chars[_charsDataEnd - 1] == CarriageReturn)
                {
                    var extraChar = _reader.Peek();
                    if (extraChar == LineFeed)
                    {
                        var readChar = (char)_reader.Read();
                        A.Assert(extraChar == readChar);
                        _chars[_charsDataEnd] = readChar;
                        ++_charsDataEnd;
                        ++readCount;
                    }
                }
                totalBytesRead += readCount;
            }
            if (paddingLength > 0)
            {
                _chars.ClearPaddingAfterData(_charsDataEnd, paddingLength);
            }
            //Console.WriteLine($"Read: {readCount} BufferSize: {freeSpan.Length} Buffer Length: {_chars.BufferLength}");
            return totalBytesRead == 0;
        }
        else
        {
            return false;
        }
    }

    static bool TryGetTextReaderLength(TextReader reader, out long length)
    {
        // utf8 bytes can only get shorter when converted to characters so
        // if we can determine an underlying stream length, then we can
        // allocate a smaller buffer.
        if (reader is StreamReader sr && sr.CurrentEncoding.CodePage == Encoding.UTF8.CodePage)
        {
            var s = sr.BaseStream;
            if (s.CanSeek)
            {
                length = s.Length;
                return true;
            }
        }
        length = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void TryDetectSeparatorInitializeParser(bool nothingLeftToRead)
    {
        // Detect separator if no parser defined
        var validChars = _chars.AsSpan(_charsDataStart.._charsDataEnd);
        var maybeSep = SepReaderExtensions.DetectSep(validChars, nothingLeftToRead);
        if (maybeSep.HasValue)
        {
            var sep = maybeSep.Value;
            _separator = sep.Separator;
            _parser = SepParserFactory.CreateBest(sep);
            // TODO: Initialize other members
            _charsPaddingLength = _parser.PaddingLength;
        }
    }

    [ExcludeFromCodeCoverage]
    [Conditional(TraceCondition), Conditional(AssertCondition)]
    void CheckPoint(string name, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        TraceState(name, filePath, lineNumber);
        AssertState(name, filePath, lineNumber);
    }

    [ExcludeFromCodeCoverage]
    [Conditional(TraceCondition)]
    void TraceState(string name, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        T.WriteLine($"{filePath}({lineNumber}): {name}");
        T.WriteLine($"{nameof(_chars),-10}:{_chars.Length,5} [{_charsDataStart,4},{_charsDataEnd,4}] ({_charsParseStart,2}) '{FormatValidChars()}'");
        if (_colUnquoteUnescape == 0)
        {
            var colEnds = GetColsEntireSpanAs<int>();
            T.WriteLine($"{nameof(colEnds),-10}:{colEnds.Length,5} [{0,4},{_currentRowColCount,4}] {string.Join(',', colEnds[0..Math.Min(_currentRowColCount, colEnds.Length)].ToArray())}");
        }
        else
        {
            var colInfos = GetColsEntireSpanAs<SepColInfo>();
            T.WriteLine($"{nameof(colInfos),-10}:{colInfos.Length,5} [{0,4},{_currentRowColCount,4}] {string.Join(',', colInfos[0..Math.Min(_currentRowColCount, colInfos.Length)].ToArray())}");
        }

        [ExcludeFromCodeCoverage]
        Span<char> FormatValidChars()
        {
            return _charsDataStart < _charsDataEnd
                && _charsDataStart >= 0 && _charsDataStart < _chars.Length
                && _charsDataEnd >= 0 && _charsDataEnd < _chars.Length
                ? _chars.AsSpan().Slice(_charsDataStart, _charsDataEnd - _charsDataStart)
                : default;
        }
    }

    [ExcludeFromCodeCoverage]
    [Conditional(AssertCondition)]
    void AssertState(string name, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        A.Assert(_chars.Length > 0, $"{name}", filePath, lineNumber);
        A.Assert(0 <= _charsDataStart && _charsDataStart <= _chars.Length, $"{name}", filePath, lineNumber);
        A.Assert(0 <= _charsDataEnd && _charsDataEnd <= _chars.Length, $"{name}", filePath, lineNumber);
        A.Assert(_charsDataStart <= _charsDataEnd, $"{name}", filePath, lineNumber);
        A.Assert(_charsDataStart <= _parsingRowCharsStartIndex && _parsingRowCharsStartIndex <= _charsDataEnd, $"{name}", filePath, lineNumber);
        A.Assert((_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount + 1) * GetIntegersPerColInfo() <= _colEndsOrColInfos.Length);

        if (_colUnquoteUnescape == 0)
        {
            var colEnds = GetColsEntireSpanAs<int>();
            A.Assert(colEnds.Length > 0, $"{name}", filePath, lineNumber);
            A.Assert(-1 <= _currentRowColCount && _currentRowColCount <= colEnds.Length, $"{name}", filePath, lineNumber);
            for (var i = 0; i < _currentRowColCount; i++)
            {
                var colEnd = colEnds[i];
                // colEnds are one before, so first may be before data starts
                colEnd += i == 0 ? 1 : 0;
                A.Assert(_parsingRowCharsStartIndex <= colEnd && colEnd < _charsDataEnd, $"{name}", filePath, lineNumber);
            }
        }
        else
        {
            var colInfos = GetColsEntireSpanAs<SepColInfo>();
            A.Assert(colInfos.Length > 0, $"{name}", filePath, lineNumber);
            A.Assert(-1 <= _currentRowColCount && _currentRowColCount <= colInfos.Length, $"{name}", filePath, lineNumber);
            for (var i = 0; i < _currentRowColCount; i++)
            {
                var (colEnd, _) = colInfos[i];
                // colEnds are one before, so first may be before data starts
                colEnd += i == 0 ? 1 : 0;
                A.Assert(_parsingRowCharsStartIndex <= colEnd && colEnd < _charsDataEnd, $"{name}", filePath, lineNumber);
                if (i > 0)
                {
                    var colStart = colInfos[i - 1].ColEnd + 1;
                    var colLength = colEnd - colStart;
                    A.Assert(0 <= colLength && colLength < 1024 * 1024, $"ColIndex {i} Start {colStart} End {colEnd} Length {colLength}");
                }
            }
        }
        if (_colNameCache != null)
        {
            A.Assert(_colNameCache.Length >= 0, $"{name}", filePath, lineNumber);
            A.Assert(0 <= _cacheIndex && _cacheIndex <= _colNameCache.Length, $"{name}", filePath, lineNumber);
        }
    }

    internal override void DisposeManaged()
    {
        _reader.Dispose();
        base.DisposeManaged();
    }
}
