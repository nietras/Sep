using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepColInfoTest
{
    [TestMethod]
    public void PackedEndAndStates()
    {
        foreach (var end in new[] { -1, 0, 12, SepDefaults.RowLengthMax - 2 })
        {
            foreach (var (count, state) in new[] {
                (0, SepColInfo.NoQuotes), (1, SepColInfo.OneQuote),
                (2, SepColInfo.TwoQuotes), (3, SepColInfo.ManyQuotes),
                (100, SepColInfo.ManyQuotes) })
            {
                var info = SepColInfo.Create(end, count);
                Assert.AreEqual(end, SepColInfo.GetColEnd(info));
                Assert.AreEqual(state, SepColInfo.GetState(info));
                var unescaped = SepColInfo.SetUnescaped(info);
                Assert.AreEqual(end, SepColInfo.GetColEnd(unescaped));
                Assert.IsTrue(SepColInfo.GetState(unescaped) < 0);
            }
        }
    }
}
