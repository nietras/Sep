using System;
using System.IO;
using System.Threading.Tasks;

namespace nietras.SeparatedValues;

interface ISepTextReaderDisposer
{
    void Dispose(TextReader reader);
    ValueTask DisposeAsync(TextReader reader);
}

sealed class SepTextReaderDisposer : ISepTextReaderDisposer
{
    SepTextReaderDisposer() { }
    public static SepTextReaderDisposer Instance { get; } = new();
    public void Dispose(TextReader reader) => reader.Dispose();
    public ValueTask DisposeAsync(TextReader reader)
    {
        if (reader is IAsyncDisposable asyncDisposable)
        {
            return asyncDisposable.DisposeAsync();
        }
        reader.Dispose();
        return ValueTask.CompletedTask;
    }
}

sealed class NoopSepTextReaderDisposer : ISepTextReaderDisposer
{
    NoopSepTextReaderDisposer() { }
    public static NoopSepTextReaderDisposer Instance { get; } = new();
    public void Dispose(TextReader reader) { }
    public ValueTask DisposeAsync(TextReader reader) => ValueTask.CompletedTask;
}
