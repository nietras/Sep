//#define SYNC
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if !SYNC
using System.Threading;
using System.Threading.Tasks;
#endif
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

public sealed partial class SepReader
{
#if SYNC
    internal void Initialize(in SepReaderOptions options)
#else
    internal async ValueTask InitializeAsync(SepReaderOptions options, CancellationToken cancellationToken)
#endif
    {
        // Parse first row/header
#if SYNC
        if (MoveNext())
#else
        if (await MoveNextAsync(cancellationToken))
#endif
        {
            A.Assert(_parsedRowsCount > 0);

            IsEmpty = false;
            var firstRowColCount = _parsedRows[0].ColCount;
            _colCountExpected = firstRowColCount;
            if (options.HasHeader)
            {
                var headerRow = new string(RowSpan());
                var colNameComparer = options.ColNameComparer;
                var colNameToIndex = new Dictionary<string, int>(firstRowColCount, colNameComparer);
                for (var colIndex = 0; colIndex < firstRowColCount; colIndex++)
                {
                    var colName = ToStringDirect(colIndex);
                    if (!colNameToIndex.TryAdd(colName, colIndex))
                    {
                        SepThrow.ArgumentException_DuplicateColNamesFound(this, colNameToIndex,
                            colName, firstRowColCount, colNameComparer, headerRow);
                    }
                }
                _header = new(headerRow, colNameToIndex);

                HasHeader = true;

                // Check if more data available and hence minimum 1 row after header
                // What if \n after \r after header only? Where \n lingering after MoveNext?
                HasRows = _parsedRowsCount > 1 || _charsDataEnd > _charsParseStart || _parsingRowColCount > 0;
                if (!HasRows)
                {
#if SYNC
                    HasRows = !FillAndMaybeDoubleCharsBuffer(_charsPaddingLength);
#else
                    HasRows = !await FillAndMaybeDoubleCharsBufferAsync(_charsPaddingLength, cancellationToken);
#endif
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
        _header ??= SepReaderHeader.Empty;

        _colCountExpected = options.DisableColCountCheck ? -1 : _colCountExpected;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if SYNC
    public bool MoveNext()
#else
    public async ValueTask<bool> MoveNextAsync(CancellationToken cancellationToken = default)
#endif
    {
        do
        {
            if (MoveNextAlreadyParsed())
            {
                return true;
            }
#if SYNC
        } while (ParseNewRows());
#else
        } while (await ParseNewRowsAsync(cancellationToken));
#endif
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#if SYNC
    internal bool ParseNewRows()
#else
    internal async ValueTask<bool> ParseNewRowsAsync(CancellationToken cancellationToken)
#endif
    {
        _parsedRowsCount = 0;
        _parsedRowIndex = 0;
        _currentRowColCount = -1;
        _currentRowColEndsOrInfosOffset = 0;

#if SYNC
        CheckPoint($"{nameof(ParseNewRows)} BEGINNING");
#else
        CheckPoint($"{nameof(ParseNewRowsAsync)} BEGINNING");
#endif

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

#if SYNC
        endOfFile = EnsureInitializeAndReadData(endOfFile);
#else
        endOfFile = await EnsureInitializeAndReadDataAsync(endOfFile, cancellationToken);
#endif
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

    [MemberNotNullWhen(false, nameof(_parser))]
#if SYNC
    bool EnsureInitializeAndReadData(bool endOfFile)
#else
    async ValueTask<bool> EnsureInitializeAndReadDataAsync(bool endOfFile, CancellationToken cancellationToken)
#endif
    {
#if SYNC
        var nothingLeftToRead = FillAndMaybeDoubleCharsBuffer(_charsPaddingLength);
        CheckPoint($"{nameof(FillAndMaybeDoubleCharsBuffer)} AFTER");
#else
        var nothingLeftToRead = await FillAndMaybeDoubleCharsBufferAsync(_charsPaddingLength, cancellationToken);
        CheckPoint($"{nameof(FillAndMaybeDoubleCharsBufferAsync)} AFTER");
#endif
        if (_parser == null)
        {
            TryDetectSeparatorInitializeParser(nothingLeftToRead);

            CheckPoint($"{nameof(TryDetectSeparatorInitializeParser)} AFTER");
        }

        if (_parser == null || _charsParseStart >= _charsDataEnd)
        {
            if (nothingLeftToRead)
            {
                // Make sure room for any col at end of file
                CheckColInfosCapacityMaybeDouble(paddingLength: 0);
                // If nothing has been read, then at end of file.
                endOfFile = true;
            }
        }
        else
        {
            CheckColInfosCapacityMaybeDouble(_parser.PaddingLength);
        }
        return endOfFile;
    }


#if SYNC
    bool FillAndMaybeDoubleCharsBuffer(int paddingLength)
#else
    async ValueTask<bool> FillAndMaybeDoubleCharsBufferAsync(int paddingLength, CancellationToken cancellationToken)
#endif
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

#if SYNC
        var freeChars = new Span<char>(_chars, _charsDataEnd, freeLength);
#else
        var freeChars = new Memory<char>(_chars, _charsDataEnd, freeLength);
#endif
        A.Assert(freeLength > 0, $"Free span at end of buffer length {freeLength} not greater than 0");

        // Read until full or no more data
        var totalBytesRead = 0;
        var readCount = 0;
#if SYNC
        while (totalBytesRead < freeLength &&
               ((readCount = _reader.Read(freeChars.Slice(totalBytesRead))) > 0))
#else
        while (totalBytesRead < freeLength &&
                ((readCount = await _reader.ReadAsync(freeChars.Slice(totalBytesRead), cancellationToken)) > 0))
#endif
        {
            _charsDataEnd += readCount;
            // Ensure carriage return always followed by line feed
            if (_chars[_charsDataEnd - 1] == CarriageReturn)
            {
                var extraChar = _reader.Peek();
                if (extraChar == LineFeed)
                {
                    var readChar = (char)_reader.Read();
                    //#if SYNC
                    //                    var readChar = (char)_reader.Read();
                    //#else
                    //                    var readChar = (char)await _reader.ReadAsync();
                    //#endif
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
        return totalBytesRead == 0;
    }
}
