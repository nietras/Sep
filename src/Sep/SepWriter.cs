using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace nietras.SeparatedValues;

public sealed partial class SepWriter : IDisposable
{
    const int DefaultCapacity = 16;
    readonly Sep _sep;
    readonly CultureInfo? _cultureInfo;
    internal readonly bool _writeHeader;
    readonly bool _escape;
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
        _escape = options.Escape;
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
                    _writer.Write(sb.GetSpan());
                }
                notFirst = true;
            }
        }
        _writer.WriteLine();

        _newRowActive = false;
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

                if (_escape) { WriteEscaped(name); }
                else { _writer.Write(name); }

                _colNamesHeader[colIndex] = name;
                notFirstHeader = true;
            }
            _writer.WriteLine();
        }
        _headerWrittenOrSkipped = true;
    }

    void WriteEscaped(ColBuilder sb)
    {
        var separator = _sep.Separator;
        uint containsSpecialChar = 0;

        var span = sb.GetSpan();
        containsSpecialChar |= ContainsSpecialCharacters(span, separator);

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
    static uint ContainsSpecialCharacters(ReadOnlySpan<char> span, char separator)
    {
        foreach (var c in span)
        {
            var se = c == separator ? 1u : 0u;
            var qe = c == SepDefaults.Quote ? 1u : 0u;
            var ce = c == SepDefaults.CarriageReturn ? 1u : 0u;
            var le = c == SepDefaults.LineFeed ? 1u : 0u;
            var containsSpecialChar = (se | qe) | (ce | le);
            if (containsSpecialChar != 0) { return 1; }
        }
        return 0;

        // http://0x80.pl/notesen/2023-03-06-swar-find-any.html
        // Tried adopting to 16-bit char (only little endian) (DOES NOT WORK)
        //var specialCharacters = (ulong)separator << 48 | SepDefaults.Quote << 32 | SepDefaults.CarriageReturn << 16 | SepDefaults.LineFeed;
        //foreach (var c in span)
        //{
        //    var broadcast = Broadcast(c);
        //    var compare = broadcast ^ specialCharacters; // Zero if equal since XOR
        //    var hasZero = HasZero(compare);
        //    if (hasZero != 0)
        //    {
        //        return 1u;
        //    }
        //}
        //return 0u;
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //static ulong Broadcast(char c) => 0x0001000100010001ul * c;
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //static ulong HasZero(ulong v) => ((v - 0x0001000100010001ul) & ~(v) & 0x8000800080008000);
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //static ulong IndexOfFirstSet(ulong v) => ((((v - 1) & 0x0001000100010001ul) * 0x0001000100010001ul) >> 60) - 1;
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
            col.Text.Dispose();
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
