using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using CsvHelper;
using CsvHelper.Configuration;
using Sylvan.Data.Csv;
using T = nietras.SeparatedValues.ComparisonBenchmarks.FloatsTestData;

namespace nietras.SeparatedValues.ComparisonBenchmarks;

[HideColumns("InvocationCount")]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByParams)]
public abstract class FloatsReaderBench
{
    const int FloatsCount = 20;
    readonly ReaderSpec[] _readers;

    protected FloatsReaderBench(string scope, int lineCount)
    {
        Scope = scope;
        Rows = lineCount;
        _readers = new ReaderSpec[]
        {
            ReaderSpec.FromString("String", new(() => T.GenerateText(Rows, FloatsCount))),
            //ReaderSpec.FromBytes("Stream", new(() => T.GenerateBytes(Rows, FloatsCount))),
        };
        Reader = _readers.First();
    }

    [ParamsSource(nameof(ScopeParams))] // Attributes for params is challenging 👇
    public string Scope { get; set; }
    public IEnumerable<string> ScopeParams() => new[] { Scope };

    [ParamsSource(nameof(Readers))]
    public ReaderSpec Reader { get; set; }
    public IEnumerable<ReaderSpec> Readers() => _readers;

    [ParamsSource(nameof(RowsParams))] // Attributes for params is challenging 👇
    public int Rows { get; set; }
    public IEnumerable<int> RowsParams() => new[] { Rows };
}

[BenchmarkCategory("0_Row")]
public class RowFloatsReaderBench : FloatsReaderBench
{
#if DEBUG
    const int DefaultLineCount = 10_000;
#else
    const int DefaultLineCount = 25_000;
#endif

    public RowFloatsReaderBench() : base("Row", DefaultLineCount) { }

    delegate string SpanToString(ReadOnlySpan<char> chars);

    [Benchmark(Baseline = true)]
    public void Sep______()
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false })
                              .From(Reader.CreateReader());
        foreach (var row in reader)
        { }
    }

    [Benchmark]
    public void Sylvan___()
    {
        using var reader = Reader.CreateReader();
        var options = new CsvDataReaderOptions
        {
            HasHeaders = false,
        };
        var buffer = ArrayPool<char>.Shared.Rent(32 * 1024);
        try
        {
            using var csvReader = Sylvan.Data.Csv.CsvDataReader.Create(reader, buffer, options);
            while (csvReader.Read())
            {
            }
        }
        finally { ArrayPool<char>.Shared.Return(buffer); }
    }

#if SEPBENCHSLOWONES
    [Benchmark]
#endif
    public void ReadLine_()
    {
        using var reader = Reader.CreateReader();
        string? line = null;
        while ((line = reader.ReadLine()) != null)
        {
            var cols = line.Split(';');
        }
    }

#if SEPBENCHSLOWONES
    [Benchmark]
#endif
    public void CsvHelper()
    {
        using var reader = Reader.CreateReader();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
#if USE_STRING_POOLING
            CacheFields = true,
#endif
        };
        using var csvParser = new CsvParser(reader, config);
        while (csvParser.Read())
        {
        }
    }
}

[BenchmarkCategory("1_Cols")]
public class ColsFloatsReaderBench : FloatsReaderBench
{
#if DEBUG
    const int DefaultLineCount = 10_000;
#else
    const int DefaultLineCount = 25_000;
#endif

    public ColsFloatsReaderBench() : base("Cols", DefaultLineCount) { }

    delegate string SpanToString(ReadOnlySpan<char> chars);

    [Benchmark(Baseline = true)]
    public void Sep______()
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false })
                              .From(Reader.CreateReader());
        foreach (var row in reader)
        {
            for (var i = 0; i < row.ColCount; i++)
            {
                var span = row[i].Span;
            }
        }
    }

    [Benchmark]
    public void Sylvan___()
    {
        using var reader = Reader.CreateReader();
        var options = new CsvDataReaderOptions
        {
            HasHeaders = false,
        };
        var buffer = ArrayPool<char>.Shared.Rent(32 * 1024);
        try
        {
            using var csvReader = Sylvan.Data.Csv.CsvDataReader.Create(reader, buffer, options);
            while (csvReader.Read())
            {
                for (var i = 0; i < csvReader.FieldCount; i++)
                {
                    var span = csvReader.GetFieldSpan(i);
                }
            }
        }
        finally { ArrayPool<char>.Shared.Return(buffer); }
    }

#if SEPBENCHSLOWONES
    [Benchmark]
#endif
    public void ReadLine_()
    {
        using var reader = Reader.CreateReader();
        string? line = null;
        while ((line = reader.ReadLine()) != null)
        {
            var cols = line.Split(';');
            for (var i = 0; i < cols.Length; i++)
            {
                var s = cols[i].AsSpan();
            }
        }
    }

#if SEPBENCHSLOWONES
    [Benchmark]
#endif
    public void CsvHelper()
    {
        using var reader = Reader.CreateReader();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
#if USE_STRING_POOLING
            CacheFields = true,
#endif
        };
        using var csvParser = new CsvParser(reader, config);
        while (csvParser.Read())
        {
            // CsvHelper has no Span-based API
            for (var i = 0; i < csvParser.Count; i++)
            {
                var s = csvParser[i].AsSpan();
            }
        }
    }
}

[BenchmarkCategory("2_Floats")]
public class FloatsFloatsReaderBench : FloatsReaderBench
{
#if DEBUG
    const int DefaultLineCount = 1_000;
#else
    const int DefaultLineCount = 25_000;
#endif

    public FloatsFloatsReaderBench() : base("Floats", DefaultLineCount) { }

    delegate string SpanToString(ReadOnlySpan<char> chars);

    [Benchmark(Baseline = true)]
    public double Sep______()
    {
        using var reader = Sep.Reader().From(Reader.CreateReader());

        var groundTruthColNames = reader.Header.NamesStartingWith(T.GroundTruthColNamePrefix);
        var resultColNames = groundTruthColNames.Select(n =>
            n.Replace(T.GroundTruthColNamePrefix, T.ResultColNamePrefix, StringComparison.Ordinal))
            .ToArray();

        var sum = 0.0;
        var count = 0;
        foreach (var row in reader)
        {
            var gts = row[groundTruthColNames].Parse<float>();
            var res = row[resultColNames].Parse<float>();

            sum += MeanSquaredError(gts, res);
            ++count;
        }
        return sum / count;
    }

    [Benchmark]
    public double Sylvan___()
    {
        using var reader = Reader.CreateReader();

        var buffer = ArrayPool<char>.Shared.Rent(32 * 1024);
        try
        {
            using var csvReader = Sylvan.Data.Csv.CsvDataReader.Create(reader, buffer);

            var groundTruthColNames = new List<string>();
            for (var i = 0; i < csvReader.FieldCount; i++)
            {
                var colName = csvReader.GetName(i);
                if (colName.StartsWith(T.GroundTruthColNamePrefix, StringComparison.Ordinal))
                {
                    groundTruthColNames.Add(colName);
                }
            }
            var resultColNames = groundTruthColNames.Select(n =>
                n.Replace(T.GroundTruthColNamePrefix, T.ResultColNamePrefix, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var sum = 0.0;
            var count = 0;

            Span<float> gts = stackalloc float[groundTruthColNames.Count];
            Span<float> res = stackalloc float[resultColNames.Length];

            while (csvReader.Read())
            {
                for (var i = 0; i < groundTruthColNames.Count; i++)
                {
                    var colIndex = csvReader.GetOrdinal(groundTruthColNames[i]);
                    var value = csvReader.GetFloat(colIndex);
                    gts[i] = value;
                }
                for (var i = 0; i < resultColNames.Length; i++)
                {
                    var colIndex = csvReader.GetOrdinal(resultColNames[i]);
                    var value = csvReader.GetFloat(colIndex);
                    res[i] = value;
                }

                sum += MeanSquaredError(gts, res);
                ++count;
            }
            return sum / count;
        }
        finally { ArrayPool<char>.Shared.Return(buffer); }
    }

#if SEPBENCHSLOWONES
    [Benchmark]
#endif
    public double ReadLine_()
    {
        using var reader = Reader.CreateReader();

        var line = reader.ReadLine();
        var headerCols = line!.Split(';');
        var colNameToIndex = Enumerable.Range(0, headerCols.Length).ToDictionary(i => headerCols[i], i => i);

        var groundTruthColNames = new List<string>();
        for (var i = 0; i < headerCols.Length; i++)
        {
            var colName = headerCols[i];
            if (colName.StartsWith(T.GroundTruthColNamePrefix, StringComparison.Ordinal))
            {
                groundTruthColNames.Add(colName);
            }
        }
        var resultColNames = groundTruthColNames.Select(n =>
            n.Replace(T.GroundTruthColNamePrefix, T.ResultColNamePrefix, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var sum = 0.0;
        var count = 0;

        Span<float> gts = stackalloc float[groundTruthColNames.Count];
        Span<float> res = stackalloc float[resultColNames.Length];

        while ((line = reader.ReadLine()) != null)
        {
            var cols = line.Split(';');
            for (var i = 0; i < groundTruthColNames.Count; i++)
            {
                var colIndex = colNameToIndex[groundTruthColNames[i]];
                var value = float.Parse(cols[colIndex]);
                gts[i] = value;
            }
            for (var i = 0; i < resultColNames.Length; i++)
            {
                var colIndex = colNameToIndex[groundTruthColNames[i]];
                var value = float.Parse(cols[colIndex]);
                res[i] = value;
            }

            sum += MeanSquaredError(gts, res);
            ++count;
        }
        return sum / count;
    }

#if SEPBENCHSLOWONES
    [Benchmark]
#endif
    public double CsvHelper()
    {
        using var reader = Reader.CreateReader();
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            DetectDelimiter = true,
            HasHeaderRecord = true,
        };
        using var csvParser = new CsvParser(reader, configuration);

        // Read header
        csvParser.Read();

        var colNameToIndex = Enumerable.Range(0, csvParser.Count).ToDictionary(i => csvParser[i], i => i);

        var groundTruthColNames = new List<string>();
        for (var i = 0; i < csvParser.Count; i++)
        {
            var colName = csvParser[i];
            if (colName.StartsWith(T.GroundTruthColNamePrefix, StringComparison.Ordinal))
            {
                groundTruthColNames.Add(colName);
            }
        }
        var resultColNames = groundTruthColNames.Select(n =>
            n.Replace(T.GroundTruthColNamePrefix, T.ResultColNamePrefix, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var sum = 0.0;
        var count = 0;

        Span<float> gts = stackalloc float[groundTruthColNames.Count];
        Span<float> res = stackalloc float[resultColNames.Length];

        while (csvParser.Read())
        {
            for (var i = 0; i < groundTruthColNames.Count; i++)
            {
                var colIndex = colNameToIndex[groundTruthColNames[i]];
                var value = float.Parse(csvParser[colIndex]);
                gts[i] = value;
            }
            for (var i = 0; i < resultColNames.Length; i++)
            {
                var colIndex = colNameToIndex[groundTruthColNames[i]];
                var value = float.Parse(csvParser[colIndex]);
                res[i] = value;
            }

            sum += MeanSquaredError(gts, res);
            ++count;
        }
        return sum / count;
    }

    static float MeanSquaredError(Span<float> gts, Span<float> res)
    {
        var sumSquaredError = 0f;
        for (var i = 0; i < gts.Length; i++)
        {
            var diff = res[i] - gts[i];
            sumSquaredError += diff * diff;
        }
        return sumSquaredError / gts.Length; // Assume > 0
    }
}
