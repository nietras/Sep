using System;
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
    public void SepReaderColTest_ToStringRaw()
    {
        Run(col => Assert.AreEqual(ColText, col.ToStringRaw()));
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

    static void Run(SepReader.ColAction action, string colValue = ColText, Func<SepReaderOptions, SepReaderOptions>? configure = null)
    {
        Func<SepReaderOptions, SepReaderOptions> defaultConfigure = static c => c;
        using var reader = CreateReader(colValue, configure ?? defaultConfigure);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        action(row[ColName]);
    }

    static SepReader CreateReader(string colValue, Func<SepReaderOptions, SepReaderOptions> configure)
    {
        var text = $"{ColName}{Environment.NewLine}{colValue}{Environment.NewLine}";
        return Sep.Reader(configure).FromText(text);
    }

    static void AssertParseFloats(Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Run(col => Assert.AreEqual(ColValue, col.Parse<float>()), configure: configure);
        Run(col => Assert.AreEqual(ColValue, col.Parse<double>()), configure: configure);
    }

    static void AssertTryParseReturnFloats(Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Run(col => Assert.AreEqual(ColValue, col.TryParse<float>()), configure: configure);
        Run(col => Assert.AreEqual(null, col.TryParse<float>()), "a", configure: configure);
        Run(col => Assert.AreEqual(ColValue, col.TryParse<double>()), configure: configure);
        Run(col => Assert.AreEqual(null, col.TryParse<double>()), "a", configure: configure);
    }

    static void AssertTryParseOutFloats(Func<SepReaderOptions, SepReaderOptions> configure)
    {
        Run(col => Assert.AreEqual((float?)ColValue, col.TryParse<float>(out var v) ? v : null), configure: configure);
        Run(col => Assert.AreEqual((float?)null, col.TryParse<float>(out var v) ? v : null), "a", configure: configure);
        Run(col => Assert.AreEqual((double?)ColValue, col.TryParse<double>(out var v) ? v : null), configure: configure);
        Run(col => Assert.AreEqual((double?)null, col.TryParse<double>(out var v) ? v : null), "a", configure: configure);
    }
}
