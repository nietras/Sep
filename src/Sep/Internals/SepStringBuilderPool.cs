using System.Collections.Concurrent;
using System.Text;

namespace nietras.SeparatedValues;

static class SepStringBuilderPool
{
    const int DefaultCapacity = 256;
    const int MaxPoolSize = 64;
    // Downside is these are kept around forever after usage
    static readonly ConcurrentQueue<StringBuilder> _free = new();

    internal static StringBuilder Take() => _free.TryDequeue(out var sb) ? sb : new StringBuilder(DefaultCapacity);

    internal static void Return(StringBuilder sb)
    {
        // Count could be off a little bit (but insignificantly) due to race
        // condition here since not checking count/enqueueing atomically
        if (_free.Count <= MaxPoolSize)
        {
            sb.Clear();
            _free.Enqueue(sb);
        }
    }
}
