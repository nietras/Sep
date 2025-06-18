#if NET9_0_OR_GREATER
using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test.Internals;

[TestClass]
public class SepParsersTest
{
#if NET9_0_OR_GREATER
    [TestMethod]
    public void SepParsersTest_AdvSimd_BulkMoveMask()
    {
        byte L = 0;
        byte H = 255;
        var p0 = Vector128.Create(H, L, L, L, L, L, L, L, L, L, L, L, L, L, L, L);
        var p1 = Vector128.Create(L, H, L, L, L, L, L, L, L, L, L, L, L, L, L, L);
        var p2 = Vector128.Create(L, L, H, L, L, L, L, L, L, L, L, L, L, L, L, L);
        var p3 = Vector128.Create(L, L, L, H, L, L, L, L, L, L, L, L, L, L, L, L);
        if (AdvSimd.Arm64.IsSupported)
        {
            var m0 = SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt.MoveMask(p0, p1, p2, p3);

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
                            $"p0: {p0}\np1: {p1}\np2: {p2}\np3: {p3}\n" +
                            $"t0: {t0}\nt1: {t1}\nt2: {t2}\nt3: {t3}\n" +
                            $"t4: {t4}\n"
                            );
            }
        }
        static string ToBitString(ulong value) => Convert.ToString(unchecked((long)value), 2).PadLeft(64, '0');
    }
#endif
}
