using System;
using System.Buffers;
using System.Collections.Generic;
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
/// This class works directly with byte sequences for optimal UTF-8 performance.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class SepUtf8Writer : IDisposable, IAsyncDisposable
{
    internal delegate string DebuggerDisplayFunc(Info info, Stream stream);
    internal readonly record struct Info(object Source, DebuggerDisplayFunc DebuggerDisplay);
    internal string DebuggerDisplay => _info.DebuggerDisplay(_info, _stream);

    readonly Info _info;
    readonly Stream _stream;
    readonly SepUtf8WriterOptions _options;
    readonly byte _separator;
    readonly CultureInfo? _cultureInfo;
    bool _disposed;

    // Column tracking
    internal readonly List<(string ColName, int ColIndex)> _colNameCache = [];
    internal readonly Dictionary<string, ColImpl> _colNameToCol = [];
    internal readonly List<ColImpl> _cols = [];
    internal bool _headerWrittenOrSkipped = false;
    bool _newRowActive = false;

    internal SepUtf8Writer(Info info, in SepUtf8WriterOptions options, Stream stream)
    {
        _info = info;
        _options = options;
        _stream = stream;
        _separator = (byte)(options.Sep.Separator);
        _cultureInfo = options.CultureInfo;
    }

    /// <summary>
    /// Gets the separator specification used by this writer.
    /// </summary>
    public SepSpec Spec => new(new((char)_separator), _cultureInfo, _options.AsyncContinueOnCapturedContext);

    /// <summary>
    /// Gets the header writer.
    /// </summary>
    public SepUtf8WriterHeader Header => new(this);

    /// <summary>
    /// Creates a new row for writing.
    /// </summary>
    /// <returns>A new row reference.</returns>
    public Row NewRow()
    {
        PrepareNewRow();
        return new(this);
    }

    /// <summary>
    /// Creates a new row for writing with cancellation support.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new row reference.</returns>
    public Row NewRow(CancellationToken cancellationToken)
    {
        PrepareNewRow();
        return new(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void PrepareNewRow()
    {
        if (_newRowActive) { SepThrow.InvalidOperationException_WriterAlreadyHasActiveRow(); }
        _newRowActive = true;
        foreach (var col in _cols) { col.Clear(); }
    }

    /// <summary>
    /// Internal column implementation.
    /// </summary>
    internal sealed class ColImpl(SepUtf8Writer writer, int index, string name)
    {
        internal const int MinimumLength = 256;
        internal readonly SepUtf8Writer _writer = writer;
        internal byte[] _buffer = ArrayPool<byte>.Shared.Rent(MinimumLength);
        internal int _position = 0;

        public int Index { get; } = index;
        public string Name { get; } = name;
        public bool HasBeenSet { get; set; } = false;

        public void Clear() { HasBeenSet = false; _position = 0; }

        public void Append(ReadOnlySpan<byte> source)
        {
            EnsureCapacity(source.Length);
            source.CopyTo(_buffer.AsSpan(_position));
            _position += source.Length;
        }

        public ReadOnlySpan<byte> GetSpan() => _buffer.AsSpan(0, _position);

        public void Dispose()
        {
            if (_buffer != null)
            {
                ArrayPool<byte>.Shared.Return(_buffer);
                _buffer = null!;
            }
        }

        void EnsureCapacity(int additionalLength)
        {
            if (_position + additionalLength > _buffer.Length)
            {
                GrowBuffer(additionalLength);
            }
        }

        void GrowBuffer(int additionalLength)
        {
            var newSize = Math.Max(_buffer.Length * 2, _position + additionalLength);
            var newBuffer = ArrayPool<byte>.Shared.Rent(newSize);
            _buffer.AsSpan(0, _position).CopyTo(newBuffer);
            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = newBuffer;
        }
    }

    /// <summary>
    /// Represents a column in a UTF-8 writer row.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly ref struct Col
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        readonly ColImpl _impl;

        internal Col(ColImpl impl) => _impl = impl;

        internal int ColIndex => _impl.Index;
        internal string ColName => _impl.Name;

        /// <summary>
        /// Sets the column value from a UTF-8 byte span.
        /// </summary>
        /// <param name="utf8Span">The UTF-8 byte span to set.</param>
        public void Set(ReadOnlySpan<byte> utf8Span)
        {
            var impl = _impl;
            impl.Clear();
            impl.Append(utf8Span);
            MarkSet();
        }

        /// <summary>
        /// Sets the column value from a string (will be converted to UTF-8).
        /// </summary>
        /// <param name="value">The string value to set.</param>
        public void Set(string value)
        {
            var impl = _impl;
            impl.Clear();
            var byteCount = Encoding.UTF8.GetByteCount(value);
            var buffer = impl._buffer;
            if (byteCount > buffer.Length)
            {
                buffer = ArrayPool<byte>.Shared.Rent(byteCount);
                ArrayPool<byte>.Shared.Return(impl._buffer);
                impl._buffer = buffer;
            }
            var written = Encoding.UTF8.GetBytes(value, buffer);
            impl._position = written;
            MarkSet();
        }

        void MarkSet() => _impl.HasBeenSet = true;
    }

    /// <summary>
    /// Represents a row in the UTF-8 writer.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly ref struct Row
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        readonly SepUtf8Writer _writer;

        internal Row(SepUtf8Writer writer) => _writer = writer;

        /// <summary>
        /// Gets a column by index.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <returns>A column reference.</returns>
        public Col this[int colIndex]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(_writer._cols[colIndex]);
        }

        /// <summary>
        /// Gets a column by name.
        /// </summary>
        /// <param name="colName">The column name.</param>
        /// <returns>A column reference.</returns>
        public Col this[string colName]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(_writer._colNameToCol[colName]);
        }

        /// <summary>
        /// Disposes the row, writing it to the output stream.
        /// </summary>
        public void Dispose()
        {
            _writer._newRowActive = false;
            // Placeholder - actual write logic would go here
        }
    }

    #region Dispose
    public void Dispose()
    {
        if (!_disposed)
        {
            _stream?.Dispose();
            foreach (var col in _cols)
            {
                col.Dispose();
            }
            _disposed = true;
        }
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
    #endregion
}

/// <summary>
/// Represents header information for a UTF-8 writer.
/// </summary>
public readonly ref struct SepUtf8WriterHeader
{
    readonly SepUtf8Writer _writer;

    internal SepUtf8WriterHeader(SepUtf8Writer writer) => _writer = writer;

    /// <summary>
    /// Adds a column to the header.
    /// </summary>
    /// <param name="colName">The column name.</param>
    /// <returns>The column index.</returns>
    public int Add(string colName)
    {
        if (_writer._headerWrittenOrSkipped)
        {
            SepThrow.InvalidOperationException_CannotAddColNameHeaderAlreadyWritten(colName);
        }

        var index = _writer._cols.Count;
        var col = new SepUtf8Writer.ColImpl(_writer, index, colName);
        _writer._cols.Add(col);
        _writer._colNameToCol.Add(colName, col);
        _writer._colNameCache.Add((colName, index));
        return index;
    }

    /// <summary>
    /// Writes the header row to the output.
    /// </summary>
    public void Write()
    {
        if (_writer._headerWrittenOrSkipped)
        {
            SepThrow.InvalidOperationException_CannotAddColNameHeaderAlreadyWritten(string.Empty);
        }
        _writer._headerWrittenOrSkipped = true;
        // Placeholder - actual write logic would go here
    }
}
