using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

public sealed partial class SepWriter : IDisposable
{
    const int DefaultCapacity = 16;
    readonly Sep _sep;
    readonly CultureInfo? _cultureInfo;
    internal readonly bool _writeHeader;
    readonly bool _disableColCountCheck;
    readonly SepColNotSetOption _colNotSetOption;
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
    internal int _headerOrFirstRowColCount = -1;
    bool _newRowActive = false;
    int _cacheIndex = 0;

    internal SepWriter(SepWriterOptions options, TextWriter writer, Action<TextWriter> disposeTextWriter)
    {
        _sep = options.Sep;
        _cultureInfo = options.CultureInfo;
        _writeHeader = options.WriteHeader;
        _disableColCountCheck = options.DisableColCountCheck;
        _colNotSetOption = options.ColNotSetOption;
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

    public override string ToString()
    {
        if (_writer is StringWriter stringWriter)
        {
            return stringWriter.ToString();
        }
        SepThrow.NotSupportedException_ToStringOnNotStringWriter(_writer);
        return null;
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
