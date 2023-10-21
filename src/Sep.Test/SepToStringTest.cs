using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepToStringTest
{
    [TestMethod]
    public void SepToStringTest_Direct()
    {
        var create = SepToString.Direct;
        using var pool0 = create(null, colCount: 1);
        using var pool1 = create(SepHeader.Empty, colCount: 1);

        var chars = new char[] { 'a' };
        var str0 = pool0.ToString(chars, colIndex: 0);
        var str1 = pool0.ToString(chars, colIndex: 0);

        Assert.AreSame(SepToString.Direct, create);
        Assert.AreSame(pool1, pool0);
        Assert.AreNotSame(str1, str0);
    }

    [TestMethod]
    public void SepToStringTest_OnePool()
    {
        var create = SepToString.OnePool();
        using var pool0 = create(null, colCount: 1);
        using var pool1 = create(SepHeader.Empty, colCount: 1);

        var chars = new char[] { 'a' };
        var str00 = pool0.ToString(chars, colIndex: 0);
        var str01 = pool0.ToString(chars, colIndex: 0);
        var str10 = pool1.ToString(chars, colIndex: 1);

        Assert.AreSame(pool1, pool0);
        Assert.AreSame(str01, str00);
        Assert.AreSame(str10, str00);
    }

    [TestMethod]
    public void SepToStringTest_PoolPerCol()
    {
        var create = SepToString.PoolPerCol();
        var header = new SepHeader("A", new Dictionary<string, int>() { { "A", 0 } });
        using var pool0 = create(header, colCount: 1);
        using var pool1 = create(null, colCount: 1);

        var chars = new char[] { 'a' };
        var str00 = pool0.ToString(chars, colIndex: 0);
        var str01 = pool0.ToString(chars, colIndex: 0);
        var str02 = pool0.ToString(chars, colIndex: 1);
        var str10 = pool1.ToString(chars, colIndex: 0);
        var str11 = pool1.ToString(chars, colIndex: 0);

        Assert.AreNotSame(pool1, pool0);
        Assert.AreSame(str01, str00);
        Assert.AreNotSame(str02, str00);
        Assert.AreNotSame(str10, str00);
        Assert.AreSame(str11, str10);
    }
}
