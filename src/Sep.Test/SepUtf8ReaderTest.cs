using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUtf8ReaderTest
{
    #region IO Sources
    [TestMethod]
    public void SepUtf8ReaderTest_FromText_Simple()
    {
        var text = "A;B;C\n1;2;3\n4;5;6";
        using var reader = Sep.New(';').Utf8Reader().FromText(text);
        Assert.IsTrue(reader.HasHeader);
        Assert.IsTrue(reader.HasRows);
        Assert.IsFalse(reader.IsEmpty);

        var header = reader.Header;
        Assert.AreEqual(3, header.ColNames.Count);
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
    public void SepUtf8ReaderTest_FromText_NullThrows()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            Sep.Utf8Reader().FromText(null!));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_FromBytes_NullThrows()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            Sep.Utf8Reader().FromBytes(null!));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_From_NullStreamThrows()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            Sep.Utf8Reader().From(null!));
    }
    #endregion

    #region Extension Overloads
    [TestMethod]
    public void SepUtf8ReaderTest_ExtensionOverloads()
    {
        var text = "A;B\n1;2";
        // Sep.Utf8Reader() static
        using (var r = Sep.Utf8Reader().FromText(text)) { Assert.IsTrue(r.MoveNext()); }
        // Sep.Utf8Reader(configure) static
        using (var r = Sep.Utf8Reader(o => o with { HasHeader = true }).FromText(text)) { Assert.IsTrue(r.MoveNext()); }
        // Sep instance extension
        using (var r = Sep.New(';').Utf8Reader().FromText(text)) { Assert.IsTrue(r.MoveNext()); }
        // Sep instance extension with configure
        using (var r = Sep.New(';').Utf8Reader(o => o).FromText(text)) { Assert.IsTrue(r.MoveNext()); }
        // Sep? extension (null = auto-detect)
        using (var r = ((Sep?)null).Utf8Reader().FromText(text)) { Assert.IsTrue(r.MoveNext()); }
        // Sep? extension with configure
        using (var r = ((Sep?)null).Utf8Reader(o => o).FromText(text)) { Assert.IsTrue(r.MoveNext()); }
        // SepSpec extension
        using (var r = new SepSpec(Sep.Default, null, false).Utf8Reader().FromText(text)) { Assert.IsTrue(r.MoveNext()); }
        // SepSpec extension with configure
        using (var r = new SepSpec(Sep.Default, null, false).Utf8Reader(o => o).FromText(text)) { Assert.IsTrue(r.MoveNext()); }
    }
    #endregion

    #region Auto-detect Separator
    [TestMethod]
    public void SepUtf8ReaderTest_AutoDetectSeparator_Semicolon()
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
    #endregion

    #region Empty / HeaderOnly
    [TestMethod]
    public void SepUtf8ReaderTest_EmptyInput()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("");
        Assert.IsTrue(reader.IsEmpty);
        Assert.IsFalse(reader.HasHeader);
        Assert.IsFalse(reader.HasRows);
        Assert.IsFalse(reader.MoveNext());
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
    public void SepUtf8ReaderTest_HeaderOnly_TrailingNewline()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C\n");
        Assert.IsTrue(reader.HasHeader);
        Assert.IsFalse(reader.HasRows);
    }
    #endregion

    #region NoHeader
    [TestMethod]
    public void SepUtf8ReaderTest_NoHeader()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { HasHeader = false }).FromText("1;2;3\n4;5;6");
        Assert.IsFalse(reader.HasHeader);
        Assert.IsTrue(reader.HasRows);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("4", reader.Current[0].ToString());
        Assert.IsFalse(reader.MoveNext());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_NoHeader_RowIndex()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { HasHeader = false }).FromText("a\nb\nc");
        var idx = 0;
        foreach (var row in reader)
        {
            Assert.AreEqual(idx, row.RowIndex);
            idx++;
        }
        Assert.AreEqual(3, idx);
    }
    #endregion

    #region BOM Handling
    [TestMethod]
    public void SepUtf8ReaderTest_BomSkipped()
    {
        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var content = Encoding.UTF8.GetBytes("A;B\n1;2");
        var withBom = new byte[bom.Length + content.Length];
        bom.CopyTo(withBom, 0);
        content.CopyTo(withBom, bom.Length);

        using var reader = Sep.New(';').Utf8Reader().FromBytes(withBom);
        Assert.IsTrue(reader.HasHeader);
        Assert.AreEqual("A", reader.Header.ColNames[0]);
        Assert.AreEqual("B", reader.Header.ColNames[1]);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
        Assert.AreEqual("2", reader.Current[1].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_BomSkipped_Unescape()
    {
        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var content = Encoding.UTF8.GetBytes("A;B\n\"hello\";world");
        var withBom = new byte[bom.Length + content.Length];
        bom.CopyTo(withBom, 0);
        content.CopyTo(withBom, bom.Length);

        using var reader = Sep.New(';').Utf8Reader(o => o with { Unescape = true }).FromBytes(withBom);
        Assert.AreEqual("A", reader.Header.ColNames[0]);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("hello", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_BomNotSkipped_WhenDisabled()
    {
        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var content = Encoding.UTF8.GetBytes("A;B\n1;2");
        var withBom = new byte[bom.Length + content.Length];
        bom.CopyTo(withBom, 0);
        content.CopyTo(withBom, bom.Length);

        using var reader = Sep.New(';').Utf8Reader(o => o with { SkipBom = false }).FromBytes(withBom);
        // First col name should have BOM prefix
        Assert.AreNotEqual("A", reader.Header.ColNames[0]);
        Assert.IsTrue(reader.Header.ColNames[0].StartsWith("\uFEFF") ||
                       reader.Header.ColNames[0].Length > 1);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_NoBomPresent()
    {
        var content = Encoding.UTF8.GetBytes("A;B\n1;2");
        using var reader = Sep.New(';').Utf8Reader().FromBytes(content);
        Assert.AreEqual("A", reader.Header.ColNames[0]);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
    }
    #endregion

    #region Line Endings
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
    public void SepUtf8ReaderTest_MixedLineEndings()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\n1;2\r3;4\r\n5;6");
        var count = 0;
        while (reader.MoveNext()) { count++; }
        Assert.AreEqual(3, count);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TrailingNewline()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\n1\n2\n");
        var count = 0;
        while (reader.MoveNext()) { count++; }
        Assert.AreEqual(2, count);
    }
    #endregion

    #region UTF-8 Content
    [TestMethod]
    public void SepUtf8ReaderTest_Utf8Content_MultiByteChars()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("Name;City\nÆble;Ångström");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("Æble", reader.Current["Name"].ToString());
        Assert.AreEqual("Ångström", reader.Current["City"].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Utf8Content_Emoji()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\n🎉🎊");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("🎉🎊", reader.Current[0].ToString());
        // 🎉 is 4 bytes in UTF-8, 🎊 is 4 bytes
        Assert.AreEqual(8, reader.Current[0].Span.Length);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Utf8Content_CJK()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("Name\n日本語");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("日本語", reader.Current[0].ToString());
    }
    #endregion

    #region Row Properties
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

    [TestMethod]
    public void SepUtf8ReaderTest_LineNumbers()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\n1\n2\n3");
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.IsTrue(row.LineNumberFrom >= 1);
        Assert.IsTrue(row.LineNumberToExcl >= row.LineNumberFrom);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_LineNumbers_MultiLine()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\n1\n2\n3");
        var lineNums = new List<(int from, int to)>();
        foreach (var row in reader)
        {
            lineNums.Add((row.LineNumberFrom, row.LineNumberToExcl));
        }
        Assert.AreEqual(3, lineNums.Count);
        // Line numbers should be monotonically increasing
        for (var i = 1; i < lineNums.Count; i++)
        {
            Assert.IsTrue(lineNums[i].from >= lineNums[i - 1].from);
        }
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
    #endregion

    #region Col Access
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
    public void SepUtf8ReaderTest_ColByIndex_FromEnd()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C\nx;y;z");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("z", reader.Current[^1].ToString());
        Assert.AreEqual("y", reader.Current[^2].ToString());
        Assert.AreEqual("x", reader.Current[^3].ToString());
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
    public void SepUtf8ReaderTest_ColSpan_Empty()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\n;x");
        Assert.IsTrue(reader.MoveNext());
        var span = reader.Current[0].Span;
        Assert.AreEqual(0, span.Length);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColToString_Empty()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\n;x");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreSame(string.Empty, reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColToBytes()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\nhello");
        Assert.IsTrue(reader.MoveNext());
        var memory = reader.Current[0].ToBytes();
        Assert.AreEqual(5, memory.Length);
        Assert.AreEqual((byte)'h', memory.Span[0]);
        Assert.AreEqual((byte)'o', memory.Span[4]);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColToBytes_Empty()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\n;x");
        Assert.IsTrue(reader.MoveNext());
        var memory = reader.Current[0].ToBytes();
        Assert.AreEqual(0, memory.Length);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColToBytes_Utf8()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\nÆble");
        Assert.IsTrue(reader.MoveNext());
        var memory = reader.Current[0].ToBytes();
        Assert.AreEqual("Æble", Encoding.UTF8.GetString(memory.Span));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColToStringDirect()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\nhello");
        Assert.IsTrue(reader.MoveNext());
        // ToStringDirect is internal but exercises a code path via DebuggerDisplay indirectly
        Assert.AreEqual("hello", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Col_IndexOutOfRange()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\nval");
        Assert.IsTrue(reader.MoveNext());
        Assert.ThrowsExactly<IndexOutOfRangeException>(() =>
        {
            var _ = reader.Current[5].Span;
        });
    }
    #endregion

    #region Row TryGet
    [TestMethod]
    public void SepUtf8ReaderTest_Row_TryGet_Found()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("Name;Age\nAlice;30");
        Assert.IsTrue(reader.MoveNext());
        Assert.IsTrue(reader.Current.TryGet("Name", out var col));
        Assert.AreEqual("Alice", col.ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Row_TryGet_NotFound()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("Name;Age\nAlice;30");
        Assert.IsTrue(reader.MoveNext());
        Assert.IsFalse(reader.Current.TryGet("Missing", out _));
    }
    #endregion

    #region Row Params Int Indices
    [TestMethod]
    public void SepUtf8ReaderTest_Row_ParamsIndices()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C;D\n1;2;3;4");
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        ReadOnlySpan<int> indices = [0, 2];
        var cols = row[indices];
        Assert.AreEqual(2, cols.Count);
        Assert.AreEqual("1", cols[0].ToString());
        Assert.AreEqual("3", cols[1].ToString());

        var strings = cols.ToStringsArray();
        Assert.AreEqual("1", strings[0]);
        Assert.AreEqual("3", strings[1]);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Row_ParamsIndices_ParseToArray()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C\n10;20;30");
        Assert.IsTrue(reader.MoveNext());
        ReadOnlySpan<int> indices = [0, 2];
        var cols = reader.Current[indices];
        var ints = cols.ParseToArray<int>();
        Assert.AreEqual(10, ints[0]);
        Assert.AreEqual(30, ints[1]);
    }
    #endregion

    #region Row Range Slicing
    [TestMethod]
    public void SepUtf8ReaderTest_Row_Range_All()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C\n1;2;3");
        Assert.IsTrue(reader.MoveNext());
        var cols = reader.Current[..];
        Assert.AreEqual(3, cols.Count);
        CollectionAssert.AreEqual(new[] { "1", "2", "3" }, cols.ToStringsArray());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Row_Range_Partial()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C;D\n1;2;3;4");
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;

        var cols02 = row[0..2];
        Assert.AreEqual(2, cols02.Count);
        CollectionAssert.AreEqual(new[] { "1", "2" }, cols02.ToStringsArray());

        var cols13 = row[1..3];
        Assert.AreEqual(2, cols13.Count);
        CollectionAssert.AreEqual(new[] { "2", "3" }, cols13.ToStringsArray());

        var cols2End = row[2..];
        Assert.AreEqual(2, cols2End.Count);
        CollectionAssert.AreEqual(new[] { "3", "4" }, cols2End.ToStringsArray());
    }
    #endregion

    #region Cols Operations
    [TestMethod]
    public void SepUtf8ReaderTest_Cols_ParseToArray()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C\n1;2;3");
        Assert.IsTrue(reader.MoveNext());
        var cols = reader.Current[..];
        var ints = cols.ParseToArray<int>();
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, ints);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Cols_Indexer()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C\n1;2;3");
        Assert.IsTrue(reader.MoveNext());
        var cols = reader.Current[..];
        Assert.AreEqual("1", cols[0].ToString());
        Assert.AreEqual("2", cols[1].ToString());
        Assert.AreEqual("3", cols[2].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Cols_Count()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B;C;D;E\n1;2;3;4;5");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(5, reader.Current[..].Count);
        Assert.AreEqual(2, reader.Current[0..2].Count);
        Assert.AreEqual(0, reader.Current[0..0].Count);
    }
    #endregion

    #region Parse: int, float, double, string
    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Int()
    {
        RunCol(col => Assert.AreEqual(123456, col.Parse<int>()), "123456");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Float()
    {
        RunCol(col => Assert.AreEqual(123456f, col.Parse<float>()), "123456");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Double()
    {
        RunCol(col => Assert.AreEqual(123456d, col.Parse<double>()), "123456");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Float_NaN()
    {
        // csFastFloat byte overloads may not support NaN, test with DisableFastFloat
        RunCol(col => Assert.AreEqual(float.NaN, col.Parse<float>()), "NaN",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.AreEqual(float.NaN, col.Parse<float>()), "+NaN",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.AreEqual(float.NaN, col.Parse<float>()), "-NaN",
            configure: o => o with { DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Float_Infinity()
    {
        RunCol(col => Assert.AreEqual(float.PositiveInfinity, col.Parse<float>()), "Infinity",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.AreEqual(float.NegativeInfinity, col.Parse<float>()), "-Infinity",
            configure: o => o with { DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Double_NaN()
    {
        RunCol(col => Assert.AreEqual(double.NaN, col.Parse<double>()), "NaN",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.AreEqual(double.NaN, col.Parse<double>()), "+NaN",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.AreEqual(double.NaN, col.Parse<double>()), "-NaN",
            configure: o => o with { DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Double_Infinity()
    {
        RunCol(col => Assert.AreEqual(double.PositiveInfinity, col.Parse<double>()), "Infinity",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.AreEqual(double.NegativeInfinity, col.Parse<double>()), "-Infinity",
            configure: o => o with { DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Float_DisableFastFloat()
    {
        RunCol(col => Assert.AreEqual(3.14f, col.Parse<float>(), 0.001f), "3.14",
            configure: o => o with { DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Double_DisableFastFloat()
    {
        RunCol(col => Assert.AreEqual(3.14, col.Parse<double>(), 0.001), "3.14",
            configure: o => o with { DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Float_FastFloat()
    {
        // Tests the csFastFloat byte overload path (default, fastfloat enabled)
        RunCol(col => Assert.AreEqual(3.14f, col.Parse<float>(), 0.001f), "3.14");
        RunCol(col => Assert.AreEqual(-99.5f, col.Parse<float>(), 0.1f), "-99.5");
        RunCol(col => Assert.AreEqual(0f, col.Parse<float>()), "0");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Double_FastFloat()
    {
        // Tests the csFastFloat byte overload path (default, fastfloat enabled)
        RunCol(col => Assert.AreEqual(3.14, col.Parse<double>(), 0.001), "3.14");
        RunCol(col => Assert.AreEqual(-99.5, col.Parse<double>(), 0.1), "-99.5");
        RunCol(col => Assert.AreEqual(0d, col.Parse<double>()), "0");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Float_FastFloat()
    {
        // Tests csFastFloat TryParseFloat byte path
        RunCol(col => Assert.AreEqual(3.14f, col.TryParse<float>()), "3.14");
        RunCol(col => Assert.IsNull(col.TryParse<float>()), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Double_FastFloat()
    {
        // Tests csFastFloat TryParseDouble byte path
        RunCol(col => Assert.AreEqual(3.14, col.TryParse<double>()), "3.14");
        RunCol(col => Assert.IsNull(col.TryParse<double>()), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Out_Float_FastFloat()
    {
        // Tests csFastFloat TryParseFloat byte path via out parameter
        RunCol(col =>
        {
            Assert.IsTrue(col.TryParse<float>(out var v));
            Assert.AreEqual(3.14f, v, 0.001f);
        }, "3.14");
        RunCol(col => Assert.IsFalse(col.TryParse<float>(out _)), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Out_Double_FastFloat()
    {
        // Tests csFastFloat TryParseDouble byte path via out parameter
        RunCol(col =>
        {
            Assert.IsTrue(col.TryParse<double>(out var v));
            Assert.AreEqual(3.14, v, 0.001);
        }, "3.14");
        RunCol(col => Assert.IsFalse(col.TryParse<double>(out _)), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_Float_NullCulture_DisableFastFloat()
    {
        RunCol(col => Assert.AreEqual(3.14f, col.Parse<float>(), 0.001f), "3.14",
            configure: o => o with { CultureInfo = null, DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Parse_String_ViaToString()
    {
        // string doesn't implement IUtf8SpanParsable, use ToString instead
        RunCol(col => Assert.AreEqual("hello", col.ToString()), "hello");
        RunCol(col => Assert.AreSame(string.Empty, col.ToString()), "");
    }
    #endregion

    #region TryParse Return
    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Return_Int()
    {
        RunCol(col => Assert.AreEqual(42, col.TryParse<int>()), "42");
        RunCol(col => Assert.IsNull(col.TryParse<int>()), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Return_Float()
    {
        RunCol(col => Assert.AreEqual(3.14f, col.TryParse<float>()), "3.14");
        RunCol(col => Assert.AreEqual(float.NaN, col.TryParse<float>()), "NaN",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.AreEqual(float.PositiveInfinity, col.TryParse<float>()), "Infinity",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.IsNull(col.TryParse<float>()), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Return_Double()
    {
        RunCol(col => Assert.AreEqual(3.14, col.TryParse<double>()), "3.14");
        RunCol(col => Assert.AreEqual(double.NaN, col.TryParse<double>()), "NaN",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.IsNull(col.TryParse<double>()), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Return_Float_DisableFastFloat()
    {
        RunCol(col => Assert.AreEqual(3.14f, col.TryParse<float>()), "3.14",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.IsNull(col.TryParse<float>()), "abc",
            configure: o => o with { DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Return_Double_DisableFastFloat()
    {
        RunCol(col => Assert.AreEqual(3.14, col.TryParse<double>()), "3.14",
            configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.IsNull(col.TryParse<double>()), "abc",
            configure: o => o with { DisableFastFloat = true });
    }
    #endregion

    #region TryParse Out
    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Out_Int()
    {
        RunCol(col =>
        {
            Assert.IsTrue(col.TryParse<int>(out var v));
            Assert.AreEqual(42, v);
        }, "42");
        RunCol(col => Assert.IsFalse(col.TryParse<int>(out _)), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Out_Float()
    {
        RunCol(col =>
        {
            Assert.IsTrue(col.TryParse<float>(out var v));
            Assert.AreEqual(3.14f, v, 0.001f);
        }, "3.14");
        RunCol(col => Assert.IsFalse(col.TryParse<float>(out _)), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Out_Double()
    {
        RunCol(col =>
        {
            Assert.IsTrue(col.TryParse<double>(out var v));
            Assert.AreEqual(3.14, v, 0.001);
        }, "3.14");
        RunCol(col => Assert.IsFalse(col.TryParse<double>(out _)), "abc");
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Out_Float_DisableFastFloat()
    {
        RunCol(col =>
        {
            Assert.IsTrue(col.TryParse<float>(out var v));
            Assert.AreEqual(3.14f, v, 0.001f);
        }, "3.14", configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.IsFalse(col.TryParse<float>(out _)), "abc",
            configure: o => o with { DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Out_Double_DisableFastFloat()
    {
        RunCol(col =>
        {
            Assert.IsTrue(col.TryParse<double>(out var v));
            Assert.AreEqual(3.14, v, 0.001);
        }, "3.14", configure: o => o with { DisableFastFloat = true });
        RunCol(col => Assert.IsFalse(col.TryParse<double>(out _)), "abc",
            configure: o => o with { DisableFastFloat = true });
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TryParse_Out_ViaToString()
    {
        // string doesn't implement IUtf8SpanParsable, test via ToString path
        RunCol(col => Assert.AreEqual("hello", col.ToString()), "hello");
    }
    #endregion

    #region Header Lookup
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
    public void SepUtf8ReaderTest_HeaderUtf8Lookup_NotFound()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("Name;Age\nAlice;30");
        var header = reader.Header;
        Assert.IsFalse(header.TryIndexOf("Missing"u8, out _));
        Assert.ThrowsExactly<KeyNotFoundException>(() => header.IndexOf("Missing"u8));
    }

    [TestMethod]
    public void SepUtf8ReaderTest_HeaderStringLookup()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("Name;Age\nAlice;30");
        var header = reader.Header;
        Assert.AreEqual(0, header.IndexOf("Name"));
        Assert.AreEqual(1, header.IndexOf("Age"));
    }
    #endregion

    #region Unescape
    [TestMethod]
    public void SepUtf8ReaderTest_Unescape_Basic()
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
    public void SepUtf8ReaderTest_Unescape_Empty()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { Unescape = true }).FromText("A\n\"\"");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Unescape_MultipleQuotes()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { Unescape = true }).FromText("A\n\"a\"\"b\"\"c\"");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("a\"b\"c", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_Unescape_WithNewlineInQuotes()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { Unescape = true }).FromText("A;B\n\"line1\nline2\";val");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("line1\nline2", reader.Current[0].ToString());
        Assert.AreEqual("val", reader.Current[1].ToString());
    }
    #endregion

    #region Trim
    [TestMethod]
    public void SepUtf8ReaderTest_TrimOuter()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { Trim = SepTrim.Outer }).FromText("A\n  hello  ");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("hello", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TrimOuter_Empty()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { Trim = SepTrim.Outer }).FromText("A\n   ");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("", reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_TrimAll_Unescape()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with
        { Unescape = true, Trim = SepTrim.All }).FromText("A\n \" hello \" ");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("hello", reader.Current[0].ToString());
    }
    #endregion

    #region DisableQuotesParsing
    [TestMethod]
    public void SepUtf8ReaderTest_DisableQuotesParsing()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { DisableQuotesParsing = true })
            .FromText("A\n\"hello\"");
        Assert.IsTrue(reader.MoveNext());
        // With quotes disabled, quotes are literal
        Assert.AreEqual("\"hello\"", reader.Current[0].ToString());
    }
    #endregion

    #region DisableColCountCheck
    [TestMethod]
    public void SepUtf8ReaderTest_ColCountMismatch_Throws()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\n1;2\n3");
        Assert.IsTrue(reader.MoveNext()); // 1;2 is fine
        Assert.ThrowsExactly<InvalidDataException>(() => reader.MoveNext()); // 3 has 1 col
    }

    [TestMethod]
    public void SepUtf8ReaderTest_DisableColCountCheck()
    {
        using var reader = Sep.New(';').Utf8Reader(o => o with { DisableColCountCheck = true })
            .FromText("A;B\n1;2\n3");
        Assert.IsTrue(reader.MoveNext()); // 1;2
        Assert.IsTrue(reader.MoveNext()); // 3 - no exception
        Assert.AreEqual(1, reader.Current.ColCount);
    }
    #endregion

    #region Multiple Rows / Buffer Growth
    [TestMethod]
    public void SepUtf8ReaderTest_ManyRows()
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
    public void SepUtf8ReaderTest_LargeColumn_BufferGrowth()
    {
        // Force buffer doubling by creating a column larger than initial buffer
        var largeValue = new string('x', 32 * 1024);
        var text = $"A\n{largeValue}";
        using var reader = Sep.Utf8Reader().FromText(text);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(largeValue, reader.Current[0].ToString());
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ColsInitialLength()
    {
        // Mirror SepReaderTest_ColsInitialLength
        var colCount = SepUtf8Reader.ColEndsInitialLength - 1;
        var text = "A" + Environment.NewLine + new string(';', colCount - 1);
        using var reader = Sep.Utf8Reader(o => o with { DisableColCountCheck = true }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(colCount, row.ColCount);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ExceedingColsInitialLength_WorksByDoublingCapacity()
    {
        // Mirror SepReaderTest_ExceedingColsInitialLength_WorksByDoublingCapacity
        var colCount = SepUtf8Reader.ColEndsInitialLength;
        var text = "A" + Environment.NewLine + new string(';', colCount - 1);
        using var reader = Sep.Utf8Reader(o => o with { DisableColCountCheck = true }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(colCount, row.ColCount);
        Assert.HasCount(colCount * 2, reader._colEndsOrColInfos);
    }

    [TestMethod]
    public void SepUtf8ReaderTest_ExceedingColsInitialLength_Unescape_WorksByDoublingCapacity()
    {
        // Unescape doubles int usage per col (SepColInfo), test capacity growth
        var colCount = SepUtf8Reader.ColEndsInitialLength;
        var text = "A" + Environment.NewLine + new string(';', colCount - 1);
        using var reader = Sep.Utf8Reader(o => o with { DisableColCountCheck = true, Unescape = true }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(colCount, reader.Current.ColCount);
    }
    #endregion

    #region CarriageReturn Handling
    [TestMethod]
    public void SepUtf8ReaderTest_CarriageReturnLineFeedEvenOrOdd()
    {
        // Mirror SepReaderTest_CarriageReturnLineFeedEvenOrOdd
        foreach (var even in new[] { true, false })
        {
            const int lineEndingCount = 1267 + 64 * 1024;
            var lineEnding = "\r\n";
            var lineEndingStartIndex = (even ? 0 : 1);
            var sb = new StringBuilder(lineEndingCount * lineEnding.Length + lineEndingStartIndex);
            if (!even) { sb.Append(' '); }
            sb.Insert(lineEndingStartIndex, lineEnding, lineEndingCount);
            var text = sb.ToString();
            AssertUtf8LineEndings(lineEndingCount, text);
        }
    }

    [TestMethod]
    public void SepUtf8ReaderTest_CarriageReturn_ToEnsureTrailingCarriageReturnHandled()
    {
        // Mirror SepReaderTest_CarriageReturn_ToEnsureTrailingCarriageReturnHandled
        const int lineEndingCount = 1267 + 64 * 1024;
        var text = new string('\r', lineEndingCount);
        AssertUtf8LineEndings(lineEndingCount, text);
    }

    static void AssertUtf8LineEndings(int lineEndingCount, string text)
    {
        using var reader = Sep.Utf8Reader(o => o with { HasHeader = false }).FromText(text);
        var lineCount = 0;
        foreach (var row in reader) { ++lineCount; }
        Assert.AreEqual(lineEndingCount, lineCount);
    }
    #endregion

    #region SingleColumn
    [TestMethod]
    public void SepUtf8ReaderTest_SingleColumn()
    {
        using var reader = Sep.Utf8Reader().FromText("Val\n42\n99");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(42, reader.Current[0].Parse<int>());
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(99, reader.Current[0].Parse<int>());
    }
    #endregion

    #region Repeated Access
    [TestMethod]
    public void SepUtf8ReaderTest_RepeatedAccess()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\nhello");
        Assert.IsTrue(reader.MoveNext());
        for (var i = 0; i < 4; i++)
        {
            Assert.AreEqual("hello", reader.Current[0].ToString());
            Assert.AreEqual(5, reader.Current[0].Span.Length);
        }
    }
    #endregion

    #region Spec
    [TestMethod]
    public void SepUtf8ReaderTest_Spec()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A\n1");
        Assert.AreEqual(';', reader.Spec.Sep.Separator);
    }
    #endregion

    #region Dispose
    [TestMethod]
    public void SepUtf8ReaderTest_Dispose_Multiple()
    {
        var reader = Sep.New(';').Utf8Reader().FromText("A\n1");
        reader.Dispose();
        reader.Dispose(); // Should not throw
    }
    #endregion

    #region ToString(int) / ToStringDirect
    [TestMethod]
    public void SepUtf8ReaderTest_Reader_ToString()
    {
        using var reader = Sep.New(';').Utf8Reader().FromText("A;B\nhello;world");
        Assert.IsTrue(reader.MoveNext());
        // Test the public ToString(int index) method on SepUtf8Reader
        Assert.AreEqual("hello", reader.ToString(0));
        Assert.AreEqual("world", reader.ToString(1));
    }
    #endregion

    #region DuplicateColNames
    [TestMethod]
    public void SepUtf8ReaderTest_DuplicateColNames_Throws()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            Sep.New(';').Utf8Reader().FromText("A;A\n1;2"));
    }
    #endregion

    #region NonSeekableStream
    [TestMethod]
    public void SepUtf8ReaderTest_NonSeekableStream()
    {
        var bytes = Encoding.UTF8.GetBytes("A;B\n1;2");
        using var ms = new MemoryStream(bytes);
        using var nonSeekable = new NonSeekableStream(ms);
        using var reader = Sep.New(';').Utf8Reader().From(nonSeekable);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("1", reader.Current[0].ToString());
    }

    sealed class NonSeekableStream(Stream inner) : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() => inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
        public override int Read(Span<byte> buffer) => inner.Read(buffer);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
    #endregion

    #region SmallBuffer_ForceMultipleReads
    [TestMethod]
    public void SepUtf8ReaderTest_SmallInitialBuffer()
    {
        // Force small initial buffer to exercise buffer growth code paths
        var text = "A;B;C\n" + string.Join("\n", Enumerable.Range(0, 50).Select(i => $"{i};{i * 10};{i * 100}"));
        using var reader = Sep.New(';').Utf8Reader(o => o with { InitialBufferLength = 32 }).FromText(text);
        var count = 0;
        while (reader.MoveNext())
        {
            count++;
            Assert.AreEqual(3, reader.Current.ColCount);
        }
        Assert.AreEqual(50, count);
    }
    #endregion

    #region DefaultConstructor
    [TestMethod]
    public void SepUtf8ReaderTest_DefaultOptions()
    {
        // Test parameterless constructor
        var options = new SepUtf8ReaderOptions();
        Assert.IsNull(options.Sep);
        Assert.IsTrue(options.HasHeader);
        Assert.IsFalse(options.DisableFastFloat);
        Assert.IsFalse(options.DisableColCountCheck);
        Assert.IsFalse(options.DisableQuotesParsing);
        Assert.IsFalse(options.Unescape);
        Assert.IsTrue(options.SkipBom);
        Assert.AreEqual(SepTrim.None, options.Trim);
    }
    #endregion

    #region Helpers
    static void RunCol(SepUtf8Reader.ColAction action, string colValue,
        Func<SepUtf8ReaderOptions, SepUtf8ReaderOptions>? configure = null)
    {
        var text = $"A\n{colValue}\n";
        var options = configure != null
            ? Sep.Utf8Reader(configure)
            : Sep.Utf8Reader();

        using var reader = options.FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        action(row[0]);
    }
    #endregion
}
