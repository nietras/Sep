using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepWriterColTest
{
    const string ColName = "A";
    const int ColValue = 123456;
    const string ColText = "123456";
    static readonly string ColTextLong = new('a', 2048);

    static readonly string NL = Environment.NewLine;

    [TestMethod]
    public void SepWriterColTest_ColIndex()
    {
        Run(col => Assert.AreEqual(0, col.ColIndex), null);
    }

    [TestMethod]
    public void SepWriterColTest_ColName()
    {
        Run(col => Assert.AreEqual(ColName, col.ColName), null);
    }

    [TestMethod]
    public void SepWriterColTest_Set_String()
    {
        Run(col => col.Set(ColText));
    }

    [TestMethod]
    public void SepWriterColTest_Set_String_Long()
    {
        Run(col => col.Set(ColTextLong), ColTextLong);
    }

    [TestMethod]
    public void SepWriterColTest_Set_Span()
    {
        Run(col => col.Set(ColText.AsSpan()));
    }

    [TestMethod]
    public void SepWriterColTest_Set_Span_Long()
    {
        Run(col => col.Set(ColTextLong.AsSpan()), ColTextLong);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString()
    {
        Run(col => col.Set($"{ColValue}"));
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_F2()
    {
        Run(col => col.Set($"{ColValue:F2}"), ColText + ".00");
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_F2_CultureInfoAsConfig()
    {
        Run(col => col.Set($"{ColValue:F2}"), ColText + ",00", CultureInfo.GetCultureInfo("da-DK"));
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_F2_CultureInfoAsParam()
    {
        Run(col => col.Set(CultureInfo.GetCultureInfo("da-DK"), $"{ColValue:F2}"), ColText + ",00");
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendLiteral()
    {
        Run(col => col.Set($"{ColValue} {"Literal"}"), ColText + " Literal");
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Format_Alignment()
    {
        Run(col => col.Set($"{ColValue,16:F2}"), new string(' ', 16 - ColText.Length - 3) + ColText + ".00");
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Alignment()
    {
        Run(col => col.Set($"{ColValue,16}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Span()
    {
        Run(col => col.Set($"{ColText.AsSpan()}"), ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Span_Alignment()
    {
        Run(col => col.Set($"{ColText.AsSpan(),16}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_String_Alignment()
    {
        string? nullableString = ColText;
        Run(col => col.Set($"{nullableString,16:s}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_AppendFormatted_Object_Alignment()
    {
        object? nullableObject = ColText;
        Run(col => col.Set($"{nullableObject,16:s}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public void SepWriterColTest_Format()
    {
        Run(col => col.Format(ColValue));
    }

    [TestMethod]
    public void SepWriterColTest_Format_Long()
    {
        var f = new LongSpanFormattable();
        Run(col => col.Format(f), f.Text);
    }

    public class LongSpanFormattable : ISpanFormattable
    {
        public string Text { get; } = ColTextLong;

        public string ToString(string? format, IFormatProvider? formatProvider) => Text;

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            charsWritten = Text.Length;
            return Text.TryCopyTo(destination);
        }
    }

    // No escaping needed
    [DataRow("", "")]
    [DataRow(" ", " ")]
    [DataRow("a", "a")]
    [DataRow(",.|", ",.|")]
    // Special characters - escaping needed
    [DataRow(";", "\";\"")]
    [DataRow("\r", "\"\r\"")]
    [DataRow("\n", "\"\n\"")]
    [DataRow("\"", "\"\"\"\"")]
    [DataRow("\r\n", "\"\r\n\"")]
    [DataRow("a;b\rc\nd\"e", "\"a;b\rc\nd\"\"e\"")]
    [DataTestMethod]
    public void SepWriterColTest_Escape(string textCol, string expectedCol)
    {
        using var writer = Sep.Writer(o => o with { Escape = true }).ToText();
        {
            using var row = writer.NewRow();
            // Use both for col name and col value so both tested
            row[textCol].Set(textCol);
        }
        var expected = $"{expectedCol}{NL}{expectedCol}{NL}";
        Assert.AreEqual(expected, writer.ToString());
    }

    static void Run(SepWriter.ColAction action, string? expectedColValue = ColText, CultureInfo? cultureInfo = null)
    {
        Func<SepWriter>[] createWriters =
        [
            () => Sep.Writer(o => o with { CultureInfo = cultureInfo ?? SepDefaults.CultureInfo }).ToText(),
            () => Sep.Default.Writer(o => o with { CultureInfo = cultureInfo ?? SepDefaults.CultureInfo }).ToText(),
            () => new SepSpec(Sep.Default, cultureInfo ?? SepDefaults.CultureInfo).Writer(o => o with { }).ToText(),
        ];
        foreach (var createWriter in createWriters)
        {
            using var writer = createWriter();
            {
                using var row = writer.NewRow();
                action(row[ColName]);
            }
            if (expectedColValue is not null)
            {
                var expectedText = $"{ColName}{Environment.NewLine}{expectedColValue}{Environment.NewLine}";
                var actualText = writer.ToString();
                Assert.AreEqual(expectedText, actualText);
            }
        }
    }
}
