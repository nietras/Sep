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

public class RowsStatisticColumn : IColumn
{
    readonly Func<IReadOnlyList<ParameterInstance>, int> _getRows;
    readonly Func<int, BenchmarkCase, Statistics, string> _format;

    public RowsStatisticColumn(
        string columnName,
        Func<IReadOnlyList<ParameterInstance>, int> getRows,
        Func<int, BenchmarkCase, Statistics, string> format)
    {
        ColumnName = columnName;
        _getRows = getRows ?? throw new ArgumentNullException(nameof(getRows));
        _format = format ?? throw new ArgumentNullException(nameof(format));
    }

    public static IColumn NSPerRow() => new RowsStatisticColumn("ns/row",
        RowsFromParameters, FormatNSPerRow);

    internal static int RowsFromParameters(IReadOnlyList<ParameterInstance> parameters)
    {
        return parameters.Where(p => p.Name == nameof(PackageAssetsBench.Rows)).Select(p => (int)p.Value).Single()!;
    }

    internal static string FormatNSPerRow(int lines, BenchmarkCase benchmarkCase, Statistics statistics)
    {
        var nsPerLine = statistics.Mean / lines; // Results always in nanoseconds
        return $"{nsPerLine:F1}";
    }

    public string Id => nameof(RowsStatisticColumn) + "." + ColumnName;
    public string ColumnName { get; }
    public string Legend => ColumnName;
    public UnitType UnitType => UnitType.Time;
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
                var rows = _getRows(benchmarkCase.Parameters.Items);
                return _format(rows, benchmarkCase, statistics);
            }
        }
        return "n/a";
    }

    public override string ToString() => ColumnName;
}
