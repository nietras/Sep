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
        var p0 = Vector128.Create(H, L, H, L, H, L, H, L, H, L, H, L, H, L, H, L);
        var p1 = Vector128.Create(H, L, L, L, H, L, L, L, H, L, L, L, H, L, L, L);
        var p2 = Vector128.Create(L, H, L, H, L, H, L, H, L, H, L, H, L, H, L, H);
        var p3 = Vector128.Create(H, H, H, H, H, H, H, H, H, H, H, H, H, H, H, H);
        if (AdvSimd.Arm64.IsSupported)
        {
            var m0 = SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt.MoveMask(p0, p1, p2, p3);
            var m1 = SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt.MoveMask2(p0, p1, p2, p3);
            if (m0 != m1)
            {
                Assert.Fail($"{ToBitString(m0)} != {ToBitString(m1)} {m0} != {m1}");
            }
        }
        static string ToBitString(ulong value) => Convert.ToString(unchecked((long)value), 2).PadLeft(64, '0');
    }
#endif
}
