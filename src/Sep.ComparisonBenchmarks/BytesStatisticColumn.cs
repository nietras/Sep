using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Parameters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace nietras.SeparatedValues.ComparisonBenchmarks;

public class BytesStatisticColumn : IColumn
{
    readonly Func<IReadOnlyList<ParameterInstance>, long> _getBytes;
    readonly Func<long, BenchmarkCase, Statistics, string> _format;

    public BytesStatisticColumn(
        string columnName,
        Func<IReadOnlyList<ParameterInstance>, long> getBytes,
        Func<long, BenchmarkCase, Statistics, string> format)
    {
        ColumnName = columnName;
        _getBytes = getBytes ?? throw new ArgumentNullException(nameof(getBytes));
        _format = format ?? throw new ArgumentNullException(nameof(format));
    }

    internal static string FormatMBPerSec(long bytes, BenchmarkCase benchmarkCase, Statistics statistics)
    {
        var mb = bytes / (1024.0 * 1024.0);
        var mbPerSec = mb / (statistics.Mean / 1_000_000_000); // Results always in nanoseconds
        return $"{mbPerSec:F1}";
    }

    public string Id => nameof(BytesStatisticColumn) + "." + ColumnName;
    public string ColumnName { get; }
    public string Legend => ColumnName;
    public UnitType UnitType => UnitType.Dimensionless;
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Metric;
    public int PriorityInCategory => 1;
    public bool IsNumeric => true;
    public bool IsAvailable(Summary summary) => true;
    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => GetValue(summary, benchmarkCase, SummaryStyle.Default);

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        Contract.Assume(summary is not null);
        Contract.Assume(benchmarkCase is not null);

        var displayInfo = benchmarkCase.DisplayInfo;
        var query = summary.Reports.Where(x => x.BenchmarkCase.DisplayInfo == displayInfo).ToList();
        if (query.Count > 0)
        {
            var s = query[0];
            var statistics = s.ResultStatistics;
            if (statistics != null)
            {
                var bytes = _getBytes(benchmarkCase.Parameters.Items);
                if (benchmarkCase.Descriptor.WorkloadMethod.Name == "Cursively")
                {
                    // Cursively operates on the bytes, and input is all ASCII, so this gives it a
                    // more appropriate result for the associated columns.
                    bytes /= 2;
                }
                return _format(bytes, benchmarkCase, statistics);
            }
        }
        return "n/a";
    }

    public override string ToString() => ColumnName;
}
