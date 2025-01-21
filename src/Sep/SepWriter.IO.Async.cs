//#define SYNC
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
            await WriteHeaderAsync(cancellationToken);
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
                        await _writer.WriteAsync(_sep.Separator);
#endif
                    }
#if SYNC
                    var span = col.GetSpan();
#else
                    var span = col.GetMemory();
#endif
                    if (_escape)
                    {
#if SYNC
                        WriteEscaped(span);
#else
                        await WriteEscapedAsync(span, cancellationToken);
#endif
                    }
                    else
                    {
#if SYNC
                        _writer.Write(span);
#else
                        await _writer.WriteAsync(span, cancellationToken);
#endif
                    }
                    notFirst = true;
                }
            }
        }
#if SYNC
        _writer.WriteLine();
#else
        await _writer.WriteLineAsync();
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
                    await _writer.WriteAsync(_sep.Separator);
#endif
                }
                var name = col.Name;

#if SYNC
                if (_escape) { WriteEscaped(name); }
                else { _writer.Write(name); }
#else
                if (_escape) { await WriteEscapedAsync(name.AsMemory(), cancellationToken); }
                else { await _writer.WriteAsync(name.AsMemory(), cancellationToken); }
#endif
                _colNamesHeader[colIndex] = name;
                notFirstHeader = true;
            }
#if SYNC
            _writer.WriteLine();
#else
            await _writer.WriteLineAsync();
#endif
            _headerOrFirstRowColCount = cols.Count;
        }
        _headerWrittenOrSkipped = true;
    }

#if SYNC
    void WriteEscaped(ReadOnlySpan<char> span)
#else
    async ValueTask WriteEscapedAsync(ReadOnlyMemory<char> span, CancellationToken cancellationToken)
#endif
    {
#if SYNC
        var containsSpecialChar = ContainsSpecialCharacters(span, _sep.Separator);
#else
        var containsSpecialChar = ContainsSpecialCharacters(span.Span, _sep.Separator);
#endif
        if (containsSpecialChar != 0)
        {
#if SYNC
            _writer.Write(SepDefaults.Quote);
            WriteQuotesEscaped(span);
#else
            await _writer.WriteAsync(SepDefaults.Quote);
            await WriteQuotesEscapedAsync(span, cancellationToken);
#endif
#if SYNC
            _writer.Write(SepDefaults.Quote);
#else
            await _writer.WriteAsync(SepDefaults.Quote);
#endif
        }
        else
        {
#if SYNC
            _writer.Write(span);
#else
            await _writer.WriteAsync(span, cancellationToken);
#endif
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if SYNC
    void WriteQuotesEscaped(ReadOnlySpan<char> span)
#else
    async ValueTask WriteQuotesEscapedAsync(ReadOnlyMemory<char> span, CancellationToken cancellationToken)
#endif
    {
        var start = 0;
        while (start < span.Length)
        {
            var remainingSpan = span.Slice(start);
#if SYNC
            var quoteIndex = remainingSpan.IndexOf(SepDefaults.Quote);
#else
            var quoteIndex = remainingSpan.Span.IndexOf(SepDefaults.Quote);
#endif
            if (quoteIndex == -1)
            {
#if SYNC
                _writer.Write(remainingSpan);
#else
                await _writer.WriteAsync(remainingSpan, cancellationToken);
#endif
                break;
            }
            else
            {
#if SYNC
                _writer.Write(remainingSpan.Slice(0, quoteIndex + 1));
                _writer.Write(SepDefaults.Quote);
#else
                await _writer.WriteAsync(remainingSpan.Slice(0, quoteIndex + 1), cancellationToken);
                await _writer.WriteAsync(SepDefaults.Quote);
#endif
                start += quoteIndex + 1;
            }
        }
        // Original basic loop implementation
        //foreach (var c in span)
        //{
        //    _writer.Write(c);
        //    if (c == SepDefaults.Quote)
        //    {
        //        _writer.Write(SepDefaults.Quote);
        //    }
        //}
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
            await WriteHeaderAsync(cancellationToken);
#endif
        }

#if SYNC
        _disposeTextWriter(_writer);
#else
        // TODO TODO TODO !!!
        _disposeTextWriter(_writer);
#endif
        _arrayPool.Dispose();
        foreach (var col in _colNameToCol.Values)
        {
            col.Dispose();
        }
        _colNameToCol.Clear();
    }
}
