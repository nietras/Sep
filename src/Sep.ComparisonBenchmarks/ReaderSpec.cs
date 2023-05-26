#define USE_STRING_POOLING
//#define PARSE_ASSETS
using System;
using System.IO;

namespace nietras.SeparatedValues.ComparisonBenchmarks;

public record ReaderSpec(string Name, int Bytes, Func<TextReader> CreateReader)
{
    public static ReaderSpec FromString(string name, string text)
    {
        if (text is null) { throw new ArgumentNullException(nameof(text)); }
        return new(name, text.Length * sizeof(char), () => new StringReader(text));
    }

    public static ReaderSpec FromBytes(string name, byte[] bytes)
    {
        if (bytes is null) { throw new ArgumentNullException(nameof(bytes)); }
        return new(name, bytes.Length, () => new StreamReader(new MemoryStream(bytes)));
    }

    public override string ToString() => Name;
}
