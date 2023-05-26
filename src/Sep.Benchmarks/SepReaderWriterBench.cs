using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace nietras.SeparatedValues.Benchmarks;

[InvocationCount(200_000)]
[MemoryDiagnoser]
public class SepReaderWriterBench
{
    readonly string _scsv = GenerateSCSV(10_000_000, seed: 42);
    SepReader? _reader;
    SepWriter? _writer;
    SepReader? _enumerator;

    [IterationSetup]
    public void Setup()
    {
        _reader = Sep.Reader().FromText(_scsv);
        _enumerator = _reader.GetEnumerator();
        _writer = _reader.Spec.Writer().ToText(128 * 1024 * 1024);
    }

    [IterationCleanup]
    public void Cleanup()
    {
        _reader?.Dispose();
        _reader = null;
        _writer?.Dispose();
        _writer = null;
    }

    [Benchmark]
    public void ParseAndFormat_ByColName()
    {
        if (_enumerator!.MoveNext())
        {
            var readRow = _enumerator.Current;
            var i8 = readRow[C.SByte].Parse<sbyte>();
            var i16 = readRow[C.Short].Parse<short>();
            var i32 = readRow[C.Int].Parse<int>();
            var i64 = readRow[C.Long].Parse<long>();
            var f32 = readRow[C.Float].Parse<float>();
            var f64 = readRow[C.Double].Parse<double>();

            using var writeRow = _writer!.NewRow();
            writeRow[C.SByte].Format(i8);
            writeRow[C.Short].Format(i16);
            writeRow[C.Int].Format(i32);
            writeRow[C.Long].Format(i64);
            writeRow[C.Float].Format(f32);
            writeRow[C.Double].Format(f64);
        }
    }

    public static string GenerateSCSV(int lines, int seed)
    {
        var random = new Random(seed);
        var sb = new StringBuilder(lines * 32);
        sb.Append($"{C.Name};{C.SByte};{C.Short};{C.Int};{C.Long};{C.Float};{C.Double}");
        for (var i = 0; i < lines; i++)
        {
            sb.AppendLine();
            sb.Append($"Row{i:D9};{(sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue)};" +
                $"{(short)random.Next(short.MinValue, short.MaxValue)};" +
                $"{random.Next()};{random.NextInt64()};" +
                $"{random.NextSingle()};{random.NextDouble()}");
        }
        var text = sb.ToString();
        return text;

    }
}

