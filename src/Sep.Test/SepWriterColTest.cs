using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
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
    public async ValueTask SepWriterColTest_ColIndex()
    {
        await Run(col => Assert.AreEqual(0, col.ColIndex), null);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_ColName()
    {
        await Run(col => Assert.AreEqual(ColName, col.ColName), null);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_String()
    {
        await Run(col => col.Set(ColText));
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_String_Long()
    {
        await Run(col => col.Set(ColTextLong), ColTextLong);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_Span()
    {
        await Run(col => col.Set(ColText.AsSpan()));
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_Span_Long()
    {
        await Run(col => col.Set(ColTextLong.AsSpan()), ColTextLong);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString()
    {
        await Run(col => col.Set($"{ColValue}"));
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_F2()
    {
        await Run(col => col.Set($"{ColValue:F2}"), ColText + ".00");
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_F2_CultureInfoAsConfig()
    {
        await Run(col => col.Set($"{ColValue:F2}"), ColText + ",00", CultureInfo.GetCultureInfo("da-DK"));
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_F2_CultureInfoAsParam()
    {
        await Run(col => col.Set(CultureInfo.GetCultureInfo("da-DK"), $"{ColValue:F2}"), ColText + ",00");
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_F2_CultureInfoAsConfig_Null()
    {
        await Run(col => col.Set($"{ColValue:F2}"), ColText + ".00", null);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_F2_CultureInfoAsParam_Null()
    {
        await Run(col => col.Set(provider: null, $"{ColValue:F2}"), ColText + ".00");
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_AppendLiteral()
    {
        await Run(col => col.Set($"{ColValue} {"Literal"}"), ColText + " Literal");
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_AppendFormatted_Format_Alignment()
    {
        await Run(col => col.Set($"{ColValue,16:F2}"), new string(' ', 16 - ColText.Length - 3) + ColText + ".00");
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_AppendFormatted_Alignment()
    {
        await Run(col => col.Set($"{ColValue,16}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_AppendFormatted_Span()
    {
        await Run(col => col.Set($"{ColText.AsSpan()}"), ColText);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_AppendFormatted_Span_Alignment()
    {
        await Run(col => col.Set($"{ColText.AsSpan(),16}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_AppendFormatted_String_Alignment()
    {
        string? nullableString = ColText;
        await Run(col => col.Set($"{nullableString,16:s}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_AppendFormatted_Object_Alignment()
    {
        object? nullableObject = ColText;
        await Run(col => col.Set($"{nullableObject,16:s}"), new string(' ', 16 - ColText.Length) + ColText);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Format()
    {
        await Run(col => col.Format(ColValue));
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Format_Long()
    {
        var f = new LongSpanFormattable();
        await Run(col => col.Format(f), f.Text);
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
    public async ValueTask SepWriterColTest_Escape(string textCol, string expectedCol)
    {
        {
            using var writer = Sep.Writer(o => o with { Escape = true }).ToText();
            {
                using var row = writer.NewRow();
                // Use both for col name and col value so both tested
                row[textCol].Set(textCol);
            }
            Assert(expectedCol, writer);
        }
        {
            await using var writer = Sep.Writer(o => o with { Escape = true }).ToText();
            {
                await using var row = writer.NewRow();
                // Use both for col name and col value so both tested
                row[textCol].Set(textCol);
            }
            Assert(expectedCol, writer);
        }

        static void Assert(string expectedCol, SepWriter writer)
        {
            var expected = $"{expectedCol}{NL}{expectedCol}{NL}";
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, writer.ToString());
        }
    }

    static async ValueTask Run(SepWriter.ColAction action, string? expectedColValue = ColText, CultureInfo? cultureInfo = null)
    {
        Func<SepWriter>[] createWriters =
        [
            () => Sep.Writer(o => o with { CultureInfo = cultureInfo ?? SepDefaults.CultureInfo }).ToText(),
            () => Sep.Default.Writer(o => o with { CultureInfo = cultureInfo ?? SepDefaults.CultureInfo }).ToText(),
            () => new SepSpec(Sep.Default, cultureInfo ?? SepDefaults.CultureInfo).Writer(o => o with { }).ToText(),
        ];
        foreach (var createWriter in createWriters)
        {
            // Sync
            {
                using var writer = createWriter();
                {
                    using var row = writer.NewRow();
                    action(row[ColName]);
                }
                AssertCol(expectedColValue, writer);
            }
            // Async
            {
                await using var writer = createWriter();
                {
                    var cts = new CancellationTokenSource();
                    await using var row = writer.NewRow(cts.Token);
                    action(row[ColName]);
                }
                AssertCol(expectedColValue, writer);
            }
        }

        static void AssertCol(string? expectedColValue, SepWriter writer)
        {
            if (expectedColValue is not null)
            {
                var expectedText = $"{ColName}{Environment.NewLine}{expectedColValue}{Environment.NewLine}";
                var actualText = writer.ToString();
                Assert.AreEqual(expectedText, actualText);
            }
        }
    }
}
