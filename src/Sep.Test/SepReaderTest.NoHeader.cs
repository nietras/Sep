using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

public partial class SepReaderTest
{
    [TestMethod]
    public async ValueTask SepReaderTest_NoHeader_From_Empty()
    {
        await FromSyncAsync(string.Empty, options: new() { HasHeader = false }, reader =>
        {
            AssertState(reader, isEmpty: true, hasHeader: false, hasRows: false);
            Assert.IsFalse(reader.MoveNext());
        });
    }

    [TestMethod]
    public async ValueTask SepReaderTest_NoHeader_From_NewLine()
    {
        await FromSyncAsync(Environment.NewLine, options: new() { HasHeader = false }, reader =>
        {
            AssertState(reader, isEmpty: false, hasHeader: false, hasRows: true);
            Assert.IsTrue(reader.MoveNext());
            Assert.AreEqual(1, reader.Current.ColCount);
        });
    }

    [TestMethod]
    public async ValueTask SepReaderTest_NoHeader_Enumerate_Empty()
    {
        var text = string.Empty;
        var expected = Array.Empty<Values>();
        await AssertEnumerateSyncAsync(text, expected, isEmpty: true, hasHeader: false, hasRows: false);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_NoHeader_Enumerate_Rows_1()
    {
        var text = """
                   10;A;20.1
                   """;
        var expected = new Values[]
        {
            new("10", "A", "20.1"),
        };
        await AssertEnumerateSyncAsync(text, expected, hasHeader: false);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_NoHeader_Enumerate_Rows_2()
    {
        var text = """
                   10;A;20.1
                   11;B;20.2
                   """;
        var expected = new Values[]
        {
            new("10", "A", "20.1"),
            new("11", "B", "20.2"),
        };
        await AssertEnumerateSyncAsync(text, expected, hasHeader: false);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_NoHeader_Enumerate_Rows_2_NewLineAtEnd()
    {
        var text = """
                   10;A;20.1
                   11;B;20.2

                   """;
        var expected = new Values[]
        {
            new("10", "A", "20.1"),
            new("11", "B", "20.2"),
        };
        await AssertEnumerateSyncAsync(text, expected, hasHeader: false);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_NoHeader_Enumerate_Quotes_Rows_1()
    {
        var text = """
                   10;"A;";20";"11
                   """;
        var expected = new Values[]
        {
            new("10", "\"A;\"", "20\";\"11"),
        };
        await AssertEnumerateSyncAsync(text, expected, hasHeader: false);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_NoHeader_Enumerate_Quotes_Rows_2()
    {
        var text = """
                   10;"A;";20";"11
                   "11";";"B;"20;00"
                   """;
        var expected = new Values[]
        {
            new("10", "\"A;\"", "20\";\"11"),
            new("\"11\"", "\";\"B", "\"20;00\""),
        };
        await AssertEnumerateSyncAsync(text, expected, hasHeader: false);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_NoHeader_Enumerate_Quotes_Rows_2_NewLineAtEnd()
    {
        var text = """
                   10;"A;";20";"11
                   "11";";"B;"20;00"

                   """;
        var expected = new Values[]
        {
            new("10", "\"A;\"", "20\";\"11"),
            new("\"11\"", "\";\"B", "\"20;00\""),
        };
        await AssertEnumerateSyncAsync(text, expected, hasHeader: false);
    }

    [DataTestMethod]
    [DynamicData(nameof(ColCountMismatchData))]
    public void SepReaderTest_NoHeader_ColumnCountMismatch(string text, string message, int[] _)
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
    public void SepReaderTest_NoHeader_ColumnCountMismatch_DisableColCountCheck(
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
    public void SepReaderTest_NoHeader_ColsInitialLength()
    {
        var initialColCountCapacity = SepReader.ColEndsInitialLength - 1; // -1 since col ends is 1 longer due to having row start
        var text = new string(';', initialColCountCapacity - 1);
        using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(initialColCountCapacity, row.ColCount);
    }

    [TestMethod]
    public void SepReaderTest_NoHeader_ExceedingColsInitialLength_WorksByDoublingCapacity()
    {
        var initialColCountCapacity = SepReader.ColEndsInitialLength;
        var text = new string(';', initialColCountCapacity - 1);
        using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(text);
        Assert.AreEqual(initialColCountCapacity * 2, reader._colEndsOrColInfos.Length);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(initialColCountCapacity, row.ColCount);
    }
}
