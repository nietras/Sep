using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace nietras.SeparatedValues.Benchmarks;

[MemoryDiagnoser]
public class SepEndToEndBench
{
    string _scsv = string.Empty;

    //[Params(0, 1, 10, 1000, 10_000, 1_000_000)]
    public int Rows { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _scsv = GenerateSCSV(Rows, seed: 42);
    }

    [Benchmark]
    public void ReaderParseWriterFormat()
    {
        using var reader = Sep.Reader().FromText(_scsv);
        using var writer = reader.Spec.Writer().ToText(_scsv.Length); //.To(TextWriter.Null); //
        foreach (var readRow in reader)
        {
            var i8 = readRow[C.SByte].Parse<sbyte>();
            var i16 = readRow[C.Short].Parse<short>();
            var i32 = readRow[C.Int].Parse<int>();
            var i64 = readRow[C.Long].Parse<long>();
            var f32 = readRow[C.Float].Parse<float>();
            var f64 = readRow[C.Double].Parse<double>();

            using var writeRow = writer.NewRow();
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

