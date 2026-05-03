using System.IO;

namespace nietras.SeparatedValues;

interface ISepTextReaderDisposer
{
    void Dispose(TextReader reader);
}

sealed class SepTextReaderDisposer : ISepTextReaderDisposer
{
    SepTextReaderDisposer() { }
    public static SepTextReaderDisposer Instance { get; } = new();
    public void Dispose(TextReader reader) => reader.Dispose();
}

sealed class NoopSepTextReaderDisposer : ISepTextReaderDisposer
{
    NoopSepTextReaderDisposer() { }
    public static NoopSepTextReaderDisposer Instance { get; } = new();
    public void Dispose(TextReader reader) { }
}
