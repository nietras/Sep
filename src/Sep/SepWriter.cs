using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace nietras.SeparatedValues;

public partial class SepWriter : IDisposable
{
    const int DefaultCapacity = 16;
    readonly Sep _sep;
    readonly CultureInfo? _cultureInfo;
    readonly TextWriter _writer;
    internal readonly List<(string ColName, int ColIndex)> _colNameCache = new(DefaultCapacity);

    // TODO: Add Stack<ColImpl> for remove/add cols when manipulating
    readonly Dictionary<string, ColImpl> _colNameToCol = new(DefaultCapacity);
    // Once header is written cols cannot be added or removed
    internal List<ColImpl> _cols = new(DefaultCapacity);
    internal string[] _colNamesHeader = Array.Empty<string>();

    internal readonly SepArrayPoolAccessIndexed _arrayPool = new();
    bool _headerWritten = false;
    bool _newRowActive = false;
    int _cacheIndex = 0;

    public SepWriter(SepWriterOptions options, TextWriter writer)
    {
        _sep = options.Sep;
        _cultureInfo = options.CultureInfo;
        _writer = writer;
    }

    public SepSpec Spec => new(_sep, _cultureInfo);

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

        A.Assert(_colNameToCol.Count == _cols.Count);
        var cols = _cols;

        // Header
        if (!_headerWritten)
        {
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
            _headerWritten = true;
        }
        else
        {
            for (var colIndex = 0; colIndex < cols.Count; ++colIndex)
            {
                var col = cols[colIndex];
                if (!col.HasBeenSet)
                {
                    SepThrow.InvalidOperationException_NotAllColsSet(cols, _colNamesHeader);
                }
            }
            A.Assert(cols.Count == _colNamesHeader.Length);
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
                foreach (var chunk in sb.GetChunks())
                {
                    _writer.Write(chunk.Span);
                }
                notFirst = true;
            }
        }
        _writer.WriteLine();

        _newRowActive = false;
    }


    public override string ToString()
    {
        if (_writer is StringWriter stringWriter)
        {
            return stringWriter.ToString();
        }
        SepThrow.NotSupportedException_ToStringOnNotStringWriter(_writer);
        return null;
    }

    void DisposeManaged()
    {
        _writer.Dispose();
        _arrayPool.Dispose();
        foreach (var col in _colNameToCol.Values)
        {
            SepStringBuilderPool.Return(col.Text);
        }
        _colNameToCol.Clear();
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
