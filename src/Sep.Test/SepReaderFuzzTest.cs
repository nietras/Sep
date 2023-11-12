using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderFuzzTest
{
    static readonly char Separator = SepDefaults.Separator;
    readonly Random _random = new(23768213);

    [TestMethod]
    public void SepReaderFuzzTest_Fuzz()
    {
        var (source, unescaped) = GenerateRandomColText(_random, 16);
        //Assert.AreEqual("", source, unescaped);
    }

    static (string source, string unescaped) GenerateRandomColText(Random random, int maxColLength)
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
            // if last index and quote count uneven, always use quote
            if (i == (colLength - 1) && (quoteCount & 1) == 1)
            {
                c = SepDefaults.Quote;
                isQuote = true;
            }
            firstCharQuote |= i == 0 && isQuote;
            quoteCount += isQuote ? 1 : 0;
            source[i] = c;
            // Unescape that is skip char in unescaped if first char is a quote and
            // if either first char or current char is a quote and quote count is even
            var unescape = firstCharQuote && (i == 0 || (isQuote && ((quoteCount & 1) == 0)));
            if (!unescape)
            {
                unescaped[unescapedLength] = c;
                ++unescapedLength;
            }

        }
        return (new string(source), new string(unescaped.Slice(0, unescapedLength)));
    }

    static char GenerateRandomChar(Random random, int quoteCount)
    {
        // Generate random specific chars based on hard-coded probabilities
        var p = random.NextDouble();
        return p switch
        {
            < 0.2 => SepDefaults.Quote,
            < 0.4 => (quoteCount & 1) == 1 ? Separator : 'a',
            _ => (char)random.Next(32, 127),
        };
    }
}
