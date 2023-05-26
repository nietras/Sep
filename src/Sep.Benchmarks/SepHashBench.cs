using System;
using BenchmarkDotNet.Attributes;

namespace nietras.SeparatedValues.Benchmarks;

public class SepHashBench
{
    readonly Random _random = new(31);
    string _string = "";

    [Params(2, 4, 16, 128)]
    public int Length { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _string = RandomString(_random, Length);
    }

    [Benchmark(Baseline = true)]
    public uint Naive() => SepHash.SumMultiply31(_string);

    [Benchmark]
    public uint Unroll() => SepHash.UnrollSumMultiply31(_string);

    [Benchmark]
    public uint Pool() => SepHash.SumMultiplyPrimesNUInt(_string);

    [Benchmark]
    public uint FNV1a() => SepHash.FNV1aNUInt(_string);

    static string RandomString(Random random, int length)
    {
        Span<char> span = stackalloc char[length];
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = (char)random.Next(char.MinValue, char.MaxValue);
        }
        return new(span);
    }
}
