using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nietras.SeparatedValues;

/// <summary>
/// Represents a UTF-8 byte-based separated values (CSV) reader.
/// This class provides a byte-oriented API while leveraging the proven char-based implementation internally.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed partial class SepUtf8Reader : IDisposable
{
    internal readonly record struct Info(object Source, Func<Info, string> DebuggerDisplay);
    internal string DebuggerDisplay => _info.DebuggerDisplay(_info);

    readonly Info _info;
    readonly SepReader _innerReader;
    readonly StreamReader _streamReader;
    readonly SepUtf8ReaderOptions _options;
    byte _separator;
    bool _disposed;

    internal SepUtf8Reader(Info info, in SepUtf8ReaderOptions options, Stream stream)
    {
        _info = info;
        _options = options;
        _separator = options.Sep.HasValue ? (byte)options.Sep.Value.Separator : (byte)';';

        // Create StreamReader from the UTF-8 stream
        _streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: options.InitialBufferLength, leaveOpen: false);

        // Create inner reader with corresponding options
        var innerOptions = new SepReaderOptions(options.Sep)
        {
            InitialBufferLength = options.InitialBufferLength,
            CultureInfo = options.CultureInfo,
            HasHeader = options.HasHeader,
            ColNameComparer = options.ColNameComparer,
            DisableFastFloat = options.DisableFastFloat,
            DisableColCountCheck = options.DisableColCountCheck,
            DisableQuotesParsing = options.DisableQuotesParsing,
            Unescape = options.Unescape,
            Trim = options.Trim,
            AsyncContinueOnCapturedContext = options.AsyncContinueOnCapturedContext
        };

        _innerReader = new SepReader(new SepReader.Info(info.Source, i => info.DebuggerDisplay(new Info(i.Source, info.DebuggerDisplay))), innerOptions, _streamReader);
        _innerReader.Initialize(innerOptions);
    }

    /// <summary>
    /// Gets whether the source is empty (no rows or headers).
    /// </summary>
    public bool IsEmpty => _innerReader.IsEmpty;

    /// <summary>
    /// Gets the separator specification used by this reader.
    /// </summary>
    public SepSpec Spec => _innerReader.Spec;

    /// <summary>
    /// Gets whether this reader has a header row.
    /// </summary>
    public bool HasHeader => _innerReader.HasHeader;

    /// <summary>
    /// Gets whether there are any data rows (excluding header).
    /// </summary>
    public bool HasRows => _innerReader.HasRows;

    /// <summary>
    /// Gets the header information.
    /// </summary>
    public SepUtf8ReaderHeader Header
    {
        get
        {
            var innerHeader = _innerReader.Header;
            return new SepUtf8ReaderHeader(innerHeader);
        }
    }

    /// <summary>
    /// Gets the current row.
    /// </summary>
    public Row Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this);
    }

    /// <summary>
    /// Moves to the next row.
    /// </summary>
    /// <returns>True if there is a next row, false if end of stream.</returns>
    public bool MoveNext() => _innerReader.MoveNext();

    /// <summary>
    /// Asynchronously moves to the next row.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if there is a next row, false if end of stream.</returns>
    public ValueTask<bool> MoveNextAsync(CancellationToken cancellationToken = default)
        => _innerReader.MoveNextAsync(cancellationToken);

    /// <summary>
    /// Gets the enumerator for this reader (allows foreach usage).
    /// </summary>
    public SepUtf8Reader GetEnumerator() => this;

    /// <summary>
    /// Represents a row in the UTF-8 reader.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly ref struct Row
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        readonly SepUtf8Reader _reader;
        readonly SepReader.Row _innerRow;

        internal Row(SepUtf8Reader reader)
        {
            _reader = reader;
            _innerRow = reader._innerReader.Current;
        }

        /// <summary>
        /// Gets the number of columns in this row.
        /// </summary>
        public int ColCount => _innerRow.ColCount;

        /// <summary>
        /// Gets the column at the specified index as a byte span.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The column data as a readonly byte span.</returns>
        public ReadOnlySpan<byte> this[int colIndex]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var charSpan = _innerRow[colIndex].Span;
                return Encoding.UTF8.GetBytes(charSpan.ToArray());
            }
        }

        /// <summary>
        /// Gets the column by name as a byte span.
        /// </summary>
        /// <param name="colName">The column name.</param>
        /// <returns>The column data as a readonly byte span.</returns>
        public ReadOnlySpan<byte> this[string colName]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var charSpan = _innerRow[colName].Span;
                return Encoding.UTF8.GetBytes(charSpan.ToArray());
            }
        }

        /// <summary>
        /// Converts the column at the specified index to a string.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The column value as a string.</returns>
        public string ToString(int colIndex) => _innerRow[colIndex].ToString();

        /// <summary>
        /// Gets the entire row as a byte span.
        /// </summary>
        public ReadOnlySpan<byte> Span
        {
            get
            {
                var charSpan = _innerRow.Span;
                return Encoding.UTF8.GetBytes(charSpan.ToArray());
            }
        }
    }

    #region Dispose
    public void Dispose()
    {
        if (!_disposed)
        {
            _innerReader?.Dispose();
            _streamReader?.Dispose();
            _disposed = true;
        }
    }
    #endregion
}

/// <summary>
/// Represents header information for a UTF-8 reader.
/// </summary>
public readonly record struct SepUtf8ReaderHeader
{
    internal static readonly SepUtf8ReaderHeader Empty = new(SepReaderHeader.Empty);

    readonly SepReaderHeader _innerHeader;

    internal SepUtf8ReaderHeader(SepReaderHeader innerHeader)
    {
        _innerHeader = innerHeader;
    }

    /// <summary>
    /// Gets the column names.
    /// </summary>
    public IReadOnlyList<string> ColNames => _innerHeader.ColNames;

    /// <summary>
    /// Gets the index of the column with the specified name.
    /// </summary>
    /// <param name="colName">The column name.</param>
    /// <returns>The column index, or -1 if not found.</returns>
    public int IndexOf(string colName) => _innerHeader.IndexOf(colName);

    /// <summary>
    /// Tries to get the index of the column with the specified name.
    /// </summary>
    /// <param name="colName">The column name.</param>
    /// <param name="colIndex">The column index if found.</param>
    /// <returns>True if the column was found, false otherwise.</returns>
    public bool TryIndexOf(string colName, out int colIndex) => _innerHeader.TryIndexOf(colName, out colIndex);
}
