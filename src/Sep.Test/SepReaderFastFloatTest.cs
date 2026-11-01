using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderFastFloatTest
{
    [TestMethod]
    [DataRow("0")]
    [DataRow("-0")]
    [DataRow("+1.25")]
    [DataRow("1.2345678901234567E-05")]
    [DataRow("1.000000059604644776")]
    [DataRow("9007199254740993.0000000000000000000000000000000001")]
    [DataRow("9,007,199,254,740,993.0000000000000000000000000000000001")]
    [DataRow("1,234,567.89")]
    [DataRow("  1.5")]
    [DataRow("\t-0")]
    [DataRow("\v\f+1.25e-2")]
    [DataRow(" 1.5 ")]
    [DataRow("NaN")]
    [DataRow("-Infinity")]
    [DataRow("inf")]
    [DataRow("")]
    [DataRow("not-a-number")]
    [DataRow("1e")]
    [DataRow("1.2x")]
    [DataRow("１")]
    public void InvariantCulture_MatchesBcl(string text)
    {
        AssertReaderMatchesBcl(text, CultureInfo.InvariantCulture, disableFastFloat: false, '.');
        AssertReaderMatchesBcl(text, CultureInfo.InvariantCulture, disableFastFloat: true, '\0');
    }

    [TestMethod]
    [DataRow("de-DE", "123,456789")]
    [DataRow("de-DE", "1.234.567,89")]
    [DataRow("de-DE", " 1,5 ")]
    [DataRow("de-DE", "1.2")]
    [DataRow("en-US", "123.456789")]
    [DataRow("en-US", "1,234,567.89")]
    public void ReadOnlyCulture_MatchesBcl(string cultureName, string text)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);
        AssertReaderMatchesBcl(text, culture, disableFastFloat: false, culture.NumberFormat.NumberDecimalSeparator[0]);
        AssertReaderMatchesBcl(text, culture, disableFastFloat: true, '\0');
    }

    [TestMethod]
    [DataRow("en-US", "1,234,567.89")]
    [DataRow("en-US", "1,23,4")]
    [DataRow("en-US", "1,,234,")]
    [DataRow("en-US", "  -1,234.5E+2  ")]
    [DataRow("en-US", "9,007,199,254,740,993.0000000000000000000000000000000001")]
    [DataRow("de-DE", "1.234.567,89")]
    [DataRow("de-DE", "1.234,5e-2")]
    public void GroupedNumbers_MatchBcl(string cultureName, string text)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);
        Assert.IsTrue(double.TryParse(text, culture, out _), text);
        Assert.IsTrue(float.TryParse(text, culture, out _), text);
        AssertReaderMatchesBcl(text, culture, disableFastFloat: false);
        AssertReaderMatchesBcl(text, culture, disableFastFloat: true, '\0');
    }

    [TestMethod]
    public void GroupedNumbers_LongMantissas_MatchBcl()
    {
        foreach (var text in new[] { "1," + new string('0', 800) + "e-1100", "9," + new string('9', 800) + "e-800" })
        {
            Assert.IsTrue(double.TryParse(text, CultureInfo.InvariantCulture, out _), text);
            Assert.IsTrue(float.TryParse(text, CultureInfo.InvariantCulture, out _), text);
            AssertReaderMatchesBcl(text, CultureInfo.InvariantCulture, disableFastFloat: false);
            AssertReaderMatchesBcl(text, CultureInfo.InvariantCulture, disableFastFloat: true, '\0');
        }
    }

    [TestMethod]
    [DataRow("en-US")]
    [DataRow("de-DE")]
    [DataRow("en-IN")]
    [DataRow("fr-FR")]
    [DataRow("de-CH")]
    [DataRow("ar-EG")]
    public void GroupedNumbers_FormattedSpecificCultures(string cultureName)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);
        foreach (var value in new[] { 1234567.89, -1234567.89, 9007199254740991.0 })
        {
            var text = value.ToString("N2", culture);
            Assert.IsTrue(text.Contains(culture.NumberFormat.NumberGroupSeparator, StringComparison.Ordinal), cultureName);
            Assert.AreEqual(value, double.Parse(text, culture), cultureName);
            AssertReaderMatchesBcl(text, culture, disableFastFloat: false);
            AssertReaderMatchesBcl(text, culture, disableFastFloat: true, '\0');
        }
    }

    [TestMethod]
    [DataRow("__", "1__234__567.89")]
    [DataRow("e", "1e2")]
    [DataRow("1", "1123.5")]
    public void GroupedNumbers_CustomSeparators(string groupSeparator, string text)
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.NumberGroupSeparator = groupSeparator;
        culture = CultureInfo.ReadOnly(culture);
        Assert.IsTrue(double.TryParse(text, culture, out _), text);
        Assert.IsTrue(float.TryParse(text, culture, out _), text);
        AssertReaderMatchesBcl(text, culture, disableFastFloat: false, '\0');
        AssertReaderMatchesBcl(text, culture, disableFastFloat: true, '\0');
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void GroupedNumbers_ColumnSelections(bool disableFastFloat)
    {
        const string text = "A|B|C\n1,234.5|-12,345.25|invalid\n";
        using var reader = new Sep('|').Reader(o => o with
        {
            CultureInfo = CultureInfo.InvariantCulture,
            DisableFastFloat = disableFastFloat,
        }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var row = reader.Current;
        var range = row[0..2];
        CollectionAssert.AreEqual(new[] { 1234.5, -12345.25 }, range.ParseToArray<double>());
        CollectionAssert.AreEqual(new[] { 1234.5f, -12345.25f }, range.Parse<float>().ToArray());
        CollectionAssert.AreEqual(new double?[] { 1234.5, -12345.25, null }, row[0..3].TryParse<double>().ToArray());
        var indices = row[new[] { 1, 0 }];
        CollectionAssert.AreEqual(new[] { -12345.25, 1234.5 }, indices.ParseToArray<double>());
        CollectionAssert.AreEqual(new float?[] { -12345.25f, 1234.5f }, indices.TryParse<float>().ToArray());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void GroupedNumbers_QuotedCsvFields(bool disableFastFloat)
    {
        const string text = "A,B\n\"1,234.5\",x\n";
        using var reader = new Sep(',').Reader(o => o with
        {
            CultureInfo = CultureInfo.InvariantCulture,
            DisableFastFloat = disableFastFloat,
            Unescape = true,
        }).FromText(text);
        Assert.IsTrue(reader.MoveNext());
        var col = reader.Current[0];
        Assert.AreEqual(1234.5, col.Parse<double>());
        Assert.AreEqual(1234.5f, col.Parse<float>());
        Assert.IsTrue(col.TryParse<double>(out var d));
        Assert.AreEqual(1234.5, d);
        Assert.AreEqual(1234.5f, col.TryParse<float>());
    }

    [TestMethod]
    public void CustomReadOnlyCultures_MatchBcl()
    {
        var comma = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        comma.NumberFormat.NumberDecimalSeparator = ",";
        comma.NumberFormat.NumberGroupSeparator = ".";
        comma = CultureInfo.ReadOnly(comma);
        AssertReaderMatchesBcl("12,34567890", comma, false, ',');
        AssertReaderMatchesBcl("1.234,5", comma, false, ',');

        var unicodeDecimal = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        unicodeDecimal.NumberFormat.NumberDecimalSeparator = "\u066B";
        unicodeDecimal = CultureInfo.ReadOnly(unicodeDecimal);
        AssertReaderMatchesBcl("123\u066B45678901e-2", unicodeDecimal, false, '\u066B');

        var noGroup = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        noGroup.NumberFormat.NumberGroupSeparator = "";
        noGroup = CultureInfo.ReadOnly(noGroup);
        AssertReaderMatchesBcl("12.5", noGroup, false, '.');

        var signs = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        signs.NumberFormat.PositiveSign = "^";
        signs.NumberFormat.NegativeSign = "~";
        signs = CultureInfo.ReadOnly(signs);
        AssertReaderMatchesBcl("~1.25", signs, false, '\0');
        AssertReaderMatchesBcl("-1.25", signs, false, '\0');
        AssertReaderMatchesBcl("1e~3", signs, false, '\0');

        var exponentGroup = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        exponentGroup.NumberFormat.NumberGroupSeparator = "e";
        exponentGroup = CultureInfo.ReadOnly(exponentGroup);
        AssertReaderMatchesBcl("1e2", exponentGroup, false, '\0');

        var digitGroup = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        digitGroup.NumberFormat.NumberGroupSeparator = "1";
        digitGroup = CultureInfo.ReadOnly(digitGroup);
        AssertReaderMatchesBcl("1123.5", digitGroup, false, '\0');

        var sameSeparators = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        sameSeparators.NumberFormat.NumberGroupSeparator = ".";
        sameSeparators = CultureInfo.ReadOnly(sameSeparators);
        AssertReaderMatchesBcl("1.5", sameSeparators, false, '\0');

        var multi = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        multi.NumberFormat.NumberDecimalSeparator = "..";
        multi.NumberFormat.NumberGroupSeparator = "__";
        multi = CultureInfo.ReadOnly(multi);
        AssertReaderMatchesBcl("1__234..5", multi, false, '\0');

        var whitespace = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        whitespace.NumberFormat.NumberDecimalSeparator = " ";
        whitespace = CultureInfo.ReadOnly(whitespace);
        AssertReaderMatchesBcl("1 5", whitespace, false, '\0');

        var special = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        special.NumberFormat.NaNSymbol = "1e2";
        special.NumberFormat.PositiveInfinitySymbol = "123.4";
        special.NumberFormat.NegativeInfinitySymbol = "-9.5";
        special = CultureInfo.ReadOnly(special);
        AssertReaderMatchesBcl("1e2", special, false, '.');
        AssertReaderMatchesBcl("123.4", special, false, '.');
        AssertReaderMatchesBcl("-9.5", special, false, '.');
        AssertReaderMatchesBcl("NaN", special, false, '.');
    }

    [TestMethod]
    public void SpecificCultures_MatchBcl()
    {
        double[] values = [0.125, -1.23456789012345, 1e-14, double.MaxValue, -0.0];
        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            var readOnly = CultureInfo.ReadOnly(culture);
            foreach (var value in values)
            {
                AssertReaderMatchesBcl(value.ToString("R", readOnly), readOnly, false);
            }
            foreach (var text in new[] { "1e2", "1E+3", "-0", "1.25", "1,5", " 1.5 " })
            {
                AssertReaderMatchesBcl(text, readOnly, false);
            }
        }
    }

    [TestMethod]
    public void MutableCulture_UsesCurrentFormattingAfterReaderCreation()
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        using var reader = CreateReader("1,5", culture, disableFastFloat: false);
        Assert.AreEqual('\0', reader._fastFloatDecimalSeparatorOrZero);

        culture.NumberFormat.NumberDecimalSeparator = ",";
        culture.NumberFormat.NumberGroupSeparator = ".";
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(1.5, reader.Current[0].Parse<double>());
        Assert.AreEqual(1.5f, reader.Current[0].Parse<float>());
        Assert.IsTrue(reader.Current[0].TryParse<double>(out var decimalValue));
        Assert.AreEqual(1.5, decimalValue);
        Assert.AreEqual(1.5f, reader.Current[0].TryParse<float>());

        culture.NumberFormat.NumberDecimalSeparator = ".";
        culture.NumberFormat.NumberGroupSeparator = ",";
        Assert.AreEqual(15.0, reader.Current[0].Parse<double>());
        Assert.AreEqual(15.0f, reader.Current[0].Parse<float>());
        Assert.IsTrue(reader.Current[0].TryParse<double>(out var groupedValue));
        Assert.AreEqual(15.0, groupedValue);
        Assert.AreEqual(15.0f, reader.Current[0].TryParse<float>());
    }

    [TestMethod]
    public void NullCulture_UsesCurrentCultureAtParseTime()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            using var reader = CreateReader("1,5", null, disableFastFloat: false);
            Assert.AreEqual('\0', reader._fastFloatDecimalSeparatorOrZero);
            Assert.IsTrue(reader.MoveNext());
            Assert.AreEqual(15.0, reader.Current[0].Parse<double>());
            Assert.IsTrue(reader.Current[0].TryParse<double>(out var groupedValue));
            Assert.AreEqual(15.0, groupedValue);
            Assert.AreEqual(15.0f, reader.Current[0].TryParse<float>());

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            Assert.AreEqual(1.5, reader.Current[0].Parse<double>());
            Assert.AreEqual(1.5f, reader.Current[0].Parse<float>());
            Assert.IsTrue(reader.Current[0].TryParse<double>(out var decimalValue));
            Assert.AreEqual(1.5, decimalValue);
            Assert.AreEqual(1.5f, reader.Current[0].TryParse<float>());
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [TestMethod]
    public void LongMantissa_PreservesCorrectValueOnDotNet8()
    {
        var text = "1" + new string('0', 800) + "e-1100";
        using var reader = CreateReader(text, CultureInfo.InvariantCulture, disableFastFloat: false);
        Assert.IsTrue(reader.MoveNext());
        Assert.AreEqual(1e-300, reader.Current[0].Parse<double>());
        Assert.IsTrue(reader.Current[0].TryParse<double>(out var value));
        Assert.AreEqual(1e-300, value);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ParallelEnumerate_PreservesFastFloatSetting(bool disabled)
    {
        const int count = 1000;
        var text = "A|B\n" + string.Concat(Enumerable.Repeat("1.25|x\n", count));
        using var reader = new Sep('|').Reader(o => o with { DisableFastFloat = disabled }).FromText(text);
        var values = reader.ParallelEnumerate(static row => row[0].Parse<double>(), degreeOfParallelism: 2).ToArray();
        Assert.AreEqual(count, values.Length);
        Assert.IsTrue(values.All(static value => value == 1.25));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void GroupedNumbers_ParallelEnumerate(bool disableFastFloat)
    {
        const int count = 1000;
        var text = "A|B\n" + string.Concat(Enumerable.Repeat("1,234.5|-12,345.25\n", count));
        using var reader = new Sep('|').Reader(o => o with
        {
            CultureInfo = CultureInfo.InvariantCulture,
            DisableFastFloat = disableFastFloat,
        }).FromText(text);
        var values = reader.ParallelEnumerate(static row =>
            (Double: row[0].Parse<double>(), Single: row[0].TryParse<float>(), Other: row[1].TryParse<double>()),
            degreeOfParallelism: 2).ToArray();
        Assert.AreEqual(count, values.Length);
        Assert.IsTrue(values.All(static value => value.Double == 1234.5 &&
            value.Single == 1234.5f && value.Other == -12345.25));
    }

    static SepReader CreateReader(string value, CultureInfo? culture, bool disableFastFloat) =>
        new Sep('|').Reader(o => o with { CultureInfo = culture, DisableFastFloat = disableFastFloat })
            .FromText($"A|B\n{value}|x\n");

    static void AssertReaderMatchesBcl(string text, CultureInfo? culture, bool disableFastFloat, char? expectedSeparator = null)
    {
        var expectedDoubleOk = double.TryParse(text, culture, out var expectedDouble);
        var expectedSingleOk = float.TryParse(text, culture, out var expectedSingle);
        using var reader = CreateReader(text, culture, disableFastFloat);
        if (expectedSeparator.HasValue) Assert.AreEqual(expectedSeparator.Value, reader._fastFloatDecimalSeparatorOrZero, text);
        Assert.IsTrue(reader.MoveNext(), text);
        var col = reader.Current[0];

        Assert.AreEqual(expectedDoubleOk, col.TryParse<double>(out var actualDouble), text);
        Assert.AreEqual(expectedSingleOk, col.TryParse<float>(out var actualSingle), text);
        Assert.AreEqual(BitConverter.DoubleToInt64Bits(expectedDouble), BitConverter.DoubleToInt64Bits(actualDouble), text);
        Assert.AreEqual(BitConverter.SingleToInt32Bits(expectedSingle), BitConverter.SingleToInt32Bits(actualSingle), text);

        var nullableDouble = col.TryParse<double>();
        var nullableSingle = col.TryParse<float>();
        Assert.AreEqual(expectedDoubleOk, nullableDouble.HasValue, text);
        Assert.AreEqual(expectedSingleOk, nullableSingle.HasValue, text);
        if (expectedDoubleOk)
        {
            Assert.AreEqual(BitConverter.DoubleToInt64Bits(expectedDouble), BitConverter.DoubleToInt64Bits(nullableDouble!.Value), text);
            Assert.AreEqual(BitConverter.DoubleToInt64Bits(expectedDouble), BitConverter.DoubleToInt64Bits(col.Parse<double>()), text);
        }
        else
        {
            Assert.ThrowsExactly<FormatException>(() => reader.Current[0].Parse<double>(), text);
        }

        if (expectedSingleOk)
        {
            Assert.AreEqual(BitConverter.SingleToInt32Bits(expectedSingle), BitConverter.SingleToInt32Bits(nullableSingle!.Value), text);
            Assert.AreEqual(BitConverter.SingleToInt32Bits(expectedSingle), BitConverter.SingleToInt32Bits(col.Parse<float>()), text);
        }
        else
        {
            Assert.ThrowsExactly<FormatException>(() => reader.Current[0].Parse<float>(), text);
        }
    }
}
