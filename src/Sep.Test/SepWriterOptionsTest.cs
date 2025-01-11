using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterOptionsTest
{
    [TestMethod]
    public void SepWriterOptionsTest_Defaults()
    {
        var sut = new SepWriterOptions();

        Assert.AreEqual(Sep.Default, sut.Sep);
        Assert.AreSame(SepDefaults.CultureInfo, sut.CultureInfo);
        Assert.IsTrue(sut.WriteHeader);
        Assert.IsFalse(sut.DisableColCountCheck);
        Assert.AreEqual(SepColNotSetOption.Throw, sut.ColNotSetOption);
        Assert.IsFalse(sut.Escape);
    }
}
