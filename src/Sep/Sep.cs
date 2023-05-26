using System;
using System.Diagnostics.Contracts;

namespace nietras.SeparatedValues;

public readonly record struct Sep
{
    const char _min = (char)32;
    const char _max = (char)126;

    readonly char _separator;

    public Sep() : this(SepDefaults.Separator) { }

    public Sep(char separator)
    {
        Validate(separator);
        _separator = separator;
    }

    public char Separator
    {
        get => _separator;
        init { Validate(value); _separator = value; }
    }

    public static Sep Default { get; } = new(SepDefaults.Separator);
    public static Sep? Auto => null;

    internal static Sep Min { get; } = new(_min);
    internal static Sep Max { get; } = new(_max);

    public static Sep New(char separator) => new(separator);

    public static SepReaderOptions Reader() => new(null);
    public static SepReaderOptions Reader(Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Reader());
    }

    public static SepWriterOptions Writer() => new(Default);
    public static SepWriterOptions Writer(Func<SepWriterOptions, SepWriterOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Writer());
    }

    internal static void Validate(char separator)
    {
        if (separator != '\t' && (separator < _min || separator > _max))
        {
            SepThrow.ArgumentOutOfRangeException_Separator(separator);
        }
        if (separator == SepDefaults.Comment ||
            separator == SepDefaults.Quote)
        {
            SepThrow.ArgumentException_Separator(separator);
        }
    }

    internal string[] Split(string line) => line.Split(Separator, StringSplitOptions.None);
}
