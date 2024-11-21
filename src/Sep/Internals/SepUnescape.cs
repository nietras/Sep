using System.Diagnostics.CodeAnalysis;
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
        for (var i = unescapedLength; i < length; i++)
        {
            Add(ref charRef, i) = SepDefaults.Quote;
        }
        return (int)unescapedLength;
    }

    internal static int TrimUnescapeInPlace(ref char charRef, int length)
    {
        nint unescapedLength = 0;
        nint quoteCount = 1; // We start just past first quote
        nint i = 1;
        for (; i < length && Add(ref charRef, i) == SepDefaults.Space; i++) { }
        for (; i < length; i++)
        {
            var c = Add(ref charRef, i);
            Add(ref charRef, unescapedLength) = c;
            nint quote = c == SepDefaults.Quote ? 1 : 0;
            nint notQuote = quote ^ 1;
            quoteCount += quote;
            nint increment = quoteCount & 1 | notQuote;
            unescapedLength += increment;
        }
        for (i = unescapedLength; i < length; i++)
        {
            Add(ref charRef, i) = SepDefaults.Quote;
        }
        for (; unescapedLength > 0 && Add(ref charRef, unescapedLength - 1) == SepDefaults.Space;
             --unescapedLength)
        { }
        return (int)unescapedLength;
    }

    [ExcludeFromCodeCoverage] // Trial
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
        var unescapedLength = ByteOffset(ref charRefStart, ref unescapedCharRef) / sizeof(char);
        for (var i = unescapedLength; i < length; i++)
        {
            Add(ref charRef, i) = SepDefaults.Quote;
        }
        return (int)unescapedLength;
    }

    [ExcludeFromCodeCoverage] // Trial
    internal static int UnescapeInPlaceQuoteCountBoolUNVALIDATED(ref char charRef, int length)
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
        for (var i = unescapedLength; i < length; i++)
        {
            Add(ref charRef, i) = SepDefaults.Quote;
        }
        return unescapedLength;
    }

    [ExcludeFromCodeCoverage] // Trial
    internal static int UnescapeInPlaceEvenBoolUNVALIDATED(ref char charRef, int length)
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
        for (var i = unescapedLength; i < length; i++)
        {
            Add(ref charRef, i) = SepDefaults.Quote;
        }
        return unescapedLength;
    }
}
