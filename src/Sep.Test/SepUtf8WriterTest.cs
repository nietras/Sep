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

    [TestMethod]
    public void SepUtf8WriterTest_WriteMultipleRowsByIndex()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("A");
            writer.Header.Add("B");
            writer.Header.Add("C");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row[0].Set("1");
                row[1].Set("2");
                row[2].Set("3");
            }

            using (var row = writer.NewRow())
            {
                row[0].Set("4");
                row[1].Set("5");
                row[2].Set("6");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(3, lines);
        Assert.AreEqual("A;B;C", lines[0]);
        Assert.AreEqual("1;2;3", lines[1]);
        Assert.AreEqual("4;5;6", lines[2]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_Format()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("Int");
            writer.Header.Add("Double");
            writer.Header.Add("Decimal");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Int"].Format(42);
                row["Double"].Format(3.14159);
                row["Decimal"].Format(123.456m);
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.Contains("42", lines[1]);
        Assert.Contains("3.14159", lines[1]);
        Assert.Contains("123.456", lines[1]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_EscapeWithSeparator()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer(o => o with { Escape = true }).To(stream))
        {
            writer.Header.Add("Data");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Data"].Set("Value;With;Semicolons");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.Contains("\"Value;With;Semicolons\"", lines[1]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_EscapeWithNewlines()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer(o => o with { Escape = true }).To(stream))
        {
            writer.Header.Add("Data");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Data"].Set("Line1\nLine2");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Contains("\"Line1\nLine2\"", result);
    }

    [TestMethod]
    public void SepUtf8WriterTest_LargeFile()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("Index");
            writer.Header.Add("Value");
            writer.Header.Write();

            for (int i = 0; i < 1000; i++)
            {
                using var row = writer.NewRow();
                row["Index"].Set($"{i}");
                row["Value"].Set($"{i * 2}");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(1001, lines); // 1 header + 1000 data rows
    }

    [TestMethod]
    public void SepUtf8WriterTest_EmptyValues()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("A");
            writer.Header.Add("B");
            writer.Header.Add("C");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["A"].Set("");
                row["B"].Set("");
                row["C"].Set("");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.AreEqual("A;B;C", lines[0]);
        Assert.AreEqual(";;", lines[1]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_SpecialCharacters()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("Special");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Special"].Set("!@#$%^&*()");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Contains("!@#$%^&*()", result);
    }

    [TestMethod]
    public void SepUtf8WriterTest_UnicodeEmojis()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("Emoji");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Emoji"].Set("😀🎉🚀");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Contains("😀🎉🚀", result);
    }

    [TestMethod]
    public void SepUtf8WriterTest_TabSeparator()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer(o => o with { Sep = Sep.New('\t') }).To(stream))
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
        Assert.AreEqual("A\tB", lines[0]);
        Assert.AreEqual("1\t2", lines[1]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_PipeSeparator()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer(o => o with { Sep = Sep.New('|') }).To(stream))
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
        Assert.AreEqual("A|B", lines[0]);
        Assert.AreEqual("1|2", lines[1]);
    }

    [TestMethod]
    public void SepUtf8WriterTest_MultipleHeaderColumns()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            for (int i = 0; i < 10; i++)
            {
                writer.Header.Add($"Col{i}");
            }
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                for (int i = 0; i < 10; i++)
                {
                    row[$"Col{i}"].Set($"Value{i}");
                }
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        var lines = result.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.Contains("Col0", lines[0]);
        Assert.Contains("Col9", lines[0]);
        Assert.Contains("Value0", lines[1]);
        Assert.Contains("Value9", lines[1]);
    }

    [TestMethod]
    public async Task SepUtf8WriterTest_ToFileAsync()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            await using (var writer = await Sep.Utf8Writer().ToFileAsync(tempFile))
            {
                writer.Header.Add("Test");
                writer.Header.Write();

                using (var row = writer.NewRow())
                {
                    row["Test"].Set("Value");
                }
            }

            Assert.IsTrue(File.Exists(tempFile));
            var result = File.ReadAllText(tempFile);
            Assert.Contains("Test", result);
            Assert.Contains("Value", result);
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
    public async Task SepUtf8WriterTest_ToStreamAsync()
    {
        using var stream = new MemoryStream();
        await using (var writer = await Sep.Utf8Writer().ToAsync(stream))
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
    public void SepUtf8WriterTest_ToUtf8Bytes_ThrowsNotSupported()
    {
        using var stream = new MemoryStream();
        using var writer = Sep.Utf8Writer().To(stream);
        Assert.ThrowsExactly<NotSupportedException>(() => writer.ToUtf8Bytes());
    }

    [TestMethod]
    public void SepUtf8WriterTest_SetCharSpan()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("Test");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Test"].Set("Value".AsSpan());
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Contains("Test", result);
        Assert.Contains("Value", result);
    }

    [TestMethod]
    public void SepUtf8WriterTest_FormatWithFormatString()
    {
        using var stream = new MemoryStream();
        using (var writer = Sep.Utf8Writer().To(stream))
        {
            writer.Header.Add("Value");
            writer.Header.Write();

            using (var row = writer.NewRow())
            {
                row["Value"].Format(123.456, "F2");
            }
        }

        var result = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Contains("123.46", result);
    }

    [TestMethod]
    public void SepUtf8WriterTest_DebuggerDisplay()
    {
        using var stream = new MemoryStream();
        using var writer = Sep.Utf8Writer().To(stream);

        // Access DebuggerDisplay through reflection since it's internal
        var debugDisplay = writer.GetType().GetProperty("DebuggerDisplay",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(writer);

        Assert.IsNotNull(debugDisplay);
        Assert.IsInstanceOfType(debugDisplay, typeof(string));
    }

    [TestMethod]
    public void SepUtf8WriterTest_OptionsDefaultConstructor()
    {
        var options = new SepUtf8WriterOptions();
        Assert.AreEqual(';', options.Sep.Separator);
        Assert.IsTrue(options.WriteHeader);
    }
}
