using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace nietras.SeparatedValues.Benchmarks;

[InvocationCount(400_000)]
[MemoryDiagnoser]
public class SepReaderBench
{
    readonly string _scsv = GenerateSCSV(10_000_000, seed: 42);
    SepReader? _reader;
    int _sbyteIndex;
    int _shortIndex;
    int _intIndex;
    int _longIndex;
    int _floatIndex;
    int _doubleIndex;
    int[]? _colIndeces;
    readonly string[] _colNames = new[] { C.SByte, C.Short, C.Int, C.Long, C.Float, C.Double };
    SepReader? _enumerator;

    [IterationSetup]
    public void Setup()
    {
        _reader = Sep.Reader().FromText(_scsv);
        _sbyteIndex = _reader.Header.IndexOf(C.SByte);
        _shortIndex = _reader.Header.IndexOf(C.Short);
        _intIndex = _reader.Header.IndexOf(C.Int);
        _longIndex = _reader.Header.IndexOf(C.Long);
        _floatIndex = _reader.Header.IndexOf(C.Float);
        _doubleIndex = _reader.Header.IndexOf(C.Double);
        _colIndeces = new[] { _sbyteIndex, _shortIndex, _intIndex, _longIndex, _floatIndex, _doubleIndex };
        _enumerator = _reader.GetEnumerator();
    }

    [IterationCleanup]
    public void Cleanup()
    {
        _reader?.Dispose();
        _reader = null;
    }

    [Benchmark(Baseline = true)]
    public void ParseRow()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
        }
    }

    [Benchmark]
    public void NextRow_ColFromIndex()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var i8 = row[_sbyteIndex];
            var i16 = row[_shortIndex];
            var i32 = row[_intIndex];
            var i64 = row[_longIndex];
            var f32 = row[_floatIndex];
            var f64 = row[_doubleIndex];
        }
    }

    [Benchmark]
    public void NextRow_ColFromIndex_ToString_Byte()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var s = row[_sbyteIndex].ToString();
        }
    }

    [Benchmark]
    public void NextRow_ColFromIndex_ToString_Double()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var s = row[_doubleIndex].ToString();
        }
    }

    [Benchmark]
    public void NextRow_ColFromName()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var i8 = row[C.SByte];
            var i16 = row[C.Short];
            var i32 = row[C.Int];
            var i64 = row[C.Long];
            var f32 = row[C.Float];
            var f64 = row[C.Double];
        }
    }

    [Benchmark]
    public void NextRow_ColFromIndex_ParseFloat()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var f32 = row[_doubleIndex].Parse<float>();
        }
    }

    [Benchmark]
    public void NextRow_ColFromIndex_ParseDouble()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var f64 = row[_doubleIndex].Parse<double>();
        }
    }

    [Benchmark]
    public void NextRow_ColFromIndex_ParseAll()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var i8 = row[_sbyteIndex].Parse<sbyte>();
            var i16 = row[_shortIndex].Parse<short>();
            var i32 = row[_intIndex].Parse<int>();
            var i64 = row[_longIndex].Parse<long>();
            var f32 = row[_floatIndex].Parse<float>();
            var f64 = row[_doubleIndex].Parse<double>();
        }
    }

    [Benchmark]
    public void NextRow_ColFromName_ParseAll()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var i8 = row[C.SByte].Parse<sbyte>();
            var i16 = row[C.Short].Parse<short>();
            var i32 = row[C.Int].Parse<int>();
            var i64 = row[C.Long].Parse<long>();
            var f32 = row[C.Float].Parse<float>();
            var f64 = row[C.Double].Parse<double>();
        }
    }

    [Benchmark]
    public void NextRow_ColsFromIndex()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var cols = row[_colIndeces!];
        }
    }


    [Benchmark]
    public void NextRow_ColsFromName()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var cols = row[_colNames];
        }
    }

    [Benchmark]
    public void NextRow_ColsFromIndex_ParseAll_Double_ForLoop()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var cols = row[_colIndeces!];
            for (var i = 0; i < cols.Count; i++)
            {
                var v = cols[i].Parse<double>();
            }
        }
    }

    [Benchmark]
    public void NextRow_ColsFromName_ParseAll_Double_ForLoop()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var cols = row[_colNames];
            for (var i = 0; i < cols.Count; i++)
            {
                var v = cols[i].Parse<double>();
            }
        }
    }

    [Benchmark]
    public void NextRow_ColsFromIndex_ParseAll_Double()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var values = row[_colIndeces!].Parse<double>();
        }
    }

    [Benchmark]
    public void NextRow_ColsFromName_ParseAll_Double()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var values = row[_colNames].Parse<double>();
        }
    }

    [Benchmark]
    public void NextRow_ColsFromIndex_TryParseAll_Double()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var values = row[_colIndeces!].TryParse<double>();
        }
    }

    [Benchmark]
    public void NextRow_ColsFromName_TryParseAll_Double()
    {
        if (_enumerator!.MoveNext())
        {
            var row = _enumerator.Current;
            var values = row[_colNames].TryParse<double>();
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

