using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

public partial class SepParseMaskTest
{
    sealed record Data(nuint Mask, int[] Expected);

    public delegate ref int ParseSeparatorsMaskMethod(
        nuint mask, int charsIndex, ref int colEndsRef);

    static readonly Data[] s_data =
    [
        new(0b0000_0000_0100_0001, [0, 6,]),
        new(0b1000_1001_0100_0001, [0, 6, 8, 11, 15]),
        new(nuint.MaxValue, Enumerable.Range(0, s_nativeBitSize).ToArray()),
        // Empty mask will output index after mask, that is expected, check mask
        // before calling parse mask. Some methods will not do the same since
        // using PopCount.
        // new(0, new[] { s_nativeBitSize }),
    ];

    public static IEnumerable<object[]> Methods =>
    [
        [new ParseSeparatorsMaskMethod(SepParseMask.ParseSeparatorsMask)],
    ];

    [TestMethod]
    [DynamicData(nameof(Methods))]
    public void SepParseMaskTest_ParseSeparatorsMask_Test_(ParseSeparatorsMaskMethod parse)
    {
        Contract.Assume(parse is not null);
        foreach (var (mask, expected) in s_data)
        {
            AssertParseSeparatorsMask(mask, parse, expected);
        };
    }

    static void AssertParseSeparatorsMask(
        nuint mask, ParseSeparatorsMaskMethod parse, int[] expected)
    {
        Span<int> colEnds = stackalloc int[s_nativeBitSize + 1];
        ref var start = ref MemoryMarshal.GetReference(colEnds);

        ref var end = ref parse(mask, CharsIndexOffset, ref start);

        var count = (int)Unsafe.ByteOffset(ref start, ref end) / sizeof(int);
        // Start 1 in since col ends are added one ahead, since [0] reserved for row start/first col start
        var actual = colEnds.Slice(1, count).ToArray()
            .Select(a => a - CharsIndexOffset).ToArray();
        Assert.AreEqual(expected.Length, actual.Length);
        if (!expected.SequenceEqual(actual))
        {
            Assert.Fail($"{string.Join(',', expected)} != {string.Join(',', actual)}");
        }
        CollectionAssert.AreEqual(expected, actual);
    }
}
