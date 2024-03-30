using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderDetectSepTest
{
    [TestMethod]
    public void SepReaderDetectSepTest_DetectSep_NoneFound()
    {
        foreach (var endOfFile in new[] { false, true })
        {
            AssertDetectSep("", endOfFile, endOfFile ? Sep.Default : null);
            AssertDetectSep("a", endOfFile, endOfFile ? Sep.Default : null);
            AssertDetectSep(" ", endOfFile, endOfFile ? Sep.Default : null);
            AssertDetectSep("\",,,;;\t|\" a", endOfFile, endOfFile ? Sep.Default : null);
            AssertDetectSep("a \",,,;;\t|\"", endOfFile, endOfFile ? Sep.Default : null);
            // No line ending found and no end of file, then no separator, otherwise yes
            foreach (var auto in SepDefaults.AutoDetectSeparators)
            {
                AssertDetectSep(auto.ToString(), endOfFile, endOfFile ? new Sep(auto) : null);
            }
        }
    }

    [TestMethod]
    public void SepReaderDetectSepTest_DetectSep_AutoDetectSeparators_Single()
    {
        foreach (var auto in SepDefaults.AutoDetectSeparators)
        {
            AssertDetectSep(auto.ToString() + "\n", endOfFile: false, new(auto));
        }
    }

    [TestMethod]
    public void SepReaderDetectSepTest_DetectSep_UntilAnyLineEnd()
    {
        AssertDetectSep("abc abc \r,", endOfFile: false, Sep.Default);
        AssertDetectSep("abc abc \n,", endOfFile: false, Sep.Default);
        AssertDetectSep("abc abc \r\n,", endOfFile: false, Sep.Default);

        AssertDetectSep("abc|abc\r,,,", endOfFile: false, new('|'));
        AssertDetectSep("abc,abc\n,,,", endOfFile: false, new(','));
        AssertDetectSep("abc\tabc\r\n,,,", endOfFile: false, new('\t'));
    }

    [TestMethod]
    public void SepReaderDetectSepTest_DetectSep_Mixed()
    {
        AssertDetectSep("a;b,c|d\te;f\n", endOfFile: false, new(';'));
        AssertDetectSep("a;b,c,d\te;f\n", endOfFile: false, new(';'));
        AssertDetectSep("a\tb,c,d\te;f\n", endOfFile: false, new('\t'));
        AssertDetectSep("a|b,c,d|e;f\n", endOfFile: false, new('|'));
        AssertDetectSep("a,b,c,d;e;f\n", endOfFile: false, new(','));
    }

    [TestMethod]
    public void SepReaderDetectSepTest_DetectSep_DisableQuotesParsing()
    {
        AssertDetectSep("\"a,b,c,d,e\"\n", endOfFile: false, Sep.New(';'), disableQuotesParsing: false);
        AssertDetectSep("\"a,b,c,d,e\"\n", endOfFile: false, Sep.New(','), disableQuotesParsing: true);
    }

    static void AssertDetectSep(ReadOnlySpan<char> chars, bool endOfFile, Sep? expected, bool disableQuotesParsing = false)
    {
        var actual = SepReaderExtensions.DetectSep(chars, endOfFile, disableQuotesParsing);
        Assert.AreEqual(expected, actual);
    }
}
