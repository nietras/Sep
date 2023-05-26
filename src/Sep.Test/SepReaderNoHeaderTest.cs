using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderNoHeaderTest
{
    [TestMethod]
    public void SepReaderNoHeaderTest_Rows_0_NewLine()
    {
        var text = Environment.NewLine;
        using var reader = Sep.Reader().FromText(text);
        AssertState(reader, isEmpty: false, hasHeader: true, hasRows: false);
        Assert.AreEqual(1, reader.Header.ColNames.Count);
        Assert.IsFalse(reader.MoveNext());
    }

    [TestMethod]
    public void SepReaderNoHeaderTest_Rows_1_NewLine()
    {
        var text = Environment.NewLine;
        using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(text);
        AssertState(reader, isEmpty: false, hasHeader: false, hasRows: true);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(1, reader.Current.ColCount);
    }

    [TestMethod]
    public void SepReaderNoHeaderTest_Enumerate_Empty()
    {
        var text = string.Empty;
        var expected = Array.Empty<(string c1, string c2, string c3)>();
        AssertEnumerate(text, expected, isEmpty: true, hasHeader: false, hasRows: false);
    }

    [TestMethod]
    public void SepReaderNoHeaderTest_Enumerate_Rows_1()
    {
        var text = """
                   10;A;20.1
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "A", "20.1"),
        };
        AssertEnumerate(text, expected);
    }

    [TestMethod]
    public void SepReaderNoHeaderTest_Enumerate_Rows_2()
    {
        var text = """
                   10;A;20.1
                   11;B;20.2
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "A", "20.1"),
            ("11", "B", "20.2"),
        };
        AssertEnumerate(text, expected);
    }

    [TestMethod]
    public void SepReaderNoHeaderTest_Enumerate_Rows_2_NewLineAtEnd()
    {
        var text = """
                   10;A;20.1
                   11;B;20.2

                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "A", "20.1"),
            ("11", "B", "20.2"),
        };
        AssertEnumerate(text, expected);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Quotes_Rows_1()
    {
        var text = """
                   10;"A;";20";"11
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "\"A;\"", "20\";\"11"),
        };
        AssertEnumerate(text, expected);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Quotes_Rows_2()
    {
        var text = """
                   10;"A;";20";"11
                   "11";";"B;"20;00"
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "\"A;\"", "20\";\"11"),
            ("\"11\"", "\";\"B", "\"20;00\""),
        };
        AssertEnumerate(text, expected);
    }

    [TestMethod]
    public void SepReaderNoHeaderTest_Enumerate_Quotes_Rows_2_NewLineAtEnd()
    {
        var text = """
                   10;"A;";20";"11
                   "11";";"B;"20;00"

                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "\"A;\"", "20\";\"11"),
            ("\"11\"", "\";\"B", "\"20;00\""),
        };
        AssertEnumerate(text, expected);
    }

    [DataTestMethod]
    [DataRow("""
             C1;C2
             123
             """,
             """
             Found 1 column(s) on row 1:'123'
             Expected 2 column(s) matching header/first row 'C1;C2'
             """)]
    [DataRow("""
             C1;C2

             1;2
             """,
             """
             Found 1 column(s) on row 1:''
             Expected 2 column(s) matching header/first row 'C1;C2'
             """)]
    [DataRow("""
             C1;C2;C3
             1;2;3
             4;5
             """,
             """
             Found 2 column(s) on row 2:'4;5'
             Expected 3 column(s) matching header/first row 'C1;C2;C3'
             """)]
    [DataRow("""
             C1;C2;C3
             4;5
             1;2;3
             """,
             """
             Found 2 column(s) on row 1:'4;5'
             Expected 3 column(s) matching header/first row 'C1;C2;C3'
             """)]
    [DataRow("""
             C1

             4;5
             """,
             """
             Found 2 column(s) on row 2:'4;5'
             Expected 1 column(s) matching header/first row 'C1'
             """)]
    [DataRow("""
             C1;C2
             4;5
             1;2;3
             """,
             """
             Found 3 column(s) on row 2:'1;2;3'
             Expected 2 column(s) matching header/first row 'C1;C2'
             """)]
    [DataRow("""
             C1;C2
             4";"5
             1;2;3
             """,
             """
             Found 1 column(s) on row 1:'4";"5'
             Expected 2 column(s) matching header/first row 'C1;C2'
             """)]
    public void SepReaderNoHeaderTest_ColumnCountMismatch(string text, string message)
    {
        var e = Assert.ThrowsException<InvalidDataException>(() =>
        {
            using var reader = Sep.Reader().FromText(text);
            foreach (var readRow in reader)
            { }
        });
        Assert.AreEqual(message, e.Message);
    }

    static void AssertEnumerate(string text, (string c1, string c2, string c3)[] expected,
        bool isEmpty = false, bool hasHeader = false, bool hasRows = true)
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(text);

        var actual = Enumerate(reader).ToArray();

        AssertState(reader, isEmpty, hasHeader, hasRows);
        AssertHeader(reader.Header);
        CollectionAssert.AreEqual(expected, actual);
    }

    static void AssertState(SepReader reader, bool isEmpty, bool hasHeader, bool hasRows)
    {
        Assert.AreEqual(isEmpty, reader.IsEmpty, nameof(reader.IsEmpty));
        Assert.AreEqual(hasHeader, reader.HasHeader, nameof(reader.IsEmpty));
        Assert.AreEqual(hasRows, reader.HasRows, nameof(reader.HasRows));
    }

    static void AssertHeader(SepHeader header)
    {
        Assert.AreEqual(0, header.ColNames.Count);
        Assert.AreEqual(true, header.IsEmpty);
    }

    static IEnumerable<(string c1, string c2, string c3)> Enumerate(SepReader reader)
    {
        foreach (var row in reader)
        {
            yield return (row[0].ToString(),
                          row[1].ToString(),
                          row[2].ToString());
        }
    }
}
