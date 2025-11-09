using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nietras.SeparatedValues;

/// <summary>
/// Represents a UTF-8 byte-based separated values (CSV) writer.
/// This class provides a byte-oriented API while leveraging the proven char-based implementation internally.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class SepUtf8Writer : IDisposable, IAsyncDisposable
{
    internal delegate string DebuggerDisplayFunc(Info info, Stream stream);
    internal readonly record struct Info(object Source, DebuggerDisplayFunc DebuggerDisplay);
    internal string DebuggerDisplay => _info.DebuggerDisplay(_info, _stream);

    readonly Info _info;
    readonly Stream _stream;
    readonly StreamWriter _streamWriter;
    readonly SepWriter _innerWriter;
    readonly SepUtf8WriterOptions _options;
    readonly byte _separator;
    bool _disposed;

    internal SepUtf8Writer(Info info, in SepUtf8WriterOptions options, Stream stream)
    {
        _info = info;
        _options = options;
        _stream = stream;
        _separator = (byte)options.Sep.Separator;
        
        // Create StreamWriter that writes UTF-8 to the stream (without BOM)
        var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        _streamWriter = new StreamWriter(stream, utf8NoBom, bufferSize: 4096, leaveOpen: false);
        
        // Create inner writer with corresponding options
        var innerOptions = new SepWriterOptions(options.Sep)
        {
            CultureInfo = options.CultureInfo,
            WriteHeader = options.WriteHeader,
            DisableColCountCheck = options.DisableColCountCheck,
            ColNotSetOption = options.ColNotSetOption,
            Escape = options.Escape,
            AsyncContinueOnCapturedContext = options.AsyncContinueOnCapturedContext
        };
        
        _innerWriter = new SepWriter(
            new SepWriter.Info(info.Source, (i, w) => info.DebuggerDisplay(new Info(i.Source, info.DebuggerDisplay), stream)),
            innerOptions,
            _streamWriter,
            new StreamTextWriterDisposer()
        );
    }

    /// <summary>
    /// Gets the separator specification used by this writer.
    /// </summary>
    public SepSpec Spec => _innerWriter.Spec;

    /// <summary>
    /// Gets the header writer.
    /// </summary>
    public SepUtf8WriterHeader Header => new(_innerWriter.Header);

    /// <summary>
    /// Creates a new row for writing.
    /// </summary>
    /// <returns>A new row reference.</returns>
    public Row NewRow() => new(_innerWriter.NewRow());

    /// <summary>
    /// Creates a new row for writing with cancellation support.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new row reference.</returns>
    public Row NewRow(CancellationToken cancellationToken) => new(_innerWriter.NewRow(cancellationToken));

    /// <summary>
    /// Flushes the writer, ensuring all data is written to the underlying stream.
    /// </summary>
    public void Flush() => _innerWriter.ToString(); // This forces the inner writer to flush

    /// <summary>
    /// Represents a column in a UTF-8 writer row.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly ref struct Col
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        readonly SepWriter.Col _innerCol;

        internal Col(SepWriter.Col innerCol) => _innerCol = innerCol;

        /// <summary>
        /// Sets the column value from a UTF-8 byte span.
        /// </summary>
        /// <param name="utf8Span">The UTF-8 byte span to set.</param>
        public void Set(ReadOnlySpan<byte> utf8Span)
        {
            // Convert UTF-8 bytes to string and set
            var str = Encoding.UTF8.GetString(utf8Span);
            _innerCol.Set(str);
        }

        /// <summary>
        /// Sets the column value from a string.
        /// </summary>
        /// <param name="value">The string value to set.</param>
        public void Set(string value) => _innerCol.Set(value);

        /// <summary>
        /// Sets the column value from a span of chars.
        /// </summary>
        /// <param name="span">The char span to set.</param>
        public void Set(ReadOnlySpan<char> span) => _innerCol.Set(span);

        /// <summary>
        /// Formats and sets a value.
        /// </summary>
        public void Format<T>(T value) where T : ISpanFormattable => _innerCol.Format(value);

        /// <summary>
        /// Formats and sets a value with a format string.
        /// </summary>
        public void Format<T>(T value, ReadOnlySpan<char> format) where T : ISpanFormattable 
            => _innerCol.Format(value, format);
    }

    /// <summary>
    /// Represents a row in the UTF-8 writer.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly ref struct Row
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        readonly SepWriter.Row _innerRow;

        internal Row(SepWriter.Row innerRow) => _innerRow = innerRow;

        /// <summary>
        /// Gets a column by index.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <returns>A column reference.</returns>
        public Col this[int colIndex]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(_innerRow[colIndex]);
        }

        /// <summary>
        /// Gets a column by name.
        /// </summary>
        /// <param name="colName">The column name.</param>
        /// <returns>A column reference.</returns>
        public Col this[string colName]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(_innerRow[colName]);
        }

        /// <summary>
        /// Disposes the row, writing it to the output stream.
        /// </summary>
        public void Dispose() => _innerRow.Dispose();
    }

    #region Dispose
    public void Dispose()
    {
        if (!_disposed)
        {
            _innerWriter?.Dispose();
            _streamWriter?.Dispose();
            _disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_innerWriter != null)
            {
                await _innerWriter.DisposeAsync().ConfigureAwait(false);
            }
            if (_streamWriter != null)
            {
                await _streamWriter.DisposeAsync().ConfigureAwait(false);
            }
            _disposed = true;
        }
    }
    #endregion

    // Helper class for disposing StreamWriter
    class StreamTextWriterDisposer : ISepTextWriterDisposer
    {
        public void Dispose(TextWriter writer) => writer.Dispose();
        public ValueTask DisposeAsync(TextWriter writer) => writer.DisposeAsync();
    }
}

/// <summary>
/// Represents header information for a UTF-8 writer.
/// </summary>
public readonly ref struct SepUtf8WriterHeader
{
    readonly SepWriterHeader _innerHeader;

    internal SepUtf8WriterHeader(SepWriterHeader innerHeader) => _innerHeader = innerHeader;

    /// <summary>
    /// Adds a column to the header.
    /// </summary>
    /// <param name="colName">The column name.</param>
    public void Add(string colName) => _innerHeader.Add(colName);

    /// <summary>
    /// Writes the header row to the output.
    /// </summary>
    public void Write() => _innerHeader.Write();
}
