using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static nietras.SeparatedValues.SepParseMask;

namespace nietras.SeparatedValues.Test;

public partial class SepParseMaskTest
{
    [TestMethod]
    public void SepParseMaskTest_PrefixXor()
    {
        Assert.AreEqual((nuint)0, PrefixXor(0));
        for (var i = 0; i < s_nativeBitSize; i++)
        {
            var bit = (nuint)1 << i;
            Assert.AreEqual(~(bit - 1), PrefixXor(bit));
        }

        nuint alternating = 0;
        for (var i = 0; i < s_nativeBitSize; i += 2) { alternating |= (nuint)1 << i; }
        Assert.AreEqual(alternating, PrefixXor(nuint.MaxValue));
    }

    [TestMethod]
    public void SepParseMaskTest_ParseQuotedMask_MatchesScalar()
    {
        AssertQuotedMaskMatchesScalar<int, SepColEndMethods>();
        AssertQuotedMaskMatchesScalar<SepColInfo, SepColInfoMethods>();
    }

    static void AssertQuotedMaskMatchesScalar<TColInfo, TColInfoMethods>()
        where TColInfo : unmanaged
        where TColInfoMethods : ISepColInfoMethods<TColInfo>
    {
        var random = new Random(18473);
        ReadOnlySpan<char> alphabet = "a;\"\r\n\u0123";
        Span<char> chars = stackalloc char[s_nativeBitSize + 1];
        Span<TColInfo> expected = stackalloc TColInfo[s_nativeBitSize + 1];
        Span<TColInfo> actual = stackalloc TColInfo[s_nativeBitSize + 1];
        for (var sample = 0; sample < 2048; sample++)
        {
            for (var i = 0; i < chars.Length; i++)
            {
                // Alternate single-line and multiline blocks, with a lookahead character for CRLF.
                chars[i] = alphabet[random.Next(sample % 2 == 0 ? 3 : alphabet.Length)];
            }
            if (sample == 0) { chars.Fill('"'); }
            var specialCharMask = MaskFor(chars);
            nuint separatorsMask = 0;
            nuint lineEndingsMask = 0;
            for (var i = 0; i < s_nativeBitSize; i++)
            {
                var bit = (nuint)1 << i;
                if (chars[i] == Separator) { separatorsMask |= bit; }
                if (chars[i] is '\r' or '\n') { lineEndingsMask |= bit; }
            }
            var expectedQuotes = (nuint)(sample % 7);
            var actualQuotes = expectedQuotes;
            var expectedLine = 17;
            var actualLine = expectedLine;
            var expectedEndingOffset = 0;
            var actualEndingOffset = 0;

            ref var expectedEnd = ref ParseAnyCharsMask<TColInfo, TColInfoMethods>(
                specialCharMask, Separator, ref chars[0], CharsIndexOffset,
                ref expectedEndingOffset, ref expectedQuotes, ref expected[0], ref expectedLine);
            ref var actualEnd = ref ParseQuotedMask<TColInfo, TColInfoMethods>(
                separatorsMask, separatorsMask | lineEndingsMask, specialCharMask,
                Separator, ref chars[0], CharsIndexOffset,
                ref actualEndingOffset, ref actualQuotes, ref actual[0], ref actualLine);

            var expectedCount = (int)Unsafe.ByteOffset(ref expected[0], ref expectedEnd) / Unsafe.SizeOf<TColInfo>();
            var actualCount = (int)Unsafe.ByteOffset(ref actual[0], ref actualEnd) / Unsafe.SizeOf<TColInfo>();
            Assert.AreEqual(expectedCount, actualCount);
            for (var i = 1; i <= expectedCount; i++) { Assert.AreEqual(expected[i], actual[i]); }
            Assert.AreEqual(expectedQuotes, actualQuotes);
            Assert.AreEqual(expectedLine, actualLine);
            Assert.AreEqual(expectedEndingOffset, actualEndingOffset);
        }
    }
}
