using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterColsTest
{
    const int _cols = 3;
    static readonly string[] _colNames = new string[_cols] { "A", "B", "C" };
    static readonly int[] _colValues0 = new int[_cols] { 10, 11, 12 };
    static readonly string[] _colTexts0 = _colValues0.Select(i => i.ToString()).ToArray();
    static readonly long[] _colValues1 = new long[_cols] { 20, 21, 22 };
    static readonly string[] _colTexts1 = _colValues1.Select(i => i.ToString()).ToArray();
    const string Expected = """
                            A;B;C
                            10;11;12
                            20;21;22
                            """;

    [TestMethod]
    public void SepWriterColsTest_Length()
    {
        using var writer = CreateWriter();
        using var row = writer.NewRow();
        Assert.AreEqual(_colNames.Length, row[_colNames].Length);
    }

    [TestMethod]
    public void SepWriterColsTest_GetByIndex()
    {
        using var writer = CreateWriter();
        using (var row0 = writer.NewRow())
        {
            var cols = row0[_colNames];
            cols.Set(_colTexts0);
            cols[1].Set($"{3:D2}");
        }
        using (var row1 = writer.NewRow())
        {
            var cols = row1[_colNames];
            cols.Set(_colTexts1);
            cols[2].Set($"{1.23:F1}");
        }
        const string expected = """
                                A;B;C
                                10;03;12
                                20;21;1.2
                                """;
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_SetCols()
    {
        using var reader = Sep.Reader().FromText(Expected);
        using var writer = CreateWriter();
        foreach (var readRow in reader)
        {
            using var writeRow = writer.NewRow();
            var readCols = readRow[_colNames];
            writeRow[_colNames].Set(readCols);
        }
        Assert.AreEqual(Expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_SetCols_WrongLength_Throws()
    {
        using var reader = Sep.Reader().FromText(Expected);
        using var writer = CreateWriter();
        foreach (var readRow in reader)
        {
            var writeRow = writer.NewRow();
            var readCols = readRow[_colNames[0..1]];
            try
            {
                writeRow[_colNames].Set(readCols);
                Assert.Fail("Wrong length should fail.");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("'cols':1 must have length 3 matching columns selected", e.Message);
            }
            try
            {
                writeRow.Dispose();
            }
            catch (InvalidOperationException e)
            {
                Assert.IsNotNull(e);
            }
        }
    }

    [TestMethod]
    public void SepWriterColsTest_SetArray()
    {
        using var writer = CreateWriter();
        using (var row0 = writer.NewRow()) { row0[_colNames].Set(_colTexts0); }
        using (var row1 = writer.NewRow()) { row1[_colNames].Set(_colTexts1); }
        Assert.AreEqual(Expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_SetIReadOnlyList()
    {
        using var writer = CreateWriter();
        using (var row0 = writer.NewRow()) { row0[_colNames].Set((IReadOnlyList<string>)_colTexts0); }
        using (var row1 = writer.NewRow()) { row1[_colNames].Set((IReadOnlyList<string>)_colTexts1); }
        Assert.AreEqual(Expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_SetReadOnlySpan()
    {
        using var writer = CreateWriter();
        using (var row0 = writer.NewRow()) { row0[_colNames].Set((ReadOnlySpan<string>)_colTexts0); }
        using (var row1 = writer.NewRow()) { row1[_colNames].Set((ReadOnlySpan<string>)_colTexts1); }
        Assert.AreEqual(Expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_FormatArray()
    {
        using var writer = CreateWriter();
        using (var row0 = writer.NewRow()) { row0[_colNames].Format(_colValues0); }
        using (var row1 = writer.NewRow()) { row1[_colNames].Format(_colValues1); }
        Assert.AreEqual(Expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_FormatIReadOnlyList()
    {
        using var writer = CreateWriter();
        using (var row0 = writer.NewRow()) { row0[_colNames].Format((IReadOnlyList<int>)_colValues0); }
        using (var row1 = writer.NewRow()) { row1[_colNames].Format((IReadOnlyList<long>)_colValues1); }
        Assert.AreEqual(Expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_FormatSpan()
    {
        using var writer = CreateWriter();
        using (var row0 = writer.NewRow()) { row0[_colNames].Format((Span<int>)_colValues0); }
        using (var row1 = writer.NewRow()) { row1[_colNames].Format((Span<long>)_colValues1); }
        Assert.AreEqual(Expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_FormatReadOnlySpan()
    {
        using var writer = CreateWriter();
        using (var row0 = writer.NewRow()) { row0[_colNames].Format((ReadOnlySpan<int>)_colValues0); }
        using (var row1 = writer.NewRow()) { row1[_colNames].Format((ReadOnlySpan<long>)_colValues1); }
        Assert.AreEqual(Expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_Format_Action()
    {
        using var writer = CreateWriter();
        using (var row0 = writer.NewRow()) { row0[_colNames].Format<int>(_colValues0, static (c, v) => c.Format(v)); }
        using (var row1 = writer.NewRow()) { row1[_colNames].Format<long>(_colValues1.AsSpan(), static (c, v) => c.Format(v)); }
        Assert.AreEqual(Expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterColsTest_SetIReadOnlyList_Null_Throws()
    {
        using var writer = CreateWriter();
        using var row = writer.NewRow();
        try
        {
            IReadOnlyList<string>? colValues = null;
            row[_colNames!].Set(colValues!);
            Assert.Fail("null should fail.");
        }
        catch (ArgumentNullException e)
        {
            Assert.AreEqual("Value cannot be null. (Parameter 'values')", e.Message);
        }
    }

    [TestMethod]
    public void SepWriterColsTest_SetIReadOnlyList_WrongLength_Throws()
    {
        using var writer = CreateWriter();
        using var row = writer.NewRow();
        try
        {
            IReadOnlyList<string> colValues = new string[_colValues0.Length - 1];
            row[_colNames!].Set(colValues);
            Assert.Fail("Wrong length should fail.");
        }
        catch (ArgumentException e)
        {
            Assert.AreEqual("'values':2 must have length 3 matching columns selected", e.Message);
        }
    }

    [TestMethod]
    public void SepWriterColsTest_SetSpan_WrongLength_Throws()
    {
        using var writer = CreateWriter();
        using var row = writer.NewRow();
        try
        {
            ReadOnlySpan<string> colValues = new string[_colValues0.Length - 1];
            row[_colNames!].Set(colValues);
            Assert.Fail("Wrong length should fail.");
        }
        catch (ArgumentException e)
        {
            Assert.AreEqual("'values':2 must have length 3 matching columns selected", e.Message);
        }
    }

    [TestMethod]
    public void SepWriterColsTest_FormatArray_Null_Throws_AsEmptySpan()
    {
        using var writer = CreateWriter();
        using var row = writer.NewRow();
        try
        {
            int[]? colValues = null;
            row[_colNames!].Format(colValues!);
            Assert.Fail("null should fail.");
        }
        catch (ArgumentException e)
        {
            Assert.AreEqual("'values':0 must have length 3 matching columns selected", e.Message);
        }
    }

    [TestMethod]
    public void SepWriterColsTest_FormatIReadOnlyList_Null_Throws()
    {
        using var writer = CreateWriter();
        using var row = writer.NewRow();
        try
        {
            IReadOnlyList<int>? colValues = null;
            row[_colNames!].Format(colValues!);
            Assert.Fail("null should fail.");
        }
        catch (ArgumentNullException e)
        {
            Assert.AreEqual("Value cannot be null. (Parameter 'values')", e.Message);
        }
    }

    [TestMethod]
    public void SepWriterColsTest_FormatIReadOnlyList_WrongLength_Throws()
    {
        using var writer = CreateWriter();
        using var row = writer.NewRow();
        try
        {
            IReadOnlyList<int> colValues = new int[_colValues0.Length - 1];
            row[_colNames!].Format(colValues);
            Assert.Fail("Wrong length should fail.");
        }
        catch (ArgumentException e)
        {
            Assert.AreEqual("'values':2 must have length 3 matching columns selected", e.Message);
        }
    }

    [TestMethod]
    public void SepWriterColsTest_FormatSpan_NullFormat_Throws()
    {
        using var writer = CreateWriter();
        using var row = writer.NewRow();
        try
        {
            row[_colNames!].Format<int>(_colValues0.AsSpan(), null!);
            Assert.Fail("format null should fail.");
        }
        catch (ArgumentNullException e)
        {
            Assert.AreEqual("Value cannot be null. (Parameter 'format')", e.Message);
        }
    }

    [TestMethod]
    public void SepWriterColsTest_FormatSpan_WrongLength_Format_Throws()
    {
        using var writer = CreateWriter();
        using var row = writer.NewRow();
        try
        {
            var colValues = new int[_colValues0.Length + 1];
            row[_colNames!].Format<int>(colValues, static (c, v) => c.Format(v));
            Assert.Fail("Wrong length should fail.");
        }
        catch (ArgumentException e)
        {
            Assert.AreEqual("'values':4 must have length 3 matching columns selected", e.Message);
        }
    }

    static SepWriter CreateWriter() => Sep.Writer().ToText();
}
