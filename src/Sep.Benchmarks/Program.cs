﻿// Type 'Program' can be sealed because it has no subtypes in its containing assembly and is not externally visible
#pragma warning disable CA1852
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BenchmarkDotNet.Columns;
#pragma warning disable IDE0005
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Parameters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using nietras.SeparatedValues.Benchmarks;
using nietras.SeparatedValues.ComparisonBenchmarks;
#pragma warning restore IDE0005
[assembly: System.Runtime.InteropServices.ComVisible(false)]

Action<string> log = t => { Console.WriteLine(t); Trace.WriteLine(t); };

log($"{Environment.Version} args: {args.Length}");

if (args.Length > 0)
{
    var config = (Debugger.IsAttached ? new DebugInProcessConfig() : DefaultConfig.Instance)
        .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(200))
        .AddColumn(MBPerSecFromCharsLength());
    //BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args, config);
    //BenchmarkRunner.Run(typeof(SepReaderBench), config, args);
    //BenchmarkRunner.Run(typeof(SepWriterBench), config, args);
    //BenchmarkRunner.Run(typeof(SepReaderWriterBench), config, args);
    //BenchmarkRunner.Run(typeof(SepEndToEndBench), config, args);
    //BenchmarkRunner.Run(typeof(SepHashBench), config, args);
    BenchmarkRunner.Run(typeof(SepCharsFinderBench), config, args);
}
else
{
    var b = new SepCharsFinderBench();
    b.GlobalSetup();
    b.Find();
    Thread.Sleep(200);
    for (var i = 0; i < 200000000; i++)
    {
        b.Find();
    }

    //Runner.ParseAndFormatGenerated(log);
}

static IColumn MBPerSecFromCharsLength() => new BytesStatisticColumn("MB/s",
    BytesFromCharsLength, BytesStatisticColumn.FormatMBPerSec);

static long BytesFromCharsLength(IReadOnlyList<ParameterInstance> parameters)
{
    return parameters.Where(p => p.Name == nameof(SepCharsFinderBench.Length)).Select(p => (int)p.Value).Single()!;
}
