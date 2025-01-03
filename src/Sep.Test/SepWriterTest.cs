using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterTest
{
    [TestMethod]
    public void SepWriterTest_NoRow()
    {
        using var writer = CreateWriter();
        Assert.AreEqual("", writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_Spec()
    {
        using var writer = CreateWriter();
        var spec = writer.Spec;
        Assert.AreEqual(Sep.Default, spec.Sep);
        Assert.AreEqual(SepDefaults.CultureInfo, spec.CultureInfo);
    }

    [TestMethod]
    public void SepWriterTest_EmptyRow()
    {
        using var writer = CreateWriter();
        {
            using var row = writer.NewRow();
        }
        var expected =
@"

";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_OneRowOneCol()
    {
        using var writer = CreateWriter();
        {
            using var row = writer.NewRow();
            row["A"].Set("1");
        }
        var expected =
@"A
1
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_TwoRowsThreeCols()
    {
        using var writer = CreateWriter();
        {
            using var row = writer.NewRow();
            row["A"].Set("1");
            row["B"].Format(2);
            row["C"].Set($"{2 * 17}");
        }
        {
            using var row = writer.NewRow();
            // Order of cols is not important after first row/header written
            row["C"].Set(new Span<char>(['6', '5']));
            row["B"].Format(3);
            row["A"].Set($"{23,3}");
        }
        var expected =
@"A;B;C
1;2;34
 23;3;65
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_TwoRowsThreeCols_ReadAfterWriteDoesNotClearStringBuilder()
    {
        using var writer = CreateWriter();
        {
            using var row = writer.NewRow();
            row["A"].Set("1");
            row["B"].Format(2);
            row["C"].Set($"{2 * 17}");
            var a = row["A"];
            var b = row["B"];
            var c = row["C"];
        }
        {
            using var row = writer.NewRow();
            row["A"].Set($"{23,3}");
            row[1].Format(3);
            row["C"].Set(new Span<char>(['6', '5']));
            var a = row[0];
            var b = row[1];
            var c = row[2];
        }
        var expected =
@"A;B;C
1;2;34
 23;3;65
";
        Assert.AreEqual(expected, writer.ToString());
    }


    [TestMethod]
    public void SepWriterTest_NewRowWhenAlreadyNewRow_Throws()
    {
        using var writer = CreateWriter();
        var row0 = writer.NewRow();
        var e = Assert.ThrowsException<InvalidOperationException>(() => writer.NewRow());
        Assert.AreEqual("Writer already has an active new row. Ensure this is disposed before starting next row.", e.Message);
    }

    [TestMethod]
    public void SepWriterTest_EndRowWithoutNewRow_Throws()
    {
        using var writer = CreateWriter();
        var e = Assert.ThrowsException<InvalidOperationException>(() => writer.EndRow(default));
        Assert.AreEqual("Writer does not have an active row. " +
                        "Ensure 'NewRow()' has been called and that the row is only disposed once. " +
                        "I.e. prefer 'using var row = writer.NewRow();'", e.Message);
    }

    [TestMethod]
    public void SepWriterTest_ColWrittenNotDefinedInFirstRow_Throws()
    {
        using var writer = CreateWriter();
        {
            using var row1 = writer.NewRow();
            row1["A"].Set("1");
        }
        {
            var row2 = writer.NewRow();
            var e = AssertThrowsException<KeyNotFoundException>(row2,
                r => { var c = r["B"]; });
            Assert.AreEqual("B", e.Message);
        }
        // Expected output should only be valid rows
        var expected =
@"A
1
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_ColMissingInSecondRow()
    {
        using var writer = CreateWriter();
        {
            using var row1 = writer.NewRow();
            row1["A"].Set("1");
            row1["B"].Set("2");
        }
        {
            var row2 = writer.NewRow();
            row2["B"].Set("3");
            var e = AssertThrowsException<InvalidOperationException>(row2,
                r => { r.Dispose(); });
            // TODO: Make detailed exception message
            Assert.AreEqual("Not all expected columns 'A,B' have been set.", e.Message);
        }
        // Expected output should only be valid rows
        var expected =
@"A;B
1;2
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_ToString_ToStreamWriter_Throws()
    {
        using var stream = new MemoryStream();
        using var writer = Sep.Writer().To(stream);
        var e = Assert.ThrowsException<NotSupportedException>(() => writer.ToString());
        Assert.AreEqual("'ToString' not supported for 'System.IO.StreamWriter' only supported for 'StringWriter'", e.Message);
    }

    [TestMethod]
    public void SepWriterTest_Flush()
    {
        using var stream = new MemoryStream();
        using var writer = Sep.Writer().To(stream);
        using (var row = writer.NewRow())
        {
            row["A"].Set("123");
        }
        Assert.AreEqual(0, stream.Position);
        writer.Flush();
        Assert.AreNotEqual(0, stream.Position);
    }

    [TestMethod]
    public void SepWriterTest_Extensions_ToText_Capacity()
    {
        using var writer = Sep.Writer().ToText(capacity: 1024);
        using (var row = writer.NewRow()) { row["A"].Format(1); }
        Assert.AreEqual("""
                        A
                        1
                        
                        """, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_Extensions_ToFile()
    {
        const string fileName = nameof(SepWriterTest_Extensions_ToFile) + ".csv";
        {
            using var writer = Sep.Writer().ToFile(fileName);
            using var row = writer.NewRow();
            row["A"].Format(1);
        }
        Assert.AreEqual("""
                        A
                        1
                        
                        """, File.ReadAllText(fileName));
        File.Delete(fileName);
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void SepWriterTest_Extensions_ToStream_LeaveOpen(bool leaveOpen)
    {
        var stream = new MemoryStream();
        using (var writer = Sep.Writer().To(stream, leaveOpen))
        {
            using var row = writer.NewRow();
            row["A"].Format(1);
        }
        Assert.AreEqual(stream.CanRead && stream.CanWrite && stream.CanSeek, leaveOpen);
        var actual = Encoding.UTF8.GetString(stream.ToArray());
        Assert.AreEqual("""
                        A
                        1
                        
                        """, actual);
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public void SepWriterTest_Extensions_ToTextWriter_LeaveOpen(bool leaveOpen)
    {
        var textWriter = new StringWriter();
        using (var writer = Sep.Writer().To(textWriter, leaveOpen))
        {
            using var row = writer.NewRow();
            row["A"].Format(1);
        }
        var actual = textWriter.ToString();
        Assert.AreEqual("""
                        A
                        1
                        
                        """, actual);
        if (!leaveOpen)
        {
            Assert.ThrowsException<ObjectDisposedException>(
                () => textWriter.Write("THROW DISPOSED IF NOT LEAVEOPEN"));
        }
        else
        {
            textWriter.Write("2");
        }
    }

    [TestMethod]
    public void SepWriterTest_WriteHeader_False()
    {
        using var writer = Sep.Writer(o => o with { WriteHeader = false }).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("1");
            row[1].Format(2);
            row[2].Set($"{2 * 17}");
        }
        {
            using var row = writer.NewRow();
            // Order of cols is not important after first row written
            row[2].Set("65");
            row[1].Format(3);
            row["A"].Set($"{23,3}");
        }
        var expected =
@"1;2;34
 23;3;65
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_WriteHeader_False_UnknownColName()
    {
        using var writer = Sep.Writer(o => o with { WriteHeader = false }).ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("1");
            row[1].Format(2);
            row[2].Set($"{2 * 17}");
        }
        {
            using var row = writer.NewRow();
            row[2].Set("65");
            row[1].Format(3);
            row["A"].Set($"{23,3}");

            var e = AssertThrowsException<KeyNotFoundException>(row,
                r => { r["B"].Set("Test"); });
            Assert.AreEqual("B", e.Message);
        }
        var expected =
@"1;2;34
 23;3;65
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_WriteHeader_False_ColMissingInSecondRow()
    {
        using var writer = Sep.Writer(o => o with { WriteHeader = false }).ToText();
        {
            using var row1 = writer.NewRow();
            row1[0].Set("A");
            row1[1].Set("B");
        }
        {
            var row2 = writer.NewRow();
            row2[1].Set("Y");
            var e = AssertThrowsException<InvalidOperationException>(row2,
                r => { r.Dispose(); });
            Assert.AreEqual("Not all expected columns have been set.", e.Message);
        }
        // Expected output should only be valid rows
        var expected =
@"A;B
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_DisableColCountCheck()
    {
        using var writer = Sep.Writer(o => o with { DisableColCountCheck = true }).ToText();
        {
            using var row1 = writer.NewRow();
            row1["A"].Set("1");
            row1["B"].Set("2");
        }
        {
            using var row2 = writer.NewRow();
            row2["B"].Set("3");
        }
        var expected =
@"A;B
1;2
;3
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_NoHeader_OnlyColumnsSetWritten()
    {
        using var writer = Sep.Writer(o => o with { WriteHeader = false, DisableColCountCheck = false }).ToText();
        {
            using var row = writer.NewRow();
            row[0].Set("1");
            row[2].Set("3");
        }
        {
            using var row = writer.NewRow();
            row[1].Set("2");
        }
        var expected =
@"1;;3
;2;
";
        Assert.AreEqual(expected, writer.ToString());
    }

    static SepWriter CreateWriter() =>
        Sep.New(';').Writer().ToText();

    delegate void WriterRowAction(SepWriter.Row row);

    static TException AssertThrowsException<TException>(SepWriter.Row row,
        WriterRowAction action)
        where TException : Exception
    {
        try
        {
            action(row);
        }
        catch (TException e)
        {
            return e;
        }
        Assert.Fail($"Expected exception {typeof(TException)} not thrown.");
        return null;
    }
}
