using System;
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

        Assert.IsNull(sut.Sep);
        Assert.AreSame(CultureInfo.InvariantCulture, sut.CultureInfo);
        Assert.AreSame(StringComparer.Ordinal, sut.ColNameComparer);
        Assert.IsTrue(sut.HasHeader);
        Assert.AreSame(SepToString.Direct, sut.CreateToString);
        Assert.IsFalse(sut.DisableFastFloat);
        Assert.IsFalse(sut.DisableColCountCheck);
        Assert.IsFalse(sut.AsyncContinueOnCapturedContext);
    }

    [TestMethod]
    public void SepReaderOptionsTest_Override()
    {
        var sut = new SepReaderOptions
        {
            Sep = new(','),
            CultureInfo = CultureInfo.CreateSpecificCulture("da-Dk"),
            ColNameComparer = StringComparer.OrdinalIgnoreCase,
            InitialBufferLength = 1024,
            HasHeader = false,
            CreateToString = SepToString.OnePool(),
            DisableFastFloat = true,
            DisableColCountCheck = true,
            AsyncContinueOnCapturedContext = true,
        };

        Assert.AreEqual(new Sep(','), sut.Sep);
        Assert.AreNotSame(CultureInfo.InvariantCulture, sut.CultureInfo);
        Assert.AreNotSame(StringComparer.Ordinal, sut.ColNameComparer);
        Assert.AreEqual(1024, sut.InitialBufferLength);
        Assert.IsFalse(sut.HasHeader);
        Assert.AreNotSame(SepToString.Direct, sut.CreateToString);
        Assert.IsTrue(sut.DisableFastFloat);
        Assert.IsTrue(sut.DisableColCountCheck);
        Assert.IsTrue(sut.AsyncContinueOnCapturedContext);
    }
}
