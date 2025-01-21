using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace nietras.SeparatedValues;

public static partial class SepWriterExtensions
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

    public static SepWriterOptions Writer(this Sep sep, Func<SepWriterOptions, SepWriterOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Writer(sep));
    }
    public static SepWriterOptions Writer(this SepSpec spec, Func<SepWriterOptions, SepWriterOptions> configure)
    {
        Contract.Assume(configure != null);
        return configure(Writer(spec));
    }

    public static SepWriter ToText(this SepWriterOptions options)
    {
        var writer = new StringWriter();
        return To(options, writer);
    }

    public static SepWriter ToText(this SepWriterOptions options, int capacity)
    {
        var writer = new StringWriter(new StringBuilder(capacity));
        return To(options, writer);
    }

    public static SepWriter ToFile(this SepWriterOptions options, string filePath)
    {
        var writer = new StreamWriter(filePath, s_streamWriterOptions);
        return To(options, writer);
    }

    public static SepWriter To(this SepWriterOptions options, Stream stream) =>
        To(options, stream, leaveOpen: false);

    public static SepWriter To(this SepWriterOptions options, Stream stream, bool leaveOpen)
    {
        var writer = new StreamWriter(stream, leaveOpen: leaveOpen);
        return To(options, writer);
    }

    public static SepWriter To(this SepWriterOptions options, TextWriter writer) =>
        To(options, writer, leaveOpen: false);

    public static SepWriter To(this SepWriterOptions options, TextWriter writer, bool leaveOpen)
    {
        ISepTextWriterDisposer textWriterDisposer = leaveOpen
            ? NoopSepTextWriterDisposer.Instance
            : SepTextWriterDisposer.Instance;
        ArgumentNullException.ThrowIfNull(writer);
        return new SepWriter(options, writer, textWriterDisposer);
    }
}
