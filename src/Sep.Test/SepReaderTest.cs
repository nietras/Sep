using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public partial class SepReaderTest
{
    // Except FromFile below since do not want to hit file system for all those small tests
    static readonly IReadOnlyList<Func<SepReaderOptions, string, SepReader>> s_fromFuncsSync = [
        (o, t) => o.FromText(t),
        (o, t) => o.From(new StringReader(t)),
        (o, t) => o.From(Encoding.UTF8.GetBytes(t)),
        (o, t) => o.From(new MemoryStream(Encoding.UTF8.GetBytes(t))),
        (o, t) => o.From("name", n => new StringReader(t)),
        (o, t) => o.From("name", n => new MemoryStream(Encoding.UTF8.GetBytes(t))),
    ];
    static readonly IReadOnlyList<Func<SepReaderOptions, string, ValueTask<SepReader>>> s_fromFuncsAsync = [
        (o, t) => o.FromTextAsync(t),
        (o, t) => o.FromAsync(new StringReader(t)),
        (o, t) => o.FromAsync(Encoding.UTF8.GetBytes(t)),
        (o, t) => o.FromAsync(new MemoryStream(Encoding.UTF8.GetBytes(t))),
        (o, t) => o.FromAsync("name", n => new StringReader(t)),
        (o, t) => o.FromAsync("name", n => new MemoryStream(Encoding.UTF8.GetBytes(t))),
    ];

    record Values(string C1, string C2, string C3);

    [TestMethod]
    public void SepReaderTest_AssetsQuotes()
    {
        using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText("38769756-9bb4-4ba9-a9a5-40362d1fd96f,2020-11-28T01:48:51.2932584+00:00,\"AspNetCore.Mvc.RangedStreamResult\",\"1.3.1\",2020-11-27T20:28:44.2430000+00:00,\"AvailableAssets\",\"RuntimeAssemblies\",\"\",\"\",\"netstandard2.1\",\"\",\"\",\"\",\"\",\"\",\"lib/netstandard2.1/AspNetCore.Mvc.RangedStreamResult.dll\",\"AspNetCore.Mvc.RangedStreamResult.dll\",\".dll\",\"lib\",\"netstandard2.1\",\".NETStandard\",\"2.1.0.0\",\"\",\"\",\"0.0.0.0\"");
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(25, row.ColCount);
    }

    [TestMethod]
    public void SepReaderTest_CreateFromStreamReader()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("A;B"));
        using var reader = Sep.Reader().From(stream);
        // 3 + 128 => Pow2
        Assert.AreEqual(512, reader.CharsLength);
    }

    [TestMethod]
    public void SepReaderTest_Spec()
    {
        using var reader = Sep.Reader().FromText("a|b|c\n,;|\t,|;;");
        var spec = reader.Spec;
        Assert.AreEqual('|', spec.Sep.Separator);
        Assert.AreSame(SepDefaults.CultureInfo, spec.CultureInfo);
    }

    [TestMethod]
    public void SepReaderTest_CreateWithPredefinedSeparator()
    {
        using var reader = Sep.New('|').Reader().FromText("a|b|c\n,;|\t,|;;");
        Assert.AreEqual('|', reader.Spec.Sep.Separator);
    }

    [TestMethod]
    public void SepReaderTest_ToString_ColIndex()
    {
        using var reader = Sep.Reader().FromText("A;B\nX;Y");
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual("X", reader.ToString(0));
        Assert.AreEqual("Y", reader.ToString(1));
    }

    [TestMethod]
    public async ValueTask SepReaderTest_From_Empty()
    {
        await FromAnySyncAsync(string.Empty, options: new(), reader =>
        {
            AssertState(reader, isEmpty: true, hasHeader: false, hasRows: false);
            Assert.AreEqual(0, reader.Header.ColNames.Count);
            Assert.IsFalse(reader.MoveNext());
        });
    }

    [TestMethod]
    public async ValueTask SepReaderTest_From_NewLine()
    {
        await FromAnySyncAsync(Environment.NewLine, options: new(), reader =>
        {
            AssertState(reader, isEmpty: false, hasHeader: true, hasRows: false);
            Assert.AreEqual(1, reader.Header.ColNames.Count);
            Assert.IsFalse(reader.MoveNext());
        });
    }

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Rows_0_HeaderOnly()
    {
        var text = "C1;C2;C3";
        var expected = Array.Empty<Values>();
        await AssertEnumerateSyncAsync(text, expected, isEmpty: false, hasHeader: true, hasRows: false);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Rows_0_NewLineAtEnd()
    {
        var text = """
                   C1;C2;C3

                   """;
        var expected = Array.Empty<Values>();
        await AssertEnumerateSyncAsync(text, expected, isEmpty: false, hasHeader: true, hasRows: false);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Rows_1()
    {
        var text = """
                   C1;C2;C3
                   ;;
                   """;
        var expected = new Values[]
        {
            new("", "", ""),
        };
        await AssertEnumerateSyncAsync(text, expected, isEmpty: false, hasHeader: true, hasRows: true);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Rows_2()
    {
        var text = """
                   C1;C2;C3
                   10;A;20.1
                   11;B;20.2
                   """;
        var expected = new Values[]
        {
            new("10", "A", "20.1"),
            new("11", "B", "20.2"),
        };
        await AssertEnumerateSyncAsync(text, expected, isEmpty: false, hasHeader: true, hasRows: true);
    }

#if NET9_0_OR_GREATER
    [TestMethod]
    public void SepReaderTest_Enumerate_As_Enumerable()
    {
        var text = """
                   C1;C2;C3
                   10;A;20.1
                   11;B;20.2
                   """;
        using var reader = Sep.Reader().FromText(text);
        var expected = new Values[]
        {
            new("10", "A", "20.1"),
            new("11", "B", "20.2"),
        };
        // NET9_0_OR_GREATER SepReader implements IEnumerable<>
        var actual = FromEnumerable(reader).ToArray();

        CollectionAssert.AreEqual(expected, actual);

        static IEnumerable<Values> FromEnumerable(
            IEnumerable<SepReader.Row> rows)
        {
            foreach (var row in rows)
            {
                yield return new(row["C1"].ToString(),
                                 row["C2"].ToString(),
                                 row["C3"].ToString());
            }
        }
    }

    [TestMethod]
    public async ValueTask SepReaderTest_AsyncEnumerate_As_AsyncEnumerable()
    {
        var text = """
                   C1;C2;C3
                   10;A;20.1
                   11;B;20.2
                   """;
        using var reader = await Sep.Reader().FromTextAsync(text);
        var expected = new Values[]
        {
            new("10", "A", "20.1"),
            new("11", "B", "20.2"),
        };
        // NET9_0_OR_GREATER SepReader implements IAsyncEnumerable<>
        var index = 0;
        await foreach (var actual in FromAsyncEnumerable(reader))
        {
            Assert.AreEqual(expected[index], actual);
            ++index;
        }

        static async IAsyncEnumerable<Values> FromAsyncEnumerable(
            IAsyncEnumerable<SepReader.Row> rows)
        {
            await foreach (var row in rows)
            {
                yield return new(row["C1"].ToString(),
                                 row["C2"].ToString(),
                                 row["C3"].ToString());
            }
        }
    }

    [TestMethod]
    public void SepReaderTest_NonGenericEnumerable_Throws()
    {
        using var reader = Sep.Reader().FromText(string.Empty);
        var enumerable = (System.Collections.IEnumerable)reader;
        Assert.ThrowsException<NotSupportedException>(() => enumerable.GetEnumerator());
    }

    [TestMethod]
    public void SepReaderTest_NonGenericEnumerator_Throws()
    {
        using var reader = Sep.Reader().FromText(string.Empty);
        var enumerator = (System.Collections.IEnumerator)reader;
        Assert.ThrowsException<NotSupportedException>(() => enumerator.Current);
        Assert.ThrowsException<NotSupportedException>(() => enumerator.Reset());
    }
#endif

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Rows_2_NewLineAtEnd()
    {
        var text = """
                   C1;C2;C3
                   10;A;20.1
                   11;B;20.2

                   """;
        var expected = new Values[]
        {
            new("10", "A", "20.1"),
            new("11", "B", "20.2"),
        };
        await AssertEnumerateSyncAsync(text, expected);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Quotes_Rows_0()
    {
        var text = "\"C1;;;\";\"C;\"2;\";;C3\"";
        var expected = Array.Empty<Values>();
        await AssertEnumerateSyncAsync(text, expected,
            isEmpty: false, hasHeader: true, hasRows: false, disableQuotesParsing: false,
            "\"C1;;;\"", "\"C;\"2", "\";;C3\"");
    }

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Quotes_Rows_2()
    {
        var text = """
                   "C1;;;";"C;"2;";;C3"
                   10;"A;";20";"11
                   "11";";"B;"20;00"
                   """;
        var expected = new Values[]
        {
            new("10", "\"A;\"", "20\";\"11"),
            new("\"11\"", "\";\"B", "\"20;00\""),
        };
        await AssertEnumerateSyncAsync(text, expected,
            isEmpty: false, hasHeader: true, hasRows: true, disableQuotesParsing: false,
            "\"C1;;;\"", "\"C;\"2", "\";;C3\"");
    }

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Quotes_Rows_2_NewLineAtEnd()
    {
        var text = """
                   C1;C2;C3
                   10;"A;";20";"11
                   "11";";"B;"20;00"

                   """;
        var expected = new Values[]
        {
            new("10", "\"A;\"", "20\";\"11"),
            new("\"11\"", "\";\"B", "\"20;00\""),
        };
        await AssertEnumerateSyncAsync(text, expected);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Quotes_Rows_3_LongCols()
    {
        var longColA = new string('A', 10000);
        var longColB = new string('B', 20000);
        var longCol0 = new string('0', 32 * 1024);
        var text = $"""
                    C1;C2;C3
                    10;"{longColA};";20";"11
                    "11";";"{longColB};"20;{longCol0}"
                    """;
        var expected = new Values[]
        {
            new("10", $"\"{longColA};\"", "20\";\"11"),
            new("\"11\"", $"\";\"{longColB}", $"\"20;{longCol0}\""),
        };
        await AssertEnumerateSyncAsync(text, expected);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_Enumerate_Quotes_DisableQuotesParsing()
    {
        var text = """
                   C1","C2",C3"
                   10,"A,"20"
                   "11","B,20"
                   """;
        var expected = new Values[]
        {
            new("10", "\"A", "\"20\""),
            new("\"11\"", "\"B", "20\""),
        };
        await AssertEnumerateSyncAsync(text, expected,
            isEmpty: false, hasHeader: true, hasRows: true, disableQuotesParsing: true,
            "C1\"", "\"C2\"", "C3\"");
    }

    [TestMethod]
    public void SepReaderTest_ColNameComparer_Default()
    {
        var text = $"""
                    Aa;Bb
                    10;11
                    """;

        using var reader = Sep.Reader().FromText(text);
        var header = reader.Header;
        reader.MoveNext();

        Assert.AreEqual(0, header.IndexOf("Aa"));
        Assert.ThrowsException<KeyNotFoundException>(() => header.IndexOf("aa"));
        Assert.AreEqual(10, reader.Current["Aa"].Parse<int>());
        Assert.ThrowsException<KeyNotFoundException>(() => reader.Current["aa"].ToString());
    }

    [TestMethod]
    public void SepReaderTest_ColNameComparer_OrdinalIgnoreCase()
    {
        var text = $"""
                    Aa;Bb
                    10;11
                    """;

        using var reader = Sep.Reader(o => o with
        { ColNameComparer = StringComparer.OrdinalIgnoreCase }).FromText(text);
        var header = reader.Header;
        reader.MoveNext();

        Assert.AreEqual(0, header.IndexOf("Aa"));
        Assert.AreEqual(0, header.IndexOf("aA"));
        Assert.ThrowsException<KeyNotFoundException>(() => header.IndexOf("X"));
        Assert.AreEqual(10, reader.Current["Aa"].Parse<int>());
        Assert.AreEqual(10, reader.Current["aA"].Parse<int>());
        Assert.ThrowsException<KeyNotFoundException>(() => reader.Current["X"].ToString());
    }

    [DataTestMethod]
    [DataRow("A;B;C;A;D;E", "Col name 'A' found 2 times at 0:'A' 3:'A' in header row 'A;B;C;A;D;E'")]
    [DataRow("A;B;C;A;D;A;E;A", "Col name 'A' found 4 times at 0:'A' 3:'A' 5:'A' 7:'A' in header row 'A;B;C;A;D;A;E;A'")]
    public async ValueTask SepReaderTest_DuplicateColumnNames_ThrowsWithDetails(string text, string expected)
    {
        {
            var e = Assert.ThrowsException<ArgumentException>(
                () => Sep.Reader().FromText(text));
            Assert.AreEqual(expected, e.Message);
        }
        {
            var e = await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await Sep.Reader().FromTextAsync(text));
            Assert.AreEqual(expected, e.Message);
        }
    }

    [DataTestMethod]
    [DataRow("A;B;C;\"A\";D;E", "Col name 'A' found 2 times at 0:'A' 3:'A' in header row 'A;B;C;\"A\";D;E'")]
    [DataRow("\"A\";B;C;A;D;\"A\";E;A", "Col name 'A' found 4 times at 0:'A' 3:'A' 5:'A' 7:'A' in header row '\"A\";B;C;A;D;\"A\";E;A'")]
    public async ValueTask SepReaderTest_DuplicateColumnNames_Unescape_ThrowsWithDetails(string text, string expected)
    {
        {
            var e = Assert.ThrowsException<ArgumentException>(
                () => Sep.Reader(o => o with { Unescape = true }).FromText(text));
            Assert.AreEqual(expected, e.Message);
        }
        {
            var e = await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await Sep.Reader(o => o with { Unescape = true }).FromTextAsync(text));
            Assert.AreEqual(expected, e.Message);
        }
    }

    [DataTestMethod]
    [DataRow("A;B;C;a;D;E", "Col name 'a' found 2 times at 0:'A' 3:'a' in header row 'A;B;C;a;D;E'")]
    [DataRow("a;B;C;A;D;A;E;a", "Col name 'A' found 4 times at 0:'a' 3:'A' 5:'A' 7:'a' in header row 'a;B;C;A;D;A;E;a'")]
    public async ValueTask SepReaderTest_DuplicateColumnNames_ColNameComparerOrdinalIgnoreCase_ThrowsWithDetails(string text, string expected)
    {
        {
            var e = Assert.ThrowsException<ArgumentException>(
                () => Sep.Reader(o => o with { ColNameComparer = StringComparer.OrdinalIgnoreCase }).FromText(text));
            Assert.AreEqual(expected, e.Message);
        }
        {
            var e = await Assert.ThrowsExceptionAsync<ArgumentException>(
                async () => await Sep.Reader(o => o with { ColNameComparer = StringComparer.OrdinalIgnoreCase }).FromTextAsync(text));
            Assert.AreEqual(expected, e.Message);
        }
    }

    [TestMethod]
    public void SepReaderTest_Info_Ctor()
    {
        var info = new SepReader.Info("A", i => "B");
        Assert.IsNotNull(info.Source);
        Assert.IsNotNull(info.DebuggerDisplay);
    }

    [TestMethod]
    public void SepReaderTest_Info_Props()
    {
        var info = new SepReader.Info() { Source = "A", DebuggerDisplay = i => "B" };
        Assert.IsNotNull(info.Source);
        Assert.IsNotNull(info.DebuggerDisplay);
    }

    // TODO: Need test of quotes at end

#pragma warning disable IDE0055
    public static IEnumerable<object[]> ColCountMismatchData =>
    [
        ["""
         C1;C2
         123
         """,
         """
         Found 1 column(s) on row 1/lines [2..3]:'123'
         Expected 2 column(s) matching header/first row 'C1;C2'
         """,
         new[] { 2, 1 }],
        ["""
         C1;C2
         
         1;2
         """,
         """
         Found 1 column(s) on row 1/lines [2..3]:''
         Expected 2 column(s) matching header/first row 'C1;C2'
         """,
         new[] { 2, 1, 2 }],
        ["""
         C1;C2;C3
         1;2;3
         4;5
         """,
         """
         Found 2 column(s) on row 2/lines [3..4]:'4;5'
         Expected 3 column(s) matching header/first row 'C1;C2;C3'
         """,
         new[] { 3, 3, 2 }],
        ["""
         C1;C2;C3
         4;5
         1;2;3
         """,
         """
         Found 2 column(s) on row 1/lines [2..3]:'4;5'
         Expected 3 column(s) matching header/first row 'C1;C2;C3'
         """,
         new[] { 3, 2, 3 }],
        ["""
         C1
         
         4;5
         """,
         """
         Found 2 column(s) on row 2/lines [3..4]:'4;5'
         Expected 1 column(s) matching header/first row 'C1'
         """,
         new[] { 1, 1, 2 }],
        ["""
         C1;C2
         4;5
         1;2;3
         """,
         """
         Found 3 column(s) on row 2/lines [3..4]:'1;2;3'
         Expected 2 column(s) matching header/first row 'C1;C2'
         """,
         new[] { 2, 2, 3 }],
        ["""
         C1;C2
         4";"5
         1;2;345
         """,
         """
         Found 1 column(s) on row 1/lines [2..3]:'4";"5'
         Expected 2 column(s) matching header/first row 'C1;C2'
         """,
         new[] { 2, 1, 3 }],
    ];
#pragma warning restore IDE0055

    [DataTestMethod]
    [DynamicData(nameof(ColCountMismatchData))]
    public void SepReaderTest_ColumnCountMismatch_Throws(
        string text, string message, int[] _)
    {
        var e = Assert.ThrowsException<InvalidDataException>(() =>
        {
            using var reader = Sep.Reader().FromText(text);
            foreach (var readRow in reader)
            { }
        });
        Assert.AreEqual(message, e.Message);
    }

    [DataTestMethod]
    [DynamicData(nameof(ColCountMismatchData))]
    public void SepReaderTest_ColumnCountMismatch_DisableColCountCheck(
        string text, string _, int[] expectedColCounts)
    {
        Contract.Assume(expectedColCounts != null);
        using var reader = Sep.Reader(o => o with { DisableColCountCheck = true }).FromText(text);
        var colCountIndex = 0;
        Assert.AreEqual(expectedColCounts[colCountIndex], reader.Header.ColNames.Count);
        foreach (var readRow in reader)
        {
            ++colCountIndex;
            Assert.AreEqual(expectedColCounts[colCountIndex], readRow.ColCount);
            for (var colIndex = 0; colIndex < readRow.ColCount; colIndex++)
            {
                // Ensure ToString works, since may be outside SepToString bounds
                Assert.IsNotNull(readRow[colIndex].ToString());
            }
        }
    }

    [DataTestMethod]
    [DynamicData(nameof(ColCountMismatchData))]
    public void SepReaderTest_ColumnCountMismatch_IgnoreThrows(
        string text, string _, int[] expectedColCounts)
    {
        Contract.Assume(expectedColCounts != null);
        using var reader = Sep.Reader().FromText(text);
        var colCountIndex = 0;
        Assert.AreEqual(expectedColCounts[colCountIndex], reader.Header.ColNames.Count);
        var moveNext = false;
        do
        {
            // Assert that even if ignoring exception thrown, the reader
            // continues with next rows.
            try
            {
                moveNext = reader.MoveNext();
            }
            catch (InvalidDataException)
            {
                moveNext = true;
            }
            if (moveNext)
            {
                var readRow = reader.Current;
                ++colCountIndex;
                Assert.AreEqual(expectedColCounts[colCountIndex], readRow.ColCount);
                for (var colIndex = 0; colIndex < readRow.ColCount; colIndex++)
                {
                    // Ensure ToString works, since may be outside SepToString bounds
                    Assert.IsNotNull(readRow[colIndex].ToString());
                }
            }
        } while (moveNext);
    }


#pragma warning disable IDE0055
    public static IEnumerable<object[]> LineNumbersData =>
    [
        ["C1;C2\n123;456", new[] { (1, 2), (2, 3) }],
        ["C1;C2\n123;456\n", new[] { (1, 2), (2, 3) }],
        ["C1;C2\r\n123;456\r\n", new[] { (1, 2), (2, 3) }],
        ["C1;C2\r123;456\r", new[] { (1, 2), (2, 3) }],
        ["C1;C2\n123;456\n789;012\n", new[] { (1, 2), (2, 3), (3, 4) }],
        // Line endings in quotes
        ["""
         "C1
         ;
         ";C2
         "ab
         ";"cd
         ;
         e"
         "
         
         
         
         1";2
         """, new[] { (1, 4), (4, 8), (8, 13) }],
        ["\"C1\n\";C2\n\"1\n2\r3\";\"4\r\n56\"\n\"7\r\r\r\r\r89\";012\n",
         new[] { (1, 3), (3, 7), (7, 13) }],
    ];
#pragma warning restore IDE0055

    [DataTestMethod]
    [DynamicData(nameof(LineNumbersData))]
    public void SepReaderTest_LineNumbers(string text,
        (int LineNumberFrom, int LineNumberToExcl)[] expectedLineNumbers)
    {
        Contract.Assume(expectedLineNumbers != null);
        // Repeat test for with/without header
        foreach (var options in new[] { new SepReaderOptions(),
                                        new SepReaderOptions() { HasHeader = false } })
        {
            using var reader = Sep.Reader().FromText(text);
            foreach (var row in reader)
            {
                var e = expectedLineNumbers[row.RowIndex];
                Assert.AreEqual(e, (row.LineNumberFrom, row.LineNumberToExcl));
                Assert.AreEqual(e.LineNumberFrom, row.LineNumberFrom, nameof(row.LineNumberFrom));
                Assert.AreEqual(e.LineNumberToExcl, row.LineNumberToExcl, nameof(row.LineNumberToExcl));
            }
        }
    }

    [TestMethod]
    public void SepReaderTest_HandleReallyLongHeader()
    {
#if SEPREADERTRACE // Don't run really long with tracing enabled 😅
        const int lengthA = 1267;
        const int lengthB = 237;
#else
        const int lengthA = 1267 + 64 * 1024;
        const int lengthB = 237;
#endif
        var text = $"{new string('a', lengthA)};{new string('b', lengthB)}";

        using var reader = Sep.Reader().FromText(text);

        var colNames = reader.Header.ColNames;
        Assert.AreEqual(lengthA, colNames[0].Length);
        Assert.AreEqual(lengthB, colNames[1].Length);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async ValueTask SepReaderTest_CarriageReturnLineFeedEvenOrOdd_ToEnsureLineFeedReadAfterCarriageReturn(bool even)
    {
#if SEPREADERTRACE // Don't run really long with tracing enabled 😅
        const int lineEndingCount = 167;
#else
        const int lineEndingCount = 1267 + 64 * 1024;
#endif
        var lineEnding = "\r\n";
        var lineEndingStartIndex = (even ? 0 : 1);
        var sb = new StringBuilder(lineEndingCount * lineEnding.Length + lineEndingStartIndex);
        // Add space if odd
        if (!even) { sb.Append(' '); }
        sb.Insert(lineEndingStartIndex, lineEnding, lineEndingCount);
        var text = sb.ToString();
        await AssertLineEndings(lineEndingCount, text);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_CarriageReturn_ToEnsureTrailingCarriageReturnHandled()
    {
#if SEPREADERTRACE // Don't run really long with tracing enabled 😅
        const int lineEndingCount = 167;
#else
        const int lineEndingCount = 1267 + 64 * 1024;
#endif
        var text = new string('\r', lineEndingCount);
        await AssertLineEndings(lineEndingCount, text);
    }

    [TestMethod]
    public void SepReaderTest_ColsInitialLength()
    {
        var colCount = SepReader.ColEndsInitialLength - 1; // -1 since col ends is 1 longer due to having row start
        var text = "A" + Environment.NewLine + new string(';', colCount - 1);
        using var reader = Sep.Reader(o => o with { DisableColCountCheck = true }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(colCount, row.ColCount);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_ExceedingColsInitialLength_WorksByDoublingCapacity()
    {
        var colCount = SepReader.ColEndsInitialLength;
        var text = "A" + Environment.NewLine + new string(';', colCount - 1);

        await FromSyncAsync(new() { DisableColCountCheck = true },
            o => o.FromText(text), o => o.FromTextAsync(text), reader =>
        {
            Assert.IsTrue(reader.MoveNext());
            var row = reader.Current;
            Assert.AreEqual(colCount, row.ColCount);
            Assert.AreEqual(colCount * 2, reader._colEndsOrColInfos.Length);
        });
    }

    [TestMethod]
    public async ValueTask SepReaderTest_ColInfosLength_ArgumentOutOfRangeException_Issue_108()
    {
        // At any time during parsing there may be an incomplete row e.g. a
        // parsing row, when then new rows are about to be parsed e.g. in
        // ParseNewRows(). The col ends/infos for that row need to be copied to
        // beginning before new rows are found. At any time these col infos
        // should never exceed the end of the array of col infos. However, a bug
        // was present <= 0.4.3 as reported in issue #108
        // https://github.com/nietras/Sep/issues/108 where this was the case and
        // an `ArgumentOutOfRangeException` would occur on the slicing that
        // happens when these col infos are to be copied to beginning. This test
        // triggers that issue.
        var colCounts = Enumerable.Range(SepReader.ColEndsInitialLength - 1, 1);
        var charsLength = (int)BitOperations.RoundUpToPowerOf2(SepDefaults.InitialBufferLength);
        foreach (var colCount in colCounts)
        {
            var text = new string('A', Math.Max(1, charsLength - colCount + 1))
                + new string(';', colCount - 1) + Environment.NewLine
                + new string(';', colCount * 2);
            {
                using var reader = Sep
                    .Reader(o => o with { HasHeader = false, DisableColCountCheck = true })
                    .FromText(text);
                while (reader.MoveNext()) { }
            }
            {
                using var reader = await Sep
                    .Reader(o => o with { HasHeader = false, DisableColCountCheck = true })
                    .FromTextAsync(text);
                while (await reader.MoveNextAsync()) { }
            }
        }
    }

#if !SEPREADERTRACE // Causes OOMs in Debug due to tracing
    [TestMethod]
    public async ValueTask SepReaderTest_TooLongRow_Throws()
    {
        var maxLength = SepDefaults.RowLengthMax + 1;
        var text = new string('a', maxLength);
        var expected = $"Buffer or row has reached maximum supported length of 16777216. If no such row should exist ensure quotes \" are terminated.";
        {
            var e = Assert.ThrowsException<NotSupportedException>(() =>
                Sep.Reader().FromText(text));
            Assert.AreEqual(expected, e.Message);
        }
        {
            var e = await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
                await Sep.Reader().FromTextAsync(text));
            Assert.AreEqual(expected, e.Message);
        }
    }
#endif

    [TestMethod]
    public void SepReaderTest_Extensions_FromFile()
    {
        const string text = """
                            A;B
                            1;2
                            """;
        const string fileName = nameof(SepReaderTest_Extensions_FromFile) + ".csv";
        File.WriteAllText(fileName, text);

        using var reader = Sep.Auto.Reader().FromFile(fileName);

        Assert.AreEqual(true, reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(1, row["A"].Parse<int>());
    }

    [TestMethod]
    public async ValueTask SepReaderTest_TextReaderLengthLongerThan32Bit()
    {
        const string text = """
                            A;B
                            1;2
                            """;
        var utf8Bytes = Encoding.UTF8.GetBytes(text);
        var func = () => new FakeLongMemoryStream(utf8Bytes, int.MaxValue + 1L);

        await FromSyncAsync(new(), o => o.From(func()), o => o.FromAsync(func()), r =>
        {
            Assert.IsTrue(r.MoveNext());
        });
    }

    [TestMethod]
    public async ValueTask SepReaderTest_DebuggerDisplay_FromText()
    {
        var t = "A;B";
        await FromSyncAsync(new(), o => o.FromText(t), o => o.FromTextAsync(t), r =>
        {
            Assert.AreEqual("String Length=3", r.DebuggerDisplay);
        });
    }

    [TestMethod]
    public async ValueTask SepReaderTest_DebuggerDisplay_FromFile()
    {
        var filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, "A;B");
        await FromSyncAsync(new(), o => o.FromFile(filePath), o => o.FromFileAsync(filePath), r =>
        {
            Assert.AreEqual($"File='{filePath}'", r.DebuggerDisplay);
        });
        File.Delete(filePath);
    }

    [TestMethod]
    public async ValueTask SepReaderTest_DebuggerDisplay_FromBytes()
    {
        var bytes = Encoding.UTF8.GetBytes("A;B");
        await FromSyncAsync(new(), o => o.From(bytes), o => o.FromAsync(bytes), r =>
        {
            Assert.AreEqual($"Bytes Length=3", r.DebuggerDisplay);
        });
    }

    [TestMethod]
    public async ValueTask SepReaderTest_DebuggerDisplay_FromNameStream()
    {
        var name = "TEST";
        var func = (string n) => (Stream)new MemoryStream(Encoding.UTF8.GetBytes("A;B"));
        await FromSyncAsync(new(), o => o.From(name, func), o => o.FromAsync(name, func), r =>
        {
            Assert.AreEqual($"Stream Name='{name}'", r.DebuggerDisplay);
        });
    }
    [TestMethod]
    public async ValueTask SepReaderTest_DebuggerDisplay_FromStream()
    {
        var func = () => (Stream)new MemoryStream(Encoding.UTF8.GetBytes("A;B"));
        await FromSyncAsync(new(), o => o.From(func()), o => o.FromAsync(func()), r =>
        {
            Assert.AreEqual($"Stream='{typeof(MemoryStream)}'", r.DebuggerDisplay);
        });
    }

    [TestMethod]
    public async ValueTask SepReaderTest_DebuggerDisplay_FromNameTextReader()
    {
        var name = "TEST";
        var func = (string n) => (TextReader)new StringReader("A;B");
        await FromSyncAsync(new(), o => o.From(name, func), o => o.FromAsync(name, func), r =>
        {
            Assert.AreEqual($"TextReader Name='{name}'", r.DebuggerDisplay);
        });
    }
    [TestMethod]
    public async ValueTask SepReaderTest_DebuggerDisplay_FromTextReader()
    {
        var func = () => (TextReader)new StringReader("A;B");
        await FromSyncAsync(new(), o => o.From(func()), o => o.FromAsync(func()), r =>
        {
            Assert.AreEqual($"TextReader='{typeof(StringReader)}'", r.DebuggerDisplay);
        });
    }

    class FakeLongMemoryStream(byte[] buffer, long fakeLength) : MemoryStream(buffer)
    {
        public override long Length => fakeLength;
    }

    static async ValueTask AssertEnumerateSyncAsync(
        string text, Values[] expected,
        bool isEmpty = false, bool hasHeader = true, bool hasRows = true, bool disableQuotesParsing = false,
        string colName1 = "C1", string colName2 = "C2", string colName3 = "C3")
    {
        AssertEnumerateSync(text, expected, isEmpty, hasHeader, hasRows,
            disableQuotesParsing, colName1, colName2, colName3);

        await AssertEnumerateAsync(text, expected, isEmpty, hasHeader, hasRows,
            disableQuotesParsing, colName1, colName2, colName3);
    }

    static void AssertEnumerateSync(string text, Values[] expected,
        bool isEmpty, bool hasHeader, bool hasRows = true, bool disableQuotesParsing = false,
        string colName1 = "C1", string colName2 = "C2", string colName3 = "C3")
    {
        using var reader = Sep
            .Reader(o => o with { DisableQuotesParsing = disableQuotesParsing, HasHeader = hasHeader })
            .FromText(text);

        var actual = EnumerateSync(reader, hasHeader, colName1, colName2, colName3).ToList();

        AssertEnumerateResults(reader, isEmpty,
            hasHeader, hasRows, colName1, colName2, colName3,
            expected, actual);
    }

    static IEnumerable<Values> EnumerateSync(SepReader reader, bool hasHeader,
        string colName1, string colName2, string colName3)
    {
        if (hasHeader)
        {
            foreach (var row in reader)
            {
                yield return new(row[colName1].ToString(), row[colName2].ToString(), row[colName3].ToString());
            }
        }
        else
        {
            foreach (var row in reader)
            {
                yield return new(row[0].ToString(), row[1].ToString(), row[2].ToString());
            }
        }
    }

    static async ValueTask AssertEnumerateAsync(string text, Values[] expected,
        bool isEmpty, bool hasHeader, bool hasRows = true, bool disableQuotesParsing = false,
        string colName1 = "C1", string colName2 = "C2", string colName3 = "C3")
    {
        using var reader = await Sep
            .Reader(o => o with { DisableQuotesParsing = disableQuotesParsing, HasHeader = hasHeader })
            .FromTextAsync(text);

        var actual = await EnumerateAsync(reader, hasHeader, colName1, colName2, colName3);

        AssertEnumerateResults(reader, isEmpty,
            hasHeader, hasRows, colName1, colName2, colName3,
            expected, actual);
    }

    static async ValueTask<List<Values>> EnumerateAsync(
        SepReader reader, bool hasHeader,
        string colName1, string colName2, string colName3)
    {
        var results = new List<Values>();
        if (hasHeader)
        {
            await foreach (var row in reader)
            {
                results.Add(new(row[colName1].ToString(), row[colName2].ToString(), row[colName3].ToString()));
            }
        }
        else
        {
            await foreach (var row in reader)
            {
                results.Add(new(row[0].ToString(), row[1].ToString(), row[2].ToString()));
            }
        }
        return results;
    }

    static void AssertEnumerateResults(SepReader reader,
        bool isEmpty, bool hasHeader, bool hasRows,
        string colName1, string colName2, string colName3,
        Values[] expected,
        List<Values> actual)
    {
        AssertState(reader, isEmpty, hasHeader, hasRows);
        if (hasHeader) { AssertHeader(reader.Header, colName1, colName2, colName3); }
        else { AssertHeaderEmpty(reader.Header); }
        CollectionAssert.AreEqual(expected, actual);
    }

    static void AssertState(SepReader reader, bool isEmpty, bool hasHeader, bool hasRows)
    {
        Assert.AreEqual(isEmpty, reader.IsEmpty, nameof(reader.IsEmpty));
        Assert.AreEqual(hasHeader, reader.HasHeader, nameof(reader.HasHeader));
        Assert.AreEqual(hasRows, reader.HasRows, nameof(reader.HasRows));
    }

    static void AssertHeader(SepReaderHeader header, string colName1, string colName2, string colName3)
    {
        Assert.AreEqual(3, header.ColNames.Count);
        Assert.AreEqual(0, header.IndexOf(colName1));
        Assert.AreEqual(1, header.IndexOf(colName2));
        Assert.AreEqual(2, header.IndexOf(colName3));
    }

    static void AssertHeaderEmpty(SepReaderHeader header)
    {
        Assert.AreEqual(0, header.ColNames.Count);
        Assert.IsTrue(header.IsEmpty);
    }

    static async ValueTask FromAnySyncAsync(string text, SepReaderOptions options, Action<SepReader> assert)
    {
        // Sync
        foreach (var fromFuncSync in s_fromFuncsSync)
        {
            using var reader = fromFuncSync(options, text);
            assert(reader);
        }
        // Async
        foreach (var fromFuncAsync in s_fromFuncsAsync)
        {
            using var reader = await fromFuncAsync(options, text);
            assert(reader);
        }
    }

    static async ValueTask FromSyncAsync(
        SepReaderOptions options,
        Func<SepReaderOptions, SepReader> fromFuncSync,
        Func<SepReaderOptions, ValueTask<SepReader>> fromFuncAsync,
        Action<SepReader> assert)
    {
        // Sync
        {
            using var reader = fromFuncSync(options);
            assert(reader);
        }
        // Async
        {
            using var reader = await fromFuncAsync(options);
            assert(reader);
        }
    }

    static async Task AssertLineEndings(int lineEndingCount, string text)
    {
        await AssertLineEndings(lineEndingCount, () => new StringReader(text));
        await AssertLineEndings(lineEndingCount, () => new NoPeekStringReader(text));
    }

    static async Task AssertLineEndings(int lineEndingCount, Func<StringReader> createReader)
    {
        {
            using var reader = Sep.Reader(o => o with { HasHeader = false }).From(createReader());
            var lineCount = 0;
            foreach (var row in reader) { ++lineCount; }
            Assert.AreEqual(lineEndingCount, lineCount);
        }
        {
            using var reader = await Sep.Reader(o => o with { HasHeader = false }).FromAsync(createReader());
            var lineCount = 0;
            await foreach (var row in reader) { ++lineCount; }
            Assert.AreEqual(lineEndingCount, lineCount);
        }
    }
    // Sep previously depended on Peek to check if \r followed by \n. This
    // cannot be used reliably, though, since Peek may return -1.
    class NoPeekStringReader(string s) : StringReader(s)
    {
        public override int Peek() => -1;
    }
}
