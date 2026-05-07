using System;
using System.IO;
using System.IO.Compression;

namespace nietras.SeparatedValues;

static class SepFileByExtension
{
    public static TextWriter CreateWriter(string filePath, FileStreamOptions options,
        out SepWriter.DebuggerDisplayFunc debuggerDisplay)
    {
        var compression = GetCompression(filePath);
        debuggerDisplay = compression.FileWriterDebuggerDisplay;
        Stream stream = new FileStream(filePath, options);
        if (compression.Factory is { } factory)
        {
            stream = factory(stream, CompressionMode.Compress, leaveOpen: false);
        }
        return new StreamWriter(stream, leaveOpen: false);
    }

    public static TextWriter CreateWriter(string name, Stream stream,
        out SepWriter.DebuggerDisplayFunc debuggerDisplay, bool leaveOpen)
    {
        var compression = GetCompression(name);
        debuggerDisplay = compression.NameStreamWriterDebuggerDisplay;
        if (compression.Factory is { } factory)
        {
            stream = factory(stream, CompressionMode.Compress, leaveOpen);
        }
        return new StreamWriter(stream, leaveOpen: false);
    }

    public static TextReader CreateReader(string filePath, FileStreamOptions options,
        out Func<SepReader.Info, string> debuggerDisplay)
    {
        var compression = GetCompression(filePath);
        debuggerDisplay = compression.FileReaderDebuggerDisplay;
        Stream stream = new FileStream(filePath, options);
        if (compression.Factory is { } factory)
        {
            stream = factory(stream, CompressionMode.Decompress, leaveOpen: false);
        }
        return new StreamReader(stream, leaveOpen: false);
    }

    public static TextReader CreateReader(string name, Stream stream,
        out Func<SepReader.Info, string> debuggerDisplay, bool leaveOpen)
    {
        var compression = GetCompression(name);
        debuggerDisplay = compression.NameStreamReaderDebuggerDisplay;
        if (compression.Factory is { } factory)
        {
            stream = factory(stream, CompressionMode.Decompress, leaveOpen);
        }
        return new StreamReader(stream, leaveOpen: false);
    }

    delegate Stream CompressionStreamFactory(Stream stream, CompressionMode mode, bool leaveOpen);

    readonly record struct Compression(
        CompressionStreamFactory? Factory,
        SepWriter.DebuggerDisplayFunc FileWriterDebuggerDisplay,
        Func<SepReader.Info, string> FileReaderDebuggerDisplay,
        SepWriter.DebuggerDisplayFunc NameStreamWriterDebuggerDisplay,
        Func<SepReader.Info, string> NameStreamReaderDebuggerDisplay);

    static Compression GetCompression(string path) =>
        path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
            ? new(
                static (stream, mode, leaveOpen) => new GZipStream(stream, mode, leaveOpen),
                static (info, writer) => $"File='{info.Source}' Compression='GZip'",
                static info => $"File='{info.Source}' Decompression='GZip'",
                static (info, writer) => $"Stream Name='{info.Source}' Compression='GZip'",
                static info => $"Stream Name='{info.Source}' Decompression='GZip'")
            : path.EndsWith(".br", StringComparison.OrdinalIgnoreCase)
                ? new(
                    static (stream, mode, leaveOpen) => new BrotliStream(stream, mode, leaveOpen),
                    static (info, writer) => $"File='{info.Source}' Compression='Brotli'",
                    static info => $"File='{info.Source}' Decompression='Brotli'",
                    static (info, writer) => $"Stream Name='{info.Source}' Compression='Brotli'",
                    static info => $"Stream Name='{info.Source}' Decompression='Brotli'")
                : path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                    ? throw new NotSupportedException("Extension '.zip' is not supported. Use '.gz' or '.br' for single-stream compression.")
                    : new(
                        Factory: null,
                        FileWriterDebuggerDisplay: static (info, writer) => $"File='{info.Source}'",
                        FileReaderDebuggerDisplay: static info => $"File='{info.Source}'",
                        NameStreamWriterDebuggerDisplay: static (info, writer) => $"Stream Name='{info.Source}'",
                        NameStreamReaderDebuggerDisplay: static info => $"Stream Name='{info.Source}'");
}
