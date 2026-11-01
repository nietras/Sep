using System;
using System.Globalization;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace nietras.SeparatedValues.Benchmarks;

[MemoryDiagnoser]
[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByParams)]
public class SepFastFloatBench
{
    const int Count = 4096;
    static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    string[] _chars = [];
    byte[][] _bytes = [];
    string _rows = "";

    [Params("SepFloats", "Fixed", "Scientific", "Long", "Whitespace", "Grouped", "Integers", "Fraction7", "Fraction8")]
    public string Data { get; set; } = "";

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _chars = new string[Count];
        _bytes = new byte[Count][];
        for (var i = 0; i < Count; ++i)
        {
            var value = Data switch
            {
                "SepFloats" => random.NextSingle().ToString("R", Invariant),
                "Fixed" => (random.Next(-1_000_000, 1_000_001) / 100d).ToString("F2", Invariant),
                "Scientific" => ((random.NextDouble() - 0.5) * Math.Pow(10, random.Next(-30, 31))).ToString("E16", Invariant),
                "Long" => string.Create(Invariant, $"{random.Next(1, 10)}.{random.NextInt64(0, 1_000_000_000_000_000_000):D18}{random.NextInt64(0, 1_000_000_000_000_000_000):D18}E{random.Next(-20, 21)}"),
                "Whitespace" => "  " + random.NextSingle().ToString("R", Invariant),
                "Grouped" => (random.Next(100_000, 10_000_000) / 100d).ToString("N2", Invariant),
                "Integers" => random.NextInt64(1, long.MaxValue).ToString(Invariant),
                "Fraction7" => "0." + random.Next(10_000_000).ToString("D7", Invariant),
                "Fraction8" => "0." + random.Next(100_000_000).ToString("D8", Invariant),
                _ => throw new InvalidOperationException(Data),
            };
            _chars[i] = value;
            _bytes[i] = Encoding.UTF8.GetBytes(value);

            var expectedDouble = double.Parse(value, Invariant);
            var expectedSingle = float.Parse(value, Invariant);
            var oldDouble = ParseCsDouble(value);
            var oldSingle = ParseCsSingle(value);
            var oldDoubleUtf8 = ParseCsDouble(_bytes[i]);
            var oldSingleUtf8 = ParseCsSingle(_bytes[i]);
            var newDouble = ParseDouble(value.AsSpan());
            var newSingle = ParseSingle(value.AsSpan());
            var newDoubleUtf8 = ParseDouble(_bytes[i]);
            var newSingleUtf8 = ParseSingle(_bytes[i]);
            if (BitConverter.DoubleToInt64Bits(expectedDouble) != BitConverter.DoubleToInt64Bits(oldDouble) ||
                BitConverter.DoubleToInt64Bits(expectedDouble) != BitConverter.DoubleToInt64Bits(oldDoubleUtf8) ||
                BitConverter.DoubleToInt64Bits(expectedDouble) != BitConverter.DoubleToInt64Bits(newDouble) ||
                BitConverter.DoubleToInt64Bits(expectedDouble) != BitConverter.DoubleToInt64Bits(newDoubleUtf8) ||
                BitConverter.SingleToInt32Bits(expectedSingle) != BitConverter.SingleToInt32Bits(oldSingle) ||
                BitConverter.SingleToInt32Bits(expectedSingle) != BitConverter.SingleToInt32Bits(oldSingleUtf8) ||
                BitConverter.SingleToInt32Bits(expectedSingle) != BitConverter.SingleToInt32Bits(newSingle) ||
                BitConverter.SingleToInt32Bits(expectedSingle) != BitConverter.SingleToInt32Bits(newSingleUtf8))
            {
                throw new InvalidOperationException($"Parser mismatch: {value}");
            }
        }
        _rows = string.Join('\n', _chars) + "\n";
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Count), BenchmarkCategory("UTF16 double")]
    public double CsDoubleUtf16()
    {
        double sum = 0;
        foreach (var value in _chars) sum += ParseCsDouble(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = Count), BenchmarkCategory("UTF16 double")]
    public double SepDoubleUtf16()
    {
        double sum = 0;
        foreach (var value in _chars) sum += ParseDouble(value.AsSpan());
        return sum;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Count), BenchmarkCategory("UTF16 float")]
    public double CsSingleUtf16()
    {
        double sum = 0;
        foreach (var value in _chars) sum += ParseCsSingle(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = Count), BenchmarkCategory("UTF16 float")]
    public double SepSingleUtf16()
    {
        double sum = 0;
        foreach (var value in _chars) sum += ParseSingle(value.AsSpan());
        return sum;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Count), BenchmarkCategory("UTF8 double")]
    public double CsDoubleUtf8()
    {
        double sum = 0;
        foreach (var value in _bytes) sum += ParseCsDouble(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = Count), BenchmarkCategory("UTF8 double")]
    public double SepDoubleUtf8()
    {
        double sum = 0;
        foreach (var value in _bytes) sum += ParseDouble(value);
        return sum;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Count), BenchmarkCategory("UTF8 float")]
    public double CsSingleUtf8()
    {
        double sum = 0;
        foreach (var value in _bytes) sum += ParseCsSingle(value);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = Count), BenchmarkCategory("UTF8 float")]
    public double SepSingleUtf8()
    {
        double sum = 0;
        foreach (var value in _bytes) sum += ParseSingle(value);
        return sum;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Count), BenchmarkCategory("Reader double")]
    public double SimulatedPreviousReaderDouble()
    {
        double sum = 0;
        using var reader = Sep.Default.Reader(o => o with { HasHeader = false }).FromText(_rows);
        foreach (var row in reader) sum += ParseOldDouble(row[0].Span);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = Count), BenchmarkCategory("Reader double")]
    public double SepReaderDouble()
    {
        double sum = 0;
        using var reader = Sep.Default.Reader(o => o with { HasHeader = false }).FromText(_rows);
        foreach (var row in reader) sum += row[0].Parse<double>();
        return sum;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = Count), BenchmarkCategory("Reader float")]
    public double SimulatedPreviousReaderSingle()
    {
        double sum = 0;
        using var reader = Sep.Default.Reader(o => o with { HasHeader = false }).FromText(_rows);
        foreach (var row in reader) sum += ParseOldSingle(row[0].Span);
        return sum;
    }

    [Benchmark(OperationsPerInvoke = Count), BenchmarkCategory("Reader float")]
    public double SepReaderSingle()
    {
        double sum = 0;
        using var reader = Sep.Default.Reader(o => o with { HasHeader = false }).FromText(_rows);
        foreach (var row in reader) sum += row[0].Parse<float>();
        return sum;
    }

    static double ParseOldDouble(ReadOnlySpan<char> value)
    {
        if (!value.Contains(','))
        {
            var parsed = csFastFloat.FastDoubleParser.ParseDouble(value, out var consumed);
            if (consumed == value.Length) return parsed;
        }
        return double.Parse(value, Invariant);
    }

    static float ParseOldSingle(ReadOnlySpan<char> value)
    {
        if (!value.Contains(','))
        {
            var parsed = csFastFloat.FastFloatParser.ParseFloat(value, out var consumed);
            if (consumed == value.Length) return parsed;
        }
        return float.Parse(value, Invariant);
    }

    static double ParseCsDouble(ReadOnlySpan<char> value) =>
        csFastFloat.FastDoubleParser.TryParseDouble(value, out var consumed, out var parsed) && consumed == value.Length
            ? parsed : double.Parse(value, Invariant);

    static float ParseCsSingle(ReadOnlySpan<char> value) =>
        csFastFloat.FastFloatParser.TryParseFloat(value, out var consumed, out var parsed) && consumed == value.Length
            ? parsed : float.Parse(value, Invariant);

    static double ParseCsDouble(ReadOnlySpan<byte> value) =>
        csFastFloat.FastDoubleParser.TryParseDouble(value, out var consumed, out var parsed) && consumed == value.Length
            ? parsed : double.Parse(value, Invariant);

    static float ParseCsSingle(ReadOnlySpan<byte> value) =>
        csFastFloat.FastFloatParser.TryParseFloat(value, out var consumed, out var parsed) && consumed == value.Length
            ? parsed : float.Parse(value, Invariant);

    static double ParseDouble(ReadOnlySpan<char> value) =>
        FastFloat.TryParseDouble(value, out var parsed, allowSpecial: false, allowLeadingWhitespace: true) ? parsed : double.Parse(value, Invariant);

    static float ParseSingle(ReadOnlySpan<char> value) =>
        FastFloat.TryParseSingle(value, out var parsed, allowSpecial: false, allowLeadingWhitespace: true) ? parsed : float.Parse(value, Invariant);

    static double ParseDouble(ReadOnlySpan<byte> value) =>
        FastFloat.TryParseDouble(value, out var parsed, allowSpecial: false, allowLeadingWhitespace: true) ? parsed : double.Parse(value, Invariant);

    static float ParseSingle(ReadOnlySpan<byte> value) =>
        FastFloat.TryParseSingle(value, out var parsed, allowSpecial: false, allowLeadingWhitespace: true) ? parsed : float.Parse(value, Invariant);
}
