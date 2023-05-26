using System;
using System.IO;

namespace nietras.SeparatedValues;

public static class SepReaderExtensions
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

    public static SepReader FromText(this SepReaderOptions options, string text)
    {
        var reader = new StringReader(text);
        return From(options, reader);
    }

    public static SepReader FromFile(this SepReaderOptions options, string filePath)
    {
        var reader = new StreamReader(filePath, s_streamReaderOptions);
        return From(options, reader);
    }

    public static SepReader From(this SepReaderOptions options, Stream stream)
    {
        var reader = new StreamReader(stream);
        return From(options, reader);
    }

    public static SepReader From(this SepReaderOptions options, TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(reader);
        try
        {
            return new SepReader(options, reader);
        }
        catch (Exception)
        {
            reader.Dispose();
            throw;
        }
    }

    internal static Sep? DetectSep(ReadOnlySpan<char> chars, bool nothingLeftToRead)
    {
        var separators = SepDefaults.AutoDetectSeparators;
        Span<int> counts = stackalloc int[separators.Count];

        var quoting = false;
        var lineEnd = false;
        foreach (var c in chars)
        {
            if (c == SepDefaults.Quote)
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
