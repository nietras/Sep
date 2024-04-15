using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
    public void SepReaderNoHeaderTest_Enumerate_Quotes_Rows_1()
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
    public void SepReaderNoHeaderTest_Enumerate_Quotes_Rows_2()
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

    internal static IEnumerable<object[]> ColCountMismatchData =>
        SepReaderTest.ColCountMismatchData;

    [DataTestMethod]
    [DynamicData(nameof(ColCountMismatchData))]
    public void SepReaderNoHeaderTest_ColumnCountMismatch(string text, string message, int[] _)
    {
        Contract.Assume(message is not null);
        var e = Assert.ThrowsException<InvalidDataException>(() =>
        {
            using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(text);
            foreach (var readRow in reader)
            { }
        });
        // When no header no first row data is available
        var index = message.LastIndexOf("first row ", StringComparison.OrdinalIgnoreCase);
        message = string.Concat(message.AsSpan(0, index), "first row ''");
        Assert.AreEqual(message, e.Message);
    }

    [DataTestMethod]
    [DynamicData(nameof(ColCountMismatchData))]
    public void SepReaderNoHeaderTest_ColumnCountMismatch_DisableColCountCheck(
        string text, string _, int[] expectedColCounts)
    {
        Contract.Assume(expectedColCounts != null);
        using var reader = Sep.Reader(o => o with { HasHeader = false, DisableColCountCheck = true }).FromText(text);
        var colCountIndex = 0;
        foreach (var readRow in reader)
        {
            Assert.AreEqual(expectedColCounts[colCountIndex], readRow.ColCount);
            for (var colIndex = 0; colIndex < readRow.ColCount; colIndex++)
            {
                // Ensure ToString works, since may be outside SepToString bounds
                Assert.IsNotNull(readRow[colIndex].ToString());
            }
            ++colCountIndex;
        }
    }

    [TestMethod]
    public void SepReaderNoHeaderTest_ColsInitialLength()
    {
        var initialColCountCapacity = SepReader.ColEndsInitialLength - 1; // -1 since col ends is 1 longer due to having row start
        var text = new string(';', initialColCountCapacity - 1);
        using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(initialColCountCapacity, row.ColCount);
    }

    [TestMethod]
    public void SepReaderNoHeaderTest_ExceedingColsInitialLength_WorksByDoublingCapacity()
    {
        var initialColCountCapacity = SepReader.ColEndsInitialLength;
        var text = new string(';', initialColCountCapacity - 1);
        using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(text);
        Assert.AreEqual(initialColCountCapacity * 2, reader._colEndsOrColInfos.Length);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(initialColCountCapacity, row.ColCount);
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

    static void AssertHeader(SepReaderHeader header)
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
