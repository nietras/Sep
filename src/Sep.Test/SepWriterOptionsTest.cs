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
    }
}
