using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderColsTest
{
    const int _colsCount = 3;
    static readonly string[] _colNames = ["A", "B", "C"];
    static readonly int[] _colIndices = Enumerable.Range(0, _colsCount).ToArray();
    static readonly int[] _colValues = [10, 11, 12];
    static readonly float[] _colValuesFloat = _colValues.Select(i => (float)i).ToArray();
    static readonly string[] _colTexts = _colValues.Select(i => i.ToString()).ToArray();
    const string Text = """
                         A;B;C
                         10;11;12
                         """;
    static readonly int?[] _colValuesUnparseable = [null, null, 12];
    const string TextUnparseable = """
                                   A;B;C
                                   1a;;12
                                   """;

    [TestMethod]
    public void SepReaderColsTest_Length()
    {
        Run((cols, range) => Assert.AreEqual(range.GetOffsetAndLength(_colsCount).Length, cols.Count),
            checkIndexOutOfRange: false);
    }

    [TestMethod]
    public void SepReaderColsTest_Indexer()
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
        Run((cols, range) => CollectionAssert.AreEqual(_colTexts[range], cols.ToStringsArray()));
    }

    [TestMethod]
    public void SepReaderColsTest_ToStrings()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colTexts[range], cols.ToStrings().ToArray()));
    }

    [TestMethod]
    public void SepReaderColsTest_ParseToArray()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colValues[range], cols.ParseToArray<int>()));
        Run((cols, range) => CollectionAssert.AreEqual(_colValuesFloat[range], cols.ParseToArray<float>()));
#if NET8_0_OR_GREATER
        // string unfortunately did not implement ISpanParsable until .NET 8 see ToStringsArray
        Run((cols, range) => CollectionAssert.AreEqual(_colTexts[range], cols.ParseToArray<string>()));
#endif
    }

    [TestMethod]
    public void SepReaderColsTest_Parse()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colValues[range], cols.Parse<int>().ToArray()));
        Run((cols, range) => CollectionAssert.AreEqual(_colValuesFloat[range], cols.Parse<float>().ToArray()));
#if NET8_0_OR_GREATER
        // string unfortunately did not implement ISpanParsable until .NET 8 see ToStrings
        Run((cols, range) => CollectionAssert.AreEqual(_colTexts[range], cols.Parse<string>().ToArray()));
#endif
    }

    [TestMethod]
    public void SepReaderColsTest_Parse_IntoSpan()
    {
        Run((cols, range) =>
        {
            Span<int> colValues = stackalloc int[cols.Count];
            cols.Parse(colValues);
            CollectionAssert.AreEqual(_colValues[range], colValues.ToArray());
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
        Run((cols, range) => CollectionAssert.AreEqual(_colValuesUnparseable[range], cols.TryParse<int>().ToArray()), TextUnparseable);
    }

    [TestMethod]
    public void SepReaderColsTest_TryParse_IntoSpan()
    {
        Run((cols, range) =>
        {
            Span<int?> colValues = stackalloc int?[cols.Count];
            cols.TryParse(colValues);
            CollectionAssert.AreEqual(_colValues[range], colValues.ToArray());
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
        Run((cols, range) => CollectionAssert.AreEqual(_colTexts[range], cols.Select(c => c.ToString()).ToArray()));
    }

    [TestMethod]
    public unsafe void SepReaderColsTest_Select_MethodPointer_ToString()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colTexts[range], cols.Select(&ToString).ToArray()));
    }

    [TestMethod]
    public void SepReaderColsTest_Select_ToStringDirect()
    {
        Run((cols, range) => CollectionAssert.AreEqual(_colTexts[range], cols.Select(c => c.ToStringDirect()).ToArray()));
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("/")]
    [DataRow("<SEP>")]
    public void SepReaderColsTest_Join(string separator)
    {
        // Join
        Run((cols, range) => Assert.AreEqual(string.Join(separator, _colTexts[range]), cols.Join(separator).ToString()));
        // JoinToString
        Run((cols, range) => Assert.AreEqual(string.Join(separator, _colTexts[range]), cols.JoinToString(separator)));
    }

    static string ToString(SepReader.Col col) => col.ToString();

    static void Run(ColsTestAction action, string text = Text, bool checkIndexOutOfRange = true)
    {
        var ranges = new Range[]
        {
            ..,
            0..0,
            0..1,
            0..2,
            0.._colsCount,
            1..1,
            1..2,
            1.._colsCount,
            2..2,
            2.._colsCount,
        };
        {
            using var reader = Sep.Reader().FromText(text);
            Run(reader, ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Reader(o => o with { Unescape = true }).FromText(text);
            Run(reader, ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Reader(o => o with { Trim = SepTrim.All }).FromText(text);
            Run(reader, ranges, action, checkIndexOutOfRange);
        }
        {
            using var reader = Sep.Reader(o => o with { Unescape = true, Trim = SepTrim.All }).FromText(text);
            Run(reader, ranges, action, checkIndexOutOfRange);
        }
    }

    static void Run(SepReader reader, Range[] ranges, ColsTestAction action, bool checkIndexOutOfRange)
    {
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        action(row[_colNames], ..);
        foreach (var range in ranges)
        {
            action(row[_colNames[range]], range);
            action(row[(IReadOnlyList<string>)_colNames[range]], range);
            action(row[_colNames[range].AsSpan()], range);

            action(row[_colIndices[range]], range);
            action(row[(IReadOnlyList<int>)_colIndices[range]], range);
            action(row[_colIndices[range].AsSpan()], range);

            action(row[range], range);
        }
        if (checkIndexOutOfRange)
        {
            // Ensure index out of range causes exception (note range is not same)
            Assert.ThrowsException<IndexOutOfRangeException>(() => action(reader.Current[[-1]], 0..1));
        }
    }

    delegate void ColsTestAction(SepReader.Cols cols, Range range);
}
