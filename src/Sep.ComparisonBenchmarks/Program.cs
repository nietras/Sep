// Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible
#pragma warning disable CA1852
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Parameters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using nietras.SeparatedValues.ComparisonBenchmarks;
[assembly: System.Runtime.InteropServices.ComVisible(false)]

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

log($"{Environment.Version} args: {args.Length}");

await PackageAssetsTestData.EnsurePackageAssets().ConfigureAwait(true);

// Use args as switch to run BDN or not e.g. BDN only run when using script
if (args.Length > 0)
{
    var config = (Debugger.IsAttached ? new DebugInProcessConfig() : DefaultConfig.Instance)
        .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(120))
        .AddColumn(MB())
        .AddColumn(MBPerSec())
        .AddColumn(RowsStatisticColumn.NSPerRow())
        .WithOption(ConfigOptions.JoinSummary, true);

    //BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args, config);

    var benchTypesSet = new Type[][]
    {
        new[] { typeof(RowPackageAssetsBench), typeof(ColsPackageAssetsBench), typeof(AssetPackageAssetsBench), },
        new[] { typeof(QuotesRowPackageAssetsBench), typeof(QuotesColsPackageAssetsBench), typeof(QuotesAssetPackageAssetsBench), },
        new[] { typeof(RowFloatsReaderBench), typeof(ColsFloatsReaderBench), typeof(FloatsFloatsReaderBench), },
    };
    foreach (var benchTypes in benchTypesSet)
    {
        var summaries = BenchmarkRunner.Run(benchTypes, config, args);
        foreach (var s in summaries)
        {
            log(s.ResultsDirectoryPath);
            log(s.LogFilePath);
            foreach (var r in s.Reports)
            {
                log(r.GetHardwareIntrinsicsInfo()!);
                var metrics = r.Metrics;
                var stats = r.ResultStatistics;

            }
        }
    }
}
else
{
    //var b = new ColsPackageAssetsBench();
    //var b = new RowFloatsReaderBench();
    var b = new RowPackageAssetsBench();
#if !DEBUG
    for (var i = 0; i < 2; ++i)
    {
        b.Sylvan___();
        b.Sep______();
        b.CsvHelper();
        b.ReadLine_();
    }
    Thread.Sleep(500);
#endif
    b.CsvHelper();
    var sw = new Stopwatch();
    sw.Restart();
    b.Sylvan___();
    var sylvan_ms = sw.ElapsedMilliseconds;
    Thread.Sleep(300);
    sw.Restart();
    b.Sep______();
    var sep_ms = sw.ElapsedMilliseconds;
    log($"Sylvan {sylvan_ms:D4}");
    log($"Sep    {sep_ms:D4}");
    log($"Ratio  {sep_ms / (double)sylvan_ms:F3}");
    Thread.Sleep(300);
    for (var i = 0; i < 20; i++)
    {
        b.Sep______();
    }
}

static IColumn MB() => new BytesStatisticColumn("MB",
    BytesFromReaderSpec, (bytes, _, _) => $"{bytes / (1024 * 1024)}");

static IColumn MBPerSec() => new BytesStatisticColumn("MB/s",
    BytesFromReaderSpec, BytesStatisticColumn.FormatMBPerSec);

static long BytesFromReaderSpec(IReadOnlyList<ParameterInstance> parameters)
{
    return parameters.Select(p => p.Value as ReaderSpec).Where(r => r is not null).Single()!.Bytes;
}

//class Config : ManualConfig
//{
//    public Config()
//    {
//        AddJob(Job.MediumRun.WithToolchain(BenchmarkDotNet.Toolchains.InProcess.Emit.InProcessEmitToolchain.Instance).WithId("InProcess"));
//    }
//}
