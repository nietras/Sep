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
        debuggerDisplay = compression.WriterDebuggerDisplay;
        Stream stream = new FileStream(filePath, options);
        if (compression.Factory is { } factory)
        {
            stream = factory(stream, CompressionMode.Compress);
        }
        return new StreamWriter(stream, leaveOpen: false);
    }

    public static TextReader CreateReader(string filePath, FileStreamOptions options,
        out Func<SepReader.Info, string> debuggerDisplay)
    {
        var compression = GetCompression(filePath);
        debuggerDisplay = compression.ReaderDebuggerDisplay;
        Stream stream = new FileStream(filePath, options);
        if (compression.Factory is { } factory)
        {
            stream = factory(stream, CompressionMode.Decompress);
        }
        return new StreamReader(stream, leaveOpen: false);
    }

    delegate Stream CompressionStreamFactory(Stream stream, CompressionMode mode);

    readonly record struct Compression(
        CompressionStreamFactory? Factory,
        SepWriter.DebuggerDisplayFunc WriterDebuggerDisplay,
        Func<SepReader.Info, string> ReaderDebuggerDisplay);

    static Compression GetCompression(string path) =>
        path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
            ? new(
                static (stream, mode) => new GZipStream(stream, mode, leaveOpen: false),
                static (info, writer) => $"File='{info.Source}' Compression='GZip'",
                static info => $"File='{info.Source}' Decompression='GZip'")
            : path.EndsWith(".br", StringComparison.OrdinalIgnoreCase)
                ? new(
                    static (stream, mode) => new BrotliStream(stream, mode, leaveOpen: false),
                    static (info, writer) => $"File='{info.Source}' Compression='Brotli'",
                    static info => $"File='{info.Source}' Decompression='Brotli'")
                : path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                    ? throw new NotSupportedException("Extension '.zip' is not supported. Use '.gz' or '.br' for single-stream compression.")
                    : new(
                        Factory: null,
                        WriterDebuggerDisplay: static (info, writer) => $"File='{info.Source}'",
                        ReaderDebuggerDisplay: static info => $"File='{info.Source}'");
}
