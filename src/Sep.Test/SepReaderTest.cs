using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderTest
{
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
        Assert.AreEqual(256, reader.CharsLength);
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
    public void SepReaderTest_Enumerate_Empty()
    {
        var text = string.Empty;
        var expected = Array.Empty<(string c1, string c2, string c3)>();
        AssertEnumerate(text, expected, isEmpty: true, hasHeader: false, hasRows: false);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_0()
    {
        var text = "C1;C2;C3";
        var expected = Array.Empty<(string c1, string c2, string c3)>();
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: false);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_0_NewLineAtEnd()
    {
        var text = """
                   C1;C2;C3

                   """;
        var expected = Array.Empty<(string c1, string c2, string c3)>();
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: false);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_1()
    {
        var text = """
                   C1;C2;C3
                   ;;
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("", "", ""),
        };
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: true);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_2()
    {
        var text = """
                   C1;C2;C3
                   10;A;20.1
                   11;B;20.2
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "A", "20.1"),
            ("11", "B", "20.2"),
        };
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: true);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Rows_2_NewLineAtEnd()
    {
        var text = """
                   C1;C2;C3
                   10;A;20.1
                   11;B;20.2

                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "A", "20.1"),
            ("11", "B", "20.2"),
        };
        AssertEnumerate(text, expected);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Quotes_Rows_0()
    {
        var text = "\"C1;;;\";\"C;\"2;\";;C3\"";
        var expected = Array.Empty<(string c1, string c2, string c3)>();
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: false,
                        "\"C1;;;\"", "\"C;\"2", "\";;C3\"");
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Quotes_Rows_2()
    {
        var text = """
                   "C1;;;";"C;"2;";;C3"
                   10;"A;";20";"11
                   "11";";"B;"20;00"
                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "\"A;\"", "20\";\"11"),
            ("\"11\"", "\";\"B", "\"20;00\""),
        };
        AssertEnumerate(text, expected, isEmpty: false, hasHeader: true, hasRows: true,
                        "\"C1;;;\"", "\"C;\"2", "\";;C3\"");
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Quotes_Rows_2_NewLineAtEnd()
    {
        var text = """
                   C1;C2;C3
                   10;"A;";20";"11
                   "11";";"B;"20;00"

                   """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", "\"A;\"", "20\";\"11"),
            ("\"11\"", "\";\"B", "\"20;00\""),
        };
        AssertEnumerate(text, expected);
    }

    [TestMethod]
    public void SepReaderTest_Enumerate_Quotes_Rows_3_LongCols()
    {
        var longColA = new string('A', 10000);
        var longColB = new string('B', 20000);
        var longCol0 = new string('0', 32 * 1024);
        var text = $"""
                    C1;C2;C3
                    10;"{longColA};";20";"11
                    "11";";"{longColB};"20;{longCol0}"
                    """;
        var expected = new (string c1, string c2, string c3)[]
        {
            ("10", $"\"{longColA};\"", "20\";\"11"),
            ("\"11\"", $"\";\"{longColB}", $"\"20;{longCol0}\""),
        };
        AssertEnumerate(text, expected);
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

    [TestMethod]
    public void SepReaderTest_Info_Ctor()
    {
        var info = new SepReader.Info("A", I => "B");
        Assert.IsNotNull(info.Source);
        Assert.IsNotNull(info.DebuggerDisplay);
    }

    [TestMethod]
    public void SepReaderTest_Info_Props()
    {
        var info = new SepReader.Info() { Source = "A", DebuggerDisplay = I => "B" };
        Assert.IsNotNull(info.Source);
        Assert.IsNotNull(info.DebuggerDisplay);
    }

    // TODO: Need test of quotes at end

    internal static IEnumerable<object[]> ColCountMismatchData => new object[][]
    {
        new object[]{ """
                      C1;C2
                      123
                      """,
                      """
                      Found 1 column(s) on row 1/lines [2..3]:'123'
                      Expected 2 column(s) matching header/first row 'C1;C2'
                      """,
                      new [] { 2, 1 } },
        new object[]{ """
                      C1;C2
                      
                      1;2
                      """,
                      """
                      Found 1 column(s) on row 1/lines [2..3]:''
                      Expected 2 column(s) matching header/first row 'C1;C2'
                      """,
                      new [] { 2, 1, 2 } },
        new object[]{ """
                      C1;C2;C3
                      1;2;3
                      4;5
                      """,
                      """
                      Found 2 column(s) on row 2/lines [3..4]:'4;5'
                      Expected 3 column(s) matching header/first row 'C1;C2;C3'
                      """,
                      new [] { 3, 3, 2 } },
        new object[]{ """
                      C1;C2;C3
                      4;5
                      1;2;3
                      """,
                      """
                      Found 2 column(s) on row 1/lines [2..3]:'4;5'
                      Expected 3 column(s) matching header/first row 'C1;C2;C3'
                      """,
                      new [] { 3, 2, 3 } },
        new object[]{ """
                      C1
                      
                      4;5
                      """,
                      """
                      Found 2 column(s) on row 2/lines [3..4]:'4;5'
                      Expected 1 column(s) matching header/first row 'C1'
                      """,
                      new [] { 1, 1, 2 } },
        new object[]{ """
                      C1;C2
                      4;5
                      1;2;3
                      """,
                      """
                      Found 3 column(s) on row 2/lines [3..4]:'1;2;3'
                      Expected 2 column(s) matching header/first row 'C1;C2'
                      """,
                      new [] { 2, 2, 3 } },
        new object[]{ """
                      C1;C2
                      4";"5
                      1;2;345
                      """,
                      """
                      Found 1 column(s) on row 1/lines [2..3]:'4";"5'
                      Expected 2 column(s) matching header/first row 'C1;C2'
                      """,
                      new [] { 2, 1, 3 } },
    };

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


    internal static IEnumerable<object[]> LineNumbersData => new object[][]
    {
        new object[]{ "C1;C2\n123;456", new [] { (1, 2), (2, 3) } },
        new object[]{ "C1;C2\n123;456\n", new [] { (1, 2), (2, 3) } },
        new object[]{ "C1;C2\r\n123;456\r\n", new [] { (1, 2), (2, 3) } },
        new object[]{ "C1;C2\r123;456\r", new [] { (1, 2), (2, 3) } },
        new object[]{ "C1;C2\n123;456\n789;012\n", new [] { (1, 2), (2, 3), (3, 4) } },
        // Line endings in quotes
        new object[]{ """
                      "C1
                      ;
                      ";C2
                      "ab
                      ";"cd
                      ;
                      e"
                      "
                      
                      
                      
                      1";2
                      """, new [] { (1, 4), (4, 8), (8, 13) } },
        new object[]{ "\"C1\n\";C2\n\"1\n2\r3\";\"4\r\n56\"\n\"7\r\r\r\r\r89\";012\n",
                      new [] { (1, 3), (3, 7), (7, 13) } },
    };

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
    public void SepReaderTest_CarriageReturnLineFeedEvenOrOdd_ToEnsureLineFeedReadAfterCarriageReturn(bool even)
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
        if (!even) { sb.Append(' '); };
        sb.Insert(lineEndingStartIndex, lineEnding, lineEndingCount);
        var text = sb.ToString();

        using var reader = Sep.Reader(o => o with { HasHeader = false }).FromText(text);
        foreach (var row in reader) { }
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
    public void SepReaderTest_ExceedingColsInitialLength_WorksByDoublingCapacity()
    {
        var colCount = SepReader.ColEndsInitialLength;
        var text = "A" + Environment.NewLine + new string(';', colCount - 1);
        using var reader = Sep.Reader(o => o with { DisableColCountCheck = true }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        Assert.AreEqual(colCount, row.ColCount);
        Assert.AreEqual(colCount * 2, reader._colEndsOrColInfos.Length);
    }

    [TestMethod]
    public void SepReaderTest_ColInfosLength_ArgumentOutOfRangeException_Issue_108()
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
        var charsLength = (int)BitOperations.RoundUpToPowerOf2(SepReader.CharsMinimumLength);
        foreach (var colCount in colCounts)
        {
            var text = new string('A', Math.Max(1, charsLength - colCount + 1))
                + new string(';', colCount - 1) + Environment.NewLine
                + new string(';', colCount * 2);
            using var reader = Sep
                .Reader(o => o with { HasHeader = false, DisableColCountCheck = true })
                .FromText(text);
            while (reader.MoveNext()) { }
        }
    }

#if !SEPREADERTRACE // Causes OOMs in Debug due to tracing
    [TestMethod]
    public void SepReaderTest_TooLongRow_Throws()
    {
        var maxLength = SepDefaults.RowLengthMax + 1;
        var text = new string('a', maxLength);
        var e = Assert.ThrowsException<NotSupportedException>(() =>
            Sep.Reader().FromText(text));
        Assert.AreEqual($"Buffer or row has reached maximum supported length of 16777216. If no such row should exist ensure quotes \" are terminated.", e.Message);
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
    public void SepReaderTest_TextReaderLengthLongerThan32Bit()
    {
        const string text = """
                            A;B
                            1;2
                            """;
        var utf8Bytes = Encoding.UTF8.GetBytes(text);
        using var fakeLongStream = new FakeLongMemoryStream(utf8Bytes, int.MaxValue + 1L);
        using var reader = Sep.Auto.Reader().From(fakeLongStream);

        Assert.AreEqual(true, reader.MoveNext());
    }

    [TestMethod]
    public void SepReaderTest_DebuggerDisplay_FromText()
    {
        using var reader = Sep.Reader().FromText("A;B");
        Assert.AreEqual("String Length=3", reader.DebuggerDisplay);
    }

    [TestMethod]
    public void SepReaderTest_DebuggerDisplay_FromFile()
    {
        var filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, "A;B");
        using (var reader = Sep.Reader().FromFile(filePath))
        {
            Assert.AreEqual($"File='{filePath}'", reader.DebuggerDisplay);
        }
        File.Delete(filePath);
    }

    [TestMethod]
    public void SepReaderTest_DebuggerDisplay_FromBytes()
    {
        using var reader = Sep.Reader().From(Encoding.UTF8.GetBytes("A;B"));
        Assert.AreEqual($"Bytes Length=3", reader.DebuggerDisplay);
    }

    [TestMethod]
    public void SepReaderTest_DebuggerDisplay_FromNameStream()
    {
        var name = "TEST";
        using var reader = Sep.Reader().From(name, n => (Stream)new MemoryStream(Encoding.UTF8.GetBytes("A;B")));
        Assert.AreEqual($"Stream Name='{name}'", reader.DebuggerDisplay);
    }
    [TestMethod]
    public void SepReaderTest_DebuggerDisplay_FromStream()
    {
        using var reader = Sep.Reader().From((Stream)new MemoryStream(Encoding.UTF8.GetBytes("A;B")));
        Assert.AreEqual($"Stream='{typeof(MemoryStream)}'", reader.DebuggerDisplay);
    }

    [TestMethod]
    public void SepReaderTest_DebuggerDisplay_FromNameTextReader()
    {
        var name = "TEST";
        using var reader = Sep.Reader().From(name, n => (TextReader)new StringReader("A;B"));
        Assert.AreEqual($"TextReader Name='{name}'", reader.DebuggerDisplay);
    }
    [TestMethod]
    public void SepReaderTest_DebuggerDisplay_FromTextReader()
    {
        using var reader = Sep.Reader().From((TextReader)new StringReader("A;B"));
        Assert.AreEqual($"TextReader='{typeof(StringReader)}'", reader.DebuggerDisplay);
    }

    public class FakeLongMemoryStream : MemoryStream
    {
        readonly long _fakeLength;

        public FakeLongMemoryStream(byte[] buffer, long fakeLength) : base(buffer)
        {
            _fakeLength = fakeLength;
        }

        public override long Length => _fakeLength;
    }

    static void AssertEnumerate(string text, (string c1, string c2, string c3)[] expected,
        bool isEmpty = false, bool hasHeader = true, bool hasRows = true,
        string colName1 = "C1", string colName2 = "C2", string colName3 = "C3")
    {
        using var reader = Sep.Reader().FromText(text);

        var actual = Enumerate(reader, colName1, colName2, colName3).ToArray();

        AssertState(reader, isEmpty, hasHeader, hasRows);
        AssertHeader(reader.Header, colName1, colName2, colName3);
        CollectionAssert.AreEqual(expected, actual);
    }

    static void AssertState(SepReader reader, bool isEmpty, bool hasHeader, bool hasRows)
    {
        Assert.AreEqual(isEmpty, reader.IsEmpty, nameof(reader.IsEmpty));
        Assert.AreEqual(hasHeader, reader.HasHeader, nameof(reader.IsEmpty));
        Assert.AreEqual(hasRows, reader.HasRows, nameof(reader.HasRows));
    }

    static void AssertHeader(SepHeader header, string colName1, string colName2, string colName3)
    {
        if (header.ColNames.Count > 0)
        {
            Assert.AreEqual(3, header.ColNames.Count);
            Assert.AreEqual(0, header.IndexOf(colName1));
            Assert.AreEqual(1, header.IndexOf(colName2));
            Assert.AreEqual(2, header.IndexOf(colName3));
        }
    }

    static IEnumerable<(string c1, string c2, string c3)> Enumerate(SepReader reader,
        string colName1, string colName2, string colName3)
    {
        foreach (var row in reader)
        {
            yield return (row[colName1].ToString(),
                          row[colName2].ToString(),
                          row[colName3].ToString());
        }
    }
}
