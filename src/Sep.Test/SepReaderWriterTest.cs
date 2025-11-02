using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderWriterTest
{
    readonly Dictionary<string, Func<string, ValueTask>> _assertCopyColumns = new() {
        { nameof(AssertCopyColumnsManualSyncAsync), AssertCopyColumnsManualSyncAsync },
        { nameof(AssertCopyColumnsNewRowSyncAsync), AssertCopyColumnsNewRowSyncAsync },
    };

    // Header only copied if any other rows, this is due to how API is designed
    [TestMethod]
    [DataRow(@"")]
    [DataRow(@"C1

")]
    [DataRow(@"C1,C2
123,456
")]
    [DataRow(@"C1;C2
123;456
789;012
")]
    public async ValueTask SepReaderWriterTest_CopyColumnsIfAnyRows(string text) =>
        await AssertCopyColumnsSyncAsync(text);

    [TestMethod]
    public async ValueTask SepReaderWriterTest_CopyColumnsIfAnyRows_Long()
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
            sb.AppendLine();
            var i = 0;
            while (sb.Length < length)
            {
                sb.Append((char)('a' + i % ('z' - 'a')));
                sb.AppendLine();
                ++i;
            }
            var text = sb.ToString();
            await AssertCopyColumnsSyncAsync(text);
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

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(117)]
#if !SEPREADERTRACE
    [DataRow(17847)]
#endif
    public void SepReaderWriterTest_CopySingleEmptyColumn(int rowCountWithHeader)
    {
        var newLine = Environment.NewLine;
        var expected = new StringBuilder(rowCountWithHeader * newLine.Length)
            .Insert(0, newLine, rowCountWithHeader).ToString();

        var lineEndings = new[] { "\r", "\r\n", "\n" };
        foreach (var lineEnding in lineEndings)
        {
            var src = new StringBuilder(rowCountWithHeader * lineEnding.Length)
                .Insert(0, lineEnding, rowCountWithHeader).ToString();

            using var reader = Sep.Reader().FromText(src);
            using var writer = reader.Spec.Writer().ToText();
            var actualRowCountWithHeader = reader.HasHeader ? 1 : 0;
            foreach (var readRow in reader)
            {
                using var writeRow = writer.NewRow(readRow);
                ++actualRowCountWithHeader;
            }
            // Assert
            Assert.AreEqual(rowCountWithHeader, actualRowCountWithHeader);
            var actual = writer.ToString();
            // If no rows only header then writer won't write header, this is by
            // design given how the API looks. To fix that we need to initialize
            // writer header somehow. This is something that needs to be
            // considered going forward. A source with empty column name and no
            // rows is considered a rare case.
            var overrideExpected = rowCountWithHeader != 1 ? expected : string.Empty;
            Assert.AreEqual(overrideExpected, actual);
        }
    }

    async ValueTask AssertCopyColumnsSyncAsync(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        foreach (var assertCopyColumns in _assertCopyColumns)
        {
            await assertCopyColumns.Value(text);
        }
    }

    static async ValueTask AssertCopyColumnsManualSyncAsync(string text)
    {
        AssertCopyColumnsNewRowSync(text);
        await AssertCopyColumnsNewRowAsync(text);
    }

    static void AssertCopyColumnsManualSync(string text)
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

    static async ValueTask AssertCopyColumnsManualAsync(string text)
    {
        // Act
        using var reader = await Sep.Reader().FromTextAsync(text);
        await using var writer = reader.Spec.Writer().ToText();
        var colNames = reader.Header.ColNames.ToArray();
        await foreach (var readRow in reader)
        {
            var readCols = readRow[colNames];

            await using var writeRow = writer.NewRow();
            writeRow[colNames].Set(readCols);
        }
        await writer.FlushAsync();
        // Assert
        var actual = writer.ToString();
        AreEqual(text, actual);
    }

    static async ValueTask AssertCopyColumnsNewRowSyncAsync(string text)
    {
        AssertCopyColumnsNewRowSync(text);
        await AssertCopyColumnsNewRowAsync(text);
    }

    static void AssertCopyColumnsNewRowSync(string text)
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

    static async ValueTask AssertCopyColumnsNewRowAsync(string text)
    {
        // Act
        using var reader = await Sep.Reader().FromTextAsync(text);
        await using var writer = reader.Spec.Writer().ToText();
        var cts = new CancellationTokenSource();
        await foreach (var readRow in reader)
        {
            await using var writeRow = writer.NewRow(readRow, cts.Token);
        }
        await writer.FlushAsync(cts.Token);
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
