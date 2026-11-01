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

        culture.NumberFormat.NumberDecimalSeparator = ".";
        culture.NumberFormat.NumberGroupSeparator = ",";
        Assert.AreEqual(15.0, reader.Current[0].Parse<double>());
        Assert.AreEqual(15.0f, reader.Current[0].Parse<float>());
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

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            Assert.AreEqual(1.5, reader.Current[0].Parse<double>());
            Assert.AreEqual(1.5f, reader.Current[0].Parse<float>());
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
