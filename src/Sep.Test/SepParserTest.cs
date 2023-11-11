using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepParserTest
{
    // TODO: Add randomized long tests using baseline naive parser implementation
    readonly SepReaderState _state = new();
    readonly char[] _chars;
    readonly int[] _colEnds;

    public SepParserTest()
    {
        _chars = new char[1024];
        _state._chars = _chars;
        _colEnds = new int[1024];
        _state._colEndsOrColInfos = _colEnds;
    }

    static IEnumerable<object[]> Parsers => SepParserFactory.CreateFactories()
        .Select(f => new object[] { f.Value(Sep.Default) });

    [TestMethod]
    public void SepParserTest_CreateBest()
    {
        Assert.IsNotNull(SepParserFactory.CreateBest(Sep.Default));
    }

    [TestMethod]
    public void SepParserTest_CreateFactories()
    {
        var factories = SepParserFactory.CreateFactories();
        Assert.IsTrue(factories.Count > 0);
    }

    [TestMethod]
    public void SepParserTest_CreateAcceleratedFactories()
    {
        var factories = SepParserFactory.CreateAcceleratedFactories();
        Assert.IsTrue(factories.Count > 0);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_Properties(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;
        Assert.IsTrue(parser.PaddingLength >= 0);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_Sequence(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;

        _state._charsDataEnd = FillChars(new(Enumerable.Range(0, 256).Select(i => (char)i).ToArray()));
        _state._lineNumber = 3;

        var rowLineEndingOffset = parser.ParseColEnds(_state);

        // No assert, test is mainly for debugging SIMD code easily
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_Short(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;

        var charsEnd = FillChars("ˉ_;___;ˉˉ\n");
        _state._charsDataEnd = charsEnd;
        _state._lineNumber = 3;

        var rowLineEndingOffset = parser.ParseColEnds(_state);

        var expected = new int[] { 2, 6, 9 };
        AreEqual(expected, _colEnds, 0, _state._colCount);
        Assert.AreEqual(charsEnd, _state._charsParseStart);
        Assert.AreEqual(1, rowLineEndingOffset);
        Assert.AreEqual(4, _state._lineNumber);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_Long(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;

        var charsEnd = FillChars(";ˉ;ˉ\n\rˉ\";\";ˉˉ#ˉˉˉˉˉ\rˉˉˉˉ\nˉˉ\r\nˉ;ˉˉ\";\r\n\"ˉˉ,ˉ;ˉ.ˉ;ˉ\nˉˉ\rˉ");
        var expectedSet = new Expected[]
        {
            new(new[]{ 0, 2, 4, }, 5, RowLineEndingOffset: 1, 4),
            new(new[]{ 5, }, 6, RowLineEndingOffset: 1, 5),
            new(new[]{ 10, 19 }, 20, RowLineEndingOffset: 1, 6),
            new(new[]{ 24 }, 25, RowLineEndingOffset: 1, 7),
            new(new[]{ 27 }, 29, RowLineEndingOffset: 2, 8),
            new(new[]{ 30, 42, 46, 48 }, 49, RowLineEndingOffset: 1, 10),
            new(new[]{ 51 }, 52, RowLineEndingOffset: 1, 11),
            new(Array.Empty<int>(), charsEnd, RowLineEndingOffset: 0, 11),
        };
        AssertParserOutput(parser, charsStart: 0, charsEnd, expectedSet);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_Long_SeparatorsOnly(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;
        var charsEnd = FillChars(";ˉ;ˉ;;ˉ;ˉ;;ˉˉ#ˉˉˉˉˉ;ˉˉˉˉ;ˉˉ;;ˉ;ˉ" + "ˉ;ˉˉˉ;ˉˉ,ˉ;ˉ.ˉ;ˉ;ˉˉˉ;");
        var expectedSet = new Expected[]
        {
            new(new[]{ 0, 2, 4, 5, 7, 9, 10, 19, 24, 27, 28, 30, 33, 37, 42, 46, 48, 52 }, charsEnd, RowLineEndingOffset: 0, 3),
        };
        AssertParserOutput(parser, charsStart: 0, charsEnd, expectedSet);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_Long_At_ParseStart(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;
        var charsEnd = FillChars(";ˉ;ˉ\n\rˉ\"ˉ\";ˉˉ#ˉˉˉˉˉ\rˉˉˉˉ\nˉˉ\r\nˉ;ˉ" + "ˉ\"ˉˉˉ\"ˉˉ,ˉ;ˉ.ˉ;ˉ\nˉˉˉ\r");
        var expectedSet = new Expected[]
        {
            new(new[]{ 10, 19 }, 20, RowLineEndingOffset: 1, 4),
            new(new[]{ 24 }, 25, RowLineEndingOffset: 1, 5),
            new(new[]{ 27 }, 29, RowLineEndingOffset: 2, 6),
            new(new[]{ 30, 42, 46, 48 }, 49, RowLineEndingOffset: 1, 7),
            new(new[]{ 52 }, charsEnd, RowLineEndingOffset: 1, 8),
        };
        AssertParserOutput(parser, charsStart: 7, charsEnd, expectedSet);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_Long_ColEndsAlmostFilled(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;
        var charsEnd = FillChars(";ˉ;ˉ\n\rˉ" + "\"ˉ\";ˉˉ#ˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉ;ˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉ\nˉˉ\r\nˉ;ˉ" + "ˉ\"ˉˉˉ\"ˉˉ,ˉ;ˉ.ˉ;ˉ\nˉˉˉ\r");
        var paddingOffset = (parser.PaddingLength > 0 ? parser.PaddingLength : 1);
        _state._colCount = _colEnds.Length - paddingOffset;
        var charsStart = 7;
        var ends = new[] { 10, 46 }.Where(i => i < (charsStart + paddingOffset)).ToArray();
        var nextStart = charsStart + paddingOffset;
        var lineEndingOffset = 0;
        var expected = new Expected(ends, nextStart, lineEndingOffset, 3 + lineEndingOffset);
        var expectedSet = new Expected[] { expected };

        AssertParserOutput(parser, charsStart, charsEnd, expectedSet);
    }

    int FillChars(string text)
    {
        text.AsSpan().CopyTo(_chars);
        return text.Length;
    }

    record struct Expected(int[] ColEnds, int NextStart, int RowLineEndingOffset, int LineNumber);

    void AssertParserOutput(ISepParser parser, int charsStart, int charsEnd, Expected[] expectedSet)
    {
        var colEndsFrom = _state._colCount;
        _state._charsParseStart = charsStart;
        _state._charsDataEnd = charsEnd;
        _state._lineNumber = 3;
        foreach (var (expected, expectedNextStart, expectedRowLineEndingOffset, expectedLineNumber) in expectedSet)
        {
            var rowLineEndingOffset = parser.ParseColEnds(_state);

            AreEqual(expected, _colEnds, colEndsFrom, _state._colCount);
            Assert.AreEqual(expectedNextStart, _state._charsParseStart, nameof(_state._charsParseStart));
            Assert.AreEqual(expectedRowLineEndingOffset, rowLineEndingOffset, nameof(rowLineEndingOffset));
            Assert.AreEqual(expectedLineNumber, _state._lineNumber, nameof(_state._lineNumber));
            charsStart = _state._charsParseStart;
            colEndsFrom = _state._colCount;
        }
    }

    static void AreEqual(ReadOnlySpan<int> expected, int[] actual, int actualFrom, int actualTo)
    {
        // "+ 1" in since col ends are added one ahead, since [0] reserved for row start/first col start
        AreEqual(expected, actual.AsSpan().Slice(actualFrom + 1, actualTo - actualFrom));
    }

    static void AreEqual(ReadOnlySpan<int> expected, ReadOnlySpan<int> actual)
    {
        Assert.AreEqual(expected.Length, actual.Length, nameof(actual.Length));
        for (var i = 0; i < expected.Length; i++)
        {
            var e = expected[i];
            var a = actual[i];
            Assert.AreEqual(e, a, $"{i:D2}: {e} != {a}");
        }
    }
}
