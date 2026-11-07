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
        Assert.AreEqual(unescape ? SepColInfo.Create(5, 0) : 5, reader._colEndsOrColInfos[1]);
        Assert.AreEqual(unescape ? SepColInfo.Create(14, 2) : 14, reader._colEndsOrColInfos[2]);
        if (unescape)
        {
            Assert.AreEqual(0, SepColInfo.GetState(reader._colEndsOrColInfos[1]));
            Assert.AreEqual(2, SepColInfo.GetState(reader._colEndsOrColInfos[2]));
        }
    }

    [TestMethod]
    public void PackedEndsAndStates()
    {
        foreach (var end in new[] { -1, 0, 12, SepDefaults.RowLengthMax - 2 })
        {
            foreach (var count in new[] { 0, 1, 2, 3, 100 })
            {
                var info = SepColInfoMethods.Create(end, count);
                Assert.AreEqual(end, SepColInfoMethods.GetColEnd(info));
                Assert.AreEqual(Math.Min(count, 3), SepColInfo.GetState(info));
                var cached = SepColInfo.SetUnescaped(info);
                Assert.AreEqual(end, SepColInfo.GetColEnd(cached));
                Assert.IsTrue(SepColInfo.GetState(cached) < 0);
                Assert.AreEqual(cached, SepColInfo.SetUnescaped(cached));
                Assert.AreEqual(end, SepColEndMethods.Create(end, count));
                Assert.AreEqual(end, SepColEndMethods.GetColEnd(end));
            }
        }
    }
}
