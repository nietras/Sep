using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    ISepCharsFinder? _finder;

    int _rowIndex = -1;
    // TODO: Count lines independently of rows for easy look up in e.g. notepad
    //int _lineNumber = 0;

#if DEBUG
    // To increase probability of detecting bugs start with short length to
    // force chars buffer management paths to be used.
    internal const int CharsMinimumLength = 32;
#else
    // Based on L1d typically being 32KB, so aiming for <= 16K x sizeof(char).
    // Benchmarks show below to be a good minimum length.
    internal const int CharsMinimumLength = 12 * 1024;
#endif
    internal const int _charsCheckDataAvailableWhenLessThanLength = 256;
    readonly int _charsMinimumFreeLength;
    int _charsPaddingLength;
    char[] _chars;
    int _charsDataStart = 0;
    int _charsDataEnd = 0;
    int _charsParseStart = 0;
    int _charsRowStart = 0;

    const int _positionsMinimumLength = 8;
    Pos[] _positions;
    readonly int _positionsMinimumFreeLength;
    int _positionsStart = 0;
    int _positionsEnd = 0;
    int _positionPreviousChar = LineFeed;
    int _positionPreviousPos = -1; // -1 is valid if before current row start e.g. when line end from previous row

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
        _fastFloatDecimalSeparatorOrZero = decimalSeparator.Length == 1 && _options.UseFastFloat
            ? decimalSeparator[0]
            : '\0';

        var guessPaddingLength = 128;
        var bufferLength = TryGetTextReaderLength(_reader, out var readerLength)
            ? ((readerLength < CharsMinimumLength) ? (readerLength + guessPaddingLength) : CharsMinimumLength)
            : CharsMinimumLength;
        _chars = ArrayPool<char>.Shared.Rent(bufferLength);

        var sep = options.Sep;
        if (sep.HasValue)
        {
            _finder = SepCharsFinderFactory.GetBest(sep.Value);
            _charsPaddingLength = _finder.PaddingLength;
            _charsMinimumFreeLength = Math.Max(_chars.Length / 2, _charsPaddingLength);
            _separator = sep.Value.Separator;
        }
        else
        {
            _charsPaddingLength = 32;
            _charsMinimumFreeLength = Math.Max(_chars.Length / 2, _charsPaddingLength);
        }

        var positionsPaddingLength = _finder?.PaddingLength ?? 32;
#if DEBUG
        var positionsLength = positionsPaddingLength;
#else
        // Performance quite sensitive to positions length, 1024 seems okay
        var positionsLength = _finder?.RequestedPositionsFreeLength ?? 1024;
#endif
        positionsLength = Math.Max(_positionsMinimumLength, positionsLength);
        _positions = ArrayPool<Pos>.Shared.Rent(positionsLength);
        _positionsMinimumFreeLength = Math.Max(positionsLength / 2, positionsPaddingLength);

        _colEnds = ArrayPool<int>.Shared.Rent(_colEndsMaximumLength);

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
                HasRows = _charsDataEnd > _charsDataStart;
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

        // Reset
#if DEBUG
        Array.Fill(_colEnds, -42);
#endif
        _cacheIndex = 0;
        _arrayPool.Reset();
        _colEnds[0] = _charsRowStart - 1;
        _colCount = 0;

        var endOfFile = false;
    LOOP:
        CheckPoint($"{nameof(ParsePositionsForNewRow)} BEFORE");
        // Look through existing positions for new row, if found finish
        if (_positionsStart < _positionsEnd && ParsePositionsForNewRow(endOfFile))
        {
            CheckPoint($"{nameof(ParsePositionsForNewRow)} AFTER - RETURN TRUE");
            // Remove data for next time, perhaps move to first thing above instead
            _charsDataStart = Math.Min(_charsRowStart, _charsDataEnd);
            foundRow = true;
            goto RETURN;
        }
        else if (endOfFile)
        {
            CheckPoint($"{nameof(ParsePositionsForNewRow)} AFTER - ENDOFFILE");
            foundRow = false;
            goto RETURN;
        }

        CheckPoint($"{nameof(ParsePositionsForNewRow)} AFTER");

        endOfFile = ReadDataFindNextPositions(endOfFile);
        goto LOOP;
    ROW_ALREADY_FOUND:
        _rowAlreadyFound = false;
    RETURN:
        return foundRow;
    }

#pragma warning disable CA1502 // Avoid excessive complexity
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ParsePositionsForNewRow(bool endOfFile)
#pragma warning restore CA1502 // Avoid excessive complexity
    {
        A.Assert(_positionsStart < _positionsEnd);

        var separator = _separator;

        ref var positionsRefFirst = ref MemoryMarshal.GetArrayDataReference(_positions);

        var colEnds = _colEnds;
        var colEndsLength = colEnds.Length;
        ref var colEndsRef = ref MemoryMarshal.GetArrayDataReference(colEnds);

        var positionCurrentChar = _positionPreviousChar;
        var positionCurrentPos = _positionPreviousPos;

        // Line end if found, last position of \r or \n found as part of new line
        var lineEnd = -1;
        nint positionsStart = _positionsStart;
        nint positionsEnd = _positionsEnd;

        // Fast-track for common case of separator after separator or line endings
        if (positionCurrentChar == separator || positionCurrentChar == LineFeed || positionCurrentChar == CarriageReturn)
        {
            // Enregister col count and update member field after loop
            var colCount = _colCount;
            for (; positionsStart < positionsEnd; ++positionsStart)
            {
                var newPacked = Unsafe.Add(ref positionsRefFirst, positionsStart);
                var newChar = SepCharPosition.UnpackCharacter(newPacked);
                if (newChar == separator)
                {
                    positionCurrentChar = newChar;
                    positionCurrentPos = SepCharPosition.UnpackPosition(newPacked);
                    // Pre-increment since col ends 1 forward
                    ++colCount;
                    if (colCount < colEndsLength)
                    {
                        Unsafe.Add(ref colEndsRef, colCount) = positionCurrentPos;
                    }
                    else
                    {
                        SepThrow.NotSupportedException_ColCountExceedsMaximumSupported(colEndsLength);
                    }
                    continue;
                }
                //else if (newFlags == LineFeed)
                //{
                //    var newPos = SepParserPosition.UnpackPosition(newPacked);
                //    if (positionCurrentFlags == CarriageReturn && newPos == (positionCurrentPos - 1))
                //    {
                //        // Row already ended, but move next
                //        A.Assert(_charsRowStart == positionCurrentPos, $"New row start {_charsRowStart} != {positionCurrentPos} position of carriage return");
                //        colEndsRef = _charsRowStart;
                //        ++_charsRowStart;
                //        continue;
                //    }
                //}
                break;
            }
            _colCount = colCount;
        }

    PARSE_POSITIONS_QUOTE:
        if (positionCurrentChar == Quote)
        {
            while (positionsStart < positionsEnd)
            {
                var positionsCurrent = Unsafe.Add(ref positionsRefFirst, positionsStart);
                ++positionsStart;
                var newChar = SepCharPosition.UnpackCharacter(positionsCurrent);
                if (newChar == Quote)
                {
                    positionCurrentPos = SepCharPosition.UnpackPosition(positionsCurrent);
                    // HACK: Unquote simply by marking previous as separator
                    positionCurrentChar = separator;
                    break;
                }
                // TODO: Count lines (requires prev/current char to say if quote with CR, LF.
                else
                {

                }
                // EOF/EOT?
            }
        }

        var nextRowStartOffset = 1;
        for (; positionsStart < positionsEnd && lineEnd < 0; ++positionsStart)
        {
            var previousCharacter = positionCurrentChar;
            var previousPosition = positionCurrentPos;

            var positionsCurrent = Unsafe.Add(ref positionsRefFirst, positionsStart);
            positionCurrentChar = SepCharPosition.UnpackCharacter(positionsCurrent);
            positionCurrentPos = SepCharPosition.UnpackPosition(positionsCurrent);

            var newCol = false;
            if (positionCurrentChar == separator)
            {
                // Fast track normal separator case
                if (previousCharacter == separator)
                {
                    // Pre-increment since col ends 1 forward
                    ++_colCount;
                    if (_colCount < colEndsLength)
                    {
                        Unsafe.Add(ref colEndsRef, _colCount) = positionCurrentPos;
                    }
                    else
                    {
                        SepThrow.NotSupportedException_ColCountExceedsMaximumSupported(colEndsLength);
                    }
                    continue;
                }
            }
            else if (positionCurrentChar == Quote)
            {
                ++positionsStart;
                goto PARSE_POSITIONS_QUOTE;
            }
            // Handle new line in any of the three forms: \n, \r\n, \r (LF, CR/LF, CR)
            else if (positionCurrentChar == CarriageReturn)
            {
                lineEnd = positionCurrentPos;

                // When CR there should always be an extra position after with LF if present
                var positionsIndexNext = positionsStart + 1;
                if (positionsIndexNext < _positionsEnd)
                {
                    var positionsNext = Unsafe.Add(ref positionsRefFirst, positionsIndexNext);
                    var positionNextChar = SepCharPosition.UnpackCharacter(positionsNext);
                    var positionNextPos = SepCharPosition.UnpackPosition(positionsNext);
                    // Skip LF if following CR
                    if (positionNextChar == LineFeed && (positionNextPos == positionCurrentPos + 1))
                    {
                        positionCurrentChar = positionNextChar;
                        positionCurrentPos = positionNextPos;
                        A.Assert(positionsStart < positionsEnd);
                        ++positionsStart;
                        ++nextRowStartOffset;
                    }
                }
            }
            else if (positionCurrentChar == LineFeed)
            {
                A.Assert(!(previousCharacter == CarriageReturn && previousPosition == (positionCurrentPos - 1)),
                         "Should never hit CR before LF, since handle when CR found looking at next");
                // Since sole LineFeed always new line
                lineEnd = positionCurrentPos;
            }
            // HACK: ETX is added to positions on end-of-file and then used
            //       here to finish anything on end of file
            //
            // BEWARE: Empty lines after carriage return or line feed at end
            //         of file, is handled at end of parse by checking col
            //         count == 0 and end of file
            else if (positionCurrentChar == EndOfText && positionCurrentPos != 0 &&
                     !(previousPosition == (positionCurrentPos - 1) && (previousCharacter == CarriageReturn || previousCharacter == LineFeed)))
            {
                lineEnd = positionCurrentPos;
            }

            if (lineEnd >= 0 || newCol)
            {
                var makeNewCol = previousCharacter == separator ||
                                 previousCharacter == CarriageReturn ||
                                 previousCharacter == LineFeed;
                if (makeNewCol)
                {
                    // Pre-increment since col ends 1 forward
                    ++_colCount;
                    if (_colCount < colEndsLength)
                    {
                        // CR or LF at start of first row can be same position as previous
                        A.Assert(previousPosition <= lineEnd);
                        Unsafe.Add(ref colEndsRef, _colCount) = lineEnd;
                    }
                }
            }
        }
        _positionsStart = (int)positionsStart;

        _positionPreviousChar = positionCurrentChar;
        _positionPreviousPos = positionCurrentPos;

        var newLine = lineEnd >= 0;
        if (newLine || endOfFile)
        {
            // Empty end
            var end = _colCount == 0 && endOfFile;
            // New line, end parsing of line, check cols found
            if (_colCountExpected >= 0 && _colCount != _colCountExpected && !end)
            {
                ThrowInvalidDataExceptionColCountMismatch(_colCountExpected, lineEnd);
            }
            _charsRowStart = newLine && !endOfFile ? lineEnd + nextRowStartOffset : _charsDataEnd;
            return !end;
        }
        return false;
    }

    bool ReadDataFindNextPositions(bool endOfFile)
    {
        // Check how much data in buffer and read more to batch parsing in block
        // of certain size.
        var nothingLeftToRead = false;
        if (_finder == null ||
            (_charsDataEnd - _charsParseStart) < _charsCheckDataAvailableWhenLessThanLength)
        {
            nothingLeftToRead = CheckCharsAvailableDataMaybeRead(_charsPaddingLength);

            CheckPoint($"{nameof(CheckCharsAvailableDataMaybeRead)} AFTER");
        }

        if (_finder == null)
        {
            TryDetectSeparatorInitializeFinder(nothingLeftToRead);

            CheckPoint($"{nameof(TryDetectSeparatorInitializeFinder)} AFTER");
        }

        if (_finder != null && _charsParseStart < _charsDataEnd)
        {
            ref var positions = ref _positions;
            ref var positionsStart = ref _positionsStart;
            ref var positionsEnd = ref _positionsEnd;

            CheckPoint($"{nameof(SepArrayExtensions.CheckFreeMaybeMoveMaybeDoubleLength)} {nameof(_positions)} BEFORE");

            SepArrayExtensions.CheckFreeMaybeMoveMaybeDoubleLength(
                ref positions, ref positionsStart, ref positionsEnd,
                _positionsMinimumFreeLength, paddingLengthToClear: 0);

            CheckPoint($"{nameof(_finder.Find)} BEFORE");

            // Parse and update positions
            _charsParseStart = _finder.Find(_chars, _charsParseStart, _charsDataEnd,
                                            positions, positionsStart, ref positionsEnd);

            CheckPoint($"{nameof(_finder.Find)} AFTER");

            // Finders may stop due to positions not having enough padding
            // Ensure positions always has CR followed by LF by checking manually here
            if (positionsEnd > positionsStart &&
                SepCharPosition.UnpackCharacter(_positions[positionsEnd - 1]) == CarriageReturn &&
                _charsParseStart < _charsDataEnd &&
                _chars[_charsParseStart] == LineFeed)
            {
                _positions[positionsEnd] = SepCharPosition.Pack(LineFeed, _charsParseStart);
                ++_charsParseStart;
                ++positionsEnd;
            }
        }
        else
        {
            if (nothingLeftToRead)
            {
                // Note this relies on positions always having padding
                A.Assert(_positionsEnd < _positions.Length);
                _positions[_positionsEnd] = SepCharPosition.Pack(EndOfText, _charsDataEnd);
                ++_positionsEnd;
                // If nothing has been read, then at end of file.
                endOfFile = true;
            }
        }
        return endOfFile;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void ThrowInvalidDataExceptionColCountMismatch(int colCountExpected, int lineEnd)
    {
        var rowStart = _charsRowStart;
        AssertState(lineEnd, rowStart);
        var line = new string(_chars, rowStart, lineEnd - rowStart);
        SepThrow.InvalidDataException_ColCountMismatch(_colCount, _rowIndex, line,
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
        if (_chars.Length > SepCharPosition.MaxLength)
        {
            SepThrow.NotSupportedException_BufferOrRowLengthExceedsMaximumSupported(SepCharPosition.MaxLength);
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
        // Adjust found positions
        A.Assert(_positionsStart == _positionsEnd, "Positions should currently always been emptied before data move.");
        // Keeping adjustment as a safety for future changes
        for (var i = _positionsStart; i < _positionsEnd; ++i)
        {
            ref var p = ref _positions[i];
            // HACK: Utilize we know position > offset so we can just
            //       subtract due to byte packed in most significant bits
            A.Assert(SepCharPosition.UnpackPosition(p) >= offset);
            Unsafe.As<Pos, int>(ref p) -= offset;
        }
        // Previous position might be before data start since old line end or
        // similar. Hence, 2 due to for example `\r\n`.
        A.Assert(_positionPreviousPos >= (offset - 2));
        _positionPreviousPos -= offset;
        // Adjust found cols, note includes _colCount since +1
        var colEnds = _colEnds;
        for (var i = 0; i <= _colCount; i++)
        {
            ref var colEnd = ref colEnds[i];
            colEnd -= offset;
        }
    }

    static bool TryGetTextReaderLength(TextReader reader, out int length)
    {
        // utf8 bytes can only get shorter when converted to characters so
        // if we can determine an underlying stream length, then we can
        // allocate a smaller buffer.
        if (reader is StreamReader sr && sr.CurrentEncoding.CodePage == Encoding.UTF8.CodePage)
        {
            var s = sr.BaseStream;
            if (s.CanSeek)
            {
                length = (int)s.Length;
                return true;
            }
        }
        length = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void TryDetectSeparatorInitializeFinder(bool nothingLeftToRead)
    {
        // Detect separator if no finder defined
        var validChars = _chars.AsSpan(_charsDataStart.._charsDataEnd);
        var maybeSep = SepReaderExtensions.DetectSep(validChars, nothingLeftToRead);
        if (maybeSep.HasValue)
        {
            var sep = maybeSep.Value;
            _separator = sep.Separator;
            _finder = SepCharsFinderFactory.GetBest(sep);
            // TODO: Initialize other members
            _charsPaddingLength = _finder.PaddingLength;
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
        T.WriteLine($"{nameof(_positions),-10}:{_positions.Length,5} [{_positionsStart,4},{_positionsEnd,4}] {FormatValidPositions()}");
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

        [ExcludeFromCodeCoverage]
        string FormatValidPositions()
        {
            if (_positionsStart < _positionsEnd)
            {
                var range = Enumerable.Range(_positionsStart, _positionsEnd - _positionsStart);
                return string.Join(',', range.Select(i =>
                {
                    var p = _positions[i];
                    return $"('{Format((char)SepCharPosition.UnpackCharacter(p))}',{SepCharPosition.UnpackPosition(p)})";
                }));
            }
            return string.Empty;
        }

        [ExcludeFromCodeCoverage]
        static string Format(char c) => c switch
        {
            '\r' => "\\r",
            '\n' => "\\n",
            EndOfText => "EOT",
            _ => c.ToString(),
        };
    }

    [ExcludeFromCodeCoverage]
    [Conditional(AssertCondition)]
    void AssertState(string name, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        A.Assert(_chars.Length > 0, $"{name}", filePath, lineNumber);
        A.Assert(0 <= _charsDataStart && _charsDataStart <= _chars.Length, $"{name}", filePath, lineNumber);
        A.Assert(0 <= _charsDataEnd && _charsDataEnd <= _chars.Length, $"{name}", filePath, lineNumber);
        A.Assert(_charsDataStart <= _charsDataEnd, $"{name}", filePath, lineNumber);

        A.Assert(_positions.Length > 0, $"{name}", filePath, lineNumber);
        A.Assert(0 <= _positionsStart && _positionsStart <= _positions.Length, $"{name}", filePath, lineNumber);
        A.Assert(0 <= _positionsEnd && _positionsEnd <= _positions.Length, $"{name}", filePath, lineNumber);
        A.Assert(_positionsStart <= _positionsEnd, $"{name}", filePath, lineNumber);

        for (var i = _positionsStart; i < _positionsEnd; i++)
        {
            var p = _positions[i];
            var character = SepCharPosition.UnpackCharacter(p);
            var position = SepCharPosition.UnpackPosition(p);
            if (character != EndOfText)
            {
                A.Assert(_charsDataStart <= position && position < _charsDataEnd, $"{name} at index {i}", filePath, lineNumber);
            }
            else
            {
                A.Assert(_charsDataStart <= position && position <= _charsDataEnd, $"{name} at index {i}", filePath, lineNumber);
            }
        }

        A.Assert(_colEnds.Length > 0, $"{name}", filePath, lineNumber);
        A.Assert(0 <= _colCount && _colCount <= _colEnds.Length, $"{name}", filePath, lineNumber);
        for (var i = 0; i < _colCount; i++)
        {
            var colEnd = _colEnds[i];
            // colEnds are one before, so first may be before data starts
            colEnd += i == 0 ? 1 : 0;
            A.Assert(_charsDataStart <= colEnd && colEnd < _charsDataEnd, $"{name}", filePath, lineNumber);
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
        ArrayPool<Pos>.Shared.Return(_positions);
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
