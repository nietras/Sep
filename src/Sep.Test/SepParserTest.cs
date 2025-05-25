using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepParserTest
{
    // TODO: Add randomized long tests using baseline naive parser implementation
    readonly SepReaderState _state = new();
    readonly SepReaderState _stateUnescape = new(colUnquoteUnescape: true);
    readonly char[] _chars;
    readonly int[] _colEndsOrColInfos;

    public SepParserTest()
    {
        _chars = new char[2048];
        _colEndsOrColInfos = new int[2048];

        _state._chars = _chars;
        _state._colEndsOrColInfos = _colEndsOrColInfos;

        _stateUnescape._chars = _chars;
        _stateUnescape._colEndsOrColInfos = _colEndsOrColInfos;
    }

    public static IEnumerable<object[]> Parsers => SepParserFactory.AvailableFactories
        .Select(f => new object[] { f.Value(new(Sep.Default)) });

    [TestMethod]
    public void SepParserTest_CreateBest()
    {
        var best = SepParserFactory.CreateBest(new(Sep.Default));
        Assert.IsNotNull(best);

        var forceParserName = SepParserFactory.GetForceParserName();
        if (string.IsNullOrEmpty(forceParserName))
        {
            Assert.IsInstanceOfType(best, GetBestExpectedType());

        }
        else if (!SepParserFactory.AvailableFactories.ContainsKey(forceParserName))
        {
            Assert.Inconclusive($"Unknown or unavailable forced parser name '{forceParserName}'");
        }

        static Type GetBestExpectedType()
        {
#if NET8_0_OR_GREATER
            if (Environment.Is64BitProcess && Avx512BW.IsSupported)
            { return typeof(SepParserAvx512To256CmpOrMoveMaskTzcnt); }
            if (Avx512BW.IsSupported)
            { return typeof(SepParserAvx512To256CmpOrMoveMaskTzcnt); }
            if (Environment.Is64BitProcess && Vector512.IsHardwareAccelerated)
            { return typeof(SepParserVector512NrwCmpExtMsbTzcnt); }
#endif
            if (Avx2.IsSupported) { return typeof(SepParserAvx2PackCmpOrMoveMaskTzcnt); }
            if (Sse2.IsSupported) { return typeof(SepParserSse2PackCmpOrMoveMaskTzcnt); }
            if (Environment.Is64BitProcess && AdvSimd.Arm64.IsSupported) { return typeof(SepParserAdvSimdX8NrwCmpOrMoveMaskTzcnt); }
            if (Vector256.IsHardwareAccelerated) { return typeof(SepParserVector256NrwCmpExtMsbTzcnt); }
            if (Vector128.IsHardwareAccelerated) { return typeof(SepParserVector128NrwCmpExtMsbTzcnt); }
            if (Vector64.IsHardwareAccelerated) { return typeof(SepParserVector64NrwCmpExtMsbTzcnt); }
            return typeof(SepParserIndexOfAny);
        }
    }

    [TestMethod]
    public void SepParserTest_AcceleratedFactories()
    {
        var factories = SepParserFactory.AcceleratedFactories;
        Assert.IsTrue(factories.Count > 0);
    }

    [TestMethod]
    public void SepParserTest_AvailableFactories()
    {
        var factories = SepParserFactory.AvailableFactories;
        Assert.IsTrue(factories.Count > 0);
    }


    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_Properties(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;
        Assert.IsTrue(parser.PaddingLength >= 0);
        Assert.IsTrue(parser.QuoteCount == 0);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_Sequence(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;

        _state._charsDataEnd = FillChars(new(Enumerable.Range(0, 256).Select(i => (char)i).ToArray()));
        _state._parsingLineNumber = 3;

        parser.ParseColEnds(_state);

        // No assert, test is mainly for debugging SIMD code easily
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColInfos_Sequence(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;

        _stateUnescape._charsDataEnd = FillChars(new(Enumerable.Range(0, 256).Select(i => (char)i).ToArray()));
        _stateUnescape._parsingLineNumber = 3;

        parser.ParseColInfos(_stateUnescape);

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
        _state._parsingLineNumber = 3;

        parser.ParseColEnds(_state);

        var expected = new int[] { 2, 6, 9 };
        Assert.AreEqual(1, _state._parsedRowsCount);
        var row = _state._parsedRows[0];
        AreEqual(expected, _colEndsOrColInfos, 0, row.ColCount);
        Assert.AreEqual(charsEnd, _state._charsParseStart);
        Assert.AreEqual(4, _state._parsingLineNumber);
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
            new([0, 2, 4,], NextStart : 5, RowLineEndingOffset: 1, 4),
            new([5,], NextStart : 6, RowLineEndingOffset: 1, 5),
            new([10, 19], NextStart : 20, RowLineEndingOffset: 1, 6),
            new([24], NextStart : 25, RowLineEndingOffset: 1, 7),
            new([27], NextStart : 30, RowLineEndingOffset: 2, 8),
            new([30, 42, 46, 48], NextStart : 49, RowLineEndingOffset: 1, 10),
            new([51], NextStart : 52, RowLineEndingOffset: 1, 11),
            //new(Array.Empty<int>(), charsEnd, RowLineEndingOffset: 0, 11),
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
            new([0, 2, 4, 5, 7, 9, 10, 19, 24, 27, 28, 30, 33, 37, 42, 46, 48, 52], charsEnd, RowLineEndingOffset: 0, 3),
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
            new([10, 19], NextStart: 20, RowLineEndingOffset: 1, 4),
            new([24], NextStart: 25, RowLineEndingOffset: 1, 5),
            new([27], NextStart: 30, RowLineEndingOffset: 2, 6),
            new([30, 42, 46, 48], NextStart: 49, RowLineEndingOffset: 1, 7),
            new([52], charsEnd, RowLineEndingOffset: 1, 8),
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
        _state._currentRowColCount = _colEndsOrColInfos.Length - paddingOffset;
        var charsStart = 7;
        var ends = new[] { 10, 46 }.Where(i => i < (charsStart + paddingOffset)).ToArray();
        var nextStart = charsStart + paddingOffset;
        var lineEndingOffset = 0;
        var expected = new Expected(ends, nextStart, lineEndingOffset, 3 + lineEndingOffset);
        var expectedSet = new Expected[] { expected };

        var expectedOutput = new ExpectedOutput([
          new(ColEnds: [0, 10, 46, 83], LineNumberTo: 4),
            new(ColEnds: [83, 86], LineNumberTo: 5),
            new(ColEnds: [87, 89, 101, 105, 107], LineNumberTo: 6),
            new(ColEnds: [107, 111], LineNumberTo: 7),
        ], new(ColEnds: [111], LineNumberTo: 7, CharsStartIndex: 112, ColEndsStartIndex: 13, ColCount: 0));

        AssertOutput(parser, charsStart, charsEnd, expectedOutput);
    }

    [TestMethod]
    [DynamicData(nameof(Parsers))]
    public void SepParserTest_ParseColEnds_NewLinesOnly(object parserObject)
    {
        Contract.Assume(parserObject is not null);
        var parser = (ISepParser)parserObject;
        var charsEnd = _chars.Length - parser.PaddingLength;
        _chars.AsSpan(0, charsEnd).Fill('\n');
        _state._currentRowColCount = 0;
        var parsedRowsLength = _state._parsedRows.Length;

        var rowCount = Math.Min(charsEnd, parsedRowsLength);
        var charsStart = 0;
        var lineNumberOffset = 4;
        var expectedOutput = new ExpectedOutput(
            Enumerable.Range(0, rowCount)
                      .Select(i => new ExpectedParsedRow(ColEnds: [Math.Max(0, i - 1), i], i + lineNumberOffset)).ToArray(),
            new(ColEnds: [19], LineNumberTo: rowCount + lineNumberOffset - 1,
                CharsStartIndex: rowCount, ColEndsStartIndex: rowCount * 2, ColCount: 0));

        AssertOutput(parser, charsStart, charsEnd, expectedOutput);
    }

    int FillChars(string text)
    {
        text.AsSpan().CopyTo(_chars);
        return text.Length;
    }

    record struct Expected(int[] ColEnds, int NextStart, int RowLineEndingOffset, int LineNumber);

    record ExpectedParsedRow(int[] ColEnds, int LineNumberTo);
    record ExpectedParsingRow(int[] ColEnds, int LineNumberTo, int CharsStartIndex, int ColEndsStartIndex, int ColCount);
    record ExpectedOutput(ExpectedParsedRow[] ExpectedRows, ExpectedParsingRow ExpectedParsingRow);

    void AssertParserOutput(ISepParser parser, int charsStart, int charsEnd, Expected[] expectedSet)
    {
        var s = _state;
        s._charsParseStart = charsStart;
        s._charsDataEnd = charsEnd;
        s._parsingLineNumber = 3;

        parser.ParseColEnds(s);

        TraceParseState(s);

        if (expectedSet.Length > 0 && expectedSet[0].RowLineEndingOffset > 0)
        {
            Assert.AreEqual(expectedSet.Length, s._parsedRowsCount, nameof(s._parsedRowsCount));
        }

        var colEndsFrom = 0;
        for (var rowIndex = 0; rowIndex < s._parsedRowsCount; rowIndex++)
        {
            var (expected, expectedNextStart, expectedRowLineEndingOffset, expectedLineNumber) = expectedSet[rowIndex];
            if (expectedRowLineEndingOffset > 0)
            {
                var rowInfo = s._parsedRows[rowIndex];
                Assert.AreEqual(expected.Length, rowInfo.ColCount, nameof(rowInfo.ColCount));
                Assert.AreEqual(expectedLineNumber, rowInfo.LineNumberTo, nameof(rowInfo.LineNumberTo));
                AreEqual(expected, _colEndsOrColInfos, colEndsFrom, colEndsFrom + rowInfo.ColCount);
                colEndsFrom += rowInfo.ColCount + 1;

                var nextStart = _colEndsOrColInfos[colEndsFrom] + expectedRowLineEndingOffset;

                Assert.AreEqual(expectedNextStart, nextStart, "nextStart");
            }
            else
            {
                AreEqual(expected, _colEndsOrColInfos, colEndsFrom, colEndsFrom + s._parsingRowColCount);
            }
        }
    }

    void AssertOutput(ISepParser parser, int charsStart, int charsEnd, ExpectedOutput expectedOutput)
    {
        var s = _state;
        s._charsParseStart = charsStart;
        s._charsDataEnd = charsEnd;
        s._parsingLineNumber = 3;

        parser.ParseColEnds(s);

        TraceParseState(s);

        Assert.AreEqual(expectedOutput.ExpectedRows.Length, s._parsedRowsCount, nameof(s._parsedRowsCount));
        var colEndsFrom = 0;
        for (var rowIndex = 0; rowIndex < s._parsedRowsCount; rowIndex++)
        {
            var expected = expectedOutput.ExpectedRows[rowIndex];
            var actual = s._parsedRows[rowIndex];
            var actualColEndsCount = actual.ColCount + 1;

            Assert.AreEqual(expected.ColEnds.Length, actualColEndsCount, nameof(actualColEndsCount));
            Assert.AreEqual(expected.LineNumberTo, actual.LineNumberTo, nameof(actual.LineNumberTo));
            AreEqual(expected.ColEnds, _colEndsOrColInfos[colEndsFrom..(colEndsFrom + actualColEndsCount)]);

            colEndsFrom += actualColEndsCount;
        }
        var expectedParsingRow = expectedOutput.ExpectedParsingRow;
        Assert.AreEqual(expectedParsingRow.ColEnds.Length, s._parsingRowColCount + 1, nameof(s._parsingRowColCount));
        Assert.AreEqual(expectedParsingRow.LineNumberTo, s._parsingLineNumber, nameof(s._parsingLineNumber));
        Assert.AreEqual(expectedParsingRow.CharsStartIndex, s._parsingRowCharsStartIndex, nameof(s._parsingRowCharsStartIndex));
        Assert.AreEqual(expectedParsingRow.ColEndsStartIndex, s._parsingRowColEndsOrInfosStartIndex, nameof(s._parsingRowColEndsOrInfosStartIndex));
        Assert.AreEqual(expectedParsingRow.ColCount, s._parsingRowColCount, nameof(s._parsingRowColCount));
    }

    [Conditional("SEPREADERTRACE")]
    void TraceParseState(SepReaderState s)
    {
        var sb = new StringBuilder();
        var indent = "        ";
        sb.AppendLine($"{indent}var expectedOutput = new {nameof(ExpectedOutput)}(new {nameof(ExpectedParsedRow)}[] {{");
        var colEndsFrom = 0;
        for (var rowIndex = 0; rowIndex < s._parsedRowsCount; rowIndex++)
        {
            var rowInfo = s._parsedRows[rowIndex];
            var colEnds = _colEndsOrColInfos.AsSpan().Slice(colEndsFrom, rowInfo.ColCount + 1);
            sb.AppendLine($"{indent}  new({nameof(ExpectedParsedRow.ColEnds)}: new[]{{ {string.Join(", ", colEnds.ToArray())} }}, {nameof(ExpectedParsedRow.LineNumberTo)}: {rowInfo.LineNumberTo}),");
            colEndsFrom += rowInfo.ColCount + 1;
        }
        var colEndsParsing = _colEndsOrColInfos.AsSpan().Slice(colEndsFrom, s._parsingRowColCount + 1);
        sb.AppendLine($"{indent}}}, new({nameof(ExpectedParsingRow.ColEnds)}: new[]{{ {string.Join(", ", colEndsParsing.ToArray())} }}, {nameof(ExpectedParsingRow.LineNumberTo)}: {s._parsingLineNumber}, {nameof(ExpectedParsingRow.CharsStartIndex)}: {s._parsingRowCharsStartIndex}, {nameof(ExpectedParsingRow.ColEndsStartIndex)}: {s._parsingRowColEndsOrInfosStartIndex}, {nameof(ExpectedParsingRow.ColCount)}: {s._parsingRowColCount}));");
        Trace.WriteLine(sb.ToString());
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
