using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepSpecTest
{
    [TestMethod]
    public void SepSpecTest_Defaults()
    {
        var sut = new SepSpec();

        Assert.AreEqual(Sep.Default, sut.Sep);
        Assert.AreSame(SepDefaults.CultureInfo, sut.CultureInfo);
    }

    [TestMethod]
    public void SepSpecTest_With()
    {
        var sut = new SepSpec() with { Sep = new(';'), CultureInfo = CultureInfo.CurrentCulture };

        Assert.AreEqual(new Sep(';'), sut.Sep);
        Assert.AreSame(CultureInfo.CurrentCulture, sut.CultureInfo);
    }
}
