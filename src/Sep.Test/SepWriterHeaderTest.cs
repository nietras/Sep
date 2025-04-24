using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterHeaderTest
{
    // SepWriterHeader tests are all done through SepWriter

    [TestMethod]
    public async ValueTask SepWriterHeaderTest_DebuggerDisplay_WriteHeader_true()
    {
        var expected =
@"A;B;C
";
        {
            using var writer = CreateWriter();
            AssertHeaderAndPrepare(writer, "Not yet written");
            writer.Header.Write();
            AssertHeaderFinal(expected, writer, "Written");
        }
        {
            await using var writer = CreateWriter();
            AssertHeaderAndPrepare(writer, "Not yet written");
            await writer.Header.WriteAsync();
            AssertHeaderFinal(expected, writer, "Written");
        }
    }

    [TestMethod]
    public async ValueTask SepWriterHeaderTest_DebuggerDisplay_WriteHeader_false()
    {
        var expected = @"";
        var options = new SepWriterOptions(new(';')) { WriteHeader = false };
        {
            using var writer = options.ToText();
            AssertHeaderAndPrepare(writer, "To be skipped");
            writer.Header.Write();
            AssertHeaderFinal(expected, writer, "Skipped");
        }
        {
            await using var writer = options.ToText();
            AssertHeaderAndPrepare(writer, "To be skipped");
            await writer.Header.WriteAsync();
            AssertHeaderFinal(expected, writer, "Skipped");
        }
    }

    [TestMethod]
    public async ValueTask SepWriterHeaderTest_Add_Array_Rows_0()
    {
        var colNames = new string[] { "A", "B", "C" };
        var expected =
@"A;B;C
";
        {
            using var writer = CreateWriter();
            writer.Header.Add(colNames);
            // Header written on Dispose
            writer.Dispose();

            Assert.AreEqual(expected, writer.ToString());
        }
        {
            await using var writer = CreateWriter();
            writer.Header.Add(colNames);
            // Header written on Dispose
            await writer.DisposeAsync();

            Assert.AreEqual(expected, writer.ToString());
        }
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
    public void SepWriterHeaderTest_Add_ReadOnlySpan_Params_Rows_0()
    {
        using var writer = CreateWriter();
        writer.Header.Add("A", "B", "C");
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
    public void SepWriterHeaderTest_TryAdd_Once_Is_True()
    {
        using var writer = CreateWriter();
        var header = writer.Header;

        Assert.IsTrue(header.TryAdd("A"));
    }
    
    [TestMethod]
    public void SepWriterHeaderTest_TryAdd_Twice_Is_False()
    {
        using var writer = CreateWriter();
        var header = writer.Header;
        header.Add("A");

        Assert.IsFalse(header.TryAdd("A"));
    }
    
    [TestMethod]
    public void SepWriterHeaderTest_TryAdd_After_Written_Is_False()
    {
        using var writer = CreateWriter();
        var header = writer.Header;
        header.Add("A");
        header.Write();
        
        Assert.IsFalse(header.TryAdd("B"));
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

    static void AssertHeaderAndPrepare(SepWriter writer, string state)
    {
        Assert.AreEqual($"Count = 0 State = '{state}'", writer.Header.DebuggerDisplay);

        var colNames = new string[] { "A", "B", "C" };
        writer.Header.Add(colNames);

        Assert.AreEqual($"Count = 3 State = '{state}'", writer.Header.DebuggerDisplay);
    }

    static void AssertHeaderFinal(string expected, SepWriter writer, string state)
    {
        Assert.AreEqual($"Count = 3 State = '{state}'", writer.Header.DebuggerDisplay);
        Assert.AreEqual(expected, writer.ToString());
    }
}
