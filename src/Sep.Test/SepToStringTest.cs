using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepToStringTest
{
    [TestMethod]
    public void SepToStringTest_Direct()
    {
        var create = SepToString.Direct;
        using var pool0 = create(SepHeader.Empty, colIndex: 0);
        using var pool1 = create(SepHeader.Empty, colIndex: 0);

        var chars = new char[] { 'a' };
        var str0 = pool0.ToString(chars);
        var str1 = pool0.ToString(chars);

        Assert.AreSame(SepToString.Direct, create);
        Assert.AreSame(pool1, pool0);
        Assert.AreNotSame(str1, str0);
    }

    [TestMethod]
    public void SepToStringTest_OnePool()
    {
        var create = SepToString.OnePool();
        using var pool0 = create(SepHeader.Empty, colIndex: 0);
        using var pool1 = create(SepHeader.Empty, colIndex: 1);

        var chars = new char[] { 'a' };
        var str00 = pool0.ToString(chars);
        var str01 = pool0.ToString(chars);
        var str10 = pool1.ToString(chars);

        Assert.AreSame(pool1, pool0);
        Assert.AreSame(str01, str00);
        Assert.AreSame(str10, str00);
    }

    [TestMethod]
    public void SepToStringTest_PoolPerCol()
    {
        var create = SepToString.PoolPerCol();
        using var pool0 = create(SepHeader.Empty, colIndex: 0);
        using var pool1 = create(SepHeader.Empty, colIndex: 1);

        var chars = new char[] { 'a' };
        var str00 = pool0.ToString(chars);
        var str01 = pool0.ToString(chars);
        var str10 = pool1.ToString(chars);

        Assert.AreNotSame(pool1, pool0);
        Assert.AreSame(str01, str00);
        Assert.AreNotSame(str10, str00);
    }
}
