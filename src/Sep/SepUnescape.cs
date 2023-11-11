using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

static class SepUnescape
{
    //A | B | A XOR B
    //--------------
    //0 | 0 |    0
    //0 | 1 |    1
    //1 | 0 |    1
    //1 | 1 |    0

    internal static int UnescapeInplace(ref char charRef, int length)
    {
        var unescapedLength = 0;
        var quoteCount = 1; // We start just past first quote
        for (var i = 1; i < length; i++)
        {
            var c = Unsafe.Add(ref charRef, i);
            Unsafe.Add(ref charRef, unescapedLength) = c;
            var quote = c == SepDefaults.Quote ? 1 : 0;
            var notQuote = quote ^ 1;
            quoteCount += quote;
            var s = quoteCount & 1 | notQuote;
            unescapedLength += s;
        }
        return unescapedLength;
    }

    internal static int AfterFirstQuoteRemoveEverySecondQuoteInPlace(ref char charRef, int length)
    {
        // After first quote
        var quoteCount = 0;
        var unescapedLength = 0;
        for (var i = 1; i < length; i++)
        {
            var c = Unsafe.Add(ref charRef, i);
            var quoteNumber = c == SepDefaults.Quote ? 1 : 0;
            quoteCount += quoteNumber;
            unescapedLength += (quoteCount & 1) == 0 ? 1 : 0;
            Unsafe.Add(ref charRef, unescapedLength) = c;
        }
        return unescapedLength;
    }
}
