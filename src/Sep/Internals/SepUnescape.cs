using static System.Runtime.CompilerServices.Unsafe;

namespace nietras.SeparatedValues;

static class SepUnescape
{
    internal static int UnescapeInPlace(ref char charRef, int length)
    {
        nint unescapedLength = 0;
        nint quoteCount = 1; // We start just past first quote
        for (var i = 1; i < length; i++)
        {
            var c = Add(ref charRef, i);
            Add(ref charRef, unescapedLength) = c;
            nint quote = c == SepDefaults.Quote ? 1 : 0;
            nint notQuote = quote ^ 1;
            quoteCount += quote;
            nint increment = quoteCount & 1 | notQuote;
            unescapedLength += increment;
        }
        return (int)unescapedLength;
    }

    internal static int UnescapeInPlaceRefs(ref char charRef, int length)
    {
        nint quoteCount = 1; // We start just past first quote
        ref var charRefStart = ref charRef;
        ref var charRefEnd = ref Add(ref charRef, length);
        ref var unescapedCharRef = ref charRef;
        charRef = ref Add(ref charRef, 1);
        for (; !AreSame(ref charRef, ref charRefEnd); charRef = ref Add(ref charRef, 1))
        {
            var c = charRef;
            unescapedCharRef = c;
            nint quote = c == SepDefaults.Quote ? 1 : 0;
            nint notQuote = quote ^ 1;
            quoteCount += quote;
            nint increment = quoteCount & 1 | notQuote;
            unescapedCharRef = ref Add(ref unescapedCharRef, increment);
        }
        return (int)(ByteOffset(ref charRefStart, ref unescapedCharRef) / sizeof(char));
    }

    internal static int AfterFirstQuoteRemoveEverySecondQuoteInPlace(ref char charRef, int length)
    {
        // After first quote
        var quoteCount = 0;
        var unescapedLength = 0;
        for (var i = 1; i < length; i++)
        {
            var c = Add(ref charRef, i);
            var quoteNumber = c == SepDefaults.Quote ? 1 : 0;
            quoteCount += quoteNumber;
            unescapedLength += (quoteCount & 1) == 0 ? 1 : 0;
            Add(ref charRef, unescapedLength) = c;
        }
        return unescapedLength;
    }

    internal static int AfterFirstQuoteRemoveEverySecondQuoteInPlaceBool(ref char charRef, int length)
    {
        // After first quote, so quote 0, hence even
        var evenQuote = false;
        var unescapedLength = 0;
        for (var i = 1; i < length; i++)
        {
            var c = Add(ref charRef, i);
            evenQuote ^= c == SepDefaults.Quote;
            unescapedLength += evenQuote ? 1 : 0;
            Add(ref charRef, unescapedLength) = c;
        }
        return unescapedLength;
    }
}
