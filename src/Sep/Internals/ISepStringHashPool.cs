using System;

namespace nietras.SeparatedValues;

interface ISepStringHashPool : IDisposable
{
    int Count { get; }
    string ToString(ReadOnlySpan<char> chars);
    string ToStringThreadSafe(ReadOnlySpan<char> chars);
}
