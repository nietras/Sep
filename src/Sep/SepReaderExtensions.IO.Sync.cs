#define SYNC
#pragma warning disable CA2007
using System;
using System.IO;
#if !SYNC
using System.Threading;
using System.Threading.Tasks;
#endif

namespace nietras.SeparatedValues;

public static partial class SepReaderExtensions
{
#if SYNC
    static readonly FileStreamOptions s_streamReaderOptionsSync = new()
    {
        Access = FileAccess.Read,
        Mode = FileMode.Open,
        Options = FileOptions.SequentialScan,
        // Consider whether to define larger BufferSize
    };
#else
    static readonly FileStreamOptions s_streamReaderOptionsAsync = new()
    {
        Access = FileAccess.Read,
        Mode = FileMode.Open,
        Options = FileOptions.SequentialScan | FileOptions.Asynchronous,
        // Consider whether to define larger BufferSize
    };
#endif

#if SYNC
    public static SepReader FromText(this in SepReaderOptions options, string text)
#else
    public static async ValueTask<SepReader> FromTextAsync(this SepReaderOptions options, string text, CancellationToken cancellationToken = default)
#endif
    {
        ArgumentNullException.ThrowIfNull(text);
        var reader = new StringReader(text);
        Func<SepReader.Info, string> display = static info => $"String Length={((string)info.Source).Length}";
#if SYNC
        return FromWithInfo(new(text, display), options, reader, leaveOpen: false);
#else
        return await FromWithInfoAsync(new(text, display), options, reader, leaveOpen: false, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader FromFile(this in SepReaderOptions options, string filePath)
#else
    public static async ValueTask<SepReader> FromFileAsync(this SepReaderOptions options, string filePath, CancellationToken cancellationToken = default)
#endif
    {
#if SYNC
        var reader = new StreamReader(filePath, s_streamReaderOptionsSync);
#else
        var reader = new StreamReader(filePath, s_streamReaderOptionsAsync);
#endif
        Func<SepReader.Info, string> display = static info => $"File='{info.Source}'";
#if SYNC
        return FromWithInfo(new(filePath, display), options, reader, leaveOpen: false);
#else
        return await FromWithInfoAsync(new(filePath, display), options, reader, leaveOpen: false, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader From(this in SepReaderOptions options, byte[] buffer)
#else
    public static async ValueTask<SepReader> FromAsync(this SepReaderOptions options, byte[] buffer, CancellationToken cancellationToken = default)
#endif
    {
        var reader = new StreamReader(new MemoryStream(buffer));
        Func<SepReader.Info, string> display = static info => $"Bytes Length={((byte[])info.Source).Length}";
#if SYNC
        return FromWithInfo(new(buffer, display), options, reader, leaveOpen: false);
#else
        return await FromWithInfoAsync(new(buffer, display), options, reader, leaveOpen: false, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader From(this in SepReaderOptions options, string name, Func<string, Stream> nameToStream) =>
        From(options, name, nameToStream, leaveOpen: false);

    public static SepReader From(this in SepReaderOptions options, string name, Func<string, Stream> nameToStream, bool leaveOpen)
#else
    public static ValueTask<SepReader> FromAsync(this SepReaderOptions options, string name, Func<string, Stream> nameToStream, CancellationToken cancellationToken = default) =>
        FromAsync(options, name, nameToStream, leaveOpen: false, cancellationToken);

    public static async ValueTask<SepReader> FromAsync(this SepReaderOptions options, string name, Func<string, Stream> nameToStream, bool leaveOpen, CancellationToken cancellationToken = default)
#endif
    {
        ArgumentNullException.ThrowIfNull(nameToStream);
        var reader = new StreamReader(nameToStream(name), leaveOpen: leaveOpen);
        Func<SepReader.Info, string> display = static info => $"Stream Name='{info.Source}'";
        // leaveOpen: false for StreamReader is not the same as for Stream
#if SYNC
        return FromWithInfo(new(name, display), options, reader, leaveOpen: false);
#else
        return await FromWithInfoAsync(new(name, display), options, reader, leaveOpen: false, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader From(this in SepReaderOptions options, Stream stream) =>
        From(options, stream, leaveOpen: false);

    public static SepReader From(this in SepReaderOptions options, Stream stream, bool leaveOpen)
#else
    public static ValueTask<SepReader> FromAsync(this SepReaderOptions options, Stream stream, CancellationToken cancellationToken = default) =>
        FromAsync(options, stream, leaveOpen: false, cancellationToken);

    public static async ValueTask<SepReader> FromAsync(this SepReaderOptions options, Stream stream, bool leaveOpen, CancellationToken cancellationToken = default)
#endif
    {
        var reader = new StreamReader(stream, leaveOpen: leaveOpen);
        Func<SepReader.Info, string> display = static info => $"Stream='{info.Source}'";
        // leaveOpen: false for StreamReader is not the same as for Stream
#if SYNC
        return FromWithInfo(new(stream, display), options, reader, leaveOpen: false);
#else
        return await FromWithInfoAsync(new(stream, display), options, reader, leaveOpen: false, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader From(this in SepReaderOptions options, string name, Func<string, TextReader> nameToReader) =>
        From(options, name, nameToReader, leaveOpen: false);

    public static SepReader From(this in SepReaderOptions options, string name, Func<string, TextReader> nameToReader, bool leaveOpen)
#else
    public static ValueTask<SepReader> FromAsync(this SepReaderOptions options, string name, Func<string, TextReader> nameToReader, CancellationToken cancellationToken = default) =>
        FromAsync(options, name, nameToReader, leaveOpen: false, cancellationToken);

    public static async ValueTask<SepReader> FromAsync(this SepReaderOptions options, string name, Func<string, TextReader> nameToReader, bool leaveOpen, CancellationToken cancellationToken = default)
#endif
    {
        ArgumentNullException.ThrowIfNull(nameToReader);
        var reader = nameToReader(name);
        Func<SepReader.Info, string> display = static info => $"TextReader Name='{info.Source}'";
#if SYNC
        return FromWithInfo(new(name, display), options, reader, leaveOpen);
#else
        return await FromWithInfoAsync(new(name, display), options, reader, leaveOpen, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader From(this in SepReaderOptions options, TextReader reader) =>
        From(options, reader, leaveOpen: false);

    public static SepReader From(this in SepReaderOptions options, TextReader reader, bool leaveOpen)
#else
    public static ValueTask<SepReader> FromAsync(this SepReaderOptions options, TextReader reader, CancellationToken cancellationToken = default) =>
        FromAsync(options, reader, leaveOpen: false, cancellationToken);

    public static async ValueTask<SepReader> FromAsync(this SepReaderOptions options, TextReader reader, bool leaveOpen, CancellationToken cancellationToken = default)
#endif
    {
        Func<SepReader.Info, string> display = static info => $"TextReader='{info.Source}'";
#if SYNC
        return FromWithInfo(new(reader, display), options, reader, leaveOpen);
#else
        return await FromWithInfoAsync(new(reader, display), options, reader, leaveOpen, cancellationToken);
#endif
    }

#if SYNC
    internal static SepReader FromWithInfo(SepReader.Info info, in SepReaderOptions options, TextReader reader, bool leaveOpen)
#else
    internal static async ValueTask<SepReader> FromWithInfoAsync(SepReader.Info info, SepReaderOptions options, TextReader reader, bool leaveOpen, CancellationToken cancellationToken)
#endif
    {
        ArgumentNullException.ThrowIfNull(reader);
        ISepTextReaderDisposer textReaderDisposer = leaveOpen
            ? NoopSepTextReaderDisposer.Instance
            : SepTextReaderDisposer.Instance;
        SepReader? sepReader = null;
        try
        {
            sepReader = new SepReader(info, options, reader, textReaderDisposer);
#if SYNC
            sepReader.Initialize(options);
            return sepReader;
#else
            await sepReader.InitializeAsync(options, cancellationToken);
            return sepReader;
#endif
        }
        catch (Exception)
        {
            sepReader?.Dispose();
            textReaderDisposer.Dispose(reader);
            throw;
        }
    }
}
