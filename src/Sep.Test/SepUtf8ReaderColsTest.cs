using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static nietras.SeparatedValues.Test.SepColsTestData;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUtf8ReaderColsTest
{
    [TestMethod]
    public void SepUtf8ReaderColsTest_Length()
    {
        Run((cols, range) => Assert.AreEqual(range.GetOffsetAndLength(ColsCount).Length, cols.Count),
            checkIndexOutOfRange: false);
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_Indexer()
    {
        Run((cols, range) =>
        {
            var expectedTexts = ColTexts[range];
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
        Run((cols, range) => CollectionAssert.AreEqual(ColTexts[range], cols.ToStringsArray()));
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_ToStrings()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColTexts[range], cols.ToStrings().ToArray()));
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_ParseToArray()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColValues[range], cols.ParseToArray<int>()));
        Run((cols, range) => CollectionAssert.AreEqual(ColValuesFloat[range], cols.ParseToArray<float>()));
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_Parse()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColValues[range], cols.Parse<int>().ToArray()));
        Run((cols, range) => CollectionAssert.AreEqual(ColValuesFloat[range], cols.Parse<float>().ToArray()));
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_Parse_IntoSpan()
    {
        Run((cols, range) =>
        {
            Span<int> colValues = stackalloc int[cols.Count];
            cols.Parse(colValues);
            CollectionAssert.AreEqual(ColValues[range], colValues.ToArray());
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
        Run((cols, range) => CollectionAssert.AreEqual(ColValuesUnparseable[range], cols.TryParse<int>().ToArray()), TextUnparseable);
    }

    [TestMethod]
    public void SepUtf8ReaderColsTest_TryParse_IntoSpan()
    {
        Run((cols, range) =>
        {
            Span<int?> colValues = stackalloc int?[cols.Count];
            cols.TryParse(colValues);
            CollectionAssert.AreEqual(ColValues[range], colValues.ToArray());
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
        {
            using var reader = Sep.Utf8Reader().FromText(text);
            Run(reader, Ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Utf8Reader(o => o with { Unescape = true }).FromText(text);
            Run(reader, Ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Utf8Reader(o => o with { Trim = SepTrim.All }).FromText(text);
            Run(reader, Ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Utf8Reader(o => o with { Unescape = true, Trim = SepTrim.All }).FromText(text);
            Run(reader, Ranges, action, checkIndexOutOfRange);
        }
    }

    static void Run(SepUtf8Reader reader, Range[] ranges, ColsTestAction action, bool checkIndexOutOfRange)
    {
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        action(row[ColIndices.AsSpan()], ..);
        foreach (var range in ranges)
        {
            action(row[ColIndices[range].AsSpan()], range);
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