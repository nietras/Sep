#if DEBUG
#define USEMANUALCONFIG
#endif
// Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible
#pragma warning disable CA1852
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Parameters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using nietras.SeparatedValues.ComparisonBenchmarks;
#if USEMANUALCONFIG
using BenchmarkDotNet.Jobs;
using Perfolizer.Horology;
#endif


[assembly: System.Runtime.InteropServices.ComVisible(false)]

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

log($"{Environment.Version} args: {args.Length} versions: {GetVersions()}");

await PackageAssetsTestData.EnsurePackageAssets().ConfigureAwait(true);

// Use args as switch to run BDN or not e.g. BDN only run when using script
if (args.Length > 0)
{
    var exporter = new CustomMarkdownExporter();

    var baseConfig = ManualConfig.CreateEmpty()
        .AddColumnProvider(DefaultColumnProviders.Instance)
        .AddExporter(exporter)
        .AddLogger(ConsoleLogger.Default);

    var config =
#if USEMANUALCONFIG
        baseConfig
#else
        (Debugger.IsAttached ? new DebugInProcessConfig() : DefaultConfig.Instance)
#endif
        .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(120))
        .AddColumn(MB())
        .AddColumn(MBPerSec())
        .AddColumn(RowsStatisticColumn.NSPerRow())
        .WithOption(ConfigOptions.JoinSummary, true)
#if USEMANUALCONFIG
        .AddJob(Job.InProcess.WithIterationTime(TimeInterval.FromMilliseconds(100)).WithMinIterationCount(2).WithMaxIterationCount(5))
#endif
        ;

    //BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args, config);

    var nameToBenchTypesSet = new Dictionary<string, Type[]>()
    {
        { nameof(PackageAssetsBench) + "-GcServer", new[] { typeof(GcServerAssetPackageAssetsBench), typeof(GcServerLongAssetPackageAssetsBench), } },
        { nameof(PackageAssetsBench) + "-Parsers", new[] { typeof(ParsersRowPackageAssetsBench), } },
        { nameof(PackageAssetsBench) + "Quotes" + "-GcServer", new[] { typeof(GcServerQuotesAssetPackageAssetsBench), typeof(GcServerLongQuotesAssetPackageAssetsBench), } },
        { nameof(PackageAssetsBench), new[] { typeof(RowPackageAssetsBench), typeof(ColsPackageAssetsBench), typeof(AssetPackageAssetsBench), typeof(LongAssetPackageAssetsBench), } },
        { nameof(PackageAssetsBench) + "Quotes", new[] { typeof(QuotesRowPackageAssetsBench), typeof(QuotesColsPackageAssetsBench), typeof(QuotesAssetPackageAssetsBench), typeof(LongQuotesAssetPackageAssetsBench), } },
        { nameof(PackageAssetsBench) + "SpacesQuotes", new[] { typeof(SpacesQuotesColsPackageAssetsBench) } },
        { nameof(FloatsReaderBench), new[] { typeof(RowFloatsReaderBench), typeof(ColsFloatsReaderBench), typeof(FloatsFloatsReaderBench), } },
    };
    foreach (var (name, benchTypes) in nameToBenchTypesSet)
    {
        var summaries = BenchmarkRunner.Run(benchTypes, config, args);
        foreach (var s in summaries)
        {
            var cpuInfo = s.HostEnvironmentInfo.CpuInfo.Value;
            var processorName = ProcessorBrandStringHelper.Prettify(cpuInfo);
            var processorNameInDirectory = processorName
                .Replace(" Processor", "").Replace(" CPU", "")
                .Replace(" ", ".").Replace("/", "").Replace("\\", "")
                .Replace(".Graphics", "");
            log(processorName);

            var sourceDirectory = GetSourceDirectory();
            var directory = $"{sourceDirectory}/../../benchmarks/{processorNameInDirectory}";
            if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
            var filePath = Path.Combine(directory, $"{name}.md");

            using var logger = new StreamLogger(filePath);
            exporter.ExportToLog(s, logger);

            var versions = GetVersions();
            File.WriteAllText(Path.Combine(directory, "Versions.txt"), versions);
        }
    }
}
else
{
    //var b = new ColsPackageAssetsBench();
    //var b = new RowFloatsReaderBench();
    var b = new AssetPackageAssetsBench();
    //var b = new FloatsFloatsReaderBench();
    b.Sep_MT___();
#if !DEBUG
    for (var i = 0; i < 2; ++i)
    {
        //b.Sylvan___();
        b.Sep_MT___();
        //b.CsvHelper();
        //b.ReadLineP();
        //b.ReadLine_();
    }
    Thread.Sleep(500);
#endif
    var sw = new Stopwatch();
    //sw.Restart();
    //b.Sylvan___();
    //var sylvan_ms = sw.ElapsedMilliseconds;
    //Thread.Sleep(300);
    sw.Restart();
    b.Sep_MT___();
    var sep_ms = sw.ElapsedMilliseconds;
    log($"Sep    {sep_ms:D4}");
    Thread.Sleep(300);
    //log($"Ratio    {sep_ms / (double)sylvan_ms:F3}");
    Thread.Sleep(300);
    for (var i = 0; i < 20; i++)
    {
        b.Sep_MT___();
    }
}

static IColumn MB() => new BytesStatisticColumn("MB",
    BytesFromReaderSpec, (bytes, _, _) => $"{bytes / (1024 * 1024)}");

static IColumn MBPerSec() => new BytesStatisticColumn("MB/s",
    BytesFromReaderSpec, BytesStatisticColumn.FormatMBPerSec);

static long BytesFromReaderSpec(IReadOnlyList<ParameterInstance> parameters)
{
    return parameters.Select(p => p.Value as ReaderSpec).Where(r => r is not null).Single()!.Bytes.Value;
}

static string GetVersions() =>
     $"Sep {GetFileVersion(typeof(nietras.SeparatedValues.SepReader).Assembly)}, " +
     $"Sylvan  {GetFileVersion(typeof(Sylvan.Data.Csv.CsvDataReader).Assembly)}, " +
     $"CsvHelper {GetFileVersion(typeof(CsvHelper.CsvReader).Assembly)}";

static string GetFileVersion(Assembly assembly) =>
    FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion!;

static string GetSourceDirectory([CallerFilePath] string filePath = "") => Path.GetDirectoryName(filePath)!;

class CustomMarkdownExporter : MarkdownExporter
{
    public CustomMarkdownExporter()
    {
        Dialect = "GitHub";
        UseCodeBlocks = true;
        CodeBlockStart = "```";
        StartOfGroupHighlightStrategy = MarkdownHighlightStrategy.None;
        ColumnsStartWithSeparator = true;
        EscapeHtml = true;
    }
}
