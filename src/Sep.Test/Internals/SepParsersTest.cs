#if NET9_0_OR_GREATER
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using VecUI16 = System.Runtime.Intrinsics.Vector128<ushort>;
using VecUI8 = System.Runtime.Intrinsics.Vector128<byte>;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test.Internals;

[TestClass]
public class SepParsersTest
{
#if NET9_0_OR_GREATER
    [TestMethod]
    public unsafe void SepParsersTest_AdvSimd_BulkMoveMask()
    {
        byte L = 0;
        byte H = 255;
        var p0 = Vector128.Create(H, L, L, L, L, L, L, L, L, L, L, L, L, L, L, L);
        var p1 = Vector128.Create(L, H, L, L, L, L, L, L, L, L, L, L, L, L, L, L);
        var p2 = Vector128.Create(L, L, H, L, L, L, L, L, L, L, L, L, L, L, L, L);
        var p3 = Vector128.Create(L, L, L, H, L, L, L, L, L, L, L, L, L, L, L, L);
        var d = stackalloc byte[Vector128<byte>.Count * 4];
        Vector128.Store(p0, d + Vector128<byte>.Count * 0);
        Vector128.Store(p1, d + Vector128<byte>.Count * 1);
        Vector128.Store(p2, d + Vector128<byte>.Count * 2);
        Vector128.Store(p3, d + Vector128<byte>.Count * 3);
        if (AdvSimd.Arm64.IsSupported)
        {
            var m0 = SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt.MoveMaskNormal(p0, p1, p2, p3);

            (p0, p1, p2, p3) = AdvSimd.Arm64.Load4xVector128AndUnzip(d);

            // Combine with shifting to pack into one vector
            var t0 = AdvSimd.ShiftRightAndInsert(p1, p0, 1); // vsriq_n_u8(p1, p0, 1)
            var t1 = AdvSimd.ShiftRightAndInsert(p3, p2, 1); // vsriq_n_u8(p3, p2, 1)
            var t2 = AdvSimd.ShiftRightAndInsert(t1, t0, 2); // vsriq_n_u8(t1, t0, 2)
            var t3 = AdvSimd.ShiftRightAndInsert(t2, t2, 4); // vsriq_n_u8(t2, t2, 4)

            // Narrow to 8 bytes (shift right by 4 bits each 16-bit lane, then take lower half)
            var t4 = AdvSimd.ShiftRightLogicalNarrowingLower(t3.AsUInt16(), 4); // vshrn_n_u16
            var m1 = (nuint)t4.AsUInt64().GetElement(0);

            //var m1 = SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt.MoveMask2(p0, p1, p2, p3);
            if (m0 != m1)
            {
                Assert.Fail($"{ToBitString(m0)} != {ToBitString(m1)} {m0} != {m1}\n" +
                            $"p0: {ToBitString(p0)}\np1: {ToBitString(p1)}\np2: {ToBitString(p2)}\np3: {ToBitString(p3)}\n" +
                            $"t0: {ToBitString(t0)}\nt1: {ToBitString(t1)}\nt2: {ToBitString(t2)}\nt3: {ToBitString(t3)}\n" +
                            $"t4: {ToBitString(t4.AsByte())}\n"
                            );
            }
        }
    }

    [TestMethod]
    public unsafe void SepParsersTest_Load4()
    {
        if (AdvSimd.Arm64.IsSupported)
        {
            var a = Enumerable.Range(1, 64).Select(i => (ushort)i).ToArray();
            var b = new byte[a.Length];
            fixed (ushort* charsPtr = a)
            fixed (byte* bytesPtr = b)
            {
                var (ushort0, ushort1, ushort2, ushort3) = AdvSimd.Arm64
                    .Load4xVector128AndUnzip((ushort*)charsPtr);
                var (ushort4, ushort5, ushort6, ushort7) = AdvSimd.Arm64
                    .Load4xVector128AndUnzip((ushort*)charsPtr + Vector128<ushort>.Count * 4);
                var bytes0 = NarrowSaturated(ushort0, ushort4);
                var bytes1 = NarrowSaturated(ushort1, ushort5);
                var bytes2 = NarrowSaturated(ushort2, ushort6);
                var bytes3 = NarrowSaturated(ushort3, ushort7);

                Vector128.Store(bytes0, bytesPtr + Vector128<byte>.Count * 0);
                Vector128.Store(bytes1, bytesPtr + Vector128<byte>.Count * 1);
                Vector128.Store(bytes2, bytesPtr + Vector128<byte>.Count * 2);
                Vector128.Store(bytes3, bytesPtr + Vector128<byte>.Count * 3);
            }
            var expected = new byte[b.Length];
            for (int i = 0; i < expected.Length / 4; i++)
            {
                expected[i] = (byte)(i * 4 + 1);
                expected[i + 1 * expected.Length / 4] = (byte)(i * 4 + 2);
                expected[i + 2 * expected.Length / 4] = (byte)(i * 4 + 3);
                expected[i + 3 * expected.Length / 4] = (byte)(i * 4 + 4);
            }
            var equal = expected.SequenceEqual(b);
            if (!equal)
            {
                var expectedBits = string.Join("-", expected);
                var actualBits = string.Join("-", b);
                Assert.Fail($"Expected: {expectedBits}\nActual  : {actualBits}");
            }
        }
    }
    private static VecUI8 NarrowSaturated(VecUI16 v0, VecUI16 v1)
    {
        var r0 = AdvSimd.ExtractNarrowingSaturateLower(v0);
        var r1 = AdvSimd.ExtractNarrowingSaturateUpper(r0, v1);
        return r1;
    }

    static string ToBitString(ulong value) => Convert.ToString(unchecked((long)value), 2).PadLeft(64, '0');

    static string ToBitString(Vector128<byte> v)
    {
        var s = "";
        for (var i = 0; i < Vector128<byte>.Count; ++i)
        {
            s += Convert.ToString(v[i], 2).PadLeft(8, '0');
            s += "-";
        }
        return s;
    }

    static string ToBitString(Vector64<byte> v)
    {
        var s = "";
        for (var i = 0; i < Vector64<byte>.Count; ++i)
        {
            s += Convert.ToString(v[i], 2).PadLeft(8, '0');
            s += "-";
        }
        return s;
    }
#endif
}
