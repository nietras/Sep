using System;
using System.Diagnostics.Contracts;

namespace nietras.SeparatedValues;

public static partial class SepReaderExtensions
{

    public static SepReaderOptions Reader(this Sep sep) => new(sep);
    public static SepReaderOptions Reader(this Sep? sep) => new(sep);
    public static SepReaderOptions Reader(this SepSpec spec) =>
        new SepReaderOptions(spec.Sep) with
        {
            CultureInfo = spec.CultureInfo,
            AsyncContinueOnCapturedContext = spec.AsyncContinueOnCapturedContext
        };

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
