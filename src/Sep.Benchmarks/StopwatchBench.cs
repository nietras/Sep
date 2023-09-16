using System;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace nietras.SeparatedValues.Benchmarks;

public class StopwatchBench
{
#pragma warning disable CA1822 // Mark members as static
    [Benchmark] // 22.5 ns on 5950X
    public DateTime DateTimeUtcNow() => DateTime.UtcNow;

    [Benchmark] // 23.2 ns on 5950X
    public DateTimeOffset DateTimeOffsetUtcNow() => DateTimeOffset.UtcNow;

    [Benchmark] // 15.5 ns on 5950X
    public long GetTimestamp() => Stopwatch.GetTimestamp();

    [Benchmark] //  1.1 ns on 5950X
    public long Frequency() => Stopwatch.Frequency;
#pragma warning restore CA1822 // Mark members as static
}
