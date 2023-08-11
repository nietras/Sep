using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public partial class SepParseMaskTest
{
    const char Separator = ';';
    const int CharsIndexOffset = 17;

    static uint MaskFor(ReadOnlySpan<char> chars)
    {
        var mask = 0;
        for (var i = 0; i < Math.Min(chars.Length, 32); i++)
        {
            if (IsSpecialChar(chars[i]))
            {
                mask |= 1 << i;
            }
        }
        return (uint)mask;
    }

    static bool IsSpecialChar(char c) => c switch
    {
        Separator => true,
        SepDefaults.LineFeed => true,
        SepDefaults.CarriageReturn => true,
        SepDefaults.Quote => true,
        _ => false,
    };

    static void AssertParseState(int[] expected,
        Span<int> colEnds, ref int start, ref int end,
        int expectedRowLineEndingOffset, int rowLineEndingOffset,
        nuint expectedQuoting, nuint quoting,
        int expectedLineNumber, int lineNumber)
    {
        var count = (int)Unsafe.ByteOffset(ref start, ref end) / sizeof(int);
        // Start 1 in since col ends are added one ahead, since [0] reserved for row start/first col start
        var actual = colEnds.Slice(1, count).ToArray();
        Assert.AreEqual(expected.Length, actual.Length);
        if (!expected.SequenceEqual(actual))
        {
            Assert.Fail($"{string.Join(',', expected)} != {string.Join(',', actual)}");
        }
        CollectionAssert.AreEqual(expected, actual);
        Assert.AreEqual(expectedRowLineEndingOffset, rowLineEndingOffset);
        Assert.AreEqual(expectedQuoting, quoting);
        Assert.AreEqual(expectedLineNumber, lineNumber);
    }
}
