using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderWriterTest
{
    readonly Dictionary<string, Action<string>> _assertCopyColumns = new() {
        { nameof(AssertCopyColumnsManual), AssertCopyColumnsManual },
        { nameof(AssertCopyColumnsNewRow), AssertCopyColumnsNewRow },
    };

    // Header only copied if any other rows, this is due to how API is designed
    [DataTestMethod]
    [DataRow(@"")]
    [DataRow(@"C1
\n")]
    [DataRow(@"C1,C2
123,456")]
    [DataRow(@"C1;C2
123;456
789;012")]
    public void SepReaderWriterTest_CopyColumnsIfAnyRows(string text) =>
        AssertCopyColumns(text);

    [TestMethod]
    public void SepReaderWriterTest_CopyColumnsIfAnyRows_Long()
    {
#if SEPREADERTRACE // Don't run really long with tracing enabled 😅
        var lengths = new[] { 32, 64, 128, 512, 1024 };
#else
        var lengths = new[] { 32, 64, 128, 512, 1024, 1024 * 1024 };
#endif
        foreach (var length in lengths)
        {
            var sb = new StringBuilder(length);
            sb.Append('H');
            var i = 0;
            while (sb.Length < length)
            {
                sb.AppendLine();
                sb.Append((char)('a' + i % ('z' - 'a')));
                ++i;
            }
            var text = sb.ToString();
            AssertCopyColumns(text);
        }
    }

    [TestMethod]
    public void SepReaderWriterTest_ParseFormatExample()
    {
        var text = """
                   A;B;C
                   x;1;1.1
                   y;2;2.2
                   """;

        using var reader = Sep.Reader().FromText(text);
        using var writer = reader.Spec.Writer().ToText();
        foreach (var readRow in reader)
        {
            var a = readRow["A"].Span;
            var b = readRow["B"].Parse<int>();
            var c = readRow["C"].Parse<double>();

            using var writeRow = writer.NewRow();
            writeRow["A"].Set(a);
            writeRow["B"].Format(b * 2);
            writeRow["C"].Set($"{c / 2}");
        }

        var expected = """
                       A;B;C
                       x;2;0.55
                       y;4;1.1
                       """;
        Assert.AreEqual(expected, writer.ToString());
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(117)]
    [DataRow(17847)]
    public void SepReaderWriterTest_CopySingleEmptyColumn(int rowCount)
    {
        var newLine = Environment.NewLine;
        var expected = new StringBuilder(rowCount * newLine.Length)
            .Insert(0, newLine, rowCount).ToString();

        var lineEndings = new[] { "\r", "\r\n", "\n" };
        foreach (var lineEnding in lineEndings)
        {
            var src = new StringBuilder(rowCount * lineEnding.Length)
                .Insert(0, lineEnding, rowCount).ToString();

            using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(src);
            using var writer = reader.Spec.Writer().ToText();
            foreach (var readRow in reader)
            {
                using var writeRow = writer.NewRow();
                writeRow[""].Set(string.Empty);
            }
            // Assert
            var actual = writer.ToString();
            Assert.AreEqual(expected, actual);
        }
    }


    void AssertCopyColumns(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        foreach (var assertCopyColumns in _assertCopyColumns)
        {
            Trace.WriteLine($"{assertCopyColumns.Key} of text length {text.Length}");
            assertCopyColumns.Value(text);
        }
    }

    static void AssertCopyColumnsManual(string text)
    {
        // Act
        using var reader = Sep.Reader().FromText(text);
        using var writer = reader.Spec.Writer().ToText();
        var colNames = reader.Header.ColNames.ToArray();
        foreach (var readRow in reader)
        {
            var readCols = readRow[colNames];

            using var writeRow = writer.NewRow();
            writeRow[colNames].Set(readCols);
        }
        // Assert
        var actual = writer.ToString();
        AreEqual(text, actual);
    }

    static void AssertCopyColumnsNewRow(string text)
    {
        // Act
        using var reader = Sep.Reader().FromText(text);
        using var writer = reader.Spec.Writer().ToText();
        foreach (var readRow in reader)
        {
            using var writeRow = writer.NewRow(readRow);
        }
        // Assert
        var actual = writer.ToString();
        AreEqual(text, actual);
    }

    static void AreEqual(string text, string actual)
    {
        if (text != actual)
        {
            if (Math.Max(text.Length, actual.Length) > 1024)
            {
                Assert.Fail("Copy not equal to expected for long text.");
            }
            else
            {
                Assert.AreEqual(text, actual);
            }
        }
    }
}
