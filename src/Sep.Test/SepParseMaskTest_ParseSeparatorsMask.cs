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
        }
    }

    [TestMethod]
    public void ParseSeparatorsMask_DifferentDensitiesAndBounds()
    {
        var random = new Random(12345);
        for (var iteration = 0; iteration < 1024 + 3 * s_nativeBitSize; ++iteration)
        {
            var mask = iteration < s_nativeBitSize ? (nuint)1 << iteration :
                iteration < 2 * s_nativeBitSize ? nuint.MaxValue >> (2 * s_nativeBitSize - iteration - 1) :
                iteration < 3 * s_nativeBitSize ? 0 : (nuint)(ulong)random.NextInt64();
            if (iteration >= 2 * s_nativeBitSize && iteration < 3 * s_nativeBitSize)
            {
                for (var i = 0; i <= iteration - 2 * s_nativeBitSize; ++i)
                { mask |= (nuint)1 << ((i * 13 + 19) % s_nativeBitSize); }
            }
            if (iteration >= 3 * s_nativeBitSize && (iteration & 1) != 0)
            { mask |= (nuint)1 << (s_nativeBitSize - 1); }
            if (mask == 0) { mask = 1; }
            var expected = Enumerable.Range(0, s_nativeBitSize)
                .Where(i => (mask & ((nuint)1 << i)) != 0).ToArray();
            var ends = Enumerable.Repeat(-1, expected.Length + 2).ToArray();
            ref var end = ref SepParseMask.ParseSeparatorsMask(mask, CharsIndexOffset, ref ends[0]);
            Assert.AreEqual(expected.Length, (int)Unsafe.ByteOffset(ref ends[0], ref end) / sizeof(int));
            Assert.AreEqual(-1, ends[0]);
            Assert.AreEqual(-1, ends[^1]);
            CollectionAssert.AreEqual(expected.Select(i => i + CharsIndexOffset).ToArray(), ends[1..^1]);

            var capacity = expected.Length + 2;
            var infos = Enumerable.Repeat(-1, capacity * 2).ToArray();
            ref var infoEnd = ref SepParseMask.ParseSeparatorsMask<SepColInfoMethods>(
                mask, CharsIndexOffset, ref infos[0], capacity);
            Assert.AreEqual(expected.Length, (int)Unsafe.ByteOffset(ref infos[0], ref infoEnd) / sizeof(int));
            Assert.AreEqual(-1, infos[0]);
            Assert.AreEqual(-1, infos[capacity - 1]);
            Assert.AreEqual(-1, infos[capacity]);
            Assert.AreEqual(-1, infos[^1]);
            CollectionAssert.AreEqual(expected.Select(i => i + CharsIndexOffset).ToArray(), infos[1..(capacity - 1)]);
            CollectionAssert.AreEqual(new int[expected.Length], infos[(capacity + 1)..^1]);
        }
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
        Assert.HasCount(expected.Length, actual);
        if (!expected.SequenceEqual(actual))
        {
            Assert.Fail($"{string.Join(',', expected)} != {string.Join(',', actual)}");
        }
        CollectionAssert.AreEqual(expected, actual);
    }
}
