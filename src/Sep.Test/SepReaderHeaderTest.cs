using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderHeaderTest
{
    [TestMethod]
    public void SepReaderHeaderTest_Empty()
    {
        // If no line read i.e. line == null
        var header = SepReaderHeader.Empty;

        Assert.IsTrue(header.IsEmpty);
        Assert.IsEmpty(header.ColNames);
        Assert.AreEqual(string.Empty, header.ToString());
    }

    [TestMethod]
    public void SepReaderHeaderTest_EmptyString()
    {
        var header = Parse(Sep.Default, string.Empty);

        Assert.IsFalse(header.IsEmpty);
        Assert.HasCount(1, header.ColNames);
        Assert.AreEqual(0, header.IndexOf(""));
#if NET9_0_OR_GREATER
        Assert.AreEqual(0, header.IndexOf("".AsSpan()));
#endif
        Assert.AreEqual(string.Empty, header.ToString());
    }

    [TestMethod]
    public void SepReaderHeaderTest_NotEmpty()
    {
        var header = Parse(Sep.New(';'), "A;B;C");

        Assert.IsFalse(header.IsEmpty);
        Assert.HasCount(3, header.ColNames);
        AreEqual(["A", "B", "C"], header.ColNames);
        {
            Assert.AreEqual(1, header.IndexOf("B"));
            var tryTrue = header.TryIndexOf("B", out var tryTrueIndex);
            Assert.IsTrue(tryTrue);
            Assert.AreEqual(1, tryTrueIndex);
            var tryFalse = header.TryIndexOf("XX", out var tryFalseIndex);
            Assert.IsFalse(tryFalse);
            Assert.AreEqual(0, tryFalseIndex);
        }
#if NET9_0_OR_GREATER
        {
            Assert.AreEqual(1, header.IndexOf("B".AsSpan()));
            var tryTrue = header.TryIndexOf("B".AsSpan(), out var tryTrueIndex);
            Assert.IsTrue(tryTrue);
            Assert.AreEqual(1, tryTrueIndex);
            var tryFalse = header.TryIndexOf("XX".AsSpan(), out var tryFalseIndex);
            Assert.IsFalse(tryFalse);
            Assert.AreEqual(0, tryFalseIndex);
        }
#endif
        AreEqual([2, 0, 1], header.IndicesOf("C", "A", "B"));
        AreEqual([1, 2, 0], header.IndicesOf(new[] { "B", "C", "A" }));
        AreEqual([0, 2], header.IndicesOf((ReadOnlySpan<string>)["A", "C"]));
        AreEqual([2, 0], header.IndicesOf((IReadOnlyList<string>)["C", "A"]));


        var actualIndices = new int[2];
        header.IndicesOf((ReadOnlySpan<string>)["A", "C"], actualIndices);
        AreEqual([0, 2], actualIndices);

        Assert.AreEqual("A;B;C", header.ToString());
    }

    [TestMethod]
    public void SepReaderHeaderTest_NamesStartingWith()
    {
        var header = Parse(Sep.New(';'), "A;B;C;GT_0;RE_0;GT_1;RE_1");
        AreEqual(new[] { "GT_0", "GT_1" }, header.NamesStartingWith("GT_"));
    }

    [TestMethod]
    public void SepReaderHeaderTest_IndicesOf_LengthsNotSame_Throws()
    {
        var header = Parse(Sep.New(';'), "A;B;C");

        var e = Assert.ThrowsExactly<ArgumentException>(() =>
        {
            var colNames = new[] { "A", "B" };
            Span<int> colIndices = stackalloc int[1];
            header.IndicesOf(colNames, colIndices);
        });
        Assert.AreEqual("'colIndices':1 must have same length as 'colNames':2", e.Message);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(3)]
    [DataRow(40)]
    [DataRow(129)]
    public void SepReaderHeaderTest_LookupsMatchDictionary(int colCount)
    {
        IEqualityComparer<string>[] comparers =
        [
            StringComparer.Ordinal,
            StringComparer.OrdinalIgnoreCase,
            StringComparer.InvariantCultureIgnoreCase,
            EqualityComparer<string>.Default,
            new CollisionComparer(),
#if NET9_0_OR_GREATER
            new AlternateCollisionComparer(),
#endif
        ];
        foreach (var comparer in comparers)
        {
            var colNames = new string[colCount];
            var dictionary = new Dictionary<string, int>(colCount, comparer);
            for (var i = 0; i < colCount; ++i)
            {
                colNames[i] = $"Feature_{i}_\u00C6\u03A3";
                dictionary.Add(colNames[i], i);
            }
            var header = new SepReaderHeader(string.Join(';', colNames), colNames, comparer);
            Assert.AreEqual(colCount, header.ColCount);
            for (var i = 0; i <= colCount; ++i)
            {
                var colName = i < colCount ? colNames[i] : "Missing";
                foreach (var query in new[] { colName, colName.ToLowerInvariant() })
                {
                    var expected = dictionary.TryGetValue(query, out var expectedIndex);
                    Assert.AreEqual(expected, header.TryIndexOf(query, out var actualIndex));
                    Assert.AreEqual(expectedIndex, actualIndex);
#if NET9_0_OR_GREATER
                    if (dictionary.TryGetAlternateLookup<ReadOnlySpan<char>>(out var lookup))
                    {
                        Assert.AreEqual(lookup.TryGetValue(query.AsSpan(), out expectedIndex),
                            header.TryIndexOf(query.AsSpan(), out actualIndex));
                        Assert.AreEqual(expectedIndex, actualIndex);
                    }
#endif
                }
            }
#if NET9_0_OR_GREATER
            if (!dictionary.TryGetAlternateLookup<ReadOnlySpan<char>>(out _))
            {
                var expected = Assert.ThrowsExactly<InvalidOperationException>(
                    () => dictionary.GetAlternateLookup<ReadOnlySpan<char>>());
                var actual = Assert.ThrowsExactly<InvalidOperationException>(
                    () => header.TryIndexOf("Missing".AsSpan(), out _));
                Assert.AreEqual(expected.Message, actual.Message);
            }
#endif
        }
    }

    [TestMethod]
    public void SepReaderHeaderTest_InvalidNamesMatchDictionary()
    {
        var dictionary = new Dictionary<string, int>() { { "A", 0 } };
        var header = Parse(Sep.Default, "A");
        Assert.ThrowsExactly<ArgumentNullException>(() => header.TryIndexOf((string)null!, out _));
        var expectedMissing = Assert.ThrowsExactly<KeyNotFoundException>(() => _ = dictionary["Missing"]);
        var actualMissing = Assert.ThrowsExactly<KeyNotFoundException>(() => header.IndexOf("Missing"));
        Assert.AreEqual(expectedMissing.Message, actualMissing.Message);
#if NET9_0_OR_GREATER
        var actualMissingSpan = Assert.ThrowsExactly<KeyNotFoundException>(() => header.IndexOf("Missing".AsSpan()));
        Assert.AreEqual(expectedMissing.Message, actualMissingSpan.Message);
#endif
    }

    sealed class CollisionComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y) => StringComparer.Ordinal.Equals(x, y);
        public int GetHashCode(string obj) => -1;
    }

#if NET9_0_OR_GREATER
    sealed class AlternateCollisionComparer : IEqualityComparer<string>,
        IAlternateEqualityComparer<ReadOnlySpan<char>, string>
    {
        public bool Equals(string? x, string? y) => StringComparer.Ordinal.Equals(x, y);
        public int GetHashCode(string obj) => -1;
        public bool Equals(ReadOnlySpan<char> alternate, string other) => alternate.SequenceEqual(other);
        public int GetHashCode(ReadOnlySpan<char> alternate) => -1;
        public string Create(ReadOnlySpan<char> alternate) => alternate.ToString();
    }
#endif

    static void AreEqual<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual) =>
        CollectionAssert.AreEqual((ICollection)expected, (ICollection)actual);

    // Convenience method for testing only
    static SepReaderHeader Parse(Sep sep, string line) =>
        Parse(sep, line, SepDefaults.ColNameComparer);

    static SepReaderHeader Parse(Sep sep, string line, IEqualityComparer<string> comparer)
    {
        var colNames = sep.Split(line);
        return new SepReaderHeader(line, colNames, comparer);
    }
}
