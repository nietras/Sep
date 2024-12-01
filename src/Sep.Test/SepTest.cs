using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepTest
{
    readonly IReadOnlyList<char> _supportedSeparators = SepDefaults
        .AutoDetectSeparators.Concat([' ', '~']).ToArray();

    [TestMethod]
    public void SepTest_Ctor_Empty()
    {
        var sep = new Sep();
        Assert.AreEqual(SepDefaults.Separator, sep.Separator);
    }

    [TestMethod]
    public void SepTest_Ctor()
    {
        foreach (var separator in _supportedSeparators)
        {
            var sep = new Sep(separator);
            Assert.AreEqual(separator, sep.Separator);
        }
    }

    [TestMethod]
    public void SepTest_Property()
    {
        var sep = new Sep();
        foreach (var separator in _supportedSeparators)
        {
            sep = sep with { Separator = separator };
            Assert.AreEqual(separator, sep.Separator);
        }
    }

    [TestMethod]
    public void SepTest_New()
    {
        foreach (var separator in _supportedSeparators)
        {
            var sep = Sep.New(separator);
            Assert.AreEqual(separator, sep.Separator);
        }
    }

    [TestMethod]
    public void SepTest_Auto()
    {
        var maybeSep = Sep.Auto;
        Assert.IsNull(maybeSep);
    }

    [TestMethod]
    public void SepTest_Equality()
    {
        var x1 = new Sep(';');
        var x2 = new Sep(';');
        var other = new Sep(',');

        Assert.IsTrue(x1 == x2);
        Assert.IsTrue(x2 == x1);
        Assert.IsFalse(x1 == other);
        Assert.IsTrue(x1 != other);

        Assert.IsTrue(x1.Equals(x2));
        Assert.IsTrue(x2.Equals(x1));
        Assert.IsFalse(x1.Equals(other));
    }

    [TestMethod]
    public void SepTest_Separator_LessThanMin_Throws()
    {
        var separator = (char)(Sep.Min.Separator - 1);
        var expectedMessage = "'\u001f':31 is not supported. Must be inside [32..126]. (Parameter 'separator')";
        AssertSeparatorThrows<ArgumentOutOfRangeException>(separator, expectedMessage);
    }

    [TestMethod]
    public void SepTest_Separator_GreaterThanMax_Throws()
    {
        var separator = (char)(Sep.Max.Separator + 1);
        var expectedMessage = "'\u007f':127 is not supported. Must be inside [32..126]. (Parameter 'separator')";
        AssertSeparatorThrows<ArgumentOutOfRangeException>(separator, expectedMessage);
    }

    [TestMethod]
    public void SepTest_Separator_LineFeed_Throws()
    {
        var separator = '\n';
        var expectedMessage = "'\n':10 is not supported. Must be inside [32..126]. (Parameter 'separator')";
        AssertSeparatorThrows<ArgumentOutOfRangeException>(separator, expectedMessage);
    }

    [TestMethod]
    public void SepTest_Separator_CarriageReturn_Throws()
    {
        var separator = '\r';
        var expectedMessage = "'\r':13 is not supported. Must be inside [32..126]. (Parameter 'separator')";
        AssertSeparatorThrows<ArgumentOutOfRangeException>(separator, expectedMessage);
    }

    [TestMethod]
    public void SepTest_Separator_Quote_Throws()
    {
        var separator = '\"';
        var expectedMessage = "'\"':34 is not supported. (Parameter 'separator')";
        AssertSeparatorThrows<ArgumentException>(separator, expectedMessage);
    }

    [TestMethod]
    public void SepTest_Separator_Comment_Throws()
    {
        var separator = '#';
        var expectedMessage = "'#':35 is not supported. (Parameter 'separator')";
        AssertSeparatorThrows<ArgumentException>(separator, expectedMessage);
    }

    static void AssertSeparatorThrows<TException>(char separator, string expectedMessage)
        where TException : Exception
    {
        Action[] actions =
        [
            () => { var s = new Sep(separator); },
            () => { var s = Sep.New(separator); },
            () => { var s = Sep.Default with { Separator = separator }; },
        ];
        foreach (var action in actions)
        {
            var e = Assert.ThrowsException<TException>(action);
            Assert.AreEqual(expectedMessage, e.Message);
        }
    }
}
