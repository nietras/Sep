using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

public partial class SepReader : IDisposable
{
    const string TraceCondition = "SEPREADERTRACE";
    const string AssertCondition = "SEPREADERASSERT";

    // To avoid `call     CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE`,
    // promote cache to member here.
    readonly string[] _singleCharToString = SepStringCache.SingleCharToString;
    readonly SepReaderOptions _options;
    readonly char _fastFloatDecimalSeparatorOrZero;
    char _separator;
    readonly SepHeader _header;
    readonly TextReader _reader;
    readonly CultureInfo? _cultureInfo;
    ISepParser? _parser;

    int _rowIndex = -1;
    int _rowLineNumberFrom = 0;
    int _lineNumber = 1;

#if DEBUG
    // To increase probability of detecting bugs start with short length to
    // force chars buffer management paths to be used.
    internal const int CharsMinimumLength = 64;
#else
    // Based on L1d typically being 32KB-48KB, so aiming for 16K-24K x sizeof(char).
    // Benchmarks show below to be a good minimum length.
    internal const int CharsMinimumLength = 24 * 1024;
#endif
    internal const int _charsCheckDataAvailableWhenLessThanLength = 256;
    readonly int _charsMinimumFreeLength;
    int _charsPaddingLength;
    char[] _chars;
    int _charsDataStart = 0;
    int _charsDataEnd = 0;
    int _charsParseStart = 0;
    int _charsRowStart = 0;

    internal const int _colEndsMaximumLength = 1024;
    // [0] = Previous row/col end e.g. one before row/first col start
    // [1...] = Col ends e.g. [1] = first col end
    // Length = colCount + 1
    readonly int[] _colEnds;
    readonly int _colCountExpected = -1;
    int _colCount = 0;

    bool _rowAlreadyFound = false;

    readonly SepArrayPoolAccessIndexed _arrayPool = new();
    readonly (string colName, int colIndex)[] _colNameCache;
    int _cacheIndex = 0;
    readonly SepToString[] _colToStrings;

    internal SepReader(SepReaderOptions options, TextReader reader)
    {
        _options = options;
        _reader = reader;
        _cultureInfo = _options.CultureInfo;

        var decimalSeparator = _cultureInfo?.NumberFormat.CurrencyDecimalSeparator ??
            System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator;
        _fastFloatDecimalSeparatorOrZero =
            decimalSeparator.Length == 1 && !_options.DisableFastFloat
            ? decimalSeparator[0]
            : '\0';

        var guessPaddingLength = 128;
        int? textReaderLength = TryGetTextReaderLength(_reader, out var readerLength)
            // TextReader length can be greater than int.MaxValue so have to
            // constrain it to avoid overflow.
            ? (int)Math.Min(readerLength, int.MaxValue) : null;
        var bufferLength = textReaderLength.HasValue
            ? ((textReaderLength.Value < CharsMinimumLength) ? (textReaderLength.Value + guessPaddingLength) : CharsMinimumLength)
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

        _colEnds = ArrayPool<int>.Shared.Rent(Math.Max(_colEndsMaximumLength, paddingLength * 2));

        // Parse first row/header
        if (MoveNext())
        {
            IsEmpty = false;
            _colCountExpected = _colCount;
            if (options.HasHeader)
            {
                var colNameToIndex = new Dictionary<string, int>(_colCount);
                for (var colIndex = 0; colIndex < _colCount; colIndex++)
                {
                    var colName = ToStringRaw(colIndex);
                    colNameToIndex.Add(colName, colIndex);
                }
                var headerRow = new string(RowSpan());
                _header = new(headerRow, colNameToIndex);

                HasHeader = true;

                // Check if more data available and hence minimum 1 row after header
                // What if \n after \r after header only? Where \n lingering after MoveNext?
                HasRows = _charsDataEnd > _charsParseStart;
                if (!HasRows)
                {
                    HasRows = !CheckCharsAvailableDataMaybeRead(_charsPaddingLength);
                }
            }
            else
            {
                HasHeader = false;
                HasRows = true;
                _rowAlreadyFound = true;
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

        // Use empty header if no header
        _header ??= SepHeader.Empty;

        _colNameCache = new (string colName, int colIndex)[_colCountExpected];

        _colToStrings = new SepToString[_colCountExpected];
        for (var colIndex = 0; colIndex < _colToStrings.Length; colIndex++)
        {
            _colToStrings[colIndex] = options.CreateToString(_header, colIndex);
        }
        _colCountExpected = options.DisableColCountCheck ? -1 : _colCountExpected;
    }

    public bool IsEmpty { get; }
    public SepSpec Spec => new(new(_separator), _options.CultureInfo);
    public bool HasHeader { get; }
    public bool HasRows { get; }
    public SepHeader Header => _header;

    internal int CharsLength => _chars.Length;

    public Row Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    public SepReader GetEnumerator() => this;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool MoveNext()
    {
        var foundRow = _rowAlreadyFound;
        if (foundRow) { goto ROW_ALREADY_FOUND; }

        ++_rowIndex;
        _rowLineNumberFrom = _lineNumber;

        // Reset
#if DEBUG
        Array.Fill(_colEnds, -42);
#endif
        _cacheIndex = 0;
        _arrayPool.Reset();
        _charsDataStart = _charsRowStart;
        _colEnds[0] = _charsRowStart - 1;
        _colCount = 0;

        var endOfFile = false;
    LOOP:
        CheckPoint($"{nameof(_parser.Parse)} BEFORE");

        var rowLineEndingOffset = 0;
        _charsParseStart = _parser?.Parse(_chars, _charsParseStart, _charsDataEnd,
            _colEnds, ref _colCount, ref rowLineEndingOffset, ref _lineNumber) ?? _charsParseStart;
    MAYBEROW:
        if (rowLineEndingOffset != 0)
        {
            CheckPoint($"{nameof(_parser.Parse)} AFTER - RETURN TRUE");
            if (_colCountExpected >= 0 && _colCount != _colCountExpected)
            {
                // Capture row start and move next to be able to continue even
                // after exception.
                var rowStart = _charsRowStart;
                _charsRowStart = _charsParseStart;
                ThrowInvalidDataExceptionColCountMismatch(_colCountExpected, _colEnds[_colCount], rowStart);
            }
            _charsRowStart = _charsParseStart;
            foundRow = true;
            goto RETURN;
        }
        else if (endOfFile)
        {
            CheckPoint($"{nameof(_parser.Parse)} AFTER - ENDOFFILE");
            foundRow = false;
            goto RETURN;
        }

        CheckPoint($"{nameof(_parser.Parse)} AFTER");

        endOfFile = EnsureInitializeAndReadData(endOfFile);
        if (endOfFile && _charsRowStart < _charsDataEnd && _charsParseStart == _charsDataEnd)
        {
            ++_colCount;
            _colEnds[_colCount] = _charsDataEnd;
            rowLineEndingOffset = 1;
            ++_lineNumber;
            goto MAYBEROW;
        }
        goto LOOP;
    ROW_ALREADY_FOUND:
        _rowAlreadyFound = false;
    RETURN:
        return foundRow;
    }

    bool EnsureInitializeAndReadData(bool endOfFile)
    {
        // Check how much data in buffer and read more to batch parsing in block
        // of certain size.
        var nothingLeftToRead = false;
        if (_parser == null ||
            (_charsDataEnd - _charsParseStart) < _charsCheckDataAvailableWhenLessThanLength)
        {
            nothingLeftToRead = CheckCharsAvailableDataMaybeRead(_charsPaddingLength);

            CheckPoint($"{nameof(CheckCharsAvailableDataMaybeRead)} AFTER");
        }

        if (_parser == null)
        {
            TryDetectSeparatorInitializeParser(nothingLeftToRead);

            CheckPoint($"{nameof(TryDetectSeparatorInitializeParser)} AFTER");
        }

        if (_parser != null && _charsParseStart < _charsDataEnd)
        {
            if (_colCount > (_colEnds.Length - _parser.PaddingLength))
            {
                SepThrow.NotSupportedException_ColCountExceedsMaximumSupported(_colEnds.Length);
            }
        }
        else
        {
            if (nothingLeftToRead)
            {
                if (_colCount >= _colEnds.Length)
                {
                    SepThrow.NotSupportedException_ColCountExceedsMaximumSupported(_colEnds.Length);
                }
                // If nothing has been read, then at end of file.
                endOfFile = true;
            }
        }
        return endOfFile;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void ThrowInvalidDataExceptionColCountMismatch(int colCountExpected, int lineEnd, int rowStart)
    {
        AssertState(lineEnd, rowStart);
        var line = new string(_chars, rowStart, lineEnd - rowStart);
        SepThrow.InvalidDataException_ColCountMismatch(_colCount, _rowIndex, _rowLineNumberFrom, _lineNumber, line,
            colCountExpected, _header.ToString());

        [ExcludeFromCodeCoverage]
        void AssertState(int lineEnd, int rowStart)
        {
            A.Assert(_charsDataStart <= rowStart && lineEnd <= _charsDataEnd, $"Row not within data range {_charsDataStart} <= {rowStart} && {lineEnd} <= {_charsDataEnd}");
            A.Assert(lineEnd >= rowStart, $"Line end before row start {lineEnd} >= {rowStart}");
        }
    }

    bool CheckCharsAvailableDataMaybeRead(int paddingLength)
    {
        // Check if buffer full or little data at end of buffer then move
        var offset = SepArrayExtensions.CheckFreeMaybeMoveMaybeDoubleLength(
            ref _chars, ref _charsDataStart, ref _charsDataEnd,
            _charsMinimumFreeLength, paddingLength);
        if (_chars.Length > RowLengthMax)
        {
            SepThrow.NotSupportedException_BufferOrRowLengthExceedsMaximumSupported(RowLengthMax);
        }
        if (offset > 0)
        {
            A.Assert(_charsDataStart == 0, "Data start not at zero");
            HandleDataMoved(offset);
        }
        // Read to free buffer area
        var freeLength = _chars.Length - _charsDataEnd - paddingLength;
        // Read 1 less than free length to ensure we always read \n after \r,
        // and hence always ensure we have the two combined in buffer.
        freeLength -= 1;
        var freeSpan = new Span<char>(_chars, _charsDataEnd, freeLength);
        A.Assert(freeLength > 0, $"Free span at end of buffer length {freeLength} not greater than 0");
        var readCount = _reader.Read(freeSpan);
        if (readCount > 0)
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
            if (paddingLength > 0)
            {
                _chars.ClearPaddingAfterData(_charsDataEnd, paddingLength);
            }
        }
        //Console.WriteLine($"Read: {readCount} BufferSize: {freeSpan.Length} Buffer Length: {_chars.BufferLength}");
        return readCount == 0;
    }

    void HandleDataMoved(int offset)
    {
        // Adjust parse start
        A.Assert(_charsParseStart >= offset);
        _charsParseStart -= offset;
        // Adjust row start
        A.Assert(_charsRowStart >= offset);
        _charsRowStart -= offset;
        // Adjust found cols, note includes _colCount since +1
        var colEnds = _colEnds;
        for (var i = 0; i <= _colCount; i++)
        {
            ref var colEnd = ref colEnds[i];
            colEnd -= offset;
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
    [Conditional(TraceCondition), Conditional("SEPREADERCHECKPOINT")]
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
        T.WriteLine($"{nameof(_colEnds),-10}:{_colEnds.Length,5} [{0,4},{_colCount,4}] {string.Join(',', _colEnds[0..Math.Min(_colCount, _colEnds.Length)])}");

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
        A.Assert(_charsDataStart <= _charsRowStart && _charsRowStart <= _charsDataEnd, $"{name}", filePath, lineNumber);

        A.Assert(_colEnds.Length > 0, $"{name}", filePath, lineNumber);
        A.Assert(0 <= _colCount && _colCount <= _colEnds.Length, $"{name}", filePath, lineNumber);
        for (var i = 0; i < _colCount; i++)
        {
            var colEnd = _colEnds[i];
            // colEnds are one before, so first may be before data starts
            colEnd += i == 0 ? 1 : 0;
            A.Assert(_charsRowStart <= colEnd && colEnd < _charsDataEnd, $"{name}", filePath, lineNumber);
        }
        if (_colNameCache != null)
        {
            A.Assert(_colNameCache.Length >= 0, $"{name}", filePath, lineNumber);
            A.Assert(0 <= _cacheIndex && _cacheIndex <= _colNameCache.Length, $"{name}", filePath, lineNumber);
        }
    }

    void DisposeManaged()
    {
        _reader.Dispose();
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
