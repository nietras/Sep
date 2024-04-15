using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterHeaderTest
{
    // SepWriterHeader tests are all done through SepWriter

    [TestMethod]
    public void SepWriterHeaderTest_DebuggerDisplay_WriteHeader_true()
    {
        using var writer = CreateWriter();

        Assert.AreEqual("Count = 0 State = 'Not yet written'", writer.Header.DebuggerDisplay);

        var colNames = new string[] { "A", "B", "C" };
        writer.Header.Add(colNames);

        Assert.AreEqual("Count = 3 State = 'Not yet written'", writer.Header.DebuggerDisplay);

        writer.Header.Write();

        Assert.AreEqual("Count = 3 State = 'Written'", writer.Header.DebuggerDisplay);
    }

    [TestMethod]
    public void SepWriterHeaderTest_DebuggerDisplay_WriteHeader_false()
    {
        using var writer = Sep.New(';').Writer(o => o with { WriteHeader = false }).ToText();

        Assert.AreEqual("Count = 0 State = 'To be skipped'", writer.Header.DebuggerDisplay);

        var colNames = new string[] { "A", "B", "C" };
        writer.Header.Add(colNames);

        Assert.AreEqual("Count = 3 State = 'To be skipped'", writer.Header.DebuggerDisplay);

        writer.Header.Write();

        Assert.AreEqual("Count = 3 State = 'Skipped'", writer.Header.DebuggerDisplay);
    }

    [TestMethod]
    public void SepWriterHeaderTest_Add_Array_Rows_0()
    {
        using var writer = CreateWriter();
        var colNames = new string[] { "A", "B", "C" };
        writer.Header.Add(colNames);
        var expected =
@"A;B;C
";
        // Header written on Dispose
        writer.Dispose();

        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterHeaderTest_Add_ReadOnlyList_Rows_0()
    {
        using var writer = CreateWriter();
        IReadOnlyList<string> colNames = ["A", "B", "C"];
        writer.Header.Add(colNames);
        var expected =
@"A;B;C
";
        // Header written on Dispose
        writer.Dispose();

        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterHeaderTest_Add_ReadOnlySpan_Rows_0()
    {
        using var writer = CreateWriter();
        ReadOnlySpan<string> colNames = ["A", "B", "C"];
        writer.Header.Add(colNames);
        var expected =
@"A;B;C
";
        // Header written on Dispose
        writer.Dispose();

        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterHeaderTest_Add_String_Rows_0()
    {
        using var writer = CreateWriter();
        writer.Header.Add("A");
        var expected =
@"A
";
        // Header written on Dispose
        writer.Dispose();

        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterHeaderTest_Add_ReadOnlyList_Rows_1()
    {
        using var writer = CreateWriter();
        IReadOnlyList<string> colNames = ["A", "B", "C"];
        writer.Header.Add(colNames);
        using (var row = writer.NewRow())
        {
            row["C"].Set("3");
            row["A"].Set("1");
            row["B"].Set("2");
        }
        var expected = """
                       A;B;C
                       1;2;3
                       
                       """;
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterHeaderTest_Add_Twice_Throws()
    {
        using var writer = CreateWriter();
        var header = writer.Header;
        header.Add("A");

        var e = Assert.ThrowsException<ArgumentException>(() => header.Add("A"));
        Assert.AreEqual("Column name 'A' already exists (Parameter 'colName')", e.Message);
    }

    [TestMethod]
    public void SepWriterHeaderTest_Add_After_Written_Throws()
    {
        using var writer = CreateWriter();
        var header = writer.Header;
        header.Add("A");
        header.Write();

        var e = Assert.ThrowsException<InvalidOperationException>(() => header.Add("B"));
        Assert.AreEqual("Cannot add column name 'B since header or first row already written.", e.Message);
    }

    [TestMethod]
    public void SepWriterHeaderTest_Add_After_Skipped_Throws()
    {
        using var writer = Sep.New(';').Writer(o => o with { WriteHeader = false }).ToText();
        var header = writer.Header;
        header.Add("A");
        using (var row = writer.NewRow()) { row["A"].Set("1"); }

        var e = Assert.ThrowsException<InvalidOperationException>(() => header.Add("B"));
        Assert.AreEqual("Cannot add column name 'B since header or first row already written.", e.Message);
    }

    [TestMethod]
    public void SepWriterHeaderTest_DebugView()
    {
        using var writer = CreateWriter();
        var debugView = new SepWriterHeader.DebugView(writer.Header);

        CollectionAssert.AreEqual(Array.Empty<string>(), debugView.ColNames);

        var colNames = new string[] { "A", "B", "C" };
        writer.Header.Add(colNames);

        CollectionAssert.AreEqual(colNames, debugView.ColNames);
    }


    static SepWriter CreateWriter() =>
        Sep.New(';').Writer().ToText();
}
