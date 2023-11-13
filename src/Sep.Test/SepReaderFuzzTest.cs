using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderFuzzTest
{
    static readonly char Separator = SepDefaults.Separator;
    readonly Random _random = new(23768213);

    readonly record struct TestCol(string Raw, string Expected);
    readonly record struct TestRow(TestCol[] Cols);

    [DataRow(false, 1000, 128, false)]
    [DataRow(true, 1000, 128, false)]
    [DataTestMethod]
    public void SepReaderFuzzTest_Fuzz(bool unescape, int rowCount, int maxColCount, bool colCountSame)
    {
        Trace.WriteLine($"{nameof(unescape)} {unescape}");
        var colCount = _random.Next(0, maxColCount);
        var expectedRows = Enumerable.Range(0, rowCount).Select(_ =>
            GenerateRandomRow(_random, colCount, colCountSame, maxColLength: 16, unescape))
            .ToArray();
        var text = GetTestText(_random, expectedRows);

        using var reader = Sep.Reader(o => o with { HasHeader = false, Unescape = unescape, DisableColCountCheck = !colCountSame }).FromText(text);
        // Verify reader same as rows
        var moveNext = false;
        var rowIndex = 0;
        while ((moveNext = reader.MoveNext() && rowIndex < expectedRows.Length))
        {
            var expectedRow = expectedRows[rowIndex];
            var expectedCols = expectedRow.Cols;
            var readRow = reader.Current;
            Assert.AreEqual(expectedCols.Length, readRow.ColCount, readRow.Span.ToString());
            for (var colIndex = 0; colIndex < expectedCols.Length; colIndex++)
            {
                var col = expectedCols[colIndex];
                var readerCol = readRow[colIndex];
                Assert.AreEqual(col.Expected, readerCol.ToString());
            }
            ++rowIndex;
        }
        Assert.AreEqual(moveNext, rowIndex == expectedRows.Length);


        //var row = GenerateRandomRow(_random, 16, unescape);
        Trace.WriteLine(text);
    }

    private static string GetTestText(Random random, TestRow[] rows)
    {
        var sb = new StringBuilder();
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
            sb.Append(random.Next(0, 3) switch { 0 => "\r\n", 1 => "\n", 2 => "\r", _ => Environment.NewLine });
        }
        var text = sb.ToString();
        return text;
    }

    static TestRow GenerateRandomRow(Random random, int colCount, bool colCountSame, int maxColLength, bool unescape)
    {
        colCount = colCountSame ? colCount : random.Next(0, Math.Max(1, colCount) * 2);
        var cols = new TestCol[colCount];
        for (var colIndex = 0; colIndex < colCount; colIndex++)
        {
            cols[colIndex] = GenerateRandomColText(random, maxColLength, unescape);
        }
        return new(cols);
    }

    static TestCol GenerateRandomColText(Random random, int maxColLength, bool unescape)
    {
        var colLength = random.Next(0, maxColLength);
        Span<char> source = stackalloc char[colLength];
        Span<char> unescaped = stackalloc char[colLength];
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
        var sourceString = new string(source);
        Assert.IsTrue((source.ToArray().Count(c => c == '"') & 1) == 0);
        return new(sourceString, new string(unescaped.Slice(0, unescapedLength)));
    }

    static char GenerateRandomChar(Random random, int quoteCount)
    {
        // Generate random specific chars based on hard-coded probabilities
        var p = random.NextDouble();
        return p switch
        {
            < 0.2 => SepDefaults.Quote,
            < 0.4 => (quoteCount & 1) == 1 ? Separator : 'a',
            _ => 'b',//(char)random.Next(32, 127),
        };
    }
}
