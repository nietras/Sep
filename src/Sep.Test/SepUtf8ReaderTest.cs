using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUtf8ReaderTest
{
    [TestMethod]
    public void SepUtf8ReaderTest_CanCreate()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);
        Assert.IsNotNull(reader);
        Assert.IsFalse(reader.IsEmpty);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Spec()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);
        var spec = reader.Spec;
        Assert.AreEqual(';', spec.Sep.Separator);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_FromFile()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(tempFile, Encoding.UTF8.GetBytes("A;B;C\n1;2;3"));
            using var reader = Sep.Utf8Reader().FromFile(tempFile);
            Assert.IsNotNull(reader);
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
    public void SepUtf8ReaderTest_Header()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        Assert.IsTrue(reader.HasHeader);
        Assert.IsTrue(reader.HasRows);

        var header = reader.Header;
        Assert.HasCount(3, header.ColNames);
        Assert.AreEqual("A", header.ColNames[0]);
        Assert.AreEqual("B", header.ColNames[1]);
        Assert.AreEqual("C", header.ColNames[2]);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ReadRow()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual(3, row.ColCount);
        Assert.AreEqual("1", row.ToString(0));
        Assert.AreEqual("2", row.ToString(1));
        Assert.AreEqual("3", row.ToString(2));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ReadRowByteSpans()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        var col0 = row[0];
        var col1 = row[1];
        var col2 = row[2];

        Assert.AreEqual("1", Encoding.UTF8.GetString(col0));
        Assert.AreEqual("2", Encoding.UTF8.GetString(col1));
        Assert.AreEqual("3", Encoding.UTF8.GetString(col2));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ReadMultipleRows()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3\n4;5;6\n7;8;9");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        var rowCount = 0;
        foreach (var row in reader)
        {
            rowCount++;
            Assert.AreEqual(3, row.ColCount);
        }

        Assert.AreEqual(3, rowCount);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ReadByColumnName()
    {
        var utf8Text = Encoding.UTF8.GetBytes("Name;Age;City\nAlice;30;NYC\nBob;25;LA");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual("Alice", row.ToString(0));
        Assert.AreEqual("30", row.ToString(1));
        Assert.AreEqual("NYC", row.ToString(2));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_EmptyFile()
    {
        var utf8Text = Encoding.UTF8.GetBytes("");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        Assert.IsTrue(reader.IsEmpty);
        Assert.IsFalse(reader.HasHeader);
        Assert.IsFalse(reader.HasRows);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_NoHeader()
    {
        var utf8Text = Encoding.UTF8.GetBytes("1;2;3\n4;5;6");
        using var reader = Sep.Utf8Reader(o => o with { HasHeader = false }).FromUtf8(utf8Text);

        Assert.IsFalse(reader.HasHeader);
        Assert.IsTrue(reader.HasRows);

        var rowCount = 0;
        foreach (var row in reader)
        {
            rowCount++;
            Assert.AreEqual(3, row.ColCount);
        }

        Assert.AreEqual(2, rowCount);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_QuotedValues()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n\"Hello, World\";\"Test;Value\";Normal");
        using var reader = Sep.Utf8Reader(o => o with { Unescape = true }).FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual("Hello, World", row.ToString(0));
        Assert.AreEqual("Test;Value", row.ToString(1));
        Assert.AreEqual("Normal", row.ToString(2));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_CommaSeparator()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A,B,C\n1,2,3");
        using var reader = Sep.Utf8Reader(o => o with { Sep = Sep.New(',') }).FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual("1", row.ToString(0));
        Assert.AreEqual("2", row.ToString(1));
        Assert.AreEqual("3", row.ToString(2));
    }

    [TestMethod]
    public async Task SepUtf8ReaderTest_Async()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = await Sep.Utf8Reader().FromUtf8Async(utf8Text);

        Assert.IsTrue(await reader.MoveNextAsync());
        var row = reader.Current;

        Assert.AreEqual("1", row.ToString(0));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Utf8Characters()
    {
        var utf8Text = Encoding.UTF8.GetBytes("Name;City\nAlice;münchen\nBob;København\nCarol;日本");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        var names = new System.Collections.Generic.List<string>();
        var cities = new System.Collections.Generic.List<string>();

        foreach (var row in reader)
        {
            names.Add(row.ToString(0));
            cities.Add(row.ToString(1));
        }

        Assert.HasCount(3, names);
        Assert.AreEqual("Alice", names[0]);
        Assert.AreEqual("münchen", cities[0]);
        Assert.AreEqual("København", cities[1]);
        Assert.AreEqual("日本", cities[2]);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_RowProperties()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3\n4;5;6");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual(3, row.ColCount);
        var span = row.Span;
        Assert.IsGreaterThan(0, span.Length);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_HeaderIndexOf()
    {
        var utf8Text = Encoding.UTF8.GetBytes("Name;Age;City\nAlice;30;NYC");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        var header = reader.Header;
        Assert.AreEqual(0, header.IndexOf("Name"));
        Assert.AreEqual(1, header.IndexOf("Age"));
        Assert.AreEqual(2, header.IndexOf("City"));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_HeaderTryIndexOf()
    {
        var utf8Text = Encoding.UTF8.GetBytes("Name;Age;City\nAlice;30;NYC");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        var header = reader.Header;
        Assert.IsTrue(header.TryIndexOf("Name", out var index0));
        Assert.AreEqual(0, index0);

        Assert.IsTrue(header.TryIndexOf("Age", out var index1));
        Assert.AreEqual(1, index1);

        Assert.IsFalse(header.TryIndexOf("NotExists", out _));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_EmptyColumns()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n;;");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual(3, row.ColCount);
        Assert.AreEqual("", row.ToString(0));
        Assert.AreEqual("", row.ToString(1));
        Assert.AreEqual("", row.ToString(2));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_MultilineValues()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B\n\"Line1\nLine2\";Value2");
        using var reader = Sep.Utf8Reader(o => o with { Unescape = true }).FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual(2, row.ColCount);
        Assert.AreEqual("Line1\nLine2", row.ToString(0));
        Assert.AreEqual("Value2", row.ToString(1));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TrimOuter()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B\n  value1  ;  value2  ");
        using var reader = Sep.Utf8Reader(o => o with { Trim = SepTrim.Outer }).FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual("value1", row.ToString(0));
        Assert.AreEqual("value2", row.ToString(1));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_DisableQuotesParsing()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B\n\"value1\";\"value2\"");
        using var reader = Sep.Utf8Reader(o => o with { DisableQuotesParsing = true }).FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        // With quotes parsing disabled, quotes should be preserved
        Assert.AreEqual("\"value1\"", row.ToString(0));
        Assert.AreEqual("\"value2\"", row.ToString(1));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TabSeparator()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A\tB\tC\n1\t2\t3");
        using var reader = Sep.Utf8Reader(o => o with { Sep = Sep.New('\t') }).FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual("1", row.ToString(0));
        Assert.AreEqual("2", row.ToString(1));
        Assert.AreEqual("3", row.ToString(2));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_LargeFile()
    {
        var sb = new StringBuilder();
        sb.AppendLine("A;B;C");
        for (int i = 0; i < 1000; i++)
        {
            sb.AppendLine($"{i};{i * 2};{i * 3}");
        }

        var utf8Text = Encoding.UTF8.GetBytes(sb.ToString());
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        int rowCount = 0;
        foreach (var row in reader)
        {
            Assert.AreEqual(3, row.ColCount);
            rowCount++;
        }

        Assert.AreEqual(1000, rowCount);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_WindowsLineEndings()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\r\n1;2;3\r\n4;5;6");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        int rowCount = 0;
        foreach (var row in reader)
        {
            Assert.AreEqual(3, row.ColCount);
            rowCount++;
        }

        Assert.AreEqual(2, rowCount);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_MacLineEndings()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\r1;2;3\r4;5;6");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        int rowCount = 0;
        foreach (var row in reader)
        {
            Assert.AreEqual(3, row.ColCount);
            rowCount++;
        }

        Assert.AreEqual(2, rowCount);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_MixedLineEndings()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3\r\n4;5;6\r7;8;9");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        int rowCount = 0;
        foreach (var row in reader)
        {
            Assert.AreEqual(3, row.ColCount);
            rowCount++;
        }

        Assert.AreEqual(3, rowCount);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_SpecialCharacters()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n!@#$%^&*();(){}[];:',.<>?/\\|`~");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        Assert.AreEqual("!@#$%^&*()", row.ToString(0));
        Assert.AreEqual("(){}[]", row.ToString(1));
        Assert.AreEqual(":',.<>?/\\|`~", row.ToString(2));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_UnicodeEdgeCases()
    {
        // Test various Unicode edge cases
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n😀;🎉;🚀\n∑;∫;∂");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        var rows = new System.Collections.Generic.List<string[]>();
        foreach (var row in reader)
        {
            rows.Add([row.ToString(0), row.ToString(1), row.ToString(2)]);
        }

        Assert.HasCount(2, rows);
        Assert.AreEqual("😀", rows[0][0]);
        Assert.AreEqual("🎉", rows[0][1]);
        Assert.AreEqual("🚀", rows[0][2]);
        Assert.AreEqual("∑", rows[1][0]);
        Assert.AreEqual("∫", rows[1][1]);
        Assert.AreEqual("∂", rows[1][2]);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TrailingNewline()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3\n");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        int rowCount = 0;
        foreach (var row in reader)
        {
            rowCount++;
        }

        Assert.AreEqual(1, rowCount);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_FromStream()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var stream = new MemoryStream(utf8Text);
        using var reader = Sep.Utf8Reader().From(stream);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual("1", row.ToString(0));
    }

    [TestMethod]
    public async Task SepUtf8ReaderTest_FromUtf8Async()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = await Sep.Utf8Reader().FromUtf8Async(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual("1", row.ToString(0));
    }

    [TestMethod]
    public async Task SepUtf8ReaderTest_FromFileAsync()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(tempFile, Encoding.UTF8.GetBytes("A;B;C\n1;2;3"));
            using var reader = await Sep.Utf8Reader().FromFileAsync(tempFile);

            Assert.IsTrue(reader.MoveNext());
            var row = reader.Current;
            Assert.AreEqual("1", row.ToString(0));
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
    public async Task SepUtf8ReaderTest_FromStreamAsync()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var stream = new MemoryStream(utf8Text);
        using var reader = await Sep.Utf8Reader().FromAsync(stream);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual("1", row.ToString(0));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColumnByNameByteSpan()
    {
        var utf8Text = Encoding.UTF8.GetBytes("Name;Age\nAlice;30");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        // Access by name returns byte span
        var nameBytes = row["Name"];
        var ageBytes = row["Age"];

        Assert.AreEqual("Alice", Encoding.UTF8.GetString(nameBytes));
        Assert.AreEqual("30", Encoding.UTF8.GetString(ageBytes));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_EmptyHeader()
    {
        var emptyHeader = SepUtf8ReaderHeader.Empty;
        Assert.IsEmpty(emptyHeader.ColNames);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_DebuggerDisplay()
    {
        var utf8Text = Encoding.UTF8.GetBytes("A;B;C\n1;2;3");
        using var reader = Sep.Utf8Reader().FromUtf8(utf8Text);

        // Access DebuggerDisplay through reflection since it's internal
        var debugDisplay = reader.GetType().GetProperty("DebuggerDisplay",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(reader);

        Assert.IsNotNull(debugDisplay);
        Assert.IsInstanceOfType(debugDisplay, typeof(string));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_OptionsDefaultConstructor()
    {
        var options = new SepUtf8ReaderOptions();
        Assert.IsNull(options.Sep);
        Assert.IsTrue(options.HasHeader);
    }
}
