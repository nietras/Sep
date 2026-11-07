using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepColInfoTest
{
    [TestMethod]
    [DataRow(false, SepTrim.None)]
    [DataRow(false, SepTrim.Outer)]
    [DataRow(false, SepTrim.AfterUnescape)]
    [DataRow(false, SepTrim.All)]
    [DataRow(true, SepTrim.None)]
    [DataRow(true, SepTrim.Outer)]
    [DataRow(true, SepTrim.AfterUnescape)]
    [DataRow(true, SepTrim.All)]
    public void SplitStorageIsEnabledOnlyForUnescaping(bool unescape, SepTrim trim)
    {
        using var reader = Sep.Default.Reader(o => o with
        { HasHeader = false, Unescape = unescape, Trim = trim }).FromText("plain;\"quoted\"");
        Assert.IsTrue(reader.MoveNext());
        var capacity = Math.Max(SepReaderState.ColEndsInitialLength, SepParserFactory.BestPaddingLength * 2);
        Assert.AreEqual(capacity, reader.ColEndsCapacity);
        Assert.AreEqual(capacity * (unescape ? 2 : 1), reader._colEndsOrColInfos.Length);
        Assert.AreEqual(5, reader._colEndsOrColInfos[1]);
        Assert.AreEqual(14, reader._colEndsOrColInfos[2]);
        if (unescape)
        {
            Assert.AreEqual(0, reader._colEndsOrColInfos[capacity + 1]);
            Assert.AreEqual(2, reader._colEndsOrColInfos[capacity + 2]);
        }
    }

    [TestMethod]
    public void SplitEndsAndQuoteCounts()
    {
        var values = new int[4];
        foreach (var end in new[] { -1, 0, 12, SepDefaults.RowLengthMax - 2, int.MaxValue })
        {
            foreach (var count in new[] { 0, 1, 2, 3, 100 })
            {
                Array.Fill(values, -42);
                SepColInfoMethods.Write(ref values[1], end, count, 2);
                Assert.AreEqual(end, values[1]);
                Assert.AreEqual(count, values[3]);
                Assert.AreEqual(-42, values[0]);
                Assert.AreEqual(-42, values[2]);

                SepColEndMethods.Write(ref values[1], end + 1, count + 1, 2);
                Assert.AreEqual(end + 1, values[1]);
                Assert.AreEqual(count, values[3]);
            }
        }
    }
}
