using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace nietras.SeparatedValues;

public static partial class SepReaderExtensions
{
    static readonly FileStreamOptions s_streamReaderOptions = new()
    {
        Access = FileAccess.Read,
        Mode = FileMode.Open,
        Options = FileOptions.SequentialScan,
        // Consider whether to define larger BufferSize
    };

    public static SepReaderOptions Reader(this Sep sep) => new(sep);
    public static SepReaderOptions Reader(this Sep? sep) => new(sep);
    public static SepReaderOptions Reader(this SepSpec spec) =>
        new SepReaderOptions(spec.Sep) with { CultureInfo = spec.CultureInfo };

    public static SepReaderOptions Reader(this Sep sep, Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Reader(sep));
    }
    public static SepReaderOptions Reader(this Sep? sep, Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Reader(sep));
    }
    public static SepReaderOptions Reader(this SepSpec spec, Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Reader(spec.Sep));
    }

    public static SepReader FromText(this in SepReaderOptions options, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var reader = new StringReader(text);
        Func<SepReader.Info, string> display = static info => $"String Length={((string)info.Source).Length}";
        return FromWithInfo(new(text, display), options, reader);
    }

    public static SepReader FromFile(this in SepReaderOptions options, string filePath)
    {
        var reader = new StreamReader(filePath, s_streamReaderOptions);
        Func<SepReader.Info, string> display = static info => $"File='{info.Source}'";
        return FromWithInfo(new(filePath, display), options, reader);
    }

    public static SepReader From(this in SepReaderOptions options, byte[] buffer)
    {
        var reader = new StreamReader(new MemoryStream(buffer));
        Func<SepReader.Info, string> display = static info => $"Bytes Length={((byte[])info.Source).Length}";
        return FromWithInfo(new(buffer, display), options, reader);
    }

    public static SepReader From(this in SepReaderOptions options, string name, Func<string, Stream> nameToStream)
    {
        ArgumentNullException.ThrowIfNull(nameToStream);
        var reader = new StreamReader(nameToStream(name));
        Func<SepReader.Info, string> display = static info => $"Stream Name='{info.Source}'";
        return FromWithInfo(new(name, display), options, reader);
    }
    public static SepReader From(this in SepReaderOptions options, Stream stream)
    {
        var reader = new StreamReader(stream);
        Func<SepReader.Info, string> display = static info => $"Stream='{info.Source}'";
        return FromWithInfo(new(stream, display), options, reader);
    }

    public static SepReader From(this in SepReaderOptions options, string name, Func<string, TextReader> nameToReader)
    {
        ArgumentNullException.ThrowIfNull(nameToReader);
        var reader = nameToReader(name);
        Func<SepReader.Info, string> display = static info => $"TextReader Name='{info.Source}'";
        return FromWithInfo(new(name, display), options, reader);
    }
    public static SepReader From(this in SepReaderOptions options, TextReader reader)
    {
        Func<SepReader.Info, string> display = static info => $"TextReader='{info.Source}'";
        return FromWithInfo(new(reader, display), options, reader);
    }

    internal static SepReader FromWithInfo(SepReader.Info info, SepReaderOptions options, TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        SepReader? sepReader = null;
        try
        {
            sepReader = new SepReader(info, options, reader);
            sepReader.Initialize(options);
            return sepReader;
        }
        catch (Exception)
        {
            sepReader?.Dispose();
            reader.Dispose();
            throw;
        }
    }

    public static async Task<SepReader> FromTextAsync(this SepReaderOptions options, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var reader = new StringReader(text);
        Func<SepReader.Info, string> display = static info => $"String Length={((string)info.Source).Length}";
        return await FromWithInfoAsync(new(text, display), options, reader);
    }

    public static async Task<SepReader> FromFileAsync(this SepReaderOptions options, string filePath)
    {
        var reader = new StreamReader(filePath, s_streamReaderOptions);
        Func<SepReader.Info, string> display = static info => $"File='{info.Source}'";
        return await FromWithInfoAsync(new(filePath, display), options, reader);
    }

    public static async Task<SepReader> FromAsync(this SepReaderOptions options, byte[] buffer)
    {
        var reader = new StreamReader(new MemoryStream(buffer));
        Func<SepReader.Info, string> display = static info => $"Bytes Length={((byte[])info.Source).Length}";
        return await FromWithInfoAsync(new(buffer, display), options, reader);
    }

    public static async Task<SepReader> FromAsync(this SepReaderOptions options, string name, Func<string, Stream> nameToStream)
    {
        ArgumentNullException.ThrowIfNull(nameToStream);
        var reader = new StreamReader(nameToStream(name));
        Func<SepReader.Info, string> display = static info => $"Stream Name='{info.Source}'";
        return await FromWithInfoAsync(new(name, display), options, reader);
    }

    public static async Task<SepReader> FromAsync(this SepReaderOptions options, Stream stream)
    {
        var reader = new StreamReader(stream);
        Func<SepReader.Info, string> display = static info => $"Stream='{info.Source}'";
        return await FromWithInfoAsync(new(stream, display), options, reader);
    }

    public static async Task<SepReader> FromAsync(this SepReaderOptions options, string name, Func<string, TextReader> nameToReader)
    {
        ArgumentNullException.ThrowIfNull(nameToReader);
        var reader = nameToReader(name);
        Func<SepReader.Info, string> display = static info => $"TextReader Name='{info.Source}'";
        return await FromWithInfoAsync(new(name, display), options, reader);
    }

    public static async Task<SepReader> FromAsync(this SepReaderOptions options, TextReader reader)
    {
        Func<SepReader.Info, string> display = static info => $"TextReader='{info.Source}'";
        return await FromWithInfoAsync(new(reader, display), options, reader);
    }

    internal static async Task<SepReader> FromWithInfoAsync(SepReader.Info info, SepReaderOptions options, TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        SepReader? sepReader = null;
        try
        {
            sepReader = new SepReader(info, options, reader);
            await sepReader.InitializeAsync(options);
            return sepReader;
        }
        catch (Exception)
        {
            await sepReader?.DisposeAsync();
            await reader.DisposeAsync();
            throw;
        }
    }

    internal static Sep? DetectSep(ReadOnlySpan<char> chars, bool nothingLeftToRead, bool disableQuotesParsing)
    {
        var separators = SepDefaults.AutoDetectSeparators;
        Span<int> counts = stackalloc int[separators.Count];

        var quoting = false;
        var lineEnd = false;
        foreach (var c in chars)
        {
            if (!disableQuotesParsing && c == SepDefaults.Quote)
            {
                quoting = !quoting;
            }
            else if (!quoting)
            {
                if (c == SepDefaults.CarriageReturn || c == SepDefaults.LineFeed)
                {
                    lineEnd = true;
                    break;
                }
                for (var s = 0; s < separators.Count; s++)
                {
                    var separator = separators[s];
                    if (c == separator)
                    {
                        ++counts[s];
                    }
                }
            }
        }

        if (!lineEnd && !nothingLeftToRead)
        {
            return null;
        }

        var maxCount = 0;
        var maxIndex = -1;
        for (var s = 0; s < separators.Count; s++)
        {
            var count = counts[s];
            if (count > maxCount)
            {
                maxCount = count;
                maxIndex = s;
            }
        }
        return maxIndex >= 0 ? new(separators[maxIndex]) : Sep.Default;
    }
}
