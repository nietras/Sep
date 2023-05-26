using BenchmarkDotNet.Attributes;

namespace nietras.SeparatedValues.Benchmarks;

[InvocationCount(2_000_000)]
[MemoryDiagnoser]
public class SepWriterBench
{
    SepWriter? _writer;

    [IterationSetup]
    public void Setup()
    {
        _writer = Sep.Writer().ToText(256 * 1024 * 1024);
    }

    [IterationCleanup]
    public void Cleanup()
    {
        _writer?.Dispose();
        _writer = null;
    }

    [Benchmark(Baseline = true)]
    public void NewRow()
    {
        using var writeRow = _writer!.NewRow();
    }

    [Benchmark]
    public void SetByColName()
    {
        using var writeRow = _writer!.NewRow();
        writeRow["A"].Set("aaaaaaaaaaaaaaaaaaa");
        writeRow["B"].Set("bbbbbbbbbbbbbbbbbbbbbb");
        writeRow["C"].Set("cccccccccccccccccccccccc");
        writeRow["D"].Set("ddddddddddddddddddddddddddddddd");
    }
}
