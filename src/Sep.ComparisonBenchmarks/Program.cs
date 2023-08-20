// Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible
#pragma warning disable CA1852
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Parameters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using nietras.SeparatedValues.ComparisonBenchmarks;
using Perfolizer.Horology;

[assembly: System.Runtime.InteropServices.ComVisible(false)]

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

log($"{Environment.Version} args: {args.Length}");

await PackageAssetsTestData.EnsurePackageAssets().ConfigureAwait(true);

// Use args as switch to run BDN or not e.g. BDN only run when using script
if (true || args.Length > 0)
{
    var baseConfig = ManualConfig.CreateEmpty()
        .AddColumnProvider(DefaultColumnProviders.Instance)
        .AddExporter(MarkdownExporter.GitHub)
        .AddLogger(ConsoleLogger.Default);

    var config =
#if DEBUG
        baseConfig
#else
        (Debugger.IsAttached ? new DebugInProcessConfig() : DefaultConfig.Instance)
#endif
        .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(120))
        .AddColumn(MB())
        .AddColumn(MBPerSec())
        .AddColumn(RowsStatisticColumn.NSPerRow())
        .WithOption(ConfigOptions.JoinSummary, true)
#if DEBUG
        .AddJob(Job.InProcess.WithIterationTime(TimeInterval.FromMilliseconds(100)).WithMinIterationCount(2).WithMaxIterationCount(5))
#endif
        ;

    //BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args, config);

    var nameToBenchTypesSet = new Dictionary<string, Type[]>()
    {
        { nameof(PackageAssetsBench), new[] { typeof(RowPackageAssetsBench), typeof(ColsPackageAssetsBench), typeof(AssetPackageAssetsBench), } },
        { nameof(PackageAssetsBench) + "Quotes", new[] { typeof(QuotesRowPackageAssetsBench), typeof(QuotesColsPackageAssetsBench), typeof(QuotesAssetPackageAssetsBench), } },
        { nameof(FloatsReaderBench), new[] { typeof(RowFloatsReaderBench), typeof(ColsFloatsReaderBench), typeof(FloatsFloatsReaderBench), } },
    };
    foreach (var (name, benchTypes) in nameToBenchTypesSet)
    {
        var summaries = BenchmarkRunner.Run(benchTypes, config, args);
        foreach (var s in summaries)
        {
            var cpuInfo = s.HostEnvironmentInfo.CpuInfo.Value;
            var processorName = cpuInfo.ProcessorName;
            var processorNameInDirectory = processorName.Replace(" Processor", "").Replace(" ", ".");

            var sourceDirectory = GetSourceDirectory();
            var directory = $"{sourceDirectory}/../../benchmarks/{processorNameInDirectory}";
            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
            var filePath = Path.Combine(directory, $"{name}.md");

            var exporter = MarkdownExporter.GitHub;

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            using var logger = new StreamLogger(writer);
            exporter.ExportToLog(s, logger);
            logger.Flush();
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            text = text.Replace("**", "");
            File.WriteAllText(filePath, text);
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

static string GetSourceDirectory([CallerFilePath] string filePath = "") => Path.GetDirectoryName(filePath)!;
