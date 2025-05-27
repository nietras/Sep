using System;
using System.Buffers;
#if NET9_0_OR_GREATER
using System.Collections.Generic;
#endif
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if NET9_0_OR_GREATER
#endif

namespace nietras.SeparatedValues;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class SepReader : SepReaderState
#if NET9_0_OR_GREATER
    , IEnumerable<SepReader.Row>
    , IEnumerator<SepReader.Row>
    , IAsyncEnumerable<SepReader.Row>
#endif
{
    internal readonly record struct Info(object Source, Func<Info, string> DebuggerDisplay);
    internal string DebuggerDisplay => _info.DebuggerDisplay(_info);

    const string TraceCondition = "SEPREADERTRACE";
    const string AssertCondition = "SEPREADERASSERT";

    readonly Info _info;
    char _separator;
    readonly bool _disableQuotesParsing;
    internal readonly bool _continueOnCapturedContext;
    readonly TextReader _reader;
    ISepParser? _parser;

    readonly int _charsMinimumFreeLength;
    int _charsPaddingLength;

    internal SepReader(Info info, in SepReaderOptions options, TextReader reader)
        : base(colUnquoteUnescape: options.Unescape, trim: options.Trim)
    {
        _info = info;
        _reader = reader;
        _cultureInfo = options.CultureInfo;
        _createToString = options.CreateToString;
        _disableQuotesParsing = options.DisableQuotesParsing;
        _continueOnCapturedContext = options.AsyncContinueOnCapturedContext;
        _arrayPool = new();

        var decimalSeparator = _cultureInfo?.NumberFormat.CurrencyDecimalSeparator ??
            System.Globalization.CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator;
        _fastFloatDecimalSeparatorOrZero =
            decimalSeparator.Length == 1 && !options.DisableFastFloat
            ? decimalSeparator[0]
            : '\0';

        int? maybeReaderLengthEstimate = TryGetTextReaderLength(_reader, out var longReaderLength)
            // TextReader length can be greater than int.MaxValue so have to
            // constrain it to avoid overflow.
            ? (int)Math.Min(longReaderLength, int.MaxValue) : null;

        var sep = options.Sep;
        if (sep.HasValue)
        {
            _parser = SepParserFactory.CreateBest(new(sep.Value, _disableQuotesParsing));
            _charsPaddingLength = _parser.PaddingLength;
            _separator = sep.Value.Separator;
        }
        else
        {
            _charsPaddingLength = SepParserFactory.MaxPaddingLength;
        }

        var initialBufferLength = Math.Max(options.InitialBufferLength, _charsPaddingLength);
        var bufferLength = maybeReaderLengthEstimate.HasValue
            ? ((maybeReaderLengthEstimate.Value < initialBufferLength)
               ? (maybeReaderLengthEstimate.Value + _charsPaddingLength) : initialBufferLength)
            : initialBufferLength;

        _chars = ArrayPool<char>.Shared.Rent(bufferLength);
        _charsMinimumFreeLength = Math.Max(_chars.Length / 2, _charsPaddingLength);
        _colEndsOrColInfos = ArrayPool<int>.Shared.Rent(Math.Max(ColEndsInitialLength, _charsPaddingLength * 2));
    }

    public bool IsEmpty { get; private set; }
    public SepSpec Spec => new(new(_separator), _cultureInfo, _continueOnCapturedContext);
    public bool HasHeader { get => _hasHeader; private set => _hasHeader = value; }
    public bool HasRows { get; private set; }
    public SepReaderHeader Header => _header;

    internal int CharsLength => _chars.Length;

    public Row Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    public SepReader GetEnumerator() => this;
#if NET9_0_OR_GREATER
    IEnumerator<Row> IEnumerable<Row>.GetEnumerator() => this;
    // Legacy not supported
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new NotSupportedException();
    object System.Collections.IEnumerator.Current => throw new NotSupportedException();
    void System.Collections.IEnumerator.Reset() => throw new NotSupportedException();

    // Async
    IAsyncEnumerator<Row> IAsyncEnumerable<Row>.GetAsyncEnumerator(CancellationToken cancellationToken) =>
        GetAsyncEnumerator(cancellationToken);
#endif
    public AsyncEnumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new(this, cancellationToken);

    public readonly struct AsyncEnumerator
#if NET9_0_OR_GREATER
        // Interface requires .NET 9 for allows ref struct
        : IAsyncEnumerator<Row>
#endif
    {
        readonly SepReader _reader;
        readonly CancellationToken _cancellationToken;

        internal AsyncEnumerator(SepReader reader, CancellationToken cancellationToken)
        {
            _reader = reader;
            _cancellationToken = cancellationToken;
        }

        public Row Current => _reader.Current;

        public ValueTask<bool> MoveNextAsync() => _reader.MoveNextAsync(_cancellationToken);

        public ValueTask DisposeAsync()
        {
            // No Async dispose since TextReader has none
            _reader.Dispose();
            return ValueTask.CompletedTask;
        }
    }

    public string ToString(int index) => ToStringDefault(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool HasParsedRows() => _parsedRowIndex < _parsedRowsCount;

    void CheckColInfosCapacityMaybeDouble(int paddingLength)
    {
        // Potential end is current parsing end plus maximum col infos for next parse loop
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
        // + 1 since one more for start col
        var lengthInIntegers = (_parsingRowColEndsOrInfosStartIndex + _parsingRowColCount + 1) * factor;

        var previousColEndsSpan = previousColEnds.AsSpan().Slice(0, lengthInIntegers);
        var newColEndsSpan = _colEndsOrColInfos.AsSpan().Slice(0, lengthInIntegers);
        previousColEndsSpan.CopyTo(newColEndsSpan);
        ArrayPool<int>.Shared.Return(previousColEnds);
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
        var maybeSep = SepReaderExtensions.DetectSep(validChars, nothingLeftToRead, _disableQuotesParsing);
        if (maybeSep.HasValue)
        {
            var sep = maybeSep.Value;
            _separator = sep.Separator;
            _parser = SepParserFactory.CreateBest(new(sep, _disableQuotesParsing));
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
