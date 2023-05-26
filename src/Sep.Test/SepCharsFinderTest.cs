#if DEBUG
global using Pos = nietras.SeparatedValues.SepCharPosition;
#else
global using Pos = System.Int32;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepCharsFinderTest
{
    // TODO: Add randomized long tests using baseline naive finder implementation
    readonly char[] _chars;
    readonly Pos[] _positions;
    int _positionsStart = 0;
    int _positionsEnd = 0;

    public SepCharsFinderTest()
    {
        _chars = new char[1024];
        _positions = new Pos[128];
    }

    static IEnumerable<object[]> Finders => SepCharsFinderFactory.CreateFactories()
        .Select(f => new object[] { f.Value(Sep.Default) });

    [TestMethod]
    public void SepCharsFinderTest_GetBest()
    {
        Assert.IsNotNull(SepCharsFinderFactory.GetBest(Sep.Default));
    }

    [TestMethod]
    public void SepCharsFinderTest_CreateFactories()
    {
        var factories = SepCharsFinderFactory.CreateFactories();
        Assert.IsTrue(factories.Count > 0);
    }

    [TestMethod]
    public void SepCharsFinderTest_CreateAcceleratedFactories()
    {
        var factories = SepCharsFinderFactory.CreateAcceleratedFactories();
        Assert.IsTrue(factories.Count > 0);
    }

    [TestMethod]
    [DynamicData(nameof(Finders))]
    public void SepCharsFinderTest_Properties(ISepCharsFinder finder)
    {
        Contract.Assume(finder is not null);
        Assert.IsTrue(finder.PaddingLength <= finder.RequestedPositionsFreeLength);
    }

    [TestMethod]
    [DynamicData(nameof(Finders))]
    public void SepCharsFinderTest_Find_Short(ISepCharsFinder finder)
    {
        Contract.Assume(finder is not null);
        var charsEnd = FillChars("ˉ_;___;ˉˉ\n");

        var nextStart = finder.Find(_chars, charsStart: 0, charsEnd,
                                    _positions, _positionsStart, ref _positionsEnd);

        var expected = new Pos[] { P(';', 2), P(';', 6), P('\n', 9), };
        AreEqual(expected, _positions, _positionsStart, _positionsEnd);
        Assert.AreEqual(charsEnd, nextStart);
    }

    [TestMethod]
    [DynamicData(nameof(Finders))]
    public void SepCharsFinderTest_Find_Long(ISepCharsFinder finder)
    {
        Contract.Assume(finder is not null);
        var charsEnd = FillChars(";ˉ;ˉ\n\rˉ\"ˉ\";ˉˉ#ˉˉˉˉˉ\rˉˉˉˉ\nˉˉ\r\nˉ;ˉ" + "ˉ\"ˉˉˉ\"ˉˉ,ˉ;ˉ.ˉ;ˉ\nˉˉ\rˉ");

        var nextStart = finder.Find(_chars, charsStart: 0, charsEnd,
                                    _positions, _positionsStart, ref _positionsEnd);

        var expected = new Pos[]
        {
            P(';', 0),   P(';', 2),   P('\n', 4),  P('\r', 5),  P('\"', 7),
            P('\"', 9),  P(';', 10),  P('\r', 19), P('\n', 24), P('\r', 27),
            P('\n', 28), P(';', 30),  P('\"', 33), P('\"', 37), P(';', 42),
            P(';', 46),  P('\n', 48), P('\r', 51),
        };
        AreEqual(expected, _positions, _positionsStart, _positionsEnd);
        Assert.AreEqual(charsEnd, nextStart);
    }

    [TestMethod]
    [DynamicData(nameof(Finders))]
    public void SepCharsFinderTest_Find_Long_SeparatorsOnly(ISepCharsFinder finder)
    {
        Contract.Assume(finder is not null);
        var charsEnd = FillChars(";ˉ;ˉ;;ˉ;ˉ;;ˉˉ#ˉˉˉˉˉ;ˉˉˉˉ;ˉˉ;;ˉ;ˉ" + "ˉ;ˉˉˉ;ˉˉ,ˉ;ˉ.ˉ;ˉ;ˉˉˉ;");

        var nextStart = finder.Find(_chars, charsStart: 0, charsEnd,
                                    _positions, _positionsStart, ref _positionsEnd);

        var expected = new Pos[]
        {
            P(';', 0),   P(';', 2),   P(';', 4),  P(';', 5),  P(';', 7),
            P(';', 9),  P(';', 10),  P(';', 19), P(';', 24), P(';', 27),
            P(';', 28), P(';', 30),  P(';', 33), P(';', 37), P(';', 42),
            P(';', 46),  P(';', 48), P(';', 52),
        };
        AreEqual(expected, _positions, _positionsStart, _positionsEnd);
        Assert.AreEqual(charsEnd, nextStart);
    }

    [TestMethod]
    [DynamicData(nameof(Finders))]
    public void SepCharsFinderTest_Find_Long_At_ParseStart(ISepCharsFinder finder)
    {
        Contract.Assume(finder is not null);
        var charsEnd = FillChars(";ˉ;ˉ\n\rˉ\"ˉ\";ˉˉ#ˉˉˉˉˉ\rˉˉˉˉ\nˉˉ\r\nˉ;ˉ" + "ˉ\"ˉˉˉ\"ˉˉ,ˉ;ˉ.ˉ;ˉ\nˉˉˉ\r");

        var nextStart = finder.Find(_chars, charsStart: 8, charsEnd,
                                    _positions, _positionsStart, ref _positionsEnd);

        var expected = new Pos[]
        {
            P('\"', 9),  P(';', 10),  P('\r', 19), P('\n', 24), P('\r', 27),
            P('\n', 28), P(';', 30),  P('\"', 33), P('\"', 37), P(';', 42),
            P(';', 46),  P('\n', 48), P('\r', 52),
        };
        AreEqual(expected, _positions, _positionsStart, _positionsEnd);
        Assert.AreEqual(charsEnd, nextStart);
    }

    [TestMethod]
    [DynamicData(nameof(Finders))]
    public void SepCharsFinderTest_Find_Long_PositionsAlmostFilled(ISepCharsFinder finder)
    {
        Contract.Assume(finder is not null);
        var charsEnd = FillChars(";ˉ;ˉ\n\rˉ\"ˉ\";ˉˉ#ˉˉˉˉˉ\rˉˉˉˉ\nˉˉ\r\nˉ;ˉ" + "ˉ\"ˉˉˉ\"ˉˉ,ˉ;ˉ.ˉ;ˉ\nˉˉˉ\r");
        _positionsStart = _positions.Length - (finder.PaddingLength > 0 ? finder.PaddingLength : 1);
        _positionsEnd = _positionsStart;
        var charsStart = 8;

        var nextStart = finder.Find(_chars, charsStart, charsEnd,
                                    _positions, _positionsStart, ref _positionsEnd);

        var allExpected = new Pos[]
        {
            P('\"', 9),  P(';', 10),  P('\r', 19), P('\n', 24), P('\r', 27),
            P('\n', 28), P(';', 30),  P('\"', 33), P('\"', 37), P(';', 42),
            P(';', 46),  P('\n', 48), P('\r', 52),
        };
        // Padding equal to how many searched at a time, so need for free in positions
        var endPosition = finder.PaddingLength > 0
            ? charsStart + finder.PaddingLength
            : charsStart + 2;
        var expected = allExpected.Where(p => SepCharPosition.UnpackPosition(p) < endPosition).ToArray();

        AreEqual(expected, _positions, _positionsStart, _positionsEnd);
        Assert.AreEqual(endPosition, nextStart);
    }

    int FillChars(string text)
    {
        text.AsSpan().CopyTo(_chars);
        return text.Length;
    }

#if DEBUG
    static Pos P(char c, int pos) => SepCharPosition.Pack(c, pos);
#else
    static Pos P(char c, int pos) => SepCharPosition.Pack(c, pos);
#endif

    static void AreEqual(ReadOnlySpan<Pos> expected, Pos[] actual, int positionsStart, int positionsEnd)
    {
        AreEqual(expected, actual.AsSpan().Slice(positionsStart, positionsEnd - positionsStart));
    }

    static void AreEqual(ReadOnlySpan<Pos> expected, ReadOnlySpan<Pos> actual)
    {
        Assert.AreEqual(expected.Length, actual.Length);
        for (var i = 0; i < expected.Length; i++)
        {
            var e = expected[i];
            var a = actual[i];
            Assert.AreEqual(e, a, $"{i:D2}: E({e}) != A({a})");
        }
    }
}
