using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepArrayPoolAccessIndexedTest
{
    readonly SepArrayPoolAccessIndexed _sut = new();

    [TestMethod]
    public void SepArrayPoolAccessIndexedTest_Ctor() { }

    [TestMethod]
    public void SepArrayPoolAccessIndexedTest_RentUniqueArrayAsSpan()
    {
        AssertUniqueSpans<int>();
        AssertUniqueSpans<float>();
        AssertUniqueSpans<double>();
        AssertUniqueSpans<string>();
    }

    [TestMethod]
    public void SepArrayPoolAccessIndexedTest_RentUniqueArrayAsSpan_Reset_RentUniqueArrayAsSpan_Same()
    {
        var spanInt0 = _sut.RentUniqueArrayAsSpan<int>(length: 4);
        var spanFloat0 = _sut.RentUniqueArrayAsSpan<float>(length: 8);

        _sut.Reset();

        var spanInt1 = _sut.RentUniqueArrayAsSpan<int>(length: 4);
        var spanFloat1 = _sut.RentUniqueArrayAsSpan<float>(length: 8);

        AssertRef(spanInt0, spanInt1, areSame: true);
        AssertRef(spanFloat0, spanFloat1, areSame: true);
    }

    [TestMethod]
    public void SepArrayPoolAccessIndexedTest_RentUniqueArrayAsSpan_Reset_RentUniqueArrayAsSpan_LargerNotSame()
    {
        var spanInt0 = _sut.RentUniqueArrayAsSpan<int>(length: 4);

        _sut.Reset();

        var spanInt1 = _sut.RentUniqueArrayAsSpan<int>(length: 128);

        Assert.AreEqual(128, spanInt1.Length);
        AssertRef(spanInt0, spanInt1, areSame: false);
    }

    [TestMethod]
    public void SepArrayPoolAccessIndexedTest_RentUniqueArrayAsSpan_Dispose()
    {
        SepArrayPoolAccessIndexedTest_RentUniqueArrayAsSpan();

        _sut.Dispose();
    }

    void AssertUniqueSpans<T>()
    {
        var span4_0 = _sut.RentUniqueArrayAsSpan<T>(length: 4);
        var span4_1 = _sut.RentUniqueArrayAsSpan<T>(length: 4);
        var span8_0 = _sut.RentUniqueArrayAsSpan<T>(length: 8);
        var span8_1 = _sut.RentUniqueArrayAsSpan<T>(length: 8);

        Assert.AreEqual(4, span4_0.Length);
        Assert.AreEqual(4, span4_1.Length);
        Assert.AreEqual(8, span8_0.Length);
        Assert.AreEqual(8, span8_1.Length);

        AssertRef(span4_0, span4_1, areSame: false);
        AssertRef(span4_0, span8_0, areSame: false);
        AssertRef(span8_0, span8_1, areSame: false);
    }

    static void AssertRef<T>(Span<T> a, Span<T> b, bool areSame)
    {
        ref var refA = ref MemoryMarshal.GetReference(a);
        ref var refB = ref MemoryMarshal.GetReference(b);
        Assert.AreEqual(areSame, Unsafe.AreSame(ref refA, ref refB));
    }
}
