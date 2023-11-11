using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUnescapeTest
{
    // Should always be an even count since all quotes have to be paired
    // First char must always be quote since that is checked outside this scope
    internal static IEnumerable<object[]> UnescapeData => new object[][]
    {
        new object[] { "\"\"", ""  },
        new object[] { "\"\"\"\"", "\"" },
        new object[] { "\"\"\"\"\"\"", "\"\"" },
        new object[] { "\"a\"", "a" },
        new object[] { "\"a\"\"a\"", "a\"a" },
        new object[] { "\"a\"\"a\"\"a\"", "a\"a\"a" },
        new object[] { "\"a\"a\"a\"", "aa\"a" },
        new object[] { "\"\" ", " " },
        new object[] { "\"a\" ", "a " },
        new object[] { "\"\" ", " " },
        new object[] { "\"\"\"", "\"" },
    };

    [DataTestMethod]
    [DynamicData(nameof(UnescapeData))]
    public void SepUnescapeTest_RemoveEverySecondQuoteInPlace(string chars, string expected)
    {
        var src = new string(chars);
        var unescapedLength = SepUnescape.UnescapeInplace(
            ref MemoryMarshal.GetReference<char>(chars),
            chars!.Length);
        var actual = new string(chars.AsSpan(0, unescapedLength));
        Assert.AreEqual(expected, actual, src);
    }

    [DataRow(0, 0, 0, 0)]
    [DataRow(0, 1, 0, 1)]
    [DataTestMethod]
    public void SepUnescapeTest_Toggle(int toggle, int quote,
        int expectedOffset, int expectedToggle)
    {
        var offset = toggle & quote;
        toggle ^= quote;
        //Assert.AreEqual(expectedOffset, offset, nameof(offset));
        //Assert.AreEqual(expectedToggle, toggle, nameof(toggle));
    }
}
