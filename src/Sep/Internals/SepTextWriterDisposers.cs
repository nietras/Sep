using System.IO;
using System.Threading.Tasks;

namespace nietras.SeparatedValues;

interface ISepTextWriterDisposer
{
    void Dispose(TextWriter writer);
    ValueTask DisposeAsync(TextWriter writer);
}

sealed class SepTextWriterDisposer : ISepTextWriterDisposer
{
    SepTextWriterDisposer() { }
    public static SepTextWriterDisposer Instance { get; } = new();
    public void Dispose(TextWriter writer) => writer.Dispose();
    public ValueTask DisposeAsync(TextWriter writer) => writer.DisposeAsync();
}

sealed class NoopSepTextWriterDisposer : ISepTextWriterDisposer
{
    NoopSepTextWriterDisposer() { }
    public static NoopSepTextWriterDisposer Instance { get; } = new();
    public void Dispose(TextWriter writer) { }
    public ValueTask DisposeAsync(TextWriter writer) => ValueTask.CompletedTask;
}
