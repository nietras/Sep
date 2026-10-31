using System;
using System.Buffers;
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
                  expectedColValue: null);
    }

    [TestMethod]
    public async ValueTask SepWriterColTest_ColName()
    {
        await Run(static col => Assert.AreEqual(ColName, col.ColName),
                  expectedColValue: null);
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
        await Run(col => col.Set($"{ColValue}"), expectedAllocatedBytes: 0);
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_NoGrowth_KeepsBuffer()
    {
        using var writer = Sep.Writer().ToText();
        using var row = writer.NewRow();
        var col = row[ColName];
        var impl = writer._cols[0];
        var buffer = impl._buffer;
        col.Set($"{1}");
        Assert.AreSame(buffer, impl._buffer);
        Assert.AreEqual("1", impl.GetSpan().ToString());
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_ThrowsAfterGrowth()
    {
        using var writer = Sep.Writer().ToText();
        using var row = writer.NewRow();
        var col = row[ColName];
        var impl = writer._cols[0];
        var initialBufferLength = impl._buffer.Length;
        var longText = new string('a', initialBufferLength + 1);
        Assert.ThrowsExactly<FormatException>(() =>
            new SepWriter.Col(impl).Set($"{longText}{int.Parse("invalid", CultureInfo.InvariantCulture)}"));

        var rented = ArrayPool<char>.Shared.Rent(initialBufferLength);
        try
        {
            const char expected = '!';
            rented[0] = expected;
            col.Set(ColText.AsSpan());
            Assert.AreEqual(ColText, impl.GetSpan().ToString());
            Assert.AreEqual(expected, rented[0], "Column write corrupted another renter's buffer.");
        }
        finally
        {
            // If aliased, writer disposal returns this array; do not return it twice.
            if (!ReferenceEquals(rented, impl._buffer))
            {
                ArrayPool<char>.Shared.Return(rented);
            }
        }
    }

    [TestMethod]
    public void SepWriterColTest_Set_Utf8_AfterInterpolationThrows_GrowsEmptyBuffer()
    {
        using var writer = Sep.Writer().ToText();
        using var row = writer.NewRow();
        var col = row[ColName];
        var impl = writer._cols[0];
        Assert.ThrowsExactly<FormatException>(() =>
            new SepWriter.Col(impl).Set($"{int.Parse("invalid", CultureInfo.InvariantCulture)}"));
        Assert.AreEqual(0, impl._buffer.Length);

        col.Set("value"u8);

        Assert.AreEqual("value", impl.GetSpan().ToString());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void SepWriterColTest_Set_InterpolatedString_Throws_Dispose(bool growth)
    {
        using var writer = Sep.Writer().ToText();
        int initialBufferLength;
        {
            using var row = writer.NewRow();
            var col = row[ColName];
            var impl = writer._cols[0];
            initialBufferLength = impl._buffer.Length;
            var text = growth ? new string('a', initialBufferLength + 1) : "a";
            Assert.ThrowsExactly<FormatException>(() =>
                new SepWriter.Col(impl).Set($"{text}{int.Parse("invalid", CultureInfo.InvariantCulture)}"));
            Assert.AreEqual(0, impl.GetSpan().Length);
            Assert.AreEqual(0, impl.GetMemory().Length);
        }

        var rented = ArrayPool<char>.Shared.Rent(initialBufferLength);
        try
        {
            writer.Dispose();
            var other = ArrayPool<char>.Shared.Rent(initialBufferLength);
            try
            {
                Assert.AreNotSame(rented, other, "Writer disposal returned an outstanding renter's buffer.");
            }
            finally
            {
                if (!ReferenceEquals(rented, other)) { ArrayPool<char>.Shared.Return(other); }
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void SepWriterColTest_Set_InterpolatedString_ReentrantGrowth(bool outerGrowth, bool interpolated)
    {
        using var writer = Sep.Writer().ToText();
        using var row = writer.NewRow();
        var col = row[ColName];
        var impl = writer._cols[0];
        var longText = new string('N', impl._buffer.Length + 1);
        var prefix = outerGrowth ? longText : "P";
        col.Set($"{prefix}{Reenter()}");
        Assert.AreEqual(prefix + "X", impl.GetSpan().ToString());

        string Reenter()
        {
            if (interpolated) { new SepWriter.Col(impl).Set($"{longText}"); }
            else { new SepWriter.Col(impl).Set(longText.AsSpan()); }
            return "X";
        }
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_ReentrantUtf8()
    {
        using var writer = Sep.Writer().ToText();
        using var row = writer.NewRow();
        var col = row[ColName];
        var impl = writer._cols[0];
        col.Set($"P{Reenter()}");
        Assert.AreEqual("PX", impl.GetSpan().ToString());

        string Reenter()
        {
            new SepWriter.Col(impl).Set("N"u8);
            return "X";
        }
    }

    [TestMethod]
    public void SepWriterColTest_Set_InterpolatedString_ReentrantThrows()
    {
        using var writer = Sep.Writer().ToText();
        using var row = writer.NewRow();
        var col = row[ColName];
        var impl = writer._cols[0];
        var text = new string('N', impl._buffer.Length + 1);
        col.Set($"P{Reenter()}");
        Assert.AreEqual("PX", impl.GetSpan().ToString());

        string Reenter()
        {
            Assert.ThrowsExactly<FormatException>(() =>
                new SepWriter.Col(impl).Set($"{text}{int.Parse("invalid", CultureInfo.InvariantCulture)}"));
            return "X";
        }
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
        await Run(col => col.Set($"{TestEnum.Aaaa}"), TestEnum.Aaaa.ToString(), expectedAllocatedBytes: 0);
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
        await Run(col => col.FormatEnum(TestEnum.Aaaa), TestEnum.Aaaa.ToString(), expectedAllocatedBytes: 0);
        await Run(col => col.FormatEnum(TestEnum.Aaaa, "D"), ((int)TestEnum.Aaaa).ToString(), expectedAllocatedBytes: 0);
    }

    [Flags]
    enum LongFlags
    {
        Flag000 = 1 << 0, Flag001 = 1 << 1, Flag002 = 1 << 2, Flag003 = 1 << 3,
        Flag004 = 1 << 4, Flag005 = 1 << 5, Flag006 = 1 << 6, Flag007 = 1 << 7,
        Flag008 = 1 << 8, Flag009 = 1 << 9, Flag010 = 1 << 10, Flag011 = 1 << 11,
        Flag012 = 1 << 12, Flag013 = 1 << 13, Flag014 = 1 << 14, Flag015 = 1 << 15,
        Flag016 = 1 << 16, Flag017 = 1 << 17, Flag018 = 1 << 18, Flag019 = 1 << 19,
        Flag020 = 1 << 20, Flag021 = 1 << 21, Flag022 = 1 << 22, Flag023 = 1 << 23,
        Flag024 = 1 << 24, Flag025 = 1 << 25, Flag026 = 1 << 26, Flag027 = 1 << 27,
        Flag028 = 1 << 28, Flag029 = 1 << 29, Flag030 = 1 << 30,
    }
    [TestMethod]
    public async ValueTask SepWriterColTest_FormatEnum_Enum_Long()
    {
        var value = (LongFlags)0x7FFFFFFF;
        var expected = value.ToString();
        Assert.IsGreaterThan(256, expected.Length);

        await Run(col => col.FormatEnum(value), expected);
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
                               CultureInfo? cultureInfo = null, long? expectedAllocatedBytes = null)
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
