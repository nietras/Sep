#define USE_STRING_POOLING
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using CsvHelper;
using CsvHelper.Configuration;
using Sylvan.Data.Csv;

namespace nietras.SeparatedValues.ComparisonBenchmarks;

// https://www.joelverhagen.com/blog/2020/12/fastest-net-csv-parsers
// Inspired by https://github.com/nietras/NCsvPerf which is a IMHO not a
// particular interesting benchmark of CSV parsers since all it tests is
// splitting lines to strings basically, nothing else. This can be seen in:
// https://github.com/nietras/NCsvPerf/blob/3e07bbbef6ccbbce61f66cea098d4ed10947a494/NCsvPerf/CsvReadable/Benchmarks/PackageAsset.cs#L52
[MemoryDiagnoser]
[HideColumns("InvocationCount", "Job", "IterationTime", "MinIterationCount", "MaxIterationCount", "Type", "Quotes", "Reader", "Gen0", "Gen1", "Gen2", "Error", "Median")]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByParams)]
public abstract class PackageAssetsBench
{
    readonly ReaderSpec[] _readers;

    protected PackageAssetsBench(string scope, int lineCount,
        bool quoteAroundSomeCols = false, bool spacesAroundSomeColsAndInsideQuotes = false)
    {
        Quotes = quoteAroundSomeCols;
        Scope = scope;
        Rows = lineCount;
        _readers =
        [
            ReaderSpec.FromString("String", new(() => PackageAssetsTestData.PackageAssets(quoteAroundSomeCols, spacesAroundSomeColsAndInsideQuotes).GetString(Rows))),
            //ReaderSpec.FromBytes("Stream", new(() => PackageAssetsTestData.PackageAssets(quoteAroundSomeCols, spacesAroundSomeColsAndInsideQuotes).GetBytes(Rows))),
        ];
        Reader = _readers.First();
    }

    [ParamsSource(nameof(ScopeParams))] // Attributes for params is challenging 👇
    public string Scope { get; set; }
    public IEnumerable<string> ScopeParams() => [Scope];

    [ParamsSource(nameof(QuotesParams))] // Attributes for params is challenging 👇
    public bool Quotes { get; set; }
    public IEnumerable<bool> QuotesParams() => [Quotes];

    [ParamsSource(nameof(Readers))]
    public ReaderSpec Reader { get; set; }
    public IEnumerable<ReaderSpec> Readers() => _readers;

    [ParamsSource(nameof(RowsParams))] // Attributes for params is challenging 👇
    public int Rows { get; set; }
    public IEnumerable<int> RowsParams() => [Rows];
}

public class QuotesRowPackageAssetsBench : RowPackageAssetsBench
{
    public QuotesRowPackageAssetsBench() : base(quoteAroundSomeCols: true) { }
}

[BenchmarkCategory("0_Row")]
public class RowPackageAssetsBench : PackageAssetsBench
{
    const int DefaultLineCount = 50_000;

    public RowPackageAssetsBench() : this(false) { }
    public RowPackageAssetsBench(bool quoteAroundSomeCols) : base("Row", DefaultLineCount, quoteAroundSomeCols) { }

    delegate string SpanToString(ReadOnlySpan<char> chars);

    [Benchmark(Baseline = true)]
    public void Sep______()
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false })
                              .From(Reader.CreateReader());
        foreach (var row in reader) { }
    }

#if NET9_0_OR_GREATER
    [Benchmark]
    public async ValueTask Sep_Async()
    {
        using var reader = await Sep.Reader(o => o with { HasHeader = false })
                                    .FromAsync(Reader.CreateReader());
        await foreach (var row in reader) { }
    }
#endif

    [Benchmark]
    public void Sep_Unescape()
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false, Unescape = true })
                              .From(Reader.CreateReader());
        foreach (var row in reader) { }
    }

#if !SEPBENCHSEPONLY
    [Benchmark]
#endif
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
            while (csvReader.Read()) { }
        }
        finally { ArrayPool<char>.Shared.Return(buffer); }
    }

#if SEPBENCHSLOWONES && !SEPBENCHSEPONLY
    [Benchmark]
#endif
    public void ReadLine_()
    {
        using var reader = Reader.CreateReader();
        string? line = null;
        while ((line = reader.ReadLine()) != null)
        {
            var cols = line.Split(',');
        }
    }

#if SEPBENCHSLOWONES && !SEPBENCHSEPONLY
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
        while (csvParser.Read()) { }
    }
}


[BenchmarkCategory("1_Cols")]
public class SpacesQuotesColsPackageAssetsBench : PackageAssetsBench
{
    const int DefaultLineCount = 50_000;

    public SpacesQuotesColsPackageAssetsBench()
        : base("Cols", DefaultLineCount, quoteAroundSomeCols: true, spacesAroundSomeColsAndInsideQuotes: true) { }

    [Benchmark(Baseline = true)]
    public void Sep_()
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

    [Benchmark()]
    public void Sep_Trim()
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false, Trim = SepTrim.Outer })
                              .From(Reader.CreateReader());
        foreach (var row in reader)
        {
            for (var i = 0; i < row.ColCount; i++)
            {
                var span = row[i].Span;
            }
        }
    }

    [Benchmark()]
    public void Sep_TrimUnescape()
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false, Unescape = true, Trim = SepTrim.Outer })
                              .From(Reader.CreateReader());
        foreach (var row in reader)
        {
            for (var i = 0; i < row.ColCount; i++)
            {
                var span = row[i].Span;
            }
        }
    }

    [Benchmark()]
    public void Sep_TrimUnescapeTrim()
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false, Unescape = true, Trim = SepTrim.All })
                              .From(Reader.CreateReader());
        foreach (var row in reader)
        {
            for (var i = 0; i < row.ColCount; i++)
            {
                var span = row[i].Span;
            }
        }
    }

#if SEPBENCHSLOWONES && !SEPBENCHSEPONLY
    [Benchmark]
#endif
    public void CsvHelper_TrimUnescape()
    {
        using var reader = Reader.CreateReader();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            TrimOptions = TrimOptions.Trim,
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
                var s = csvParser[i];
            }
        }
    }

#if SEPBENCHSLOWONES && !SEPBENCHSEPONLY
    [Benchmark]
#endif
    public void CsvHelper_TrimUnescapeTrim()
    {
        using var reader = Reader.CreateReader();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            TrimOptions = TrimOptions.Trim | TrimOptions.InsideQuotes,
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
                var s = csvParser[i];
            }
        }
    }
}

public class QuotesColsPackageAssetsBench : ColsPackageAssetsBench
{
    public QuotesColsPackageAssetsBench() : base(quoteAroundSomeCols: true) { }
}

[BenchmarkCategory("1_Cols")]
public class ColsPackageAssetsBench : PackageAssetsBench
{
    const int DefaultLineCount = 50_000;

    public ColsPackageAssetsBench() : this(false) { }
    public ColsPackageAssetsBench(bool quoteAroundSomeCols, bool spacesAroundSomeColsAndInsideQuotes = false)
        : base("Cols", DefaultLineCount, quoteAroundSomeCols, spacesAroundSomeColsAndInsideQuotes) { }

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

    [Benchmark()]
    public void Sep_Unescape()
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false, Unescape = true })
                              .From(Reader.CreateReader());
        foreach (var row in reader)
        {
            for (var i = 0; i < row.ColCount; i++)
            {
                var span = row[i].Span;
            }
        }
    }

#if !SEPBENCHSEPONLY
    [Benchmark]
#endif
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

#if SEPBENCHSLOWONES && !SEPBENCHSEPONLY
    [Benchmark]
#endif
    public void ReadLine_()
    {
        using var reader = Reader.CreateReader();
        string? line = null;
        while ((line = reader.ReadLine()) != null)
        {
            var cols = line.Split(',');
            for (var i = 0; i < cols.Length; i++)
            {
                var s = cols[i].AsSpan();
            }
        }
    }

#if SEPBENCHSLOWONES && !SEPBENCHSEPONLY
    [Benchmark]
#endif
    public void CsvHelper()
    {
        using var reader = Reader.CreateReader();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            TrimOptions = TrimOptions.None,
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
                var s = csvParser[i];
            }
        }
    }
}

public class QuotesAssetPackageAssetsBench : AssetPackageAssetsBench
{
    public QuotesAssetPackageAssetsBench() : base(quoteAroundSomeCols: true) { }
}

[BenchmarkCategory("2_Asset")]
public class AssetPackageAssetsBench : PackageAssetsBench
{
#if DEBUG
    const int DefaultLineCount = 10_000;
#else
    const int DefaultLineCount = 50_000;
#endif

    public AssetPackageAssetsBench() : this(DefaultLineCount, false) { }
    public AssetPackageAssetsBench(int rowCount = DefaultLineCount, bool quoteAroundSomeCols = false) : base("Asset", rowCount, quoteAroundSomeCols) { }

    delegate string SpanToString(ReadOnlySpan<char> chars);

    [Benchmark(Baseline = true)]
    public void Sep______()
    {
        var assets = new List<PackageAsset>();

        using var reader = Sep.Reader(o => o with
        {
            HasHeader = false,
            Unescape = Quotes,
#if USE_STRING_POOLING
            CreateToString = SepToString.PoolPerCol(maximumStringLength: 128),
#endif
        })
        .From(Reader.CreateReader());

        foreach (var row in reader)
        {
            var asset = PackageAsset.Read(reader, static (r, i) => r.ToString(i));
            assets.Add(asset);
        }
    }

    [Benchmark()]
    public void Sep_MT___()
    {
        using var reader = Sep.Reader(o => o with
        {
            HasHeader = false,
            Unescape = Quotes,
#if USE_STRING_POOLING
            CreateToString = SepToString.PoolPerColThreadSafeFixedCapacity(maximumStringLength: 128),
#endif
        })
        .From(Reader.CreateReader());

        reader.ParallelEnumerate(row => PackageAsset.Read(row._state, static (s, i) => s.ToStringDefault(i)))
              .ToList();
    }

#if !SEPBENCHSEPONLY
    [Benchmark]
#endif
    public void Sylvan___()
    {
        var assets = new List<PackageAsset>();
        using var reader = Reader.CreateReader();

#if USE_STRING_POOLING
        var stringPool = new Sylvan.StringPool(128);
#endif
        var options = new Sylvan.Data.Csv.CsvDataReaderOptions
        {
            HasHeaders = false,
#if USE_STRING_POOLING
            StringFactory = stringPool.GetString,
#endif
        };
        var buffer = ArrayPool<char>.Shared.Rent(32 * 1024);
        try
        {
            using var csvReader = Sylvan.Data.Csv.CsvDataReader.Create(reader, buffer, options);
            while (csvReader.Read())
            {
                var asset = PackageAsset.Read(csvReader, static (r, i) => r.GetString(i));
                assets.Add(asset);
            }
        }
        finally { ArrayPool<char>.Shared.Return(buffer); }
    }

#if SEPBENCHSLOWONES && !SEPBENCHSEPONLY
    [Benchmark]
#endif
    public void ReadLine_()
    {
        var assets = new List<PackageAsset>();
        using var reader = Reader.CreateReader();
        string? line = null;
        while ((line = reader.ReadLine()) != null)
        {
            var cols = line.Split(',');
            var asset = PackageAsset.Read(cols, static (cs, i) => cs[i]);
            assets.Add(asset);
        }
    }

#if SEPBENCHSLOWONES && !SEPBENCHSEPONLY
    [Benchmark]
#endif
    public void CsvHelper()
    {
        var assets = new List<PackageAsset>();
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
            var asset = PackageAsset.Read(csvParser, static (p, i) => p[i]);
            assets.Add(asset);
        }
    }
}

[BenchmarkCategory("3_Asset")]
public class LongAssetPackageAssetsBench : AssetPackageAssetsBench
{
#if DEBUG
    const int DefaultLineCount = 100_000;
#else
    const int DefaultLineCount = 1_000_000;
#endif
    public LongAssetPackageAssetsBench() : this(DefaultLineCount, false) { }
    public LongAssetPackageAssetsBench(int rowCount = DefaultLineCount, bool quoteAroundSomeCols = false)
        : base(rowCount, quoteAroundSomeCols) { }
}
public class LongQuotesAssetPackageAssetsBench : LongAssetPackageAssetsBench
{
    public LongQuotesAssetPackageAssetsBench() : base(quoteAroundSomeCols: true) { }
}

[BenchmarkCategory("4_Asset")]
[GcServer(true)]
public class GcServerAssetPackageAssetsBench : AssetPackageAssetsBench
{ }

[BenchmarkCategory("4_Asset")]
[GcServer(true)]
public class GcServerQuotesAssetPackageAssetsBench : QuotesAssetPackageAssetsBench
{ }

[BenchmarkCategory("4_Asset")]
[GcServer(true)]
public class GcServerLongAssetPackageAssetsBench : LongAssetPackageAssetsBench
{ }

[BenchmarkCategory("4_Asset")]
[GcServer(true)]
public class GcServerLongQuotesAssetPackageAssetsBench : LongQuotesAssetPackageAssetsBench
{ }
