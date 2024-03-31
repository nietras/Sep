using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static nietras.SeparatedValues.SepParseMask;

namespace nietras.SeparatedValues.Test;

public partial class SepParseMaskTest
{
    [TestMethod]
    public void SepParseMaskTest_ParseAnyCharsMask_Ordinary()
    {
        AssertParseAnyCharsMask(";a;b\r\n;", [0, 2, 4],
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 2,
            lineNumber: 2, expectedLineNumber: 3);

        AssertParseAnyCharsMask(";a;b\r;", [0, 2, 4],
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 1,
            lineNumber: 2, expectedLineNumber: 3);

        AssertParseAnyCharsMask(";a;b\n;", [0, 2, 4],
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 1,
            lineNumber: 2, expectedLineNumber: 3);

        AssertParseAnyCharsMask(";aa;bbb;cccc;;", [0, 3, 7, 12, 13],
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 0,
            lineNumber: 2, expectedLineNumber: 2);

        AssertParseAnyCharsMask(new string('a', s_nativeBitSize - 1) + "\r\n", [s_nativeBitSize - 1],
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 2,
            lineNumber: 2, expectedLineNumber: 3);
    }

    [TestMethod]
    public void SepParseMaskTest_ParseAnyCharsMask_Quotes()
    {
        AssertParseAnyCharsMask(";\"a\r\nb\";\"c\"\r\n;", [0, 7, 11],
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 2,
            lineNumber: 2, expectedLineNumber: 4);

        AssertParseAnyCharsMask(";\"a\rb\";\"c\"\r;", [0, 6, 10],
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 1,
            lineNumber: 2, expectedLineNumber: 4);

        AssertParseAnyCharsMask(";\"a\nb\";\"c\"\n;", [0, 6, 10],
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 1,
            lineNumber: 2, expectedLineNumber: 4);

        AssertParseAnyCharsMask("\"" + new string('a', s_nativeBitSize - 2) + "\r\n", Array.Empty<int>(),
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 0, expectedQuoting: 1,
            lineNumber: 2, expectedLineNumber: 2); // Line number first increment when \n handled

        AssertParseAnyCharsMask("\"" + new string('\r', s_nativeBitSize - 3) + "\"\r\n", [s_nativeBitSize - 1],
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 2,
            lineNumber: 2, expectedLineNumber: 3 + s_nativeBitSize - 3);
    }

    static void AssertParseAnyCharsMask(string chars, int[] expected,
        int rowLineEndingOffset, int expectedRowLineEndingOffset,
        nuint quoting = 0, nuint expectedQuoting = 0,
        int lineNumber = -1, int expectedLineNumber = -1)
    {
        for (var i = 0; i < expected.Length; ++i) { expected[i] += CharsIndexOffset; }
        var mask = MaskFor(chars);
        Span<int> colEnds = stackalloc int[s_nativeBitSize + 1];
        ref var start = ref colEnds[0];

        ref var end = ref ParseAnyCharsMask(mask, Separator,
            ref MemoryMarshal.GetReference<char>(chars), CharsIndexOffset,
            ref rowLineEndingOffset, ref quoting, ref start, ref lineNumber);

        AssertParseState(expected, colEnds, ref start, ref end,
            expectedQuoting, quoting,
            expectedLineNumber, lineNumber);
        Assert.AreEqual(expectedRowLineEndingOffset, rowLineEndingOffset);
    }
}
