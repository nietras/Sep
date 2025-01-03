using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace nietras.SeparatedValues;

public sealed partial class SepWriter : IDisposable
{
    const int DefaultCapacity = 16;
    readonly Sep _sep;
    readonly CultureInfo? _cultureInfo;
    internal readonly bool _writeHeader;
    internal readonly bool _escape; // Added escape option
    // _writer dispose handled by _disposeTextWriter
#pragma warning disable CA2213 // Disposable fields should be disposed
    readonly TextWriter _writer;
#pragma warning restore CA2213 // Disposable fields should be disposed
    readonly Action<TextWriter> _disposeTextWriter;
    internal readonly List<(string ColName, int ColIndex)> _colNameCache = new(DefaultCapacity);

    // TODO: Add Stack<ColImpl> for remove/add cols when manipulating
    internal readonly Dictionary<string, ColImpl> _colNameToCol = new(DefaultCapacity);
    // Once header is written cols cannot be added or removed
    internal List<ColImpl> _cols = new(DefaultCapacity);
    internal string[] _colNamesHeader = Array.Empty<string>();

    internal readonly SepArrayPoolAccessIndexed _arrayPool = new();
    internal bool _headerWrittenOrSkipped = false;
    bool _newRowActive = false;
    int _cacheIndex = 0;

    internal SepWriter(SepWriterOptions options, TextWriter writer, Action<TextWriter> disposeTextWriter)
    {
        _sep = options.Sep;
        _cultureInfo = options.CultureInfo;
        _writeHeader = options.WriteHeader;
        _escape = options.Escape; // Initialize escape option
        _writer = writer;
        _disposeTextWriter = disposeTextWriter;
        Header = new(this);
    }

    public SepSpec Spec => new(_sep, _cultureInfo);
    public SepWriterHeader Header { get; }

    public Row NewRow()
    {
        if (_newRowActive) { SepThrow.InvalidOperationException_WriterAlreadyHasActiveRow(); }
        _newRowActive = true;
        _cacheIndex = 0;
        _arrayPool.Reset();
        foreach (var col in _cols) { col.Clear(); }
        return new(this);
    }

    internal void EndRow(Row row)
    {
        if (!_newRowActive) { SepThrow.InvalidOperationException_WriterDoesNotHaveActiveRow(); }

        A.Assert(!_writeHeader || _colNameToCol.Count == _cols.Count);
        var cols = _cols;

        // Header
        if (!_headerWrittenOrSkipped)
        {
            WriteHeader();
        }
        else
        {
            // Note this prevents writing different number of cols (or less cols
            // than previous row) in case of no header written. Revisit this if
            // variable cols count is needed.
            for (var colIndex = 0; colIndex < cols.Count; ++colIndex)
            {
                var col = cols[colIndex];
                if (!col.HasBeenSet)
                {
                    SepThrow.InvalidOperationException_NotAllColsSet(cols, _colNamesHeader);
                }
            }
            A.Assert(!_writeHeader || cols.Count == _colNamesHeader.Length);
        }

        // New Row
        if (cols.Count > 0)
        {
            var notFirst = false;
            for (var colIndex = 0; colIndex < cols.Count; ++colIndex)
            {
                var col = cols[colIndex];
                if (notFirst)
                {
                    _writer.Write(_sep.Separator);
                }
                var sb = col.Text;
                if (_escape)
                {
                    WriteEscaped(sb);
                }
                else
                {
                    foreach (var chunk in sb.GetChunks())
                    {
                        _writer.Write(chunk.Span);
                    }
                }
                notFirst = true;
            }
        }
        _writer.WriteLine();

        _newRowActive = false;
    }

    private void WriteEscaped(StringBuilder sb)
    {
        var quote = '"';
        var separator = _sep.Separator;
        var containsSpecialChar = false;

        foreach (var chunk in sb.GetChunks())
        {
            var span = chunk.Span;
            foreach (var c in span)
            {
                if (c == quote || c == separator || c == '\r' || c == '\n')
                {
                    containsSpecialChar = true;
                    break;
                }
            }
            if (containsSpecialChar) break;
        }

        if (containsSpecialChar)
        {
            _writer.Write(quote);
            foreach (var chunk in sb.GetChunks())
            {
                var span = chunk.Span;
                foreach (var c in span)
                {
                    if (c == quote)
                    {
                        _writer.Write(quote);
                        _writer.Write(quote);
                    }
                    else
                    {
                        _writer.Write(c);
                    }
                }
            }
            _writer.Write(quote);
        }
        else
        {
            foreach (var chunk in sb.GetChunks())
            {
                _writer.Write(chunk.Span);
            }
        }
    }

    public void Flush() => _writer.Flush();

    public override string ToString()
    {
        if (_writer is StringWriter stringWriter)
        {
            return stringWriter.ToString();
        }
        SepThrow.NotSupportedException_ToStringOnNotStringWriter(_writer);
        return null;
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
                _writer.Write(name);
                _colNamesHeader[colIndex] = name;
                notFirstHeader = true;
            }
            _writer.WriteLine();
        }
        _headerWrittenOrSkipped = true;
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
            SepStringBuilderPool.Return(col.Text);
        }
        _colNameToCol.Clear();
    }

    #region Dispose
    bool _disposed;
    void Dispose(bool disposing)
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
