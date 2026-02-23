using System;
using System.Linq;

namespace nietras.SeparatedValues.Test;

static class SepColsTestData
{
    internal const int ColsCount = 5;
    internal static readonly string[] ColNames = ["A", "B", "C", "D", "E"];
    internal static readonly int[] ColIndices = Enumerable.Range(0, ColsCount).ToArray();
    internal static readonly int[] ColValues = [10, 11, 12, 13, 14];
    internal static readonly float[] ColValuesFloat = ColValues.Select(i => (float)i).ToArray();
    internal static readonly string[] ColTexts = ColValues.Select(i => i.ToString()).ToArray();
    internal const string Text = """
                                 A;B;C;D;E
                                 10;11;12;13;14
                                 """;
    internal static readonly int?[] ColValuesUnparseable = [null, null, 12, null, 14];
    internal const string TextUnparseable = """
                                            A;B;C;D;E
                                            1a;;12;;14
                                            """;

    internal static readonly Range[] Ranges =
    [
        Range.All,
        0..0,
        0..1,
        0..2,
        0..3,
        0..4,
        0..ColsCount,
        1..1,
        1..2,
        1..3,
        1..ColsCount,
        2..2,
        2..ColsCount,
    ];
}
