using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace nietras.SeparatedValues;

public static class SepUtf8ReaderExtensions
{
    public static SepUtf8ReaderOptions Utf8Reader(this Sep sep) => new(sep);
    public static SepUtf8ReaderOptions Utf8Reader(this Sep? sep) => new(sep);
    public static SepUtf8ReaderOptions Utf8Reader(this SepSpec spec) =>
        new SepUtf8ReaderOptions(spec.Sep) with { CultureInfo = spec.CultureInfo };

    public static SepUtf8ReaderOptions Utf8Reader(this Sep sep, Func<SepUtf8ReaderOptions, SepUtf8ReaderOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Utf8Reader(sep));
    }
    public static SepUtf8ReaderOptions Utf8Reader(this Sep? sep, Func<SepUtf8ReaderOptions, SepUtf8ReaderOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Utf8Reader(sep));
    }
    public static SepUtf8ReaderOptions Utf8Reader(this SepSpec spec, Func<SepUtf8ReaderOptions, SepUtf8ReaderOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Utf8Reader(spec.Sep));
    }

    static readonly FileStreamOptions s_fileStreamOptions = new()
    {
        Access = FileAccess.Read,
        Mode = FileMode.Open,
        Options = FileOptions.SequentialScan,
    };

    public static SepUtf8Reader FromText(this in SepUtf8ReaderOptions options, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        var stream = new MemoryStream(bytes, writable: false);
        Func<SepUtf8Reader.Info, string> display = static info => $"String Length={((string)info.Source).Length}";
        return FromWithInfo(new(text, display), options, stream);
    }

    public static SepUtf8Reader FromFile(this in SepUtf8ReaderOptions options, string filePath)
    {
        var stream = new FileStream(filePath, s_fileStreamOptions);
        Func<SepUtf8Reader.Info, string> display = static info => $"File='{info.Source}'";
        return FromWithInfo(new(filePath, display), options, stream);
    }

    public static SepUtf8Reader FromBytes(this in SepUtf8ReaderOptions options, byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var stream = new MemoryStream(buffer, writable: false);
        Func<SepUtf8Reader.Info, string> display = static info => $"Bytes Length={((byte[])info.Source).Length}";
        return FromWithInfo(new(buffer, display), options, stream);
    }

    public static SepUtf8Reader From(this in SepUtf8ReaderOptions options, Stream stream)
    {
        Func<SepUtf8Reader.Info, string> display = static info => $"Stream='{info.Source}'";
        return FromWithInfo(new(stream, display), options, stream);
    }

    internal static SepUtf8Reader FromWithInfo(SepUtf8Reader.Info info, in SepUtf8ReaderOptions options, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        SepUtf8Reader? sepReader = null;
        try
        {
            sepReader = new SepUtf8Reader(info, options, stream);
            sepReader.Initialize(options);
            return sepReader;
        }
        catch (Exception)
        {
            sepReader?.Dispose();
            stream.Dispose();
            throw;
        }
    }
}
