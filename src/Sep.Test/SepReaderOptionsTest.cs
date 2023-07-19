using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderOptionsTest
{
    [TestMethod]
    public void SepReaderOptionsTest_Defaults()
    {
        var sut = new SepReaderOptions();

        Assert.AreEqual(null, sut.Sep);
        Assert.AreSame(CultureInfo.InvariantCulture, sut.CultureInfo);
        Assert.IsTrue(sut.HasHeader);
        Assert.AreSame(SepToString.Direct, sut.CreateToString);
        Assert.IsFalse(sut.DisableFastFloat);
        Assert.IsFalse(sut.DisableColCountCheck);
    }

    [TestMethod]
    public void SepReaderOptionsTest_Override()
    {
        var sut = new SepReaderOptions
        {
            Sep = new(','),
            CultureInfo = CultureInfo.CreateSpecificCulture("da-Dk"),
            HasHeader = false,
            CreateToString = SepToString.OnePool(),
            DisableFastFloat = true,
            DisableColCountCheck = true,
        };

        Assert.AreEqual(new Sep(','), sut.Sep);
        Assert.AreNotSame(CultureInfo.InvariantCulture, sut.CultureInfo);
        Assert.IsFalse(sut.HasHeader);
        Assert.AreNotSame(SepToString.Direct, sut.CreateToString);
        Assert.IsTrue(sut.DisableFastFloat);
        Assert.IsTrue(sut.DisableColCountCheck);
    }
}
