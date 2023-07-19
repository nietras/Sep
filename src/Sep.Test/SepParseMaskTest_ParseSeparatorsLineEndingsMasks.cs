using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static nietras.SeparatedValues.SepParseMask;

namespace nietras.SeparatedValues.Test;

public partial class SepParseMaskTest
{
    [TestMethod]
    public void SepParseMaskTest_ParseSeparatorsLineEndingsMasks_Ordinary()
    {
        AssertParseSeparatorsLineEndingsMasks(";a;b\r\n;", new[] { 0, 2, 4 },
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 2,
            lineNumber: 2, expectedLineNumber: 3);

        AssertParseSeparatorsLineEndingsMasks("a;;b\r;", new[] { 1, 2, 4 },
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 1,
            lineNumber: 2, expectedLineNumber: 3);

        AssertParseSeparatorsLineEndingsMasks(";a;b\n;", new[] { 0, 2, 4 },
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 1,
            lineNumber: 2, expectedLineNumber: 3);

        AssertParseSeparatorsLineEndingsMasks(";aa;bbb;cccc;\n", new[] { 0, 3, 7, 12, 13 },
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 1,
            lineNumber: 2, expectedLineNumber: 3);

        AssertParseSeparatorsLineEndingsMasks(new string('a', 31) + "\r\n", new[] { 31 },
            rowLineEndingOffset: 0, expectedRowLineEndingOffset: 2,
            lineNumber: 2, expectedLineNumber: 3);
    }

    static void AssertParseSeparatorsLineEndingsMasks(string chars, int[] expected,
        int rowLineEndingOffset, int expectedRowLineEndingOffset,
        int quoting = 0, int expectedQuoting = 0,
        int lineNumber = -1, int expectedLineNumber = -1)
    {
        for (var i = 0; i < expected.Length; ++i) { expected[i] += CharsIndexOffset; }
        var separatorsMask = SeparatorsMaskFor(chars);
        var lineEndingsMask = LineEndingsMaskFor(chars);
        Span<int> colEnds = stackalloc int[32 + 1];
        ref var start = ref colEnds[0];

        var charsIndex = CharsIndexOffset;

        ref var end = ref ParseSeparatorsLineEndingsMasks(
            separatorsMask, separatorsMask | lineEndingsMask,
            ref MemoryMarshal.GetReference<char>(chars), ref charsIndex, Separator,
            ref start, ref rowLineEndingOffset, ref lineNumber);

        AssertParseState(expected, colEnds, ref start, ref end,
            expectedRowLineEndingOffset, rowLineEndingOffset,
            expectedQuoting, quoting,
            expectedLineNumber, lineNumber);
    }

    static int SeparatorsMaskFor(ReadOnlySpan<char> chars)
    {
        var mask = 0;
        for (var i = 0; i < Math.Min(chars.Length, 32); i++)
        {
            if (chars[i] == Separator)
            {
                mask |= 1 << i;
            }
        }
        return mask;
    }

    static int LineEndingsMaskFor(ReadOnlySpan<char> chars)
    {
        var mask = 0;
        for (var i = 0; i < Math.Min(chars.Length, 32); i++)
        {
            var c = chars[i];
            if (c == SepDefaults.LineFeed || c == SepDefaults.CarriageReturn)
            {
                mask |= 1 << i;
            }
        }
        return mask;
    }
}
