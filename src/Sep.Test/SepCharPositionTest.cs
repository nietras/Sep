using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepCharPositionTest
{
#if DEBUG
    readonly SepCharPosition _positionA = SepCharPosition.Pack('a', 1);
#else
    readonly SepCharPosition _positionA = new(SepCharPosition.Pack('a', 1));
#endif
    readonly SepCharPosition _positionB = new('b', 2);

    [TestMethod]
    public void SepCharPositionTest_Internals()
    {
        Assert.AreEqual(16777216, SepCharPosition.MaxLength);
        Assert.AreEqual(16777215, SepCharPosition.MaxPosition);
    }

    [TestMethod]
    public void SepCharPositionTest_Properties()
    {
        Assert.AreEqual('a', _positionA.Character);
        Assert.AreEqual((byte)'a', _positionA.CharacterByte);
        Assert.AreEqual(1, _positionA.Position);
        Assert.AreEqual(0x61000001, _positionA.Packed);
    }

    [TestMethod]
    public void SepCharPositionTest_PackRaw_Char()
    {
        Assert.AreEqual(0x61000001, SepCharPosition.PackRaw('a', 1));
    }

    [TestMethod]
    public void SepCharPositionTest_Pack_Byte()
    {
#if DEBUG
        Assert.AreEqual(_positionA, SepCharPosition.Pack((byte)'a', 1));
#else
        Assert.AreEqual(_positionA.Packed, SepCharPosition.Pack((byte)'a', 1));
#endif
    }

    [TestMethod]
    public void SepCharPositionTest_PackRaw_Byte()
    {
        Assert.AreEqual(0x61000001, SepCharPosition.PackRaw((byte)'a', 1));
    }

    [TestMethod]
    public void SepCharPositionTest_GetHashCode()
    {
        Assert.AreEqual(_positionA.GetHashCode(), SepCharPosition.Pack('a', 1).GetHashCode());
        Assert.AreNotEqual(_positionA.GetHashCode(), _positionB.GetHashCode());
    }

    [TestMethod]
    public void SepCharPositionTest_Equality()
    {
        Assert.IsTrue(_positionA.Equals(_positionA));
        Assert.IsTrue(_positionA.Equals((object)_positionA));
        Assert.IsFalse(_positionA.Equals(_positionB));
        Assert.IsFalse(_positionA.Equals((object)_positionB));
        Assert.IsFalse(_positionB.Equals(_positionA));
        Assert.IsFalse(_positionA.Equals((object)"abc"));

        Assert.IsTrue(_positionA == new SepCharPosition('a', 1));
        Assert.IsTrue(_positionA == new SepCharPosition((byte)'a', 1));
        Assert.IsFalse(_positionA == _positionB);
        Assert.IsFalse(_positionB == _positionA);
    }

    [TestMethod]
    public void SepCharPositionTest_Inequality()
    {
        Assert.IsFalse(_positionA != new SepCharPosition('a', 1));
        Assert.IsTrue(_positionA != _positionB);
        Assert.IsTrue(_positionB != _positionA);
    }

    [TestMethod]
    public void SepCharPositionTest_Deconstruct()
    {
        var (c, p) = _positionA;
        Assert.AreEqual('a', c);
        Assert.AreEqual(1, p);
    }

    [TestMethod]
    public void SepCharPositionTest_DebuggerDisplay()
    {
        Assert.AreEqual("'a'=97=01100001 at '0001'", _positionA.DebuggerDisplay);
    }
}
