using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUtf8WriterTest
{
    [TestMethod]
    public void SepUtf8WriterTest_CanCreate()
    {
        using var writer = Sep.Utf8Writer().ToUtf8();
        Assert.IsNotNull(writer);
    }

    [TestMethod]
    public void SepUtf8WriterTest_Spec()
    {
        using var writer = Sep.Utf8Writer().ToUtf8();
        var spec = writer.Spec;
        Assert.AreEqual(';', spec.Sep.Separator);
    }

    [TestMethod]
    public void SepUtf8WriterTest_ToFile()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            using (var writer = Sep.Utf8Writer().ToFile(tempFile))
            {
                Assert.IsNotNull(writer);
            }
            Assert.IsTrue(File.Exists(tempFile));
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [TestMethod]
    public void SepUtf8WriterTest_AddHeader()
    {
        using var stream = new MemoryStream();
        using var writer = Sep.Utf8Writer().To(stream);
        var header = writer.Header;
        header.Add("Column1");
        header.Add("Column2");
        header.Write();
    }

    [TestMethod]
    public void SepUtf8WriterTest_WriteRow()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            var header = writer.Header;
            header.Add("Name");
            header.Add("Age");
            header.Add("City");
            header.Write();

            using (var row = writer.NewRow())
            {
                row["Name"].Set("Alice");
                row["Age"].Set("30");
                row["City"].Set("NYC");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.AreEqual("Name;Age;City", lines[0]);
        Assert.AreEqual("Alice;30;NYC", lines[1]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_WriteMultipleRows()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("A");
            writer.Header.Add("B");
            writer.Header.Add("C");
            writer.Header.Write();

            for (int i = 1; i <= 3; i++)
            {
                using var row = writer.NewRow();
                row["A"].Set($"{i}");
                row["B"].Set($"{i * 2}");
                row["C"].Set($"{i * 3}");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(4, lines);
        Assert.AreEqual("A;B;C", lines[0]);
        Assert.AreEqual("1;2;3", lines[1]);
        Assert.AreEqual("2;4;6", lines[2]);
        Assert.AreEqual("3;6;9", lines[3]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_WriteByteSpan()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("Data");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                var utf8Bytes = Encoding.UTF8.GetBytes("Hello, World!");
                row["Data"].Set(utf8Bytes.AsSpan());
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.AreEqual("Data", lines[0]);
        Assert.AreEqual("Hello, World!", lines[1]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_NoHeader()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer(o => o with { WriteHeader = false }).To(stream))
        {
            writer.Header.Add("A");
            writer.Header.Add("B");

            using (var row = writer.NewRow())
            {
                row[0].Set("1");
                row[1].Set("2");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(1, lines);
        Assert.AreEqual("1;2", lines[0]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_EscapeQuotes()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer(o => o with { Escape = true }).To(stream))
        {
            writer.Header.Add("Text");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Text"].Set("Hello, \"World\"!");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.Contains("\"\"", lines[1], "Should escape quotes");
    }

    [TestMethod]
    public void SepUtf8WriterTest_CommaSeparator()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer(o => o with { Sep = Sep.New(',') }).To(stream))
        {
            writer.Header.Add("A");
            writer.Header.Add("B");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["A"].Set("1");
                row["B"].Set("2");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.AreEqual("A,B", lines[0]);
        Assert.AreEqual("1,2", lines[1]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_Utf8Characters()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("Name");
            writer.Header.Add("City");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Name"].Set("Alice");
                row["City"].Set("münchen");
            }

            using (var row = writer.NewRow())
            {
                row["Name"].Set("Bob");
                row["City"].Set("København");
            }

            using (var row = writer.NewRow())
            {
                row["Name"].Set("Carol");
                row["City"].Set("日本");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(4, lines);
        Assert.Contains("münchen", lines[1]);
        Assert.Contains("København", lines[2]);
        Assert.Contains("日本", lines[3]);
    }

    [TestMethod]
    public async Task SepUtf8WriterTest_Async()
    {
        using var stream = new MemoryStream();
        await using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("Test");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Test"].Set("Value");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Contains("Test", result);
        Assert.Contains("Value", result);
    }

    [TestMethod]
    public void SepUtf8WriterTest_RoundTrip()
    {
        // Write data
        byte[] data;
        {
            using var stream = new MemoryStream();
            using (var writer = Sep.Utf8Writer().To(stream))
            {
                writer.Header.Add("Name");
                writer.Header.Add("Age");
                writer.Header.Add("City");
                writer.Header.Write();

                using (var row = writer.NewRow())
                {
                    row["Name"].Set("Alice");
                    row["Age"].Set("30");
                    row["City"].Set("NYC");
                }

                using (var row = writer.NewRow())
                {
                    row["Name"].Set("Bob");
                    row["Age"].Set("25");
                    row["City"].Set("LA");
                }
            }
            data = stream.ToArray();
        }

        // Read it back
        using var reader = Sep.Utf8Reader().FromUtf8(data);

        Assert.IsTrue(reader.HasHeader);
        Assert.HasCount(3, reader.Header.ColNames);

        var rowCount = 0;
        var firstRow = true;
        foreach (var row in reader)
        {
            rowCount++;
            if (firstRow)
            {
                Assert.AreEqual("Alice", row.ToString(0));
                Assert.AreEqual("30", row.ToString(1));
                Assert.AreEqual("NYC", row.ToString(2));
                firstRow = false;
            }
            else
            {
                Assert.AreEqual("Bob", row.ToString(0));
                Assert.AreEqual("25", row.ToString(1));
                Assert.AreEqual("LA", row.ToString(2));
            }
        }

        Assert.AreEqual(2, rowCount);
    }
}
