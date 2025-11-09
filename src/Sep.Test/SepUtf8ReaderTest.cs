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
        Assert.AreEqual(3, header.ColNames.Count);
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
        
        Assert.AreEqual(3, names.Count);
        Assert.AreEqual("Alice", names[0]);
        Assert.AreEqual("münchen", cities[0]);
        Assert.AreEqual("København", cities[1]);
        Assert.AreEqual("日本", cities[2]);
    }
}
