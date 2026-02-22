using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUtf8ReaderTest
{
    [TestMethod]
    public void SepUtf8ReaderTest_FromText_Simple()
    {
        var text = "A;B;C\n1;2;3\n4;5;6";
        using var reader = Sep.New(';').Utf8Reader().FromText(text);
        Assert.IsTrue(reader.HasHeader);
        Assert.IsTrue(reader.HasRows);
        Assert.IsFalse(reader.IsEmpty);

        var header = reader.Header;
        Assert.HasCount(3, header.ColNames);
        Assert.AreEqual("A", header.ColNames[0]);
        Assert.AreEqual("B", header.ColNames[1]);
        Assert.AreEqual("C", header.ColNames[2]);

        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(3, row.ColCount);
        Assert.AreEqual("1", row[0].ToString());
        Assert.AreEqual("2", row[1].ToString());
        Assert.AreEqual("3", row[2].ToString());

        Assert.IsTrue(reader.MoveNext());
        row = reader.Current;
        Assert.AreEqual("4", row[0].ToString());
        Assert.AreEqual("5", row[1].ToString());
        Assert.AreEqual("6", row[2].ToString());

        Assert.IsFalse(reader.MoveNext());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_FromBytes()
    {
        var bytes = Encoding.UTF8.GetBytes("A;B\n1;2");
        using var reader = Sep.New(';').Utf8Reader().FromBytes(bytes);
        Assert.IsTrue(reader.HasHeader);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
        Assert.AreEqual("2", reader.Current[1].ToString());
        Assert.IsFalse(reader.MoveNext());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_FromStream()
    {
        var bytes = Encoding.UTF8.GetBytes("A;B\n1;2");
        using var stream = new MemoryStream(bytes);
        using var reader = Sep.New(';').Utf8Reader().From(stream);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_AutoDetectSeparator()
    {
        using var reader = Sep.Utf8Reader().FromText("A;B;C\n1;2;3");
        Assert.AreEqual(';', reader.Spec.Sep.Separator);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_AutoDetectSeparator_Comma()
    {
        using var reader = Sep.Utf8Reader().FromText("A,B,C\n1,2,3");
        Assert.AreEqual(',', reader.Spec.Sep.Separator);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_AutoDetectSeparator_Tab()
    {
        using var reader = Sep.Utf8Reader().FromText("A\tB\tC\n1\t2\t3");
        Assert.AreEqual('\t', reader.Spec.Sep.Separator);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColByName()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("Name;Age\nAlice;30");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("Alice", reader.Current["Name"].ToString());
        Assert.AreEqual("30", reader.Current["Age"].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColByIndex()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\nx;y");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("x", reader.Current[0].ToString());
        Assert.AreEqual("y", reader.Current[^1].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Int()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\n42;99");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(42, reader.Current[0].Parse<int>());
        Assert.AreEqual(99, reader.Current[1].Parse<int>());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Double()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\n3.14");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(3.14, reader.Current[0].Parse<double>(), 0.001);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\n42\nabc");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(42, reader.Current[0].TryParse<int>());
        Assert.IsTrue(reader.MoveNext());
        Assert.IsNull(reader.Current[0].TryParse<int>());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_EmptyInput()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("");
        Assert.IsTrue(reader.IsEmpty);
        Assert.IsFalse(reader.HasRows);
        Assert.IsFalse(reader.MoveNext());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_NoHeader()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { HasHeader = false }).FromText("1;2;3\n4;5;6");
        Assert.IsFalse(reader.HasHeader);
        Assert.IsTrue(reader.HasRows);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_BomSkipped()
    {
        // UTF-8 BOM: EF BB BF
        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var content = Encoding.UTF8.GetBytes("A;B\n1;2");
        var withBom = new byte[bom.Length + content.Length];
        bom.CopyTo(withBom, 0);
        content.CopyTo(withBom, bom.Length);

        using var reader = Sep.New(';').Utf8Reader().FromBytes(withBom);
        Assert.IsTrue(reader.HasHeader);
        Assert.AreEqual("A", reader.Header.ColNames[0]);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_CrLfLineEnding()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\r\n1;2\r\n3;4");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("3", reader.Current[0].ToString());
        Assert.IsFalse(reader.MoveNext());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_CrLineEnding()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\r1;2\r3;4");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("3", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Utf8Content()
    {
        // Test with actual multi-byte UTF-8 characters
        using var reader = Sep.New(';').Utf8Reader().FromText("Name;City\nÆble;Ångström");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("Æble", reader.Current["Name"].ToString());
        Assert.AreEqual("Ångström", reader.Current["City"].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColSpan()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\nhello");
        Assert.IsTrue(reader.MoveNext());
        var span = reader.Current[0].Span;
        Assert.AreEqual(5, span.Length);
        Assert.AreEqual((byte)'h', span[0]);
        Assert.AreEqual((byte)'o', span[4]);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ToBytes()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\nhello");
        Assert.IsTrue(reader.MoveNext());
        var memory = reader.Current[0].ToBytes();
        Assert.AreEqual(5, memory.Length);
        Assert.AreEqual((byte)'h', memory.Span[0]);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_MultipleRows()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Value");
        for (var i = 0; i < 100; i++) { sb.AppendLine(i.ToString()); }

        using var reader = Sep.Utf8Reader().FromText(sb.ToString());
        var count = 0;
        while (reader.MoveNext()) { count++; }
        Assert.AreEqual(100, count);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_RowSpan()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\n1;2");
        Assert.IsTrue(reader.MoveNext());
        var rowSpan = reader.Current.Span;
        Assert.AreEqual("1;2", Encoding.UTF8.GetString(rowSpan));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_RowToString()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\n1;2");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1;2", reader.Current.ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Foreach()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\n1;2\n3;4");
        var count = 0;
        foreach (var row in reader)
        {
            count++;
            Assert.AreEqual(2, row.ColCount);
        }
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Cols_Range()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C\n1;2;3");
        Assert.IsTrue(reader.MoveNext());
        var cols = reader.Current[..];
        Assert.AreEqual(3, cols.Count);
        var strings = cols.ToStringsArray();
        Assert.AreEqual("1", strings[0]);
        Assert.AreEqual("2", strings[1]);
        Assert.AreEqual("3", strings[2]);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Cols_ParseToArray()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C\n1;2;3");
        Assert.IsTrue(reader.MoveNext());
        var cols = reader.Current[..];
        var ints = cols.ParseToArray<int>();
        Assert.AreEqual(1, ints[0]);
        Assert.AreEqual(2, ints[1]);
        Assert.AreEqual(3, ints[2]);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_HeaderUtf8Lookup()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("Name;Age\nAlice;30");
        var header = reader.Header;
        Assert.AreEqual(0, header.IndexOf("Name"u8));
        Assert.AreEqual(1, header.IndexOf("Age"u8));
        Assert.IsTrue(header.TryIndexOf("Name"u8, out var idx));
        Assert.AreEqual(0, idx);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Unescape()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { Unescape = true }).FromText("A;B\n\"hello\";world");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("hello", reader.Current[0].ToString());
        Assert.AreEqual("world", reader.Current[1].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Unescape_Inner()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { Unescape = true }).FromText("A\n\"he\"\"llo\"");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("he\"llo", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_FromFile()
    {
        var filePath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(filePath, "A;B\n1;2\n3;4", Encoding.UTF8);
            using var reader = Sep.New(';').Utf8Reader().FromFile(filePath);
            Assert.IsTrue(reader.MoveNext());
            Assert.AreEqual("1", reader.Current[0].ToString());
            Assert.IsTrue(reader.MoveNext());
            Assert.AreEqual("3", reader.Current[0].ToString());
            Assert.IsFalse(reader.MoveNext());
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [TestMethod]
    public void SepUtf8ReaderTest_HeaderOnly()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C");
        Assert.IsTrue(reader.HasHeader);
        Assert.IsFalse(reader.HasRows);
        Assert.IsFalse(reader.MoveNext());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_SingleColumn()
    {
        using var reader = Sep.Utf8Reader().FromText("Val\n42\n99");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(42, reader.Current[0].Parse<int>());
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(99, reader.Current[0].Parse<int>());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_RowIndex()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\n1\n2\n3");
        var rowIdx = 1; // First data row is 1 (header is 0)
        foreach (var row in reader)
        {
            Assert.AreEqual(rowIdx, row.RowIndex);
            rowIdx++;
        }
        Assert.AreEqual(4, rowIdx);
    }
}
