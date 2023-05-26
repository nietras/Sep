using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepToStringHashPoolTest
{
    [TestMethod]
    public void SepToStringHashPoolTest_Basic()
    {
        using var pool = new SepToStringHashPool(32);
        var data = new char[] { 'a', 'b', 'c', 'd' };

        var str0 = pool.ToString(data.AsSpan(0, 4));
        Assert.IsNotNull(str0);
        Assert.AreEqual("abcd", str0);

        var str1 = pool.ToString(data.AsSpan(0, 4));
        Assert.AreSame(str0, str1);

        Assert.AreEqual(1, pool.Count);
    }

    [TestMethod]
    public void SepToStringHashPoolTest_EdgeCases()
    {
        var maximumStringLength = 2;
        using var pool = new SepToStringHashPool(maximumStringLength);

        Assert.AreSame(string.Empty, pool.ToString(new Span<char>()));
        var chars = new char[maximumStringLength + 1];
        var str0 = pool.ToString(chars);
        var str1 = pool.ToString(chars);
        Assert.AreNotSame(str0, str1);
    }

    [TestMethod]
    public void SepToStringHashPoolTest_Resize()
    {
        using var pool = new SepToStringHashPool(initialCapacity: 16);
        const int length = 256;
        var strings = new string[length];

        for (var i = 0; i < length; i++)
        {
            var c = ToChar(i);
            var span = new Span<char>(ref c);
            var str = pool.ToString(span);
            strings[i] = str;
            Assert.AreEqual(c.ToString(), str);
        }

        for (var i = 0; i < length; i++)
        {
            var c = ToChar(i);
            var span = new Span<char>(ref c);
            var str = pool.ToString(span);
            Assert.AreSame(strings[i], str);
        }

        Assert.AreEqual(length, pool.Count);

    }

    [TestMethod]
    public void SepToStringHashPoolTest_MaximumCapacity()
    {
        var maximumCapacity = 16;
        using var pool = new SepToStringHashPool(initialCapacity: maximumCapacity, maximumCapacity: maximumCapacity);

        for (var i = 0; i < maximumCapacity; i++)
        {
            var c = ToChar(i);
            var span = new Span<char>(ref c);
            pool.ToString(span);
        }
        {
            var c0 = ToChar(maximumCapacity + 1);
            var c1 = ToChar(maximumCapacity + 2);
            var span0 = new Span<char>(ref c0);
            var span1 = new Span<char>(ref c1);
            var str00 = pool.ToString(span0);
            var str10 = pool.ToString(span1); // Caching last string so need intermediate
            var str01 = pool.ToString(span0);
            Assert.AreNotSame(str01, str00);
        }
    }

    [TestMethod]
    public void SepToStringHashPoolTest_InitialCapacityGreaterThanMaximumCapacity_Throws()
    {
        var e = Assert.ThrowsException<ArgumentException>(() =>
            new SepToStringHashPool(initialCapacity: 9, maximumCapacity: 8));
        Assert.AreEqual("initialCapacity:9 must be less than or equal to maximumCapacity:8", e.Message);
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
