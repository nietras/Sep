using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepParserOptionsTest
{
    [TestMethod]
    public void SepParserOptionsTest_Bytes_RoundTrip()
    {
        foreach (var bytes in new[] { 0u, 0x04030201u, 0x0D0A223Bu, uint.MaxValue })
        {
            var sut = new SepParserOptions(bytes);
            Assert.AreEqual(bytes, sut.Bytes);
            Assert.AreEqual((byte)bytes, sut.Separator);
            Assert.AreEqual((char)(byte)bytes, SepParserOptions.GetSeparator(bytes));
            Assert.AreEqual((byte)(bytes >> 8), sut.QuotesOrSeparatorIfDisabled);
            Assert.AreEqual((byte)(bytes >> 16), sut.LineFeed);
            Assert.AreEqual((byte)(bytes >> 24), sut.CarriageReturn);
            Assert.AreEqual(bytes, new SepParserOptions(sut.Separator, sut.QuotesOrSeparatorIfDisabled,
                sut.LineFeed, sut.CarriageReturn).Bytes);
        }
    }

    [TestMethod]
    public void SepParserOptionsTest_Ctor_Ordinary()
    {
        var sut = new SepParserOptions((byte)';', (byte)'"');

        Assert.AreEqual((byte)';', sut.Separator);
        Assert.AreEqual((byte)'"', sut.QuotesOrSeparatorIfDisabled);
        Assert.AreEqual((byte)'\n', sut.LineFeed);
        Assert.AreEqual((byte)'\r', sut.CarriageReturn);
        Assert.AreEqual(4, Unsafe.SizeOf<SepParserOptions>());
        Assert.AreEqual(4, Marshal.SizeOf<SepParserOptions>());
    }

    [TestMethod]
    public void SepParserOptionsTest_Ctor_Sep()
    {
        var sut = new SepParserOptions(Sep.Default);

        Assert.AreEqual((byte)';', sut.Separator);
        Assert.AreEqual((byte)'"', sut.QuotesOrSeparatorIfDisabled);
        Assert.AreEqual((byte)'\n', sut.LineFeed);
        Assert.AreEqual((byte)'\r', sut.CarriageReturn);
    }

    [TestMethod]
    public void SepParserOptionsTest_Ctor_DisableQuotes()
    {
        var sut = new SepParserOptions(Sep.Default, disableQuotesParsing: true);

        Assert.AreEqual((byte)';', sut.Separator);
        Assert.AreEqual((byte)';', sut.QuotesOrSeparatorIfDisabled);
        Assert.AreEqual((byte)'\n', sut.LineFeed);
        Assert.AreEqual((byte)'\r', sut.CarriageReturn);
    }

    [TestMethod]
    public void SepParserOptionsTest_Set()
    {
        var sut = new SepParserOptions()
        {
            Separator = (byte)',',
            QuotesOrSeparatorIfDisabled = (byte)';',
            LineFeed = (byte)'\n',
            CarriageReturn = (byte)'\r',
        };

        Assert.AreEqual((byte)',', sut.Separator);
        Assert.AreEqual((byte)';', sut.QuotesOrSeparatorIfDisabled);
        Assert.AreEqual((byte)'\n', sut.LineFeed);
        Assert.AreEqual((byte)'\r', sut.CarriageReturn);
        var changed = sut with { Separator = byte.MaxValue };
        Assert.AreEqual(byte.MaxValue, changed.Separator);
        Assert.AreEqual(sut.QuotesOrSeparatorIfDisabled, changed.QuotesOrSeparatorIfDisabled);
        Assert.AreEqual(sut.LineFeed, changed.LineFeed);
        Assert.AreEqual(sut.CarriageReturn, changed.CarriageReturn);
        Assert.AreNotEqual(sut, changed);
    }

    [TestMethod]
    public void SepParserOptionsTest_Bytes_Independent()
    {
        var sut = new SepParserOptions(1, 2, 3, 4);
        Assert.AreEqual((byte)1, sut.Separator);
        Assert.AreEqual((byte)2, sut.QuotesOrSeparatorIfDisabled);
        Assert.AreEqual((byte)3, sut.LineFeed);
        Assert.AreEqual((byte)4, sut.CarriageReturn);
        sut = sut with { QuotesOrSeparatorIfDisabled = byte.MaxValue };
        Assert.AreEqual((byte)1, sut.Separator);
        Assert.AreEqual(byte.MaxValue, sut.QuotesOrSeparatorIfDisabled);
        Assert.AreEqual((byte)3, sut.LineFeed);
        Assert.AreEqual((byte)4, sut.CarriageReturn);
        sut = sut with { LineFeed = byte.MaxValue };
        Assert.AreEqual((byte)1, sut.Separator);
        Assert.AreEqual(byte.MaxValue, sut.QuotesOrSeparatorIfDisabled);
        Assert.AreEqual(byte.MaxValue, sut.LineFeed);
        Assert.AreEqual((byte)4, sut.CarriageReturn);
        sut = sut with { CarriageReturn = byte.MaxValue };
        Assert.AreEqual((byte)1, sut.Separator);
        Assert.AreEqual(byte.MaxValue, sut.QuotesOrSeparatorIfDisabled);
        Assert.AreEqual(byte.MaxValue, sut.LineFeed);
        Assert.AreEqual(byte.MaxValue, sut.CarriageReturn);
    }
}
