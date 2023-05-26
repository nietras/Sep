using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderTest
{
    [TestMethod]
    public void SepReaderTest_CreateFromStreamReader()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("A;B"));
        using var reader = Sep.Reader().From(stream);
        // 3 + 128 => Pow2
        Assert.AreEqual(256, reader.CharsLength);
    }

    [TestMethod]
    public void SepReaderTest_Spec()
    {
        using var reader = Sep.Reader().FromText("a|b|c\n,;|\t,|;;");
        var spec = reader.Spec;
        Assert.AreEqual('|', spec.Sep.Separator);
        Assert.AreSame(SepDefaults.CultureInfo, spec.CultureInfo);
    }

    [TestMethod]
    public void SepReaderTest_CreateWithPredefinedSeparator()
    {
        using var reader = Sep.New('|').Reader().FromText("a|b|c\n,;|\t,|;;");
        Assert.AreEqual('|', reader.Spec.Sep.Separator);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Empty()
    {
        var text = string.Empty;
        var expected = Array.Empty<(string c1, string c2, string c3)>();
        AssertEnumerate(text, expected, isEmpty: true, hasHeader: false, hasRows: false);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_0()
    {
        var text = "C1;C2;C3";
        var expected = Array.Empty<(string c1, string c2, string c3)>();
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: false);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_0_NewLineAtEnd()
    {
        var text = """
                   C1;C2;C3

                   """;
        var expected = Array.Empty<(string c1, string c2, string c3)>();
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: false);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_1()
    {
        var text = """
                   C1;C2;C3
                   ;;
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("", "", ""),
        };
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: true);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_2()
    {
        var text = """
                   C1;C2;C3
                   10;A;20.1
                   11;B;20.2
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "A", "20.1"),
            ("11", "B", "20.2"),
        };
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: true);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_2_NewLineAtEnd()
    {
        var text = """
                   C1;C2;C3
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
    public void SepReaderTest_Enumerate_Quotes_Rows_0()
    {
        var text = "\"C1;;;\";\"C;\"2;\";;C3\"";
        var expected = Array.Empty<(string c1, string c2, string c3)>();
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: false,
                        "\"C1;;;\"", "\"C;\"2", "\";;C3\"");
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Quotes_Rows_2()
    {
        var text = """
                   "C1;;;";"C;"2;";;C3"
                   10;"A;";20";"11
                   "11";";"B;"20;00"
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "\"A;\"", "20\";\"11"),
            ("\"11\"", "\";\"B", "\"20;00\""),
        };
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: true,
                        "\"C1;;;\"", "\"C;\"2", "\";;C3\"");
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Quotes_Rows_2_NewLineAtEnd()
    {
        var text = """
                   C1;C2;C3
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

    // TODO: Need test of quotes at end

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
    public void SepReaderTest_ColumnCountMismatch(string text, string message)
    {
        var e = Assert.ThrowsException<InvalidDataException>(() =>
        {
            using var reader = Sep.Reader().FromText(text);
            foreach (var readRow in reader)
            { }
        });
        Assert.AreEqual(message, e.Message);
    }

    [TestMethod]
    public void SepReaderTest_HandleReallyLongHeader()
    {
#if SEPREADERTRACE // Don't run really long with tracing enabled 😅
        const int lengthA = 1267;
        const int lengthB = 237;
#else
        const int lengthA = 1267 + 64 * 1024;
        const int lengthB = 237;
#endif
        var text = $"{new string('a', lengthA)};{new string('b', lengthB)}";

        using var reader = Sep.Reader().FromText(text);

        var colNames = reader.Header.ColNames;
        Assert.AreEqual(lengthA, colNames[0].Length);
        Assert.AreEqual(lengthB, colNames[1].Length);
    }

    [TestMethod]
    public void SepReaderTest_MaximumColCount()
    {
        var maxColCount = SepReader._colEndsMaximumLength;
        var text = $"{new string(';', maxColCount - 1)}";
        using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(maxColCount, row.ColCount);
    }

    [TestMethod]
    public void SepReaderTest_ExceedingMaximumColCount_Throws()
    {
        var maxColCount = SepReader._colEndsMaximumLength;
        var text = new string(';', maxColCount);
        var e = Assert.ThrowsException<NotSupportedException>(() =>
            Sep.Reader(o => o with { HasHeader = false }).FromText(text));
        Assert.AreEqual($"Col count has reached maximum supported count of {maxColCount}.", e.Message);
    }

#if !DEBUG // Causes OOMs in Debug due to tracing
    [TestMethod]
    public void SepReaderTest_TooLongRow_Throws()
    {
        var maxLength = SepCharPosition.MaxLength + 1;
        var text = new string('a', maxLength);
        var e = Assert.ThrowsException<NotSupportedException>(() =>
            Sep.Reader().FromText(text));
        Assert.AreEqual($"Buffer or row has reached maximum supported length of 16777216. If no such row should exist ensure quotes \" are terminated.", e.Message);
    }
#endif

    [TestMethod]
    public void SepReaderTest_Extensions_FromFile()
    {
        const string text = """
                            A;B
                            1;2
                            """;
        const string fileName = nameof(SepReaderTest_Extensions_FromFile) + ".csv";
        File.WriteAllText(fileName, text);

        using var reader = Sep.Auto.Reader().FromFile(fileName);

        Assert.AreEqual(true, reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(1, row["A"].Parse<int>());
    }

    static void AssertEnumerate(string text, (string c1, string c2, string c3)[] expected,
        bool isEmpty = false, bool hasHeader = true, bool hasRows = true,
        string colName1 = "C1", string colName2 = "C2", string colName3 = "C3")
    {
        using var reader = Sep.Reader().FromText(text);

        var actual = Enumerate(reader, colName1, colName2, colName3).ToArray();

        AssertState(reader, isEmpty, hasHeader, hasRows);
        AssertHeader(reader.Header, colName1, colName2, colName3);
        CollectionAssert.AreEqual(expected, actual);
    }

    static void AssertState(SepReader reader, bool isEmpty, bool hasHeader, bool hasRows)
    {
        Assert.AreEqual(isEmpty, reader.IsEmpty, nameof(reader.IsEmpty));
        Assert.AreEqual(hasHeader, reader.HasHeader, nameof(reader.IsEmpty));
        Assert.AreEqual(hasRows, reader.HasRows, nameof(reader.HasRows));
    }

    static void AssertHeader(SepHeader header, string colName1, string colName2, string colName3)
    {
        if (header.ColNames.Count > 0)
        {
            Assert.AreEqual(3, header.ColNames.Count);
            Assert.AreEqual(0, header.IndexOf(colName1));
            Assert.AreEqual(1, header.IndexOf(colName2));
            Assert.AreEqual(2, header.IndexOf(colName3));
        }
    }

    static IEnumerable<(string c1, string c2, string c3)> Enumerate(SepReader reader,
        string colName1, string colName2, string colName3)
    {
        foreach (var row in reader)
        {
            yield return (row[colName1].ToString(),
                          row[colName2].ToString(),
                          row[colName3].ToString());
        }
    }
}
