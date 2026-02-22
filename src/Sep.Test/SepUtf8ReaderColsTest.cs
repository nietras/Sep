using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUtf8ReaderColsTest
{
    const int _colsCount = 5;
    static readonly int[] _colIndices = Enumerable.Range(0, _colsCount).ToArray();
    static readonly int[] _colValues = [10, 11, 12, 13, 14];
    static readonly float[] _colValuesFloat = _colValues.Select(i => (float)i).ToArray();
    static readonly string[] _colTexts = _colValues.Select(i => i.ToString()).ToArray();
    const string Text = """
                         A;B;C;D;E
                         10;11;12;13;14
                         """;
    static readonly int?[] _colValuesUnparseable = [null, null, 12, null, 14];
    const string TextUnparseable = """
                                   A;B;C;D;E
                                   1a;;12;;14
                                   """;

    [TestMethod]
    public void SepUtf8ReaderColsTest_Length()
    {
        Run((cols, range) => Assert.AreEqual(range.GetOffsetAndLength(_colsCount).Length, cols.Count),
            checkIndexOutOfRange: false);
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_Indexer()
    {
        Run((cols, range) =>
        {
            var expectedTexts = _colTexts[range];
            for (var i = 0; i < expectedTexts.Length; i++)
            {
                Assert.AreEqual(expectedTexts[i], cols[i].ToString());
            }
        });
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_Indexer_OutOfRange_Throws()
    {
        Run((cols, range) =>
        {
            try
            {
                var col = cols[cols.Count + 1];
                Assert.Fail("Indexer should throw for out of range index.");
            }
            catch (IndexOutOfRangeException e)
            {
                Assert.IsNotNull(e);
            }
        }, checkIndexOutOfRange: false);
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_ToStringsArray()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colTexts[range], cols.ToStringsArray()));
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_ToStrings()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colTexts[range], cols.ToStrings().ToArray()));
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_ParseToArray()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colValues[range], cols.ParseToArray<int>()));
        Run((cols, range) => CollectionAssert.AreEqual(_colValuesFloat[range], cols.ParseToArray<float>()));
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_Parse()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colValues[range], cols.Parse<int>().ToArray()));
        Run((cols, range) => CollectionAssert.AreEqual(_colValuesFloat[range], cols.Parse<float>().ToArray()));
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_Parse_IntoSpan()
    {
        Run((cols, range) =>
        {
            Span<int> colValues = stackalloc int[cols.Count];
            cols.Parse(colValues);
            CollectionAssert.AreEqual(_colValues[range], colValues.ToArray());
        });
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_Parse_IntoSpan_LengthWrong_Throws()
    {
        Run((cols, range) =>
        {
            Span<int> colValues = stackalloc int[cols.Count + 1];
            try
            {
                cols.Parse(colValues);
                Assert.Fail("Parse should throw due to wrong length.");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual($"'span':{colValues.Length} must have length/count {cols.Count} matching columns selected", e.Message);
            }
        }, checkIndexOutOfRange: false);
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_TryParse_Return()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colValuesUnparseable[range], cols.TryParse<int>().ToArray()), TextUnparseable);
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_TryParse_IntoSpan()
    {
        Run((cols, range) =>
        {
            Span<int?> colValues = stackalloc int?[cols.Count];
            cols.TryParse(colValues);
            CollectionAssert.AreEqual(_colValues[range], colValues.ToArray());
        });
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_TryParse_IntoSpan_LengthWrong_Throws()
    {
        Run((cols, range) =>
        {
            if (cols.Count <= 0) { return; }
            Span<int?> colValues = stackalloc int?[cols.Count - 1];
            try
            {
                cols.TryParse(colValues);
                Assert.Fail("Parse should throw due to wrong length.");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual($"'span':{colValues.Length} must have length/count {cols.Count} matching columns selected", e.Message);
            }
        }, checkIndexOutOfRange: false);
    }

    static void Run(ColsTestAction action, string text = Text, bool checkIndexOutOfRange = true)
    {
        var ranges = new Range[]
        {
            ..,
            0..0,
            0..1,
            0..2,
            0..3,
            0..4,
            0.._colsCount,
            1..1,
            1..2,
            1..3,
            1.._colsCount,
            2..2,
            2.._colsCount,
        };
        {
            using var reader = Sep.Utf8Reader().FromText(text);
            Run(reader, ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Utf8Reader(o => o with { Unescape = true }).FromText(text);
            Run(reader, ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Utf8Reader(o => o with { Trim = SepTrim.All }).FromText(text);
            Run(reader, ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Utf8Reader(o => o with { Unescape = true, Trim = SepTrim.All }).FromText(text);
            Run(reader, ranges, action, checkIndexOutOfRange);
        }
    }

    static void Run(SepUtf8Reader reader, Range[] ranges, ColsTestAction action, bool checkIndexOutOfRange)
    {
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        // Indices-based access via params ReadOnlySpan<int>
        action(row[_colIndices.AsSpan()], ..);
        foreach (var range in ranges)
        {
            // Indices-based
            action(row[_colIndices[range].AsSpan()], range);
            // Range-based
            action(row[range], range);
        }
        if (checkIndexOutOfRange)
        {
            Assert.ThrowsExactly<IndexOutOfRangeException>(() =>
            {
                ReadOnlySpan<int> badIndices = [-1];
                action(reader.Current[badIndices], 0..1);
            });
        }
    }

    delegate void ColsTestAction(SepUtf8Reader.Cols cols, Range range);
}
