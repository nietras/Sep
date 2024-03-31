using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepRowInfoTest
{
    [TestMethod]
    public void SepRowInfoTest_Ctor()
    {
        var sut = new SepRowInfo(12, 34);

        Assert.AreEqual(12, sut.LineNumberTo);
        Assert.AreEqual(34, sut.ColCount);
    }

    [TestMethod]
    public void SepRowInfoTest_Set()
    {
        var sut = new SepRowInfo() { LineNumberTo = 12, ColCount = 34 };

        Assert.AreEqual(12, sut.LineNumberTo);
        Assert.AreEqual(34, sut.ColCount);
    }
}
