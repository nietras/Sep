using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepStringHashPoolTest
{
    internal delegate string ToStringDelegate(ISepStringHashPool pool, ReadOnlySpan<char> chars);

    public static IEnumerable<object[]> ToStrings =>
    [
        new ToStringDelegate[] { new((pool, chars) => pool.ToString(chars)) },
        new ToStringDelegate[] { new((pool, chars) => pool.ToStringThreadSafe(chars)) },
    ];

    [TestMethod]
    [DynamicData(nameof(ToStrings))]
    public void SepStringHashPoolTest_Basic(object toStringObject)
    {
        Contract.Assume(toStringObject != null);
        var toString = (ToStringDelegate)toStringObject;

        var maximumStringLength = 32;
        var createPools = new Func<ISepStringHashPool>[] {
            () => new SepStringHashPool(maximumStringLength),
            () => new SepStringHashPoolFixedCapacity(maximumStringLength),
        };

        foreach (var createPool in createPools)
        {
            using var pool = createPool();

            var data = new char[] { 'a', 'b', 'c', 'd' };

            var str0 = toString(pool, data.AsSpan(0, 4));
            Assert.IsNotNull(str0);
            Assert.AreEqual("abcd", str0);

            var str1 = toString(pool, data.AsSpan(0, 4));
            Assert.AreSame(str0, str1);

            Assert.AreEqual(1, pool.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ToStrings))]
    public void SepStringHashPoolTest_EdgeCases(object toStringObject)
    {
        Contract.Assume(toStringObject != null);
        var toString = (ToStringDelegate)toStringObject;

        var maximumStringLength = 2;
        var createPools = new Func<ISepStringHashPool>[] {
            () => new SepStringHashPool(maximumStringLength),
            () => new SepStringHashPoolFixedCapacity(maximumStringLength),
        };

        foreach (var createPool in createPools)
        {
            using var pool = createPool();

            Assert.AreSame(string.Empty, toString(pool, new Span<char>()));
            var chars = new char[maximumStringLength + 1];
            var str0 = toString(pool, chars);
            var str1 = toString(pool, chars);
            Assert.AreNotSame(str0, str1);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ToStrings))]
    public void SepStringHashPoolTest_TryCollisions(object toStringObject)
    {
        Contract.Assume(toStringObject != null);
        var toString = (ToStringDelegate)toStringObject;

        var maximumStringLength = 32;
        var createPools = new Func<ISepStringHashPool>[] {
            () => new SepStringHashPool(maximumStringLength),
            () => new SepStringHashPoolFixedCapacity(maximumStringLength),
        };

        foreach (var createPool in createPools)
        {
            using var pool = createPool();

            // Test is highly dependent on the hash used in the pool, this is
            // known to degenerate be 0 for null strings.

            const int maxNullStringLength = 64;
            for (var nullStringLength = 1; nullStringLength < maxNullStringLength; nullStringLength++)
            {
                var nullString = new string('\0', nullStringLength);
                var str = toString(pool, nullString);
                Assert.IsNotNull(str);
                Assert.AreNotSame(nullString, str);
            }
            Assert.IsLessThan(maxNullStringLength, pool.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ToStrings))]
    public void SepStringHashPoolTest_Resize(object toStringObject)
    {
        Contract.Assume(toStringObject != null);
        var toString = (ToStringDelegate)toStringObject;

        using var pool = new SepStringHashPool(initialCapacity: 16);
        const int length = 256;
        var strings = new string[length];

        for (var i = 0; i < length; i++)
        {
            var c = ToChar(i);
            var span = new Span<char>(ref c);
            var str = toString(pool, span);
            strings[i] = str;
            Assert.AreEqual(c.ToString(), str);
        }

        for (var i = 0; i < length; i++)
        {
            var c = ToChar(i);
            var span = new Span<char>(ref c);
            var str = toString(pool, span);
            Assert.AreSame(strings[i], str);
        }

        Assert.AreEqual(length, pool.Count);
    }

    [TestMethod]
    [DynamicData(nameof(ToStrings))]
    public void SepStringHashPoolTest_MaximumCapacity(object toStringObject)
    {
        Contract.Assume(toStringObject != null);
        var toString = (ToStringDelegate)toStringObject;

        var maximumCapacity = 16;
        var createPools = new Func<ISepStringHashPool>[] {
            () => new SepStringHashPool(initialCapacity: maximumCapacity, maximumCapacity: maximumCapacity),
            () => new SepStringHashPoolFixedCapacity(capacity: maximumCapacity),
        };

        foreach (var createPool in createPools)
        {
            using var pool = createPool();

            var withinCapacityStrings = new string[maximumCapacity];

            for (var i = 0; i < maximumCapacity; i++)
            {
                var c = ToChar(i);
                var span = new Span<char>(ref c);
                withinCapacityStrings[i] = toString(pool, span);
            }
            {
                var c0 = ToChar(maximumCapacity + 1);
                var c1 = ToChar(maximumCapacity + 2);
                var span0 = new Span<char>(ref c0);
                var span1 = new Span<char>(ref c1);
                var str00 = toString(pool, span0);
                var str10 = toString(pool, span1); // Caching last string so need intermediate
                var str01 = toString(pool, span0);
                Assert.AreNotSame(str01, str00);
            }
            for (var i = 0; i < maximumCapacity; i++)
            {
                var c = ToChar(i);
                var span = new Span<char>(ref c);
                var s = toString(pool, span);
                Assert.AreSame(withinCapacityStrings[i], s);
            }

            Assert.AreEqual(maximumCapacity, pool.Count);
        }
    }

    [TestMethod]
    public void SepStringHashPoolTest_InitialCapacityGreaterThanMaximumCapacity_Throws()
    {
        var e = Assert.ThrowsExactly<ArgumentException>(() =>
            new SepStringHashPool(initialCapacity: 9, maximumCapacity: 8));
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
