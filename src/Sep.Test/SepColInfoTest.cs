using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepColInfoTest
{
    [TestMethod]
    public void SepColInfoTest_()
    {
        var colInfo = new SepColInfo(12, 34);
        Assert.AreEqual(12, colInfo.ColEnd);
        Assert.AreEqual(34, colInfo.QuoteCount);
        Assert.AreEqual("(12, 34)", colInfo.ToString());
    }
}
