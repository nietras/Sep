using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using static nietras.SeparatedValues.SepWriter;

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
        new SepWriterOptions(spec.Sep) with
        {
            CultureInfo = spec.CultureInfo,
            AsyncContinueOnCapturedContext = spec.AsyncContinueOnCapturedContext
        };

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

    public static SepWriter ToText(this in SepWriterOptions options) =>
        To(options, new StringBuilder());

    public static SepWriter ToText(this in SepWriterOptions options, int capacity) =>
        To(options, new StringBuilder(capacity));

    public static SepWriter ToFile(this in SepWriterOptions options, string filePath)
    {
        DebuggerDisplayFunc display = static (info, writer) => $"File='{info.Source}'";
        var writer = new StreamWriter(filePath, s_streamWriterOptions);
        return ToWithInfo(new(filePath, display), options, writer, leaveOpen: false);
    }

    public static SepWriter To(this in SepWriterOptions options, StringBuilder stringBuilder)
    {
        DebuggerDisplayFunc display = static (info, writer) =>
            $"{nameof(StringBuilder)} {nameof(StringBuilder.Length)}={((StringBuilder)info.Source).Length}";
        var writer = new StringWriter(stringBuilder);
        return ToWithInfo(new(stringBuilder, display), options, writer, leaveOpen: false);
    }

    public static SepWriter To(this in SepWriterOptions options, string name, Func<string, Stream> nameToStream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(nameToStream);
        // delegate not static to preserve name in debugger display (alloc acceptable)
        DebuggerDisplayFunc display = static (info, writer) =>
        {
            var stream = ((StreamWriter)writer).BaseStream;
            return $"{nameof(Stream)}='{stream}' Name='{info.Source}' Length={stream.Length}";
        };
        var stream = nameToStream(name);
        var writer = new StreamWriter(stream, leaveOpen: leaveOpen);
        // leaveOpen: false for StreamWriter is not the same as for Stream
        return ToWithInfo(new(name, display), options, writer, leaveOpen: false);
    }

    public static SepWriter To(this in SepWriterOptions options, Stream stream) =>
        To(options, stream, leaveOpen: false);

    public static SepWriter To(this in SepWriterOptions options, Stream stream, bool leaveOpen)
    {
        DebuggerDisplayFunc display = static (info, writer) => $"{nameof(Stream)}='{info.Source}' Length={((Stream)info.Source).Length}";
        var writer = new StreamWriter(stream, leaveOpen: leaveOpen);
        // leaveOpen: false for StreamWriter is not the same as for Stream
        return ToWithInfo(new(stream, display), options, writer, leaveOpen: false);
    }

    public static SepWriter To(this in SepWriterOptions options, string name, Func<string, TextWriter> nameToWriter, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(nameToWriter);
        // delegate not static to preserve name in debugger display (alloc acceptable)
        DebuggerDisplayFunc display = static (info, writer) => $"{nameof(TextWriter)}='{writer.GetType()}' Name='{info.Source}'";
        var writer = nameToWriter(name);
        return ToWithInfo(new(name, display), options, writer, leaveOpen: false);
    }

    public static SepWriter To(this in SepWriterOptions options, TextWriter writer) =>
        To(options, writer, leaveOpen: false);

    public static SepWriter To(this in SepWriterOptions options, TextWriter writer, bool leaveOpen)
    {
        // Show type only to avoid calling ToString() on StringWriter (all contents)
        DebuggerDisplayFunc display = static (info, writer) => $"{nameof(TextWriter)}='{info.Source.GetType()}'";
        return ToWithInfo(new(writer, display), options, writer, leaveOpen);
    }

    static SepWriter ToWithInfo(SepWriter.Info info, in SepWriterOptions options,
        TextWriter writer, bool leaveOpen)
    {
        ISepTextWriterDisposer textWriterDisposer = leaveOpen
            ? NoopSepTextWriterDisposer.Instance
            : SepTextWriterDisposer.Instance;
        ArgumentNullException.ThrowIfNull(writer);
        return new SepWriter(info, options, writer, textWriterDisposer);
    }
}
