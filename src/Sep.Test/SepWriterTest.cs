using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterTest
{
    [TestMethod]
    public void SepWriterTest_Spec()
    {
        using var writer = CreateWriter();
        var spec = writer.Spec;
        Assert.AreEqual(Sep.Default, spec.Sep);
        Assert.AreEqual(SepDefaults.CultureInfo, spec.CultureInfo);
    }

    [TestMethod]
    public async ValueTask SepWriterTest_NoRow()
    {
        await AssertWriterSyncAsync("");
    }

    [TestMethod]
    public async ValueTask SepWriterTest_EmptyRow()
    {
        var expected =
@"

";
        await AssertWriterSyncAsync(expected, r => { });
    }

    [TestMethod]
    public async ValueTask SepWriterTest_OneRowOneCol()
    {
        var expected =
@"A
1
";
        await AssertWriterSyncAsync(expected, r => { r["A"].Set("1"); });
    }

    [TestMethod]
    public async ValueTask SepWriterTest_TwoRowsThreeCols()
    {
        var expected =
@"A;B;C
1;2;34
 23;3;65
";
        await AssertWriterSyncAsync(expected,
            row =>
            {
                row["A"].Set("1");
                row["B"].Format(2);
                row["C"].Set($"{2 * 17}");
            },
            row =>
            {
                // Order of cols is not important after first row/header written
                row["C"].Set(new Span<char>(['6', '5']));
                row["B"].Format(3);
                row["A"].Set($"{23,3}");
            });
    }

    [TestMethod]
    public async ValueTask SepWriterTest_TwoRowsThreeCols_ReadAfterWriteDoesNotClearStringBuilder()
    {
        var expected =
@"A;B;C
1;2;34
 23;3;65
";
        await AssertWriterSyncAsync(expected,
            row =>
            {
                row["A"].Set("1");
                row["B"].Format(2);
                row["C"].Set($"{2 * 17}");
                var a = row["A"];
                var b = row["B"];
                var c = row["C"];
            },
            row =>
            {
                // Order of cols is not important after first row/header written
                row["A"].Set($"{23,3}");
                row[1].Format(3);
                row["C"].Set(new Span<char>(['6', '5']));
                var a = row[0];
                var b = row[1];
                var c = row[2];
            });
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
    public async ValueTask SepWriterTest_EndRowWithoutNewRow_Throws()
    {
        var expected = "Writer does not have an active row. " +
                       "Ensure 'NewRow()' has been called and that the row is only disposed once. " +
                       "I.e. prefer 'using var row = writer.NewRow();'";
        {
            using var writer = CreateWriter();
            var e = Assert.ThrowsException<InvalidOperationException>(
                () => writer.EndRow());
            Assert.AreEqual(expected, e.Message);
        }
        {
            await using var writer = CreateWriter();
            var e = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await writer.EndRowAsync());
            Assert.AreEqual(expected, e.Message);
        }
    }

    [TestMethod]
    public void SepWriterTest_NewRowWithCancellationToken_Dispose_Throws()
    {
        var expected = "'NewRow()' called with 'CancellationToken', if async use was intented, " +
                       "be sure to dispose this asynchronously with 'await' like " +
                       "'await using var row = writer.NewRow(cancellationToken);'";

        using var writer = Sep.New(';').Writer().ToText();
        var cts = new CancellationTokenSource();
        var e = Assert.ThrowsException<InvalidOperationException>(
            () => { using (writer.NewRow(cts.Token)) { } });
        Assert.AreEqual(expected, e.Message);
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
    public async ValueTask SepWriterTest_ColMissingInSecondRow()
    {
        var expectedMessage = "Not all expected columns 'A,B' have been set.";
        // Expected output should only be valid rows
        var expected =
@"A;B
1;2
";
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
                Assert.AreEqual(expectedMessage, e.Message);
            }
            Assert.AreEqual(expected, writer.ToString());
        }
        {
            await using var writer = CreateWriter();
            {
                await using var row1 = writer.NewRow();
                row1["A"].Set("1");
                row1["B"].Set("2");
            }
            {
                var row2 = writer.NewRow();
                row2["B"].Set("3");
                var e = AssertThrowsException<InvalidOperationException>(row2,
                    r => { r.DisposeAsync().GetAwaiter().GetResult(); }); // Call sync due to row ref struct
                Assert.AreEqual(expectedMessage, e.Message);
            }
            Assert.AreEqual(expected, writer.ToString());
        }
    }

    [TestMethod]
    public async ValueTask SepWriterTest_ColMissingInSecondRow_ColNotSetEmpty()
    {
        var expectedMessage = "Not all expected columns 'A,B' have been set.";
        // Expected output should only be valid rows
        var expected =
@"A;B
1;2
";
        {
            using var writer = Sep.Writer(
                o => o with { ColNotSetOption = SepColNotSetOption.Empty }).ToText();
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
                Assert.AreEqual(expectedMessage, e.Message);
            }
            Assert.AreEqual(expected, writer.ToString());
        }
        {
            await using var writer = Sep.Writer(
                o => o with { ColNotSetOption = SepColNotSetOption.Empty }).ToText();
            {
                await using var row1 = writer.NewRow();
                row1["A"].Set("1");
                row1["B"].Set("2");
            }
            {
                var row2 = writer.NewRow();
                row2["B"].Set("3");
                var e = AssertThrowsException<InvalidOperationException>(row2,
                    r => { r.DisposeAsync().GetAwaiter().GetResult(); });
                Assert.AreEqual(expectedMessage, e.Message);
            }
            Assert.AreEqual(expected, writer.ToString());
        }
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
    public async ValueTask SepWriterTest_FlushAsync()
    {
        await using var stream = new MemoryStream();
        await using var writer = Sep.Writer().To(stream);
        await using (var row = writer.NewRow())
        {
            row["A"].Set("123");
        }
        Assert.AreEqual(0, stream.Position);
        await writer.FlushAsync();
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
    public async ValueTask SepWriterTest_Extensions_ToStream_LeaveOpen(bool leaveOpen)
    {
        {
            var stream = new MemoryStream();
            using (var writer = Sep.Writer().To(stream, leaveOpen))
            {
                using var row = writer.NewRow();
                row["A"].Format(1);
            }
            AssertStream(leaveOpen, stream);
        }
        {
            var stream = new MemoryStream();
            await using (var writer = Sep.Writer().To(stream, leaveOpen))
            {
                await using var row = writer.NewRow();
                row["A"].Format(1);
            }
            AssertStream(leaveOpen, stream);
        }
        static void AssertStream(bool leaveOpen, MemoryStream stream)
        {
            Assert.AreEqual(stream.CanRead && stream.CanWrite && stream.CanSeek, leaveOpen);
            var actual = Encoding.UTF8.GetString(stream.ToArray());
            Assert.AreEqual("""
                        A
                        1
                        
                        """, actual);
        }
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async ValueTask SepWriterTest_Extensions_ToTextWriter_LeaveOpen(bool leaveOpen)
    {
        {
            var textWriter = new StringWriter();
            using (var writer = Sep.Writer().To(textWriter, leaveOpen))
            {
                using var row = writer.NewRow();
                row["A"].Format(1);
            }
            AssertStringWriter(leaveOpen, textWriter);
        }
        {
            var textWriter = new StringWriter();
            await using (var writer = Sep.Writer().To(textWriter, leaveOpen))
            {
                await using var row = writer.NewRow();
                row["A"].Format(1);
            }
            AssertStringWriter(leaveOpen, textWriter);
        }
        static void AssertStringWriter(bool leaveOpen, StringWriter textWriter)
        {
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
    public async ValueTask SepWriterTest_WriteHeader_False_ColMissingInSecondRow()
    {
        // Expected output should only be valid rows
        var expectedMessage = "Not all expected columns have been set.";
        var expected =
@"A;B
";
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
                Assert.AreEqual(expectedMessage, e.Message);
            }
            Assert.AreEqual(expected, writer.ToString());
        }
        {
            await using var writer = Sep.Writer(o => o with { WriteHeader = false }).ToText();
            {
                await using var row1 = writer.NewRow();
                row1[0].Set("A");
                row1[1].Set("B");
            }
            {
                var row2 = writer.NewRow();
                row2[1].Set("Y");
                var e = AssertThrowsException<InvalidOperationException>(row2,
                    r => { r.DisposeAsync().GetAwaiter().GetResult(); }); // Call sync due to row ref struct
                Assert.AreEqual(expectedMessage, e.Message);
            }
            Assert.AreEqual(expected, writer.ToString());
        }
    }

    [TestMethod]
    public async ValueTask SepWriterTest_DisableColCountCheck_ColNotSetDefaultThrow_Header_LessColumns_Throws()
    {
        var expected = "Not all expected columns 'A,B' have been set.";
        var options = new SepWriterOptions { DisableColCountCheck = true };
        {
            using var writer = options.ToText();
            {
                using var row = writer.NewRow();
                row["A"].Set("R1C1");
                row["B"].Set("R1C2");
            }
            var e = Assert.ThrowsException<InvalidOperationException>(() =>
            {
                using var row = writer.NewRow();
                row["B"].Set("R2C2");
            });
            Assert.AreEqual(expected, e.Message);
        }
        {
            await using var writer = options.ToText();
            {
                using var row = writer.NewRow();
                row["A"].Set("R1C1");
                row["B"].Set("R1C2");
            }
            var e = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await using var row = writer.NewRow();
                row["B"].Set("R2C2");
            });
            Assert.AreEqual(expected, e.Message);
        }
    }

    [TestMethod]
    public void SepWriterTest_DisableColCountCheck_ColNotSetDefaultThrow_Header_MoreColumns_Ok()
    {
        var options = new SepWriterOptions { DisableColCountCheck = true };
        using var writer = options.ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("R1C1");
            row["B"].Set("R1C2");
        }
        {
            using var row = writer.NewRow();
            row["A"].Set("R2C1");
            row["B"].Set("R2C2");
            row[2].Set("R2C3");
        };
        var expected =
@"A;B
R1C1;R1C2
R2C1;R2C2;R2C3
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_DisableColCountCheck_ColNotSetSkip_Header()
    {
        var options = new SepWriterOptions
        {
            DisableColCountCheck = true,
            ColNotSetOption = SepColNotSetOption.Skip,
        };
        using var writer = options.ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("R1C1");
            row["B"].Set("R1C2");
        }
        {
            using var row = writer.NewRow();
            row["B"].Set("R2C2");
        }
        {
            using var row = writer.NewRow();
            row["A"].Set("R3C1");
        }
        {
            using var row = writer.NewRow();
            row["A"].Set("R4C1");
            row[2].Set("R4C3");
        }
        var expected =
@"A;B
R1C1;R1C2
R2C2
R3C1
R4C1;R4C3
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_DisableColCountCheck_ColNotSetEmpty_Header()
    {
        var options = new SepWriterOptions
        {
            DisableColCountCheck = true,
            ColNotSetOption = SepColNotSetOption.Empty,
        };
        using var writer = options.ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("R1C1");
            row["B"].Set("R1C2");
            row["C"].Set("R1C3");
        }
        {
            using var row = writer.NewRow();
            row["B"].Set("R2C2");
        }
        {
            using var row = writer.NewRow();
            row["A"].Set("R3C1");
            row["C"].Set("R3C3");
        }
        {
            using var row = writer.NewRow();
            row["B"].Set("R4C2");
            row[3].Set("R4C4");
        }
        var expected =
@"A;B;C
R1C1;R1C2;R1C3
;R2C2;
R3C1;;R3C3
;R4C2;;R4C4
";
        Assert.AreEqual(expected, writer.ToString());
    }


    [TestMethod]
    public async ValueTask SepWriterTest_DisableColCountCheck_ColNotSetDefaultThrow_NoHeader_LessColumns_Throws()
    {
        var options = new SepWriterOptions { WriteHeader = false, DisableColCountCheck = true };
        {
            using var writer = options.ToText();
            {
                using var row = writer.NewRow();
                row["A"].Set("R1C1");
                row["B"].Set("R1C2");
            }
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                using var row = writer.NewRow();
                row["B"].Set("R2C2");
            });
        }
        {
            await using var writer = options.ToText();
            {
                using var row = writer.NewRow();
                row["A"].Set("R1C1");
                row["B"].Set("R1C2");
            }
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await using var row = writer.NewRow();
                row["B"].Set("R2C2");
            });
        }
    }

    [TestMethod]
    public void SepWriterTest_DisableColCountCheck_ColNotSetDefaultThrow_NoHeader_MoreColumns_Ok()
    {
        var options = new SepWriterOptions { WriteHeader = false, DisableColCountCheck = true };
        using var writer = options.ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("R1C1");
            row["B"].Set("R1C2");
        }
        {
            using var row = writer.NewRow();
            row["A"].Set("R2C1");
            row["B"].Set("R2C2");
            row[2].Set("R2C3");
        };
        var expected =
@"R1C1;R1C2
R2C1;R2C2;R2C3
";
        Assert.AreEqual(expected, writer.ToString());
    }
    [TestMethod]
    public void SepWriterTest_DisableColCountCheck_ColNotSetSkip_NoHeader()
    {
        var options = new SepWriterOptions
        {
            WriteHeader = false,
            DisableColCountCheck = true,
            ColNotSetOption = SepColNotSetOption.Skip,
        };
        using var writer = options.ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("R1C1");
            row["B"].Set("R1C2");

        }
        {
            using var row = writer.NewRow();
            row[0].Set("R2C1");
            row[1].Set("R2C2");
            row[2].Set("R2C3");
            row[3].Set("R2C4");
        }
        {
            using var row = writer.NewRow();
            row["A"].Set("R3C1");
            row[2].Set("R3C3");
            row[1].Set("R3C2");
        }
        var expected =
@"R1C1;R1C2
R2C1;R2C2;R2C3;R2C4
R3C1;R3C2;R3C3
";
        Assert.AreEqual(expected, writer.ToString());
    }

    [TestMethod]
    public void SepWriterTest_DisableColCountCheck_ColNotSetEmpty_NoHeader()
    {
        var options = new SepWriterOptions
        {
            WriteHeader = false,
            DisableColCountCheck = true,
            ColNotSetOption = SepColNotSetOption.Empty,
        };
        using var writer = options.ToText();
        {
            using var row = writer.NewRow();
            row["A"].Set("R1C1");
            row["B"].Set("R1C2");
        }
        {
            using var row = writer.NewRow();
            row["B"].Set("R2C2");
        }
        {
            using var row = writer.NewRow();
            row[0].Set("R3C1");
            row[1].Set("R3C2");
            row[2].Set("R3C3");
            row[3].Set("R3C4");
        }
        {
            using var row = writer.NewRow();
            row["A"].Set("R4C1");
            row[2].Set("R4C3");
            row[1].Set("R4C2");
        }
        {
            using var row = writer.NewRow();
            row[2].Set("R5C3");
        }
        // Note how empty columns are written depending on previously written
        // maximum column count
        var expected =
@"R1C1;R1C2
;R2C2
R3C1;R3C2;R3C3;R3C4
R4C1;R4C2;R4C3;
;;R5C3;
";
        Assert.AreEqual(expected, writer.ToString());
    }

    static SepWriter CreateWriter() =>
        Sep.New(';').Writer().ToText();

    delegate void WriterRowAction(SepWriter.Row row);

    static async ValueTask AssertWriterSyncAsync(string expected, params WriterRowAction[] actions)
    {
        AssertWriterSync(expected, actions);
        await AssertWriterAsync(expected, actions);
    }

    static void AssertWriterSync(string expected, WriterRowAction[] actions)
    {
        using var writer = CreateWriter();
        foreach (var action in actions)
        {
            using var row = writer.NewRow();
            action(row);
        }
        Assert.AreEqual(expected, writer.ToString());
    }

    static async ValueTask AssertWriterAsync(string expected, WriterRowAction[] actions)
    {
        await using var writer = CreateWriter();
        foreach (var action in actions)
        {
            await using var row = writer.NewRow();
            action(row);
        }
        Assert.AreEqual(expected, writer.ToString());
    }

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
