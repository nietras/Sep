using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepHashTest
{
    delegate uint Hash(ReadOnlySpan<char> chars);

    [TestMethod]
    public void SepHashTest_NativeReads_OffsetSlices()
    {
        const string text = "\u0123\u4567\u89AB\uCDEF\u1357\u2468\uACE0\uBDF1\uFFFF";
        var nativeCharCount = IntPtr.Size / sizeof(char);
        var buffer = new char[text.Length + nativeCharCount];
        for (var offset = 0; offset < nativeCharCount; offset++)
        {
            text.AsSpan().CopyTo(buffer.AsSpan(offset));
            for (var length = 0; length <= text.Length; length++)
            {
                var expected = text.AsSpan(0, length);
                var actual = buffer.AsSpan(offset, length);
                Assert.AreEqual(SepHash.SumMultiplyPrimesNUInt(expected), SepHash.SumMultiplyPrimesNUInt(actual));
                Assert.AreEqual(SepHash.FNV1aNUInt(expected), SepHash.FNV1aNUInt(actual));
            }
        }
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("a")]
    [DataRow("ab")]
    [DataRow("abc")]
    [DataRow("abcd")]
    public void SepHashTest_UnrollSumMultiply31_Short(string? text)
    {
        var chars = text.AsSpan();
        Assert.AreEqual(SepHash.SumMultiply31(chars), SepHash.UnrollSumMultiply31(chars));
    }

    [TestMethod]
    public void SepHashTest_SumMultiply31_AllSame()
    {
        var hashes = new List<Hash>()
        {
            SepHash.SumMultiply31,
            SepHash.UnrollSumMultiply31,
        };
        if (Sse2.IsSupported)
        {
            hashes.Add(SepHash.Sse2SumMultiply31);
        }
        var random = new Random(21323);
        for (var c = 0; c < 1000; c++)
        {
            var text = RandomString(random, 1, 1024);

            var hash = hashes[0](text);
            for (var i = 1; i < hashes.Count; i++)
            {
                Assert.AreEqual(hash, hashes[i](text), i.ToString());
            }
        }
    }

    [TestMethod]
    public void SepHashTest_Hash_All_Run()
    {
        var hashes = new List<Hash>()
        {
            SepHash.SumMultiply31,
            SepHash.UnrollSumMultiply31,
            SepHash.SumMultiplyPrimesNUInt,
            SepHash.FNV1aNUInt,
        };
        if (Sse2.IsSupported)
        {
            hashes.Add(SepHash.Sse2SumMultiply31);
        }
        var lengths = new int[] { 1, 2, 4, 8, 16, 19 };
        foreach (var length in lengths)
        {
            Span<char> text = new char[length];
            for (var i = 0; i < text.Length; i++)
            {
                text[i] = (char)(i + 1);
            }
            foreach (var hash in hashes)
            {
                var code = hash(text);
            }
        }
    }

    static string RandomString(Random random, int minLength, int maxLength)
    {
        var length = random.Next(minLength, maxLength);
        Span<char> span = stackalloc char[length];
        for (var i = 0; i < span.Length; i++)
        {
            span[i] = (char)random.Next(char.MinValue, char.MaxValue);
        }
        return new(span);
    }

    static char ToChar(int i) => (char)('a' + i);
}
