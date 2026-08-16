using System;
using System.Globalization;
using System.IO;
using System.Text;
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
    const string ColTextWithF2Format = "123456.00";
    static readonly string ColTextLong = new('a', 2048);

    static readonly string NL = Environment.NewLine;

    [TestMethod]
    public async ValueTask SepWriterColTest_ColIndex()
    {
        await Run(static col => Assert.AreEqual(0, col.ColIndex),
                  expectedColValue: null, expectedAllocatedBytes: null);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_ColName()
    {
        await Run(static col => Assert.AreEqual(ColName, col.ColName),
                  expectedColValue: null, expectedAllocatedBytes: null);
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
    public async ValueTask SepWriterColTest_Set_Utf8Span()
    {
        var bytes = Encoding.UTF8.GetBytes(ColText);
        await Run(col => col.Set(bytes));
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_Utf8Span_Long()
    {
        var bytes = Encoding.UTF8.GetBytes(ColTextLong);
        await Run(col => col.Set(bytes), ColTextLong);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_Utf8Span_Invalid_Throws()
    {
        var e = await Assert.ThrowsExactlyAsync<InvalidDataException>(async () =>
        {
            await Run(col => col.Set([0xBF]));
        });
        Assert.AreEqual("Invalid UTF-8 data.", e.Message);
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
        var cultureInfo = CultureInfo.GetCultureInfo("da-DK");
        await Run(col => col.Set($"{ColValue:F2}"), ColText + ",00", cultureInfo);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Set_InterpolatedString_F2_CultureInfoAsParam()
    {
        var cultureInfo = CultureInfo.GetCultureInfo("da-DK");
        await Run(col => col.Set(cultureInfo, $"{ColValue:F2}"), ColText + ",00");
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
    public async ValueTask SepWriterColTest_FormatWithCustomFormat()
    {
        await Run(col => col.Format(ColValue, "F2"), ColTextWithF2Format);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_Format_Long()
    {
        var f = new LongSpanFormattable();
        await Run(col => col.Format(f), f.Text);
    }

    enum TestEnum { Aaaa = 42 }
    [TestMethod]
    public async ValueTask SepWriterColTest_Set_Enum()
    {
        await Run(col => col.Set($"{TestEnum.Aaaa}"), TestEnum.Aaaa.ToString());
    }
    [TestMethod]
    public async ValueTask SepWriterColTest_Format_Enum()
    {
        // Unfortunately, formatting enum via T : ISpanFormatable allocates on
        // each format, due to boxing enum. Prefer FormatEnum/Set(..) instead.
        var expectedAllocatedBytes = System.Runtime.CompilerServices.Unsafe.SizeOf<nint>() * 3;
        await Run(col => col.Format(TestEnum.Aaaa), TestEnum.Aaaa.ToString(),
                  expectedAllocatedBytes: expectedAllocatedBytes);
        await Run(col => col.Format(TestEnum.Aaaa, "D"), ((int)TestEnum.Aaaa).ToString(),
                  expectedAllocatedBytes: expectedAllocatedBytes);
    }
    [TestMethod]
    public async ValueTask SepWriterColTest_FormatEnum_Enum()
    {
        await Run(col => col.FormatEnum(TestEnum.Aaaa), TestEnum.Aaaa.ToString());
        await Run(col => col.FormatEnum(TestEnum.Aaaa, "D"), ((int)TestEnum.Aaaa).ToString());
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
    [TestMethod]
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

    static async ValueTask Run(SepWriter.ColAction action, string? expectedColValue = ColText,
                               CultureInfo? cultureInfo = null, long? expectedAllocatedBytes = 0)
    {
        Func<SepWriter>[] createWriters =
        [
            () => Sep.Writer(o => o with { CultureInfo = cultureInfo ?? SepDefaults.CultureInfo }).ToText(),
            () => Sep.Default.Writer(o => o with { CultureInfo = cultureInfo ?? SepDefaults.CultureInfo }).ToText(),
            () => new SepSpec(Sep.Default, cultureInfo ?? SepDefaults.CultureInfo, false).Writer(o => o with { }).ToText(),
        ];
        foreach (var createWriter in createWriters)
        {
            // Sync
            {
                using var writer = createWriter();
                {
                    using var row = writer.NewRow();
                    // First row, first access will allocate ColImpl and supports, so warming up
                    action(row[ColName]);

                    var a0 = GC.GetAllocatedBytesForCurrentThread();
                    var col = row[ColName];
                    var a1 = GC.GetAllocatedBytesForCurrentThread();
                    action(col);
                    var a2 = GC.GetAllocatedBytesForCurrentThread();
                    // Getting col should always allocate zero bytes
                    Assert.AreEqual(0, a1 - a0);
                    if (expectedAllocatedBytes.HasValue)
                    {
                        Assert.AreEqual(expectedAllocatedBytes.Value, a2 - a1);
                    }
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
