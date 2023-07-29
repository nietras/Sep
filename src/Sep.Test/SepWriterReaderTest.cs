using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterReaderTest
{
    [DataTestMethod]
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
}
