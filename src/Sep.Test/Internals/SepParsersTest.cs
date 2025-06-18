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
        var p0 = Vector128.CreateSequence<byte>(1 + 16 * 0, 1);
        var p1 = Vector128.CreateSequence<byte>(1 + 16 * 1, 1);
        var p2 = Vector128.CreateSequence<byte>(1 + 16 * 2, 1);
        var p3 = Vector128.CreateSequence<byte>(1 + 16 * 3, 1);
        if (AdvSimd.Arm64.IsSupported)
        {
            var m0 = SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt.MoveMask(p0, p1, p2, p3);
            var m1 = SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt.MoveMask2(p0, p1, p2, p3);
            if (m0 != m1)
            {
                Assert.Fail($"{ToBitString(m0) != ToBitString(m1)} {m0} != {m1}");
            }
        }
        static string ToBitString(ulong value) => Convert.ToString(unchecked((long)value), 2).PadLeft(64, '0');
    }
#endif
}
