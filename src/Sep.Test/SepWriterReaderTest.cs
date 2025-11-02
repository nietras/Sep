using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterReaderTest
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(117)]
#if !SEPREADERTRACE
    [DataRow(17847)]
#endif
    public void SepWriterReaderTest_EmptyColumn(int rowCount)
    {
        using var writer = Sep.Writer().ToText();
        for (var i = 0; i < rowCount; i++)
        {
            using var writeRow = writer.NewRow();
            writeRow[string.Empty].Set(string.Empty);
        }

        var actualRowCount = 0;
        using var reader = Sep.Reader().FromText(writer.ToString());
        foreach (var readRow in reader)
        {
            Assert.AreEqual(string.Empty, readRow[string.Empty].ToString());
            ++actualRowCount;
        }
        // Assert
        Assert.AreEqual(rowCount, actualRowCount);
    }

    [TestMethod]
    public void SepWriterReaderTest_EscapeUnescapeRoundTrip()
    {
        string[] columnNames = ["A", ";B", "C\n", "\"D\""];
        string[] columnValues = ["\ra1", "b1\r\nb2", ";c1;", "\"d1\"\r\n"];
        using var writer = Sep.Writer(o => o with { Escape = true }).ToText();
        {
            using var writeRow = writer.NewRow();
            writeRow[columnNames].Set(columnValues);
        }

        using var reader = Sep.Reader(o => o with { Unescape = true }).FromText(writer.ToString());
        Assert.IsTrue(reader.HasRows);
        CollectionAssert.AreEqual(columnNames, reader.Header.ColNames.ToArray());
        foreach (var readRow in reader)
        {
            var columnValuesRead = readRow[columnNames].ToStringsArray();
            CollectionAssert.AreEqual(columnValues, columnValuesRead);
        }
    }
}
