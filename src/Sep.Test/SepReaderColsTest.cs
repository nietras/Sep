using System;
using System.Collections.Generic;
#if NET9_0_OR_GREATER
using System.IO;
#endif
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static nietras.SeparatedValues.Test.SepColsTestData;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderColsTest
{
    [TestMethod]
    public void SepReaderColsTest_Length()
    {
        Run((cols, range) => Assert.AreEqual(range.GetOffsetAndLength(ColsCount).Length, cols.Count),
            checkIndexOutOfRange: false);
    }

    [TestMethod]
    public void SepReaderColsTest_Indexer()
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
    public void SepReaderColsTest_Indexer_OutOfRange_Throws()
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
    public void SepReaderColsTest_ToStringsArray()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColTexts[range], cols.ToStringsArray()));
    }

    [TestMethod]
    public void SepReaderColsTest_ToStrings()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColTexts[range], cols.ToStrings().ToArray()));
    }

    [TestMethod]
    public void SepReaderColsTest_ParseToArray()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColValues[range], cols.ParseToArray<int>()));
        Run((cols, range) => CollectionAssert.AreEqual(ColValuesFloat[range], cols.ParseToArray<float>()));
#if NET8_0_OR_GREATER
        // string unfortunately did not implement ISpanParsable until .NET 8 see ToStringsArray
        Run((cols, range) => CollectionAssert.AreEqual(ColTexts[range], cols.ParseToArray<string>()));
#endif
    }

    [TestMethod]
    public void SepReaderColsTest_Parse()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColValues[range], cols.Parse<int>().ToArray()));
        Run((cols, range) => CollectionAssert.AreEqual(ColValuesFloat[range], cols.Parse<float>().ToArray()));
#if NET8_0_OR_GREATER
        // string unfortunately did not implement ISpanParsable until .NET 8 see ToStrings
        Run((cols, range) => CollectionAssert.AreEqual(ColTexts[range], cols.Parse<string>().ToArray()));
#endif
    }

    [TestMethod]
    public void SepReaderColsTest_Parse_IntoSpan()
    {
        Run((cols, range) =>
        {
            Span<int> colValues = stackalloc int[cols.Count];
            cols.Parse(colValues);
            CollectionAssert.AreEqual(ColValues[range], colValues.ToArray());
        });
    }

    [TestMethod]
    public void SepReaderColsTest_Parse_IntoSpan_LengthWrong_Throws()
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
    public void SepReaderColsTest_TryParse_Return()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColValuesUnparseable[range], cols.TryParse<int>().ToArray()), TextUnparseable);
    }

    [TestMethod]
    public void SepReaderColsTest_TryParse_IntoSpan()
    {
        Run((cols, range) =>
        {
            Span<int?> colValues = stackalloc int?[cols.Count];
            cols.TryParse(colValues);
            CollectionAssert.AreEqual(ColValues[range], colValues.ToArray());
        });
    }

    [TestMethod]
    public void SepReaderColsTest_TryParse_IntoSpan_LengthWrong_Throws()
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

    [TestMethod]
    public unsafe void SepReaderColsTest_Select_ToString()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColTexts[range], cols.Select(c => c.ToString()).ToArray()));
    }

    [TestMethod]
    public unsafe void SepReaderColsTest_Select_MethodPointer_ToString()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColTexts[range], cols.Select(&ToString).ToArray()));
    }

    [TestMethod]
    public void SepReaderColsTest_Select_ToStringDirect()
    {
        Run((cols, range) => CollectionAssert.AreEqual(ColTexts[range], cols.Select(c => c.ToStringDirect()).ToArray()));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("/")]
    [DataRow("<SEP>")]
    public void SepReaderColsTest_Join(string separator)
    {
        // Join
        Run((cols, range) => Assert.AreEqual(string.Join(separator, ColTexts[range]), cols.Join(separator).ToString()));
        // JoinToString
        Run((cols, range) => Assert.AreEqual(string.Join(separator, ColTexts[range]), cols.JoinToString(separator)));
    }

#if NET9_0_OR_GREATER
    [TestMethod]
    public void SepReaderColsTest_JoinPathsToString()
    {
        Run((cols, range) => Assert.AreEqual(Path.Join(ColTexts[range]), cols.JoinPathsToString()));
    }

    [TestMethod]
    public void SepReaderColsTest_CombinePathsToString()
    {
        Run((cols, range) => Assert.AreEqual(Path.Combine(ColTexts[range]), cols.CombinePathsToString()));
    }
#endif

    static string ToString(SepReader.Col col) => col.ToString();

    static void Run(ColsTestAction action, string text = Text, bool checkIndexOutOfRange = true)
    {
        {
            using var reader = Sep.Reader().FromText(text);
            Run(reader, Ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Reader(o => o with { Unescape = true }).FromText(text);
            Run(reader, Ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Reader(o => o with { Trim = SepTrim.All }).FromText(text);
            Run(reader, Ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Reader(o => o with { Unescape = true, Trim = SepTrim.All }).FromText(text);
            Run(reader, Ranges, action, checkIndexOutOfRange);
        }
    }

    static void Run(SepReader reader, Range[] ranges, ColsTestAction action, bool checkIndexOutOfRange)
    {
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        action(row[ColNames], ..);
        foreach (var range in ranges)
        {
            action(row[ColNames[range]], range);
            action(row[(IReadOnlyList<string>)ColNames[range]], range);
            action(row[ColNames[range].AsSpan()], range);

            action(row[ColIndices[range]], range);
            action(row[(IReadOnlyList<int>)ColIndices[range]], range);
            action(row[ColIndices[range].AsSpan()], range);

            action(row[range], range);
        }
        if (checkIndexOutOfRange)
        {
            // Ensure index out of range causes exception (note range is not same)
            Assert.ThrowsExactly<IndexOutOfRangeException>(() => action(reader.Current[[-1]], 0..1));
        }
    }

    delegate void ColsTestAction(SepReader.Cols cols, Range range);
}
