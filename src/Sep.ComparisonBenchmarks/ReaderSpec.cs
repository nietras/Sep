using System;
using System.IO;

namespace nietras.SeparatedValues.ComparisonBenchmarks;

public record ReaderSpec(string Name, Lazy<int> Bytes, Func<TextReader> CreateReader)
{
    public static ReaderSpec FromString(string name, Lazy<string> text)
    {
        if (text is null) { throw new ArgumentNullException(nameof(text)); }
        return new(name, new(() => text.Value.Length * sizeof(char)), () => new StringReader(text.Value));
    }

    public static ReaderSpec FromBytes(string name, Lazy<byte[]> bytes)
    {
        if (bytes is null) { throw new ArgumentNullException(nameof(bytes)); }
        return new(name, new(() => bytes.Value.Length), () => new StreamReader(new MemoryStream(bytes.Value)));
    }

    public override string ToString() => Name;
}
