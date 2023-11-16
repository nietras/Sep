using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderFuzzTest
{
    static readonly char Separator = SepDefaults.Separator;
    readonly Random _random = new(23768213);

    readonly record struct TestCol(string Raw, string Expected, string ExpectedInRow);
    readonly record struct TestRow(string Raw, string Expected, TestCol[] Cols);

    [DataRow(false, 500, 20, false, 16)]
    [DataRow(true, 500, 20, false, 16)]
#if !DEBUG
    [DataRow(false, 500, 40, true, 16)]
    [DataRow(true, 500, 40, true, 16)]
    [DataRow(false, 5000, 40, false, 64)]
    [DataRow(true, 5000, 40, false, 64)]
#endif
    [DataTestMethod]
    public void SepReaderFuzzTest_Fuzz(bool unescape, int rowCount, int maxColCount, bool colCountSame, int maxColLength)
    {
        var colCount = _random.Next(0, maxColCount);
        var sbRowRaw = new StringBuilder();
        var sbRowExpected = new StringBuilder();
        var expectedRows = Enumerable.Range(0, rowCount).Select(_ =>
            GenerateRandomTestRow(_random, sbRowRaw, sbRowExpected,
                                  colCount, colCountSame, maxColLength, unescape))
            .ToArray();
        var text = GetTestText(_random, expectedRows);

        using var reader = Sep
            .Reader(o => o with { HasHeader = false, Unescape = unescape, DisableColCountCheck = !colCountSame })
            .FromText(text);
        // Verify reader same as rows
        var moveNext = false;
        var rowIndex = 0;
        while ((moveNext = reader.MoveNext() && rowIndex < expectedRows.Length))
        {
            var expectedRow = expectedRows[rowIndex];
            var expectedCols = expectedRow.Cols;
            var readRow = reader.Current;
            Assert.AreEqual(expectedCols.Length, readRow.ColCount);

            var actualRowBeforeUnescape = readRow.Span.ToString();
            Assert.AreEqual(expectedRow.Raw, actualRowBeforeUnescape);

            for (var colIndex = 0; colIndex < expectedCols.Length; colIndex++)
            {
                var col = expectedCols[colIndex];
                var readerCol = readRow[colIndex];
                Assert.AreEqual(col.Expected, readerCol.ToString());
            }

            var actualRowAfterUnescape = readRow.Span.ToString();
            Assert.AreEqual(expectedRow.Expected, actualRowAfterUnescape);

            ++rowIndex;
        }
        Assert.AreEqual(!moveNext, rowIndex == expectedRows.Length, "MoveNext and rowIndex should match");
    }

    static string GetTestText(Random random, TestRow[] rows)
    {
        var sb = new StringBuilder(1024 * 1024);
        var previousNewLine = "";
        foreach (var row in rows)
        {
            // Use indexing
            var cols = row.Cols;
            for (var colIndex = 0; colIndex < cols.Length; colIndex++)
            {
                sb.Append(cols[colIndex].Raw);
                if (colIndex < cols.Length - 1)
                {
                    sb.Append(Separator);
                }
            }
            var newLine = RandomNewLine(random);
            // Avoid a new line that does not end up actually being a new line
            newLine = newLine == "\n" && previousNewLine == "\r" ? "\r\n" : newLine;
            sb.Append(newLine);
            previousNewLine = newLine;
        }
        var text = sb.ToString();
        return text;
    }

    static TestRow GenerateRandomTestRow(Random random, StringBuilder sbRaw, StringBuilder sbExpected,
        int colCount, bool colCountSame, int maxColLength, bool unescape)
    {
        sbRaw.Clear();
        sbExpected.Clear();
        // Always have 1 col even if empty
        colCount = colCountSame ? colCount : random.Next(1, Math.Max(1, colCount) * 2);
        var cols = new TestCol[colCount];
        for (var colIndex = 0; colIndex < colCount; colIndex++)
        {
            var col = GenerateRandomTestCol(random, maxColLength, unescape);
            cols[colIndex] = col;
            sbRaw.Append(col.Raw);
            sbExpected.Append(col.ExpectedInRow);
            Assert.AreEqual(col.Raw.Length, col.ExpectedInRow.Length);
            if (colIndex != (colCount - 1))
            {
                sbRaw.Append(Separator);
                sbExpected.Append(Separator);
            }
        }
        return new(sbRaw.ToString(), sbExpected.ToString(), cols);
    }

    static TestCol GenerateRandomTestCol(Random random, int maxColLength, bool unescape)
    {
        var colLength = random.Next(0, maxColLength);
        Span<char> source = stackalloc char[colLength];
        Span<char> unescaped = stackalloc char[colLength];
        Span<char> unescapedInRow = stackalloc char[colLength];
        var unescapedLength = 0;
        var quoteCount = 0;
        var firstCharQuote = false;
        for (var i = 0; i < colLength; i++)
        {
            var c = GenerateRandomChar(random, quoteCount);
            var isQuote = c == SepDefaults.Quote;
            // if last index and 
            if (i == (colLength - 1))
            {
                // Quote count uneven, always use quote
                if ((quoteCount & 1) == 1)
                {
                    c = SepDefaults.Quote;
                    isQuote = true;
                }
                // Quote count even, make sure not quote at end
                else if (isQuote)
                {
                    c = 'a';
                    isQuote = false;
                }
            }
            firstCharQuote |= i == 0 && isQuote;
            quoteCount += isQuote ? 1 : 0;
            source[i] = c;
            // Unescape that is skip char in unescaped if first char is a quote and
            // if either first char or current char is a quote and quote count is even
            var unescapeChar = unescape && firstCharQuote && (i == 0 || (isQuote && ((quoteCount & 1) == 0)));
            if (!unescapeChar)
            {
                unescaped[unescapedLength] = c;
                ++unescapedLength;
            }
        }

        source.CopyTo(unescapedInRow);
        if (firstCharQuote && unescape && !(quoteCount == 2 && source[^1] == SepDefaults.Quote))
        {
            // Use Unescape directly for how unescaped looks in row, actual
            // unescaping is checked via manually unescaped
            SepUnescape.UnescapeInPlace(ref MemoryMarshal.GetReference(unescapedInRow), unescapedInRow.Length);
        }
        var sourceString = new string(source);
        Assert.IsTrue((source.ToArray().Count(c => c == '"') & 1) == 0);
        return new(sourceString, new string(unescaped.Slice(0, unescapedLength)), new(unescapedInRow));
    }

    static char GenerateRandomChar(Random random, int quoteCount)
    {
        // Generate random specific chars based on hard-coded probabilities
        var quoting = (quoteCount & 1) == 1;
        var p = random.NextDouble();
        return (quoting, p) switch
        {
            (_, < 0.2) => SepDefaults.Quote,
            (true, < 0.4) => Separator,
            (true, < 0.5) => '\r',
            (true, < 0.6) => '\n',
#if DEBUG
            _ => 'a',
#else
            // Be sure values larger than byte are correctly handled too (e.g. due to narrowing)
            _ => (char)random.Next(Math.Max(Separator, SepDefaults.Quote) + 1, 256 * 2),
#endif
        };
    }

    static string RandomNewLine(Random random)
    {
        return random.Next(0, 3) switch { 0 => "\r\n", 1 => "\n", 2 => "\r", _ => Environment.NewLine };
    }
}
