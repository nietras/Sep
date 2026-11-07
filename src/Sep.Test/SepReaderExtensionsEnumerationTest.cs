using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderExtensionsEnumerationTest
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
    [DataRow(false)]
    [DataRow(true)]
    public void SepReaderExtensionsEnumerationTest_ParallelEnumerate_QuoteContinuation(bool unescape)
    {
        var expected = new List<string>();
        var csv = new StringBuilder("A;B\n");
        for (var i = 0; i < TotalRowCount; i++)
        {
            var value = new string('x', i % 257) + ";\"\r\nlast";
            var quoted = "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
            csv.Append(i).Append(';').Append(quoted).Append('\n');
            expected.Add(unescape ? value : quoted);
        }
        using var reader = Sep.Reader(o => o with { InitialBufferLength = 128, Unescape = unescape })
            .FromText(csv.ToString());
        var actual = reader.ParallelEnumerate(row => row["B"].ToString()).ToList();
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ParallelEnumerate_PackedOverflowAndTrimmedStart()
    {
        var quoted = "\"" + new string('"', 130) + "\"";
        var otherQuoted = "\"" + new string('"', 140) + "\"";
        var text = "A;B;C\n" + string.Concat(Enumerable.Repeat("x;  " + quoted + " ;" + otherQuoted + "\n", TotalRowCount));
        using var reader = Sep.Reader(o => o with
        { InitialBufferLength = 128, Unescape = true, Trim = SepTrim.All }).FromText(text);
        var expected = new string('"', 65);
        var expectedOther = new string('"', 70);
        var actual = reader.ParallelEnumerate(row =>
        {
            var first = row["B"].Span.ToString();
            var second = row["C"].Span.ToString();
            return first + "|" + second + "|" + row["B"].Span.ToString() + "|" + row["C"].Span.ToString();
        }).ToList();
        Assert.HasCount(TotalRowCount, actual);
        foreach (var value in actual)
        {
            Assert.AreEqual(expected + "|" + expectedOther + "|" + expected + "|" + expectedOther, value);
        }
    }

    [TestMethod]
    public void SepReaderExtensionsEnumerationTest_Enumerate_RowFunc()
    {
        using var reader = CreateReader();
        var actual = reader.Enumerate(Parse).ToList();
        CollectionAssert.AreEqual(s_expected, actual);
    }

    [TestMethod]
    public void SepReaderExtensionsEnumerationTest_Enumerate_RowTryFunc()
    {
        using var reader = CreateReader();
        var actual = reader.Enumerate<Seq>(TryParseEven).ToList();
        var expected = s_expected.Where(s => s.Inc % 2 == 0).ToList();
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async ValueTask SepReaderExtensionsEnumerationTest_EnumerateAsync_RowFunc()
    {
        using var reader = await CreateReaderAsync();
        var actual = await ToListAsync(reader.EnumerateAsync(Parse));
        CollectionAssert.AreEqual(s_expected, actual);
    }

    [TestMethod]
    public async ValueTask SepReaderExtensionsEnumerationTest_EnumerateAsync_RowTryFunc()
    {
        using var reader = await CreateReaderAsync();
        var actual = await ToListAsync(reader.EnumerateAsync<Seq>(TryParseEven));
        var expected = s_expected.Where(s => s.Inc % 2 == 0).ToList();
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void SepReaderExtensionsEnumerationTest_ParallelEnumerate_RowFunc()
    {
        using var reader = CreateReader();
        var actual = reader.ParallelEnumerate(Parse).ToList();
        CollectionAssert.AreEqual(s_expected, actual);
    }

    [TestMethod]
    public void SepReaderExtensionsEnumerationTest_ParallelEnumerate_RowTryFunc()
    {
        using var reader = CreateReader();
        var actual = reader.ParallelEnumerate<Seq>(TryParseEven).ToList();
        var expected = s_expected.Where(s => s.Inc % 2 == 0).ToList();
        CollectionAssert.AreEqual(expected, actual);
    }

    static SepReader CreateReader()
    {
        var csv = CreateCsv();
        // Force small initial buffer length even for Release, to force reader
        // state swapping and array swapping with increasing row length for
        // ParallelEnumerate.
        return Sep.Reader(o => o with { InitialBufferLength = 128 }).FromText(csv);
    }

    static ValueTask<SepReader> CreateReaderAsync()
    {
        var csv = CreateCsv();
        // Force small initial buffer length even for Release, to force reader
        // state swapping and array swapping with increasing row length for
        // ParallelEnumerate.
        return Sep.Reader(o => o with { InitialBufferLength = 128 }).FromTextAsync(csv);
    }

    static string CreateCsv()
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
        return sb.ToString();
    }

    static bool TryParseEven(SepReader.Row row, out Seq seq)
    {
        seq = Parse(row);
        return seq.Inc % 2 == 0;
    }

    static Seq Parse(SepReader.Row row) =>
        new(row[ColNameInc].Parse<int>(), row[ColNameDec].Parse<int>());

    static async ValueTask<List<T>> ToListAsync<T>(IAsyncEnumerable<T> e)
    {
        var values = new List<T>();
        await foreach (var v in e)
        {
            values.Add(v);
        }
        return values;
    }
}
