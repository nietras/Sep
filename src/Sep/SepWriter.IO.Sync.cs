#define SYNC
using System;
using System.Runtime.CompilerServices;
#if !SYNC
using System.Threading;
using System.Threading.Tasks;
#endif

namespace nietras.SeparatedValues;

public partial class SepWriter
{
#if SYNC
    public void Flush() => _writer.Flush();
#else
    public Task FlushAsync(CancellationToken cancellationToken = default) =>
        _writer.FlushAsync(cancellationToken);
#endif

#if SYNC
    internal void EndRow()
#else
    internal async ValueTask EndRowAsync(CancellationToken cancellationToken)
#endif
    {
        if (!_newRowActive) { SepThrow.InvalidOperationException_WriterDoesNotHaveActiveRow(); }

        var cols = _cols;

        // Header
        if (!_headerWrittenOrSkipped)
        {
#if SYNC
            WriteHeader();
#else
            await WriteHeaderAsync(cancellationToken)
                .ConfigureAwait(_continueOnCapturedContext);
#endif
        }
        else if (_colNotSetOption == SepColNotSetOption.Throw || !_disableColCountCheck)
        {
            var colSetCount = 0;
            for (var colIndex = 0; colIndex < cols.Count; ++colIndex)
            {
                var colSet = cols[colIndex].HasBeenSet;
                colSetCount += colSet ? 1 : 0;
                if (_colNotSetOption == SepColNotSetOption.Throw && !colSet)
                { SepThrow.InvalidOperationException_NotAllExpectedColsSet(cols, _colNamesHeader); }
            }
            if (!_disableColCountCheck && colSetCount != _headerOrFirstRowColCount)
            { SepThrow.InvalidOperationException_NotAllExpectedColsSet(cols, _colNamesHeader); }
        }

        // New Row
        if (cols.Count > 0)
        {
            var notFirst = false;
            for (var colIndex = 0; colIndex < cols.Count; ++colIndex)
            {
                var col = cols[colIndex];
                if (col.HasBeenSet || _colNotSetOption != SepColNotSetOption.Skip)
                {
                    if (notFirst)
                    {
#if SYNC
                        _writer.Write(_sep.Separator);
#else
                        await _writer.WriteAsync(_sep.Separator)
                            .ConfigureAwait(_continueOnCapturedContext);
#endif
                    }
#if SYNC
                    var chars = col.GetSpan();
#else
                    var chars = col.GetMemory();
#endif
                    if (_escape)
                    {
#if SYNC
                        WriteEscaped(chars);
#else
                        await WriteEscapedAsync(chars, cancellationToken)
                            .ConfigureAwait(_continueOnCapturedContext);
#endif
                    }
                    else
                    {
#if SYNC
                        _writer.Write(chars);
#else
                        await _writer.WriteAsync(chars, cancellationToken)
                            .ConfigureAwait(_continueOnCapturedContext);
#endif
                    }
                    notFirst = true;
                }
            }
        }
#if SYNC
        _writer.WriteLine();
#else
        await _writer.WriteLineAsync()
            .ConfigureAwait(_continueOnCapturedContext);
#endif
        if (_headerOrFirstRowColCount == -1)
        {
            _headerOrFirstRowColCount = cols.Count;
        }
        _newRowActive = false;
    }

#if SYNC
    internal void WriteHeader()
#else
    internal async ValueTask WriteHeaderAsync(CancellationToken cancellationToken)
#endif
    {
        if (_writeHeader)
        {
            var cols = _cols;
            A.Assert(_colNamesHeader.Length == 0);
            if (cols.Count != _colNamesHeader.Length)
            {
                _colNamesHeader = new string[cols.Count];
            }
            var notFirstHeader = false;
            for (var colIndex = 0; colIndex < cols.Count; ++colIndex)
            {
                var col = cols[colIndex];
                A.Assert(colIndex == col.Index);

                if (notFirstHeader)
                {
#if SYNC
                    _writer.Write(_sep.Separator);
#else
                    await _writer.WriteAsync(_sep.Separator)
                        .ConfigureAwait(_continueOnCapturedContext);
#endif
                }
                var name = col.Name;

#if SYNC
                if (_escape) { WriteEscaped(name); }
                else { _writer.Write(name); }
#else
                if (_escape)
                {
                    await WriteEscapedAsync(name.AsMemory(), cancellationToken)
                        .ConfigureAwait(_continueOnCapturedContext);
                }
                else
                {
                    await _writer.WriteAsync(name.AsMemory(), cancellationToken)
                        .ConfigureAwait(_continueOnCapturedContext);
                }
#endif
                _colNamesHeader[colIndex] = name;
                notFirstHeader = true;
            }
#if SYNC
            _writer.WriteLine();
#else
            await _writer.WriteLineAsync()
                .ConfigureAwait(_continueOnCapturedContext);
#endif
            _headerOrFirstRowColCount = cols.Count;
        }
        _headerWrittenOrSkipped = true;
    }

#if SYNC
    void WriteEscaped(ReadOnlySpan<char> chars)
#else
    async ValueTask WriteEscapedAsync(ReadOnlyMemory<char> chars, CancellationToken cancellationToken)
#endif
    {
#if SYNC
        var containsSpecialChar = ContainsSpecialCharacters(chars, _sep.Separator);
#else
        var containsSpecialChar = ContainsSpecialCharacters(chars.Span, _sep.Separator);
#endif
        if (containsSpecialChar != 0)
        {
#if SYNC
            _writer.Write(SepDefaults.Quote);
            WriteQuotesEscaped(chars);
#else
            await _writer.WriteAsync(SepDefaults.Quote)
                .ConfigureAwait(_continueOnCapturedContext);
            await WriteQuotesEscapedAsync(chars, cancellationToken)
                .ConfigureAwait(_continueOnCapturedContext);
#endif
#if SYNC
            _writer.Write(SepDefaults.Quote);
#else
            await _writer.WriteAsync(SepDefaults.Quote)
                .ConfigureAwait(_continueOnCapturedContext);
#endif
        }
        else
        {
#if SYNC
            _writer.Write(chars);
#else
            await _writer.WriteAsync(chars, cancellationToken)
                .ConfigureAwait(_continueOnCapturedContext);
#endif
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if SYNC
    void WriteQuotesEscaped(ReadOnlySpan<char> chars)
#else
    async ValueTask WriteQuotesEscapedAsync(ReadOnlyMemory<char> chars, CancellationToken cancellationToken)
#endif
    {
        var start = 0;
        while (start < chars.Length)
        {
            var remainingChars = chars.Slice(start);
#if SYNC
            var quoteIndex = remainingChars.IndexOf(SepDefaults.Quote);
#else
            var quoteIndex = remainingChars.Span.IndexOf(SepDefaults.Quote);
#endif
            if (quoteIndex == -1)
            {
#if SYNC
                _writer.Write(remainingChars);
#else
                await _writer.WriteAsync(remainingChars, cancellationToken)
                    .ConfigureAwait(_continueOnCapturedContext);
#endif
                break;
            }
            else
            {
#if SYNC
                _writer.Write(remainingChars.Slice(0, quoteIndex + 1));
                _writer.Write(SepDefaults.Quote);
#else
                await _writer.WriteAsync(remainingChars.Slice(0, quoteIndex + 1), cancellationToken)
                    .ConfigureAwait(_continueOnCapturedContext);
                await _writer.WriteAsync(SepDefaults.Quote)
                    .ConfigureAwait(_continueOnCapturedContext);
#endif
                start += quoteIndex + 1;
            }
        }
    }

#if SYNC
    void DisposeManaged()
#else
    async ValueTask DisposeManagedAsync(CancellationToken cancellationToken)
#endif
    {
        if (!_headerWrittenOrSkipped && _cols.Count > 0)
        {
#if SYNC
            WriteHeader();
#else
            await WriteHeaderAsync(cancellationToken)
                .ConfigureAwait(_continueOnCapturedContext);
#endif
        }

#if SYNC
        _textWriterDisposer.Dispose(_writer);
#else
        await _textWriterDisposer.DisposeAsync(_writer)
            .ConfigureAwait(_continueOnCapturedContext);
#endif
        _arrayPool.Dispose();
        foreach (var col in _colNameToCol.Values)
        {
            col.Dispose();
        }
        _colNameToCol.Clear();
    }
}
