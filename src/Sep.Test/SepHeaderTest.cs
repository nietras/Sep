using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepHeaderTest
{
    [TestMethod]
    public void SepHeaderTest_Empty()
    {
        // If no line read i.e. line == null
        var header = SepHeader.Empty;

        Assert.AreEqual(true, header.IsEmpty);
        Assert.AreEqual(0, header.ColNames.Count);
        Assert.AreEqual(string.Empty, header.ToString());
    }

    [TestMethod]
    public void SepHeaderTest_EmptyString()
    {
        var header = SepHeader.Parse(Sep.Default, string.Empty);

        Assert.AreEqual(false, header.IsEmpty);
        Assert.AreEqual(1, header.ColNames.Count);
        Assert.AreEqual(0, header.IndexOf(""));
        Assert.AreEqual(string.Empty, header.ToString());
    }

    [TestMethod]
    public void SepHeaderTest_NotEmpty()
    {
        var header = SepHeader.Parse(Sep.New(';'), "A;B;C");

        Assert.AreEqual(false, header.IsEmpty);
        Assert.AreEqual(3, header.ColNames.Count);
        AreEqual(new[] { "A", "B", "C" }, header.ColNames);

        Assert.AreEqual(1, header.IndexOf("B"));
        AreEqual(new[] { 2, 0, 1 }, header.IndicesOf("C", "A", "B"));
        AreEqual(new[] { 1, 2, 0 }, header.IndicesOf(new[] { "B", "C", "A" }.AsSpan()));
        AreEqual(new[] { 0, 2 }, header.IndicesOf((ReadOnlySpan<string>)["A", "C"]));
        AreEqual(new[] { 2, 0 }, header.IndicesOf((IReadOnlyList<string>)["C", "A"]));

        var actualIndices = new int[2];
        header.IndicesOf((ReadOnlySpan<string>)["A", "C"], actualIndices);
        AreEqual(new int[] { 0, 2 }, actualIndices);

        Assert.AreEqual("A;B;C", header.ToString());
    }

    [TestMethod]
    public void SepHeaderTest_NamesStartingWith()
    {
        var header = SepHeader.Parse(Sep.New(';'), "A;B;C;GT_0;RE_0;GT_1;RE_1");
        AreEqual(new[] { "GT_0", "GT_1" }, header.NamesStartingWith("GT_"));
    }

    [TestMethod]
    public void SepHeaderTest_IndicesOf_LengthsNotSame_Throws()
    {
        var header = SepHeader.Parse(Sep.New(';'), "A;B;C");

        var e = Assert.ThrowsException<ArgumentException>(() =>
        {
            var colNames = new[] { "A", "B" };
            Span<int> colIndices = stackalloc int[1];
            header.IndicesOf(colNames, colIndices);
        });
        Assert.AreEqual("'colIndices':1 must have same length as 'colNames':2", e.Message);
    }

    static void AreEqual<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual) =>
        CollectionAssert.AreEqual((ICollection)expected, (ICollection)actual);
}
