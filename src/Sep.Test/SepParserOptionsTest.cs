using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepParserOptionsTest
{
    [TestMethod]
    public void SepParserOptionsTest_Ctor_Ordinary()
    {
        var sut = new SepParserOptions(';', '"');

        Assert.AreEqual(';', sut.Separator);
        Assert.AreEqual('"', sut.QuotesOrSeparatorIfDisabled);
    }

    [TestMethod]
    public void SepParserOptionsTest_Ctor_Sep()
    {
        var sut = new SepParserOptions(Sep.Default);

        Assert.AreEqual(';', sut.Separator);
        Assert.AreEqual('"', sut.QuotesOrSeparatorIfDisabled);
    }

    [TestMethod]
    public void SepParserOptionsTest_Ctor_DisableQuotes()
    {
        var sut = new SepParserOptions(Sep.Default, disableQuotesParsing: true);

        Assert.AreEqual(';', sut.Separator);
        Assert.AreEqual(';', sut.QuotesOrSeparatorIfDisabled);
    }

    [TestMethod]
    public void SepParserOptionsTest_Set()
    {
        var sut = new SepParserOptions()
        {
            Separator = ',',
            QuotesOrSeparatorIfDisabled = ';',
        };

        Assert.AreEqual(',', sut.Separator);
        Assert.AreEqual(';', sut.QuotesOrSeparatorIfDisabled);
    }
}
