using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepUnescapingTest
{
    [DataRow("", "")]
    [DataRow("\"\"", "")]
    [DataRow("\"\"\"\"", "\"")]
    [DataTestMethod]
    public void SepUnescapingTest_Unescape(string chars, string expected)
    {
        var unescapedLength = SepUnescaping.Unescape(
            ref MemoryMarshal.GetReference<char>(chars),
            chars!.Length);
        var actual = new string(chars.AsSpan(0, unescapedLength));
        Assert.AreEqual(expected, actual);
    }

    [DataRow(0, 0, 0, 0)]
    [DataRow(0, 1, 0, 1)]
    [DataTestMethod]
    public void SepUnescapingTest_Toggle(int toggle, int quote,
        int expectedOffset, int expectedToggle)
    {
        var offset = toggle & quote;
        toggle ^= quote;
        Assert.AreEqual(expectedOffset, offset, nameof(offset));
        Assert.AreEqual(expectedToggle, toggle, nameof(toggle));
    }
}
