using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm; // For Sve.VectorLength
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
            if (Environment.Is64BitProcess && Vector512.IsHardwareAccelerated)
            { return typeof(SepParserVector512NrwCmpExtMsbTzcnt); }
#endif
            if (Avx2.IsSupported) { return typeof(SepParserAvx2PackCmpOrMoveMaskTzcnt); }
            if (Sse2.IsSupported) { return typeof(SepParserSse2PackCmpOrMoveMaskTzcnt); }
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

    // --- Tests for SepParserSve ---

    private ISepParser? GetSveParserForced(Sep sep)
    {
        var originalForceParser = Environment.GetEnvironmentVariable(SepParserFactory.SepForceParserEnvName);
        Environment.SetEnvironmentVariable(SepParserFactory.SepForceParserEnvName, "SepParserSve");
        ISepParser? parser = null;
        try
        {
            // Ensure options are passed if parser constructor needs them.
            var options = new SepParserOptions(sep);
            parser = SepParserFactory.CreateBest(options);

            if (parser?.GetType().Name == "SepParserSve")
            {
                return parser;
            }
            // If SVE is not supported or the factory doesn't return it for the given Sep options,
            // we can't test it. Return null to indicate this.
            return null;
        }
        finally
        {
            // Only reset if we set it. And ensure parser is captured before resetting if needed.
            Environment.SetEnvironmentVariable(SepParserFactory.SepForceParserEnvName, originalForceParser);
        }
    }

    private void ResetTestState(SepReaderState state, string input)
    {
        state._charsParseStart = 0;
        state._charsDataEnd = FillChars(input); // Assumes _chars is available and FillChars works on it
        state._parsingLineNumber = 1; // Or appropriate start line
        state._parsedRowsCount = 0;
        state._parsingRowColCount = 0;
        state._parsingRowColEndsOrInfosStartIndex = 0;
        state._parsingRowCharsStartIndex = 0;
        state._parsingRowIsUnfinishedQuoted = false;
        state._parsingRowCurrentColumnIsQuoted = false;
        Array.Clear(_colEndsOrColInfos, 0, _colEndsOrColInfos.Length);

        // If the state is _stateUnescape, ensure its internal Sep options are default for quote handling.
        if (ReferenceEquals(state, _stateUnescape))
        {
            state._options = new SepReaderOptions(Sep.Default); // Ensure quotes are enabled by default for _stateUnescape
        }
         else if (ReferenceEquals(state, _state))
        {
            // Ensure _state is configured for no quotes, or default, depending on test.
            // For ParseColEnds, quotes are typically ignored by the method itself.
            state._options = new SepReaderOptions(new Sep(quotes: false));
        }
    }


    [TestMethod]
    public void SepParserTest_Sve_PaddingLength()
    {
        var parser = GetSveParserForced(Sep.Default);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available, not forced, or SVE not supported."); return; }
        
        // The PaddingLength property in SepParserSve is Sve.VectorLength if supported, otherwise 0.
        // GetSveParserForced should only return a parser if Sve.IsSupported is true (implicitly, based on factory logic).
        Assert.AreEqual(Sve.VectorLength, parser.PaddingLength);
    }

    [TestMethod]
    public void SepParserTest_Sve_QuoteCount_AfterParseColInfos()
    {
        var sep = Sep.Default; // Assuming default separator ';' and quote '"'
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available or not forced."); return; }

        ResetTestState(_stateUnescape, "\"a\";\"b\";\"c\""); // 3 fields, 6 quotes
        // Set Sep for the state, as ParseColInfos might use it from state.Sep
        _stateUnescape._options = new SepReaderOptions(sep);
        _stateUnescape._colUnquoteUnescape = true; // Ensure unescaping is on for ParseColInfos quote handling

        parser.ParseColInfos(_stateUnescape);

        Assert.AreEqual(6, parser.QuoteCount, "Quote count should match total quotes in input.");
    }

    [TestMethod]
    public void SepParserTest_Sve_ParseColEnds_Basic()
    {
        var sepChar = ';';
        var sep = new Sep(sepChar);
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available or not forced."); return; }

        string input = $"a{sepChar}b{sepChar}c"; // No newlines, single row focused
        ResetTestState(_state, input);
        // ParseColEnds uses s.Sep.Separator
        _state._options = new SepReaderOptions(sep);


        parser.ParseColEnds(_state);

        // For "a;b;c" (indices: 01234), separators are at 1, 3.
        // ParseColEnds records the indices of the separators.
        // The SepReader then interprets these to define column boundaries.
        // _parsingRowColCount should be the number of separators found for the current line.
        // If the line doesn't end with a separator, the last field is implicitly handled by newline/EOF.
        // SepReaderState._colEndsOrInfos stores raw end positions or SepColInfo.
        // For ParseColEnds, it stores integers.
        // Expected: colEnds for 'a' is at index 1 (separator), for 'b' is at index 3 (separator).
        // The count of *separators found* is what _parsingRowColCount reflects directly from ParseColEnds.
        
        Assert.AreEqual(2, _state._parsingRowColCount, "Should find 2 separators.");
        // _colEndsOrInfos for ParseColEnds stores the separator positions
        // relative to _parsingRowCharsStartIndex (which is 0 here).
        // So, indices of separators are 1 and 3.
        Assert.AreEqual(1, _state._colEndsOrColInfos[0 + _state._parsingRowColEndsOrInfosStartIndex]);
        Assert.AreEqual(3, _state._colEndsOrColInfos[1 + _state._parsingRowColEndsOrInfosStartIndex]);
    }

    [TestMethod]
    public void SepParserTest_Sve_ParseColInfos_Basic()
    {
        var sepChar = ';';
        var quoteChar = '"';
        var sep = new Sep(separator: sepChar, quote: quoteChar);
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available or not forced."); return; }

        string input = $"{quoteChar}x{quoteChar}{sepChar}{quoteChar}y{sepChar}z{quoteChar}{sepChar}w";
        // "x";"y;z";w -> 3 columns. Indices: 012 3 45678 9 10
        // Col 0: "x"      (start 0, len 3, quoted true) -> SepColInfo(0, 3, true)
        // Col 1: "y;z"    (start 4, len 5, quoted true) -> SepColInfo(4, 5, true)
        // Col 2: w        (start 10, len 1, quoted false) -> SepColInfo(10, 1, false)
        
        ResetTestState(_stateUnescape, input);
        _stateUnescape._options = new SepReaderOptions(sep); // Ensure state uses the correct sep for parsing
        _stateUnescape._colUnquoteUnescape = true;

        parser.ParseColInfos(_stateUnescape);
        
        // _parsingRowColCount should be the number of columns fully parsed and added.
        // The last column 'w' is terminated by EOF/EOL, handled by SepReader.
        // ParseColInfos itself, if input doesn't end with separator, will record columns found by separators.
        // The SepReader is responsible for creating the SepColInfo for the last field if not separator terminated.
        // So, we expect 2 columns to be explicitly created by separators here by ParseColInfos.
        // The final column 'w' will be processed by SepReader using the remaining state.

        Assert.AreEqual(2, _stateUnescape._parsingRowColCount, "Should register 2 columns defined by separators.");
        Assert.AreEqual(4, parser.QuoteCount); // "x" (2) + "y;z" (2) = 4 quotes

        var colInfos = _stateUnescape._colEndsOrInfos;
        int baseIndex = _stateUnescape._parsingRowColEndsOrInfosStartIndex; // Should be 0 after ResetTestState

        // Check Col 0: "x"
        SepColInfo col0 = ((SepColInfo*)_stateUnescape.GetColInfosPtr())[baseIndex + 0];
        Assert.AreEqual(0, col0.Start, "Col0 Start");
        Assert.AreEqual(3, col0.Length, "Col0 Length"); // Length of "x" including quotes
        Assert.IsTrue(col0.IsQuoted, "Col0 IsQuoted");

        // Check Col 1: "y;z"
        SepColInfo col1 = ((SepColInfo*)_stateUnescape.GetColInfosPtr())[baseIndex + 1];
        Assert.AreEqual(4, col1.Start, "Col1 Start"); // Starts after "x";
        Assert.AreEqual(5, col1.Length, "Col1 Length"); // Length of "y;z" including quotes
        Assert.IsTrue(col1.IsQuoted, "Col1 IsQuoted");
        
        // The third column 'w' is not added by ParseColInfos directly in this case because
        // it's not followed by a separator. SepReader handles this.
        // We need to check the remaining state that SepReader would use.
        Assert.IsTrue(!_stateUnescape._parsingRowIsUnfinishedQuoted, "parsingRowIsUnfinishedQuoted should be false after 'w'.");
        Assert.IsTrue(!_stateUnescape._parsingRowCurrentColumnIsQuoted, "parsingRowCurrentColumnIsQuoted should be false for 'w'.");
        // _charsParseStart is advanced by SepReader. _charsProcessedNeqNewline is what the parser sets.
        // For "x";"y;z";w, length is 11. _charsProcessedNeqNewline should be 11.
        Assert.AreEqual(input.Length, _stateUnescape._charsProcessedNeqNewline);
    }

    // --- New Comprehensive Tests for SepParserSve ---

    // --- ParseColEnds Additional Tests ---

    [TestMethod]
    public void SepParserTest_Sve_ParseColEnds_EmptyInput()
    {
        var sep = new Sep(';');
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        ResetTestState(_state, "");
        _state._options = new SepReaderOptions(sep);
        parser.ParseColEnds(_state);
        Assert.AreEqual(0, _state._parsingRowColCount, "Should find 0 separators in empty input.");
        Assert.AreEqual(0, _state._charsProcessedNeqNewline);
    }

    [TestMethod]
    public void SepParserTest_Sve_ParseColEnds_NoSeparators()
    {
        var sep = new Sep(';');
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        string input = "abc";
        ResetTestState(_state, input);
        _state._options = new SepReaderOptions(sep);
        parser.ParseColEnds(_state);
        Assert.AreEqual(0, _state._parsingRowColCount, "Should find 0 separators.");
        Assert.AreEqual(input.Length, _state._charsProcessedNeqNewline);
    }

    [TestMethod]
    public void SepParserTest_Sve_ParseColEnds_OnlySeparators()
    {
        var sepChar = ';';
        var sep = new Sep(sepChar);
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        string input = $"{sepChar}{sepChar}{sepChar}"; // ";;;"
        ResetTestState(_state, input);
        _state._options = new SepReaderOptions(sep);
        parser.ParseColEnds(_state);
        Assert.AreEqual(3, _state._parsingRowColCount, "Should find 3 separators.");
        Assert.AreEqual(0, _state._colEndsOrColInfos[0]);
        Assert.AreEqual(1, _state._colEndsOrColInfos[1]);
        Assert.AreEqual(2, _state._colEndsOrColInfos[2]);
        Assert.AreEqual(input.Length, _state._charsProcessedNeqNewline);
    }

    [TestMethod]
    public void SepParserTest_Sve_ParseColEnds_LeadingSeparator()
    {
        var sepChar = ';';
        var sep = new Sep(sepChar);
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        string input = $"{sepChar}abc{sepChar}d"; // ";abc;d"
        ResetTestState(_state, input);
        _state._options = new SepReaderOptions(sep);
        parser.ParseColEnds(_state);
        Assert.AreEqual(2, _state._parsingRowColCount, "Should find 2 separators.");
        Assert.AreEqual(0, _state._colEndsOrColInfos[0]); // First sep at index 0
        Assert.AreEqual(4, _state._colEndsOrColInfos[1]); // Second sep at index 4 (after abc)
        Assert.AreEqual(input.Length, _state._charsProcessedNeqNewline);
    }

    [TestMethod]
    public void SepParserTest_Sve_ParseColEnds_TrailingSeparator()
    {
        var sepChar = ';';
        var sep = new Sep(sepChar);
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        string input = $"abc{sepChar}d{sepChar}"; // "abc;d;"
        ResetTestState(_state, input);
        _state._options = new SepReaderOptions(sep);
        parser.ParseColEnds(_state);
        Assert.AreEqual(2, _state._parsingRowColCount, "Should find 2 separators.");
        Assert.AreEqual(3, _state._colEndsOrColInfos[0]); // First sep at index 3
        Assert.AreEqual(5, _state._colEndsOrColInfos[1]); // Second sep at index 5
        Assert.AreEqual(input.Length, _state._charsProcessedNeqNewline);
    }

    [TestMethod]
    public void SepParserTest_Sve_ParseColEnds_LongerThanVector()
    {
        var sepChar = ';';
        var sep = new Sep(sepChar);
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        // Create a string longer than a typical SVE vector (e.g., > 64 chars for 512-bit SVE)
        string part = "abcdefghijklmno"; // 15 chars
        string input = $"{part}{sepChar}{part}{part}{sepChar}{part}{part}{part}{sepChar}{part}"; // 15;30;45;15 = 105 chars + 3 seps
        ResetTestState(_state, input);
        _state._options = new SepReaderOptions(sep);
        parser.ParseColEnds(_state);
        Assert.AreEqual(3, _state._parsingRowColCount);
        Assert.AreEqual(part.Length,                               _state._colEndsOrColInfos[0]);
        Assert.AreEqual(part.Length + 1 + part.Length * 2,         _state._colEndsOrColInfos[1]);
        Assert.AreEqual(part.Length + 1 + part.Length * 2 + 1 + part.Length * 3, _state._colEndsOrColInfos[2]);
        Assert.AreEqual(input.Length, _state._charsProcessedNeqNewline);
    }

    // --- ParseColInfos Additional Tests ---

    [TestMethod]
    public void SepParserTest_Sve_ParseColInfos_EmptyInput()
    {
        var sep = Sep.Default;
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        ResetTestState(_stateUnescape, "");
        _stateUnescape._options = new SepReaderOptions(sep);
        parser.ParseColInfos(_stateUnescape);
        Assert.AreEqual(0, _stateUnescape._parsingRowColCount, "Should find 0 columns.");
        Assert.AreEqual(0, parser.QuoteCount, "Should find 0 quotes.");
        Assert.AreEqual(0, _stateUnescape._charsProcessedNeqNewline);
    }

    [TestMethod]
    public void SepParserTest_Sve_ParseColInfos_NoSeparators_Unquoted()
    {
        var sep = Sep.Default;
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        string input = "abc";
        ResetTestState(_stateUnescape, input);
        _stateUnescape._options = new SepReaderOptions(sep);
        parser.ParseColInfos(_stateUnescape);
        // ParseColInfos finds columns based on separators. If none, it finds 0 columns.
        // The SepReader would then create one column from the remaining data.
        Assert.AreEqual(0, _stateUnescape._parsingRowColCount, "Should find 0 columns explicitly.");
        Assert.AreEqual(0, parser.QuoteCount);
        Assert.AreEqual(input.Length, _stateUnescape._charsProcessedNeqNewline);
        Assert.IsFalse(_stateUnescape._parsingRowIsUnfinishedQuoted);
        Assert.IsFalse(_stateUnescape._parsingRowCurrentColumnIsQuoted);
        Assert.AreEqual(0, _stateUnescape._parsingRowCharsStartIndex); // Initial start index for the field being parsed by SepReader
    }

    [TestMethod]
    public unsafe void SepParserTest_Sve_ParseColInfos_NoSeparators_Quoted()
    {
        var sep = Sep.Default; // sep ';', quote '"'
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        string input = "\"abc\""; 
        ResetTestState(_stateUnescape, input);
        _stateUnescape._options = new SepReaderOptions(sep);
        parser.ParseColInfos(_stateUnescape);
        Assert.AreEqual(0, _stateUnescape._parsingRowColCount, "Should find 0 columns explicitly.");
        Assert.AreEqual(2, parser.QuoteCount);
        Assert.AreEqual(input.Length, _stateUnescape._charsProcessedNeqNewline);
        Assert.IsFalse(_stateUnescape._parsingRowIsUnfinishedQuoted, "Should not be in quote at end of input");
        Assert.IsTrue(_stateUnescape._parsingRowCurrentColumnIsQuoted, "The column processed was quoted");
        Assert.AreEqual(0, _stateUnescape._parsingRowCharsStartIndex); 
    }


    [TestMethod]
    public unsafe void SepParserTest_Sve_ParseColInfos_QuotedFieldWithSeparator()
    {
        var sep = new Sep(';', '"');
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        string input = "\"a;b\";c"; // "a;b";c
        ResetTestState(_stateUnescape, input);
        _stateUnescape._options = new SepReaderOptions(sep);
        parser.ParseColInfos(_stateUnescape);

        Assert.AreEqual(1, _stateUnescape._parsingRowColCount); // column "a;b"
        Assert.AreEqual(2, parser.QuoteCount); // ""

        SepColInfo col0 = ((SepColInfo*)_stateUnescape.GetColInfosPtr())[0];
        Assert.AreEqual(0, col0.Start);
        Assert.AreEqual(5, col0.Length); // "\"a;b\""
        Assert.IsTrue(col0.IsQuoted);

        // State for 'c' (processed by SepReader)
        Assert.IsFalse(_stateUnescape._parsingRowIsUnfinishedQuoted);
        Assert.IsFalse(_stateUnescape._parsingRowCurrentColumnIsQuoted);
        Assert.AreEqual(input.Length, _stateUnescape._charsProcessedNeqNewline); 
    }

    [TestMethod]
    public unsafe void SepParserTest_Sve_ParseColInfos_EmptyQuotedField()
    {
        var sep = new Sep(';', '"');
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        string input = "\"\";a"; // "";a
        ResetTestState(_stateUnescape, input);
        _stateUnescape._options = new SepReaderOptions(sep);
        parser.ParseColInfos(_stateUnescape);

        Assert.AreEqual(1, _stateUnescape._parsingRowColCount); // column ""
        Assert.AreEqual(2, parser.QuoteCount);

        SepColInfo col0 = ((SepColInfo*)_stateUnescape.GetColInfosPtr())[0];
        Assert.AreEqual(0, col0.Start);
        Assert.AreEqual(2, col0.Length); // "\"\""
        Assert.IsTrue(col0.IsQuoted);
        
        // State for 'a'
        Assert.IsFalse(_stateUnescape._parsingRowIsUnfinishedQuoted);
        Assert.IsFalse(_stateUnescape._parsingRowCurrentColumnIsQuoted);
        Assert.AreEqual(input.Length, _stateUnescape._charsProcessedNeqNewline);
    }

    [TestMethod]
    public unsafe void SepParserTest_Sve_ParseColInfos_AlternatingQuotedUnquoted()
    {
        var sep = new Sep(';', '"');
        var parser = GetSveParserForced(sep);
        if (parser is null) { Assert.Inconclusive("SVE Parser not available."); return; }
        string input = "\"a\";b;\"c\";d"; // "a";b;"c";d
        ResetTestState(_stateUnescape, input);
        _stateUnescape._options = new SepReaderOptions(sep);
        parser.ParseColInfos(_stateUnescape);

        Assert.AreEqual(3, _stateUnescape._parsingRowColCount); // "a", b, "c"
        Assert.AreEqual(4, parser.QuoteCount); // "a" and "c"

        SepColInfo* cols = (SepColInfo*)_stateUnescape.GetColInfosPtr();
        Assert.AreEqual(0, cols[0].Start); Assert.AreEqual(3, cols[0].Length); Assert.IsTrue(cols[0].IsQuoted); // "a"
        Assert.AreEqual(4, cols[1].Start); Assert.AreEqual(1, cols[1].Length); Assert.IsFalse(cols[1].IsQuoted); // b
        Assert.AreEqual(6, cols[2].Start); Assert.AreEqual(3, cols[2].Length); Assert.IsTrue(cols[2].IsQuoted); // "c"
        
        // State for 'd'
        Assert.IsFalse(_stateUnescape._parsingRowIsUnfinishedQuoted);
        Assert.IsFalse(_stateUnescape._parsingRowCurrentColumnIsQuoted);
        Assert.AreEqual(input.Length, _stateUnescape._charsProcessedNeqNewline);
    }
}
