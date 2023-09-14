using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderColsTest
{
    const int _colsCount = 3;
    static readonly string[] _colNames = new string[_colsCount] { "A", "B", "C" };
    static readonly int[] _colValues = new int[_colsCount] { 10, 11, 12 };
    static readonly float[] _colValuesFloat = _colValues.Select(i => (float)i).ToArray();
    static readonly string[] _colTexts = _colValues.Select(i => i.ToString()).ToArray();
    const string Text = """
                         A;B;C
                         10;11;12
                         """;
    static readonly int?[] _colValuesUnparseable = new int?[_colsCount] { null, null, 12 };
    const string TextUnparseable = """
                                   A;B;C
                                   1a;;12
                                   """;

    [TestMethod]
    public void SepReaderColsTest_Length()
    {
        Run(cols => Assert.AreEqual(_colsCount, cols.Length));
    }

    [TestMethod]
    public void SepReaderColsTest_ToStringsArray()
    {
        Run(cols => CollectionAssert.AreEqual(_colTexts, cols.ToStringsArray()));
    }

    [TestMethod]
    public void SepReaderColsTest_ToStrings()
    {
        Run(cols => CollectionAssert.AreEqual(_colTexts, cols.ToStrings().ToArray()));
    }

    [TestMethod]
    public void SepReaderColsTest_ParseToArray()
    {
        Run(cols => CollectionAssert.AreEqual(_colValues, cols.ParseToArray<int>()));
        Run(cols => CollectionAssert.AreEqual(_colValuesFloat, cols.ParseToArray<float>()));
        Run(cols => CollectionAssert.AreEqual(_colTexts, cols.ToStringsArray()));
#if NET8_0_OR_GREATER
        // string unfortunately did not implement ISpanParsable until .NET 8 see ToStringsArray
        Run(cols => CollectionAssert.AreEqual(_colTexts, cols.ParseToArray<string>()));
#endif
    }

    [TestMethod]
    public void SepReaderColsTest_Parse()
    {
        Run(cols => CollectionAssert.AreEqual(_colValues, cols.Parse<int>().ToArray()));
        Run(cols => CollectionAssert.AreEqual(_colValuesFloat, cols.Parse<float>().ToArray()));
#if NET8_0_OR_GREATER
        // string unfortunately did not implement ISpanParsable until .NET 8 see ToStrings
        Run(cols => CollectionAssert.AreEqual(_colTexts, cols.Parse<string>().ToArray()));
#endif
    }

    [TestMethod]
    public void SepReaderColsTest_Parse_IntoSpan()
    {
        Run(cols =>
        {
            Span<int> colValues = stackalloc int[_colsCount];
            cols.Parse(colValues);
            CollectionAssert.AreEqual(_colValues, colValues.ToArray());
        });
    }

    [TestMethod]
    public void SepReaderColsTest_Parse_IntoSpan_LengthWrong_Throws()
    {
        Run(cols =>
        {
            Span<int> colValues = stackalloc int[_colsCount + 1];
            try
            {
                cols.Parse(colValues);
                Assert.Fail("Parse should throw due to wrong length.");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("'span':4 must have length 3 matching columns selected", e.Message);
            }
        });
    }

    [TestMethod]
    public void SepReaderColsTest_TryParse_Return()
    {
        Run(cols => CollectionAssert.AreEqual(_colValuesUnparseable, cols.TryParse<int>().ToArray()), TextUnparseable);
    }

    [TestMethod]
    public void SepReaderColsTest_TryParse_IntoSpan()
    {
        Run(cols =>
        {
            Span<int?> colValues = stackalloc int?[_colsCount];
            cols.TryParse(colValues);
            CollectionAssert.AreEqual(_colValues, colValues.ToArray());
        });
    }

    [TestMethod]
    public void SepReaderColsTest_TryParse_IntoSpan_LengthWrong_Throws()
    {
        Run(cols =>
        {
            Span<int?> colValues = stackalloc int?[_colsCount - 1];
            try
            {
                cols.TryParse(colValues);
                Assert.Fail("Parse should throw due to wrong length.");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("'span':2 must have length 3 matching columns selected", e.Message);
            }
        });
    }

    [TestMethod]
    public unsafe void SepReaderColsTest_Select_ToString()
    {
        Run(cols => CollectionAssert.AreEqual(_colTexts, cols.Select(c => c.ToString()).ToArray()));
    }

    [TestMethod]
    public unsafe void SepReaderColsTest_Select_MethodPointer_ToString()
    {
        Run(cols => CollectionAssert.AreEqual(_colTexts, cols.Select(&ToString).ToArray()));
    }

    [TestMethod]
    public void SepReaderColsTest_Select_ToStringRaw()
    {
        Run(cols => CollectionAssert.AreEqual(_colTexts, cols.Select(c => c.ToStringRaw()).ToArray()));
    }

    static string ToString(SepReader.Col col) => col.ToString();

    static void Run(SepReader.ColsAction action, string text = Text)
    {
        using var reader = Sep.Reader().FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        action(row[_colNames]);
    }
}
