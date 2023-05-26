using System;
using System.IO;
using System.Text;

namespace nietras.SeparatedValues;

public static class SepWriterExtensions
{
    static readonly FileStreamOptions s_streamWriterOptions = new()
    {
        Access = FileAccess.Write,
        Mode = FileMode.Create,
        Options = FileOptions.SequentialScan,
    };

    public static SepWriterOptions Writer(this Sep sep) => new(sep);
    public static SepWriterOptions Writer(this SepSpec spec) =>
        new SepWriterOptions(spec.Sep) with { CultureInfo = spec.CultureInfo };

    public static SepWriter ToText(this SepWriterOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var writer = new StringWriter();
        return To(options, writer);
    }

    public static SepWriter ToText(this SepWriterOptions options, int capacity)
    {
        ArgumentNullException.ThrowIfNull(options);
        var writer = new StringWriter(new StringBuilder(capacity));
        return To(options, writer);
    }

    public static SepWriter ToFile(this SepWriterOptions options, string filePath)
    {
        ArgumentNullException.ThrowIfNull(options);
        var writer = new StreamWriter(filePath, s_streamWriterOptions);
        return To(options, writer);
    }

    public static SepWriter To(this SepWriterOptions options, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(options);
        var writer = new StreamWriter(stream);
        return To(options, writer);
    }

    public static SepWriter To(this SepWriterOptions options, TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(writer);
        return new SepWriter(options, writer);
    }
}
