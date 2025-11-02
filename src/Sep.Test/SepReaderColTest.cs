using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderColTest
{
    const string ColName = "A";
    const int ColValue = 123456;
    const string ColText = "123456";

    [TestMethod]
    public void SepReaderColTest_DebuggerDisplay()
    {
        Run(col => Assert.AreEqual(ColText, col.DebuggerDisplay));
    }

    [TestMethod]
    public void SepReaderColTest_Span()
    {
        Run(col => Assert.IsTrue(ColText.AsSpan().SequenceEqual(col.Span)));
        Run(col => Assert.IsTrue(ReadOnlySpan<char>.Empty.SequenceEqual(col.Span)), "");
    }

    [TestMethod]
    public void SepReaderColTest_ToString()
    {
        Run(col => Assert.AreEqual(ColText, col.ToString()));
        Run(col => Assert.AreSame(string.Empty, col.ToString()), "");
        Run(col =>
        {
            var t1 = col.ToString();
            Assert.AreEqual("1", t1);
            var t2 = col.ToString();
            Assert.AreSame(t1, t2);
        }, "1");
    }

    [TestMethod]
    public void SepReaderColTest_ToStringDirect()
    {
        Run(col => Assert.AreEqual(ColText, col.ToStringDirect()));
        Run(col => Assert.AreSame(string.Empty, col.ToString()), "");
    }

    [TestMethod]
    public void SepReaderColTest_Parse()
    {
        Run(col => Assert.AreEqual(ColValue, col.Parse<int>()));
        AssertParseFloats(o => o);
        AssertParseFloats(o => o with { DisableFastFloat = true });
        AssertParseFloats(o => o with { CultureInfo = null, DisableFastFloat = true });
    }

    [TestMethod]
    public void SepReaderColTest_TryParse_Return()
    {
        Run(col => Assert.AreEqual(ColValue, col.TryParse<int>()));
        Run(col => Assert.AreEqual(null, col.TryParse<int>()), "a");

        AssertTryParseReturnFloats(o => o);
        AssertTryParseReturnFloats(o => o with { DisableFastFloat = true });
        AssertTryParseReturnFloats(o => o with { CultureInfo = null, DisableFastFloat = true });
    }


    [TestMethod]
    public void SepReaderColTest_TryParse_Out()
    {
        Run(col => Assert.AreEqual((int?)ColValue, col.TryParse<int>(out var v) ? v : null));
        Run(col => Assert.AreEqual((int?)null, col.TryParse<int>(out var v) ? v : null), "a");

        AssertTryParseOutFloats(o => o);
        AssertTryParseOutFloats(o => o with { DisableFastFloat = true });
        AssertTryParseOutFloats(o => o with { CultureInfo = null, DisableFastFloat = true });
    }

#if NET8_0_OR_GREATER
    // string unfortunately did not implement ISpanParsable until .NET 8

    [TestMethod]
    public void SepReaderColTest_Parse_String()
    {
        Run(col => Assert.AreEqual(ColText, col.Parse<string>()));
    }

    [TestMethod]
    public void SepReaderColTest_TryParse_Out_String()
    {
        Run(col => Assert.AreEqual(ColText, col.TryParse<string>(out var v) ? v : null));
    }
#endif

    public static IEnumerable<object[]> UnescapeData => SepUnescapeTest.UnescapeData.Concat(
        [
            ["a\"\"a", "a\"\"a"],
            ["a\"a\"a", "a\"a\"a"],
            ["·\"\"·", "·\"\"·"],
            ["·\"a\"·", "·\"a\"·"],
            ["·\"\"", "·\"\""],
            ["·\"a\"", "·\"a\""],
            ["a\"\"\"a", "a\"\"\"a"],
        ]);

    [TestMethod]
    [DynamicData(nameof(UnescapeData))]
    public void SepReaderColTest_Unescape_Header_Test(string src, string expected)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = true, Unescape = true }).FromText(src);
        var actual = reader.Header.ColNames[0];

        Assert.AreEqual(expected, actual, src);
    }

    [TestMethod]
    [DynamicData(nameof(UnescapeData))]
    public void SepReaderColTest_Unescape_Col_Test(string src, string expectedCol)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = false, Unescape = true }).FromText(src);
        AssertCol(reader, src, expectedCol);
    }

    public static IEnumerable<object[]> TrimOuterNoQuotesData =>
    [
        [" ", ""],
        ["   ", ""],
        ["a", "a"],
        [" a", "a"],
        ["a ", "a"],
        [" a ", "a"],
        [" a a ", "a a"],
    ];

    public static IEnumerable<object[]> TrimOuterQuotesData =>
    [
        ["\"a\"", "\"a\""],
        [" \" a\"", "\" a\""],
        ["\"a \" ", "\"a \""],
        [" \" a \" ", "\" a \""],
        [" \" a a \" ", "\" a a \""],
    ];

    public static IEnumerable<object[]> TrimOuterUnescapeData =>
        TrimOuterNoQuotesData.Concat(
    [
        ["\"a\"", "a"],
        [" \" a\" ", " a"],
        [" \"a \"", "a "],
        ["\" a \" ", " a "],
        [" \" a a \" ", " a a "],
    ]);

    public static IEnumerable<object[]> UnescapeTrimAfterUnescapeQuotesData =>
    [
        ["\"a\"", "a"],
        ["\" a\"", "a"],
        ["\"a \"", "a"],
        ["\" a \"", "a"],
        ["\" a a \"", "a a"],
        ["\"a \" ", "a"],
        ["\"a \"  ", "a"],
        ["\"a \"\"\"  ", "a \""],
        ["\"\"\" a \"\"\"  ", "\" a \""],
        ["\"  \"\" a \"\"  \"  ", "\" a \""],
    ];

    public static IEnumerable<object[]> UnescapeTrimAfterUnescapeData =>
        UnescapeTrimAfterUnescapeQuotesData.Concat(
    [
        [" \"a \"", "\"a \""], // Not seen as quotes
        [" \"a \" ", "\"a \""], // Not seen as quotes
        [" \"a \"  ", "\"a \""], // Not seen as quotes
    ]);

    public static IEnumerable<object[]> TrimOuterData =>
        TrimOuterNoQuotesData.Concat(TrimOuterQuotesData);

    public static IEnumerable<object[]> TrimAllUnescapeData =>
        TrimOuterNoQuotesData.Concat(UnescapeTrimAfterUnescapeQuotesData).Concat(
    [
        [" \"a\" ", "a"],
        [" \" a\"", "a"],
        ["\"a \" ", "a"],
        [" \" a \" ", "a"],
        [" \" a a \" ", "a a"],
        [" \"   a  a   \" ", "a  a"],
    ]);

    [TestMethod]
    [DynamicData(nameof(TrimOuterData))]
    public void SepReaderColTest_TrimOuter_Header_Test(string src, string expected)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = true, Trim = SepTrim.Outer }).FromText(src);
        var actual = reader.Header.ColNames[0];

        Assert.AreEqual(expected, actual, src);
    }
    [TestMethod]
    [DynamicData(nameof(TrimOuterData))]
    public void SepReaderColTest_TrimOuter_Col_Test(string src, string expectedCol)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = false, Trim = SepTrim.Outer }).FromText(src);
        AssertCol(reader, src, expectedCol);
    }

    [TestMethod]
    [DynamicData(nameof(TrimOuterUnescapeData))]
    public void SepReaderColTest_TrimOuterUnescape_Header_Test(string src, string expected)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = true, Unescape = true, Trim = SepTrim.Outer }).FromText(src);
        var actual = reader.Header.ColNames[0];

        Assert.AreEqual(expected, actual, src);
    }
    [TestMethod]
    [DynamicData(nameof(TrimOuterUnescapeData))]
    public void SepReaderColTest_TrimOuterUnescape_Col_Test(string src, string expectedCol)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = false, Unescape = true, Trim = SepTrim.Outer }).FromText(src);
        AssertCol(reader, src, expectedCol);
    }

    [TestMethod]
    [DynamicData(nameof(UnescapeTrimAfterUnescapeData))]
    public void SepReaderColTest_TrimAfterUnescape_Header_Test(string src, string expected)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = true, Unescape = true, Trim = SepTrim.AfterUnescape }).FromText(src);
        var actual = reader.Header.ColNames[0];

        Assert.AreEqual(expected, actual, src);
    }
    [TestMethod]
    [DynamicData(nameof(UnescapeTrimAfterUnescapeData))]
    public void SepReaderColTest_TrimAfterUnescape_Col_Test(string src, string expectedCol)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = false, Unescape = true, Trim = SepTrim.AfterUnescape }).FromText(src);
        AssertCol(reader, src, expectedCol);
    }

    [TestMethod]
    [DynamicData(nameof(TrimAllUnescapeData))]
    public void SepReaderColTest_TrimAllUnescape_Header_Test(string src, string expected)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = true, Unescape = true, Trim = SepTrim.All }).FromText(src);
        var actual = reader.Header.ColNames[0];

        Assert.AreEqual(expected, actual, src);
    }
    [TestMethod]
    [DynamicData(nameof(TrimAllUnescapeData))]
    public void SepReaderColTest_TrimAllUnescape_Col_Test(string src, string expectedCol)
    {
        using var reader = Sep.Reader(o => o with
        { HasHeader = false, Unescape = true, Trim = SepTrim.All }).FromText(src);
        AssertCol(reader, src, expectedCol);
    }

    static void AssertCol(SepReader reader, string src, string expectedCol)
    {
        Assert.IsTrue(reader.MoveNext());
        // Ensure repeated access works
        for (var i = 0; i < 4; i++)
        {
            var row = reader.Current;

            var actualCol = row[0].ToString();
            Assert.AreEqual(expectedCol, actualCol, src);

            // Ensure row can be gotten and that expectedCol is contained
            var rowText = row.Span.ToString();
            Assert.IsTrue(rowText.Contains(expectedCol));
        }
    }

    static void Run(SepReader.ColAction action, string colValue = ColText, Func<SepReaderOptions, SepReaderOptions>? configure = null)
    {
        Func<SepReaderOptions, SepReaderOptions> defaultConfigure = static c => c;
        var useConfigure = configure ?? defaultConfigure;

        var text = $"{ColName}{Environment.NewLine}{colValue}{Environment.NewLine}";
        List<Func<SepReader>> createReaders = new()
        {
            () => Sep.Reader(useConfigure).FromText(text),
            () => Sep.Default.Reader(useConfigure).FromText(text),
            () => ((Sep?)null).Reader(useConfigure).FromText(text),
            () => new SepSpec(Sep.Default, null, false).Reader(useConfigure).FromText(text),
        };
        if (configure is null)
        {
            createReaders.Add(() => new SepSpec(Sep.Default, null, false).Reader().FromText(text));
        }

        foreach (var createReader in createReaders)
        {
            using var reader = createReader();
            Assert.IsTrue(reader.MoveNext());
            var row = reader.Current;
            action(row[ColName]);
        }
    }

    static void AssertParseFloats(Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Run(col => Assert.AreEqual(ColValue, col.Parse<float>()), ColText, configure: configure);
        Run(col => Assert.AreEqual(float.NaN, col.Parse<float>()), "NaN", configure: configure);
        Run(col => Assert.AreEqual(float.NaN, col.Parse<float>()), "+NaN", configure: configure);
        Run(col => Assert.AreEqual(float.NaN, col.Parse<float>()), "-NaN", configure: configure);
        Run(col => Assert.AreEqual(float.PositiveInfinity, col.Parse<float>()), "Infinity", configure: configure);
        Run(col => Assert.AreEqual(float.NegativeInfinity, col.Parse<float>()), "-Infinity", configure: configure);

        Run(col => Assert.AreEqual(ColValue, col.Parse<double>()), ColText, configure: configure);
        Run(col => Assert.AreEqual(double.NaN, col.Parse<double>()), "NaN", configure: configure);
        Run(col => Assert.AreEqual(double.NaN, col.Parse<double>()), "+NaN", configure: configure);
        Run(col => Assert.AreEqual(double.NaN, col.Parse<double>()), "-NaN", configure: configure);
        Run(col => Assert.AreEqual(double.PositiveInfinity, col.Parse<double>()), "Infinity", configure: configure);
        Run(col => Assert.AreEqual(double.NegativeInfinity, col.Parse<double>()), "-Infinity", configure: configure);
    }

    static void AssertTryParseReturnFloats(Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Run(col => Assert.AreEqual(ColValue, col.TryParse<float>()), ColText, configure: configure);
        Run(col => Assert.AreEqual(float.NaN, col.TryParse<float>()), "NaN", configure: configure);
        Run(col => Assert.AreEqual(float.NaN, col.TryParse<float>()), "+NaN", configure: configure);
        Run(col => Assert.AreEqual(float.NaN, col.TryParse<float>()), "-NaN", configure: configure);
        Run(col => Assert.AreEqual(float.PositiveInfinity, col.TryParse<float>()), "Infinity", configure: configure);
        Run(col => Assert.AreEqual(float.NegativeInfinity, col.TryParse<float>()), "-Infinity", configure: configure);
        Run(col => Assert.AreEqual(null, col.TryParse<float>()), "a", configure: configure);

        Run(col => Assert.AreEqual(ColValue, col.TryParse<double>()), ColText, configure: configure);
        Run(col => Assert.AreEqual(double.NaN, col.TryParse<double>()), "NaN", configure: configure);
        Run(col => Assert.AreEqual(double.NaN, col.TryParse<double>()), "+NaN", configure: configure);
        Run(col => Assert.AreEqual(double.NaN, col.TryParse<double>()), "-NaN", configure: configure);
        Run(col => Assert.AreEqual(double.PositiveInfinity, col.TryParse<double>()), "Infinity", configure: configure);
        Run(col => Assert.AreEqual(double.NegativeInfinity, col.TryParse<double>()), "-Infinity", configure: configure);
        Run(col => Assert.AreEqual(null, col.TryParse<double>()), "a", configure: configure);
    }

    static void AssertTryParseOutFloats(Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Run(col => Assert.AreEqual((float?)ColValue, col.TryParse<float>(out var v) ? v : null), configure: configure);
        Run(col => Assert.AreEqual((float?)null, col.TryParse<float>(out var v) ? v : null), "a", configure: configure);
        Run(col => Assert.AreEqual((float?)float.NaN, col.TryParse<float>(out var v) ? v : null), "NaN", configure: configure);
        Run(col => Assert.AreEqual((float?)float.NaN, col.TryParse<float>(out var v) ? v : null), "+NaN", configure: configure);
        Run(col => Assert.AreEqual((float?)float.NaN, col.TryParse<float>(out var v) ? v : null), "-NaN", configure: configure);
        Run(col => Assert.AreEqual((float?)float.PositiveInfinity, col.TryParse<float>(out var v) ? v : null), "Infinity", configure: configure);
        Run(col => Assert.AreEqual((float?)float.NegativeInfinity, col.TryParse<float>(out var v) ? v : null), "-Infinity", configure: configure);

        Run(col => Assert.AreEqual((double?)ColValue, col.TryParse<double>(out var v) ? v : null), configure: configure);
        Run(col => Assert.AreEqual((double?)null, col.TryParse<double>(out var v) ? v : null), "a", configure: configure);
        Run(col => Assert.AreEqual((double?)double.NaN, col.TryParse<double>(out var v) ? v : null), "NaN", configure: configure);
        Run(col => Assert.AreEqual((double?)double.NaN, col.TryParse<double>(out var v) ? v : null), "+NaN", configure: configure);
        Run(col => Assert.AreEqual((double?)double.NaN, col.TryParse<double>(out var v) ? v : null), "-NaN", configure: configure);
        Run(col => Assert.AreEqual((double?)double.PositiveInfinity, col.TryParse<double>(out var v) ? v : null), "Infinity", configure: configure);
        Run(col => Assert.AreEqual((double?)double.NegativeInfinity, col.TryParse<double>(out var v) ? v : null), "-Infinity", configure: configure);
    }
}
