using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderParallelEnumerateTest
{
    const int PartRowCount = 1000;
    const int TotalRowCount = 2 * PartRowCount;
    const string ColNameInc = "Inc";
    const string ColNameFill = "Fill";
    const string ColNameDec = "Dec";
    const string ShortRowFill = "A";
    static readonly string s_longRowFill = new('B', 256);
    static readonly List<Seq> s_expected = Enumerable.Range(0, TotalRowCount)
        .Select(i => new Seq(i, TotalRowCount - i)).ToList();

    record struct Seq(int Inc, int Dec);

    [TestMethod]
    public void SepReaderParallelEnumerateTest_RowFunc()
    {
        using var reader = CreateReader();
        var actual = reader.ParallelEnumerate(Parse).ToList();
        CollectionAssert.AreEqual(s_expected, actual);
    }

    [TestMethod]
    public void SepReaderParallelEnumerateTest_RowTryFunc()
    {
        using var reader = CreateReader();
        var actual = reader.ParallelEnumerate<Seq>(TryParseEven).ToList();
        var expected = s_expected.Where(s => s.Inc % 2 == 0).ToList();
        CollectionAssert.AreEqual(expected, actual);
    }

    static SepReader CreateReader()
    {
        var sb = new StringBuilder(1024 * 1024);
        using var stringWriter = new StringWriter(sb);
        using (var writer = Sep.Writer().To(stringWriter))
        {
            foreach (var (inc, dec) in s_expected)
            {
                var fill = inc < PartRowCount ? ShortRowFill : s_longRowFill;
                using var row = writer.NewRow();
                row[ColNameInc].Format(inc);
                row[ColNameFill].Set(fill);
                row[ColNameDec].Format(dec);
            }
        }
        var csv = sb.ToString();
        // Force small initial buffer length even for Release, to force reader
        // state swapping and array swapping with increasing row length.
        return Sep.Reader(o => o with { InitialBufferLength = 128 }).FromText(csv);
    }

    static bool TryParseEven(SepReader.Row row, out Seq seq)
    {
        seq = Parse(row);
        return seq.Inc % 2 == 0;
    }

    static Seq Parse(SepReader.Row row) =>
        new(row[ColNameInc].Parse<int>(), row[ColNameDec].Parse<int>());
}
