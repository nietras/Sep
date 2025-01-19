using System;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

public partial class SepWriter
{
    public void Flush() => _writer.Flush();

    internal void EndRow(Row row)
    {
        if (!_newRowActive) { SepThrow.InvalidOperationException_WriterDoesNotHaveActiveRow(); }

        var cols = _cols;

        // Header
        if (!_headerWrittenOrSkipped)
        {
            WriteHeader();
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
                        _writer.Write(_sep.Separator);
                    }
                    var span = col.GetSpan();
                    if (_escape)
                    {
                        WriteEscaped(span);
                    }
                    else
                    {
                        _writer.Write(span);
                    }
                    notFirst = true;
                }
            }
        }
        _writer.WriteLine();
        if (_headerOrFirstRowColCount == -1)
        {
            _headerOrFirstRowColCount = cols.Count;
        }
        _newRowActive = false;
    }

    internal void WriteHeader()
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
                    _writer.Write(_sep.Separator);
                }
                var name = col.Name;

                if (_escape) { WriteEscaped(name); }
                else { _writer.Write(name); }

                _colNamesHeader[colIndex] = name;
                notFirstHeader = true;
            }
            _writer.WriteLine();
            _headerOrFirstRowColCount = cols.Count;
        }
        _headerWrittenOrSkipped = true;
    }

    void WriteEscaped(ReadOnlySpan<char> span)
    {
        var containsSpecialChar = ContainsSpecialCharacters(span, _sep.Separator);
        if (containsSpecialChar != 0)
        {
            _writer.Write(SepDefaults.Quote);
            WriteQuotesEscaped(span);
            _writer.Write(SepDefaults.Quote);
        }
        else
        {
            _writer.Write(span);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void WriteQuotesEscaped(ReadOnlySpan<char> span)
    {
        var start = 0;
        while (start < span.Length)
        {
            var remainingSpan = span.Slice(start);
            var quoteIndex = remainingSpan.IndexOf(SepDefaults.Quote);
            if (quoteIndex == -1)
            {
                _writer.Write(remainingSpan);
                break;
            }
            else
            {
                _writer.Write(remainingSpan.Slice(0, quoteIndex + 1));
                _writer.Write(SepDefaults.Quote);
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

    void DisposeManaged()
    {
        if (!_headerWrittenOrSkipped && _cols.Count > 0)
        {
            WriteHeader();
        }

        _disposeTextWriter(_writer);
        _arrayPool.Dispose();
        foreach (var col in _colNameToCol.Values)
        {
            col.Dispose();
        }
        _colNameToCol.Clear();
    }
}
