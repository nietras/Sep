//#define SYNC
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
    public static SepReader FromText(this in SepReaderOptions options, string text)
#else
    public static async ValueTask<SepReader> FromTextAsync(this SepReaderOptions options, string text, CancellationToken cancellationToken = default)
#endif
    {
        ArgumentNullException.ThrowIfNull(text);
        var reader = new StringReader(text);
        Func<SepReader.Info, string> display = static info => $"String Length={((string)info.Source).Length}";
#if SYNC
        return FromWithInfo(new(text, display), options, reader);
#else
        return await FromWithInfoAsync(new(text, display), options, reader, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader FromFile(this in SepReaderOptions options, string filePath)
#else
    public static async ValueTask<SepReader> FromFileAsync(this SepReaderOptions options, string filePath, CancellationToken cancellationToken = default)
#endif
    {
        var reader = new StreamReader(filePath, s_streamReaderOptions);
        Func<SepReader.Info, string> display = static info => $"File='{info.Source}'";
#if SYNC
        return FromWithInfo(new(filePath, display), options, reader);
#else
        return await FromWithInfoAsync(new(filePath, display), options, reader, cancellationToken);
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
        return FromWithInfo(new(buffer, display), options, reader);
#else
        return await FromWithInfoAsync(new(buffer, display), options, reader, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader From(this in SepReaderOptions options, string name, Func<string, Stream> nameToStream)
#else
    public static async ValueTask<SepReader> FromAsync(this SepReaderOptions options, string name, Func<string, Stream> nameToStream, CancellationToken cancellationToken = default)
#endif
    {
        ArgumentNullException.ThrowIfNull(nameToStream);
        var reader = new StreamReader(nameToStream(name));
        Func<SepReader.Info, string> display = static info => $"Stream Name='{info.Source}'";
#if SYNC
        return FromWithInfo(new(name, display), options, reader);
#else
        return await FromWithInfoAsync(new(name, display), options, reader, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader From(this in SepReaderOptions options, Stream stream)
#else
    public static async ValueTask<SepReader> FromAsync(this SepReaderOptions options, Stream stream, CancellationToken cancellationToken = default)
#endif
    {
        var reader = new StreamReader(stream);
        Func<SepReader.Info, string> display = static info => $"Stream='{info.Source}'";
#if SYNC
        return FromWithInfo(new(stream, display), options, reader);
#else
        return await FromWithInfoAsync(new(stream, display), options, reader, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader From(this in SepReaderOptions options, string name, Func<string, TextReader> nameToReader)
#else
    public static async ValueTask<SepReader> FromAsync(this SepReaderOptions options, string name, Func<string, TextReader> nameToReader, CancellationToken cancellationToken = default)
#endif
    {
        ArgumentNullException.ThrowIfNull(nameToReader);
        var reader = nameToReader(name);
        Func<SepReader.Info, string> display = static info => $"TextReader Name='{info.Source}'";
#if SYNC
        return FromWithInfo(new(name, display), options, reader);
#else
        return await FromWithInfoAsync(new(name, display), options, reader, cancellationToken);
#endif
    }

#if SYNC
    public static SepReader From(this in SepReaderOptions options, TextReader reader)
#else
    public static async ValueTask<SepReader> FromAsync(this SepReaderOptions options, TextReader reader, CancellationToken cancellationToken = default)
#endif
    {
        Func<SepReader.Info, string> display = static info => $"TextReader='{info.Source}'";
#if SYNC
        return FromWithInfo(new(reader, display), options, reader);
#else
        return await FromWithInfoAsync(new(reader, display), options, reader, cancellationToken);
#endif
    }

#if SYNC
    internal static SepReader FromWithInfo(SepReader.Info info, SepReaderOptions options, TextReader reader)
#else
    internal static async ValueTask<SepReader> FromWithInfoAsync(SepReader.Info info, SepReaderOptions options, TextReader reader, CancellationToken cancellationToken)
#endif
    {
        ArgumentNullException.ThrowIfNull(reader);
        SepReader? sepReader = null;
        try
        {
            sepReader = new SepReader(info, options, reader);
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
            reader.Dispose();
            throw;
        }
    }
}
