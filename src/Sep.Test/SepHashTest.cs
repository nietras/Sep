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
