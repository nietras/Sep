// Derived from CsvHelper's FastFloat tests and fast_float.
// Modified for Sep; CsvHelper-derived portions use Apache-2.0.
// Original licenses are in src/Sep/ThirdParty.
// CsvHelper source: https://github.com/JoshClose/CsvHelper/tree/dffe5e9a634e1894d334b6a69599d8b489c4fc78
// fast_float: https://github.com/fastfloat/fast_float
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

/// <summary>
/// FastFloat must be correctly rounded, generally matching double.Parse / float.Parse (invariant culture), must return
/// true for every well-formed number with at most 19 significant digits, and must return false for anything outside its
/// grammar <c>[+-]digits[.digits][(e|E)[+-]digits]</c> (plus Infinity/NaN).
/// </summary>
[TestClass]
public class FastFloatTests
{
	private const NumberStyles Styles = NumberStyles.Float | NumberStyles.AllowThousands;
	private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

	public TestContext TestContext { get; set; } = null!;

	// ------------------------------------------------------------------------------------------------------------
	// Hand-written edge cases
	// ------------------------------------------------------------------------------------------------------------

	public static FastFloatTheoryData<string> EdgeCases { get; } = new()
	{
		// zeros and signs
		"0", "-0", "+0", "0.0", "-0.0", "+0.0", "0.", "-0.", ".0", "-.0", "00000", "-00000.00000", "0e0", "-0e-0", "0E+0",
		"0e999999999", "-0e999999999", "0e-999999999", "-0e-999999999", "0.000000000000000000000000000000000000000000",
		// simple numbers
		"1", "-1", "+1", "1.0", "1.", ".5", "-.5", "+.5", "5.", "-5.", "5.e3", ".5e1", "1e0", "1E0", "1e+0", "1e-0",
		"12", "-12.5", "123456", "0.123456789", "1.2345678901234567E-05", "3.4028235E+38", "0.1", "0.2", "0.3", "2.5",
		"1e1", "1e10", "1e-10", "007", "-007.700", "1e0005", "1E-0005", "123.456e2", "123.456e-2", "4.35", "1.15",
		// exact fast path boundaries
		"9007199254740992", "9007199254740991", "9007199254740993", "9007199254740994", "9007199254740995",
		"9007199254740993e0", "9007199254740993e1", "900719925474099.3", "1e22", "1e-22", "1e23", "1e-23", "123e22",
		"4.9e-324", "123456789012345678", "1234567890123456789", "12345678901234567890", "9999999999999999999",
		"18446744073709551615", "18446744073709551616", "9223372036854775807", "9223372036854775808",
		"10000000000000000000", "99999999999999999999", "7.2057594037927933e16", "7.2057594037927932e16",
		// limits
		"1.7976931348623157e308", "-1.7976931348623157e308", "1.7976931348623158e308", "1.7976931348623159e308",
		"1.79769313486231570814527423731704356798070e308", "1.797693134862315807937289714053e308",
		"1.797693134862315807937289714054e308", "8.98846567431158e307", "8.98846567431157953864652595394512366e307",
		"2.2250738585072014e-308", "2.2250738585072011e-308", "2.2250738585072012e-308", "2.2250738585072013e-308",
		"2.225073858507201136057409796709131975934819546351645648e-308", "4.9406564584124654e-324", "5e-324",
		"2.4703282292062327e-324", "2.4703282292062328e-324", "3e-324", "1e-324", "1e-323", "1e-320", "-4.9e-324",
		"1e308", "1e309", "-1e309", "1e400", "-1e400", "1e-400", "-1e-400", "1e-342", "1e-343", "1e-341", "1e-307",
		"2e308", "1.8e308", "1.7e308", "9.9e307",
		// float limits (also exercise the double parser)
		"3.4028235e38", "3.40282347e38", "3.4028236e38", "3.40282356779733661637539395458142568448e38",
		"3.40282356779733661637539395458142568447e38", "1.17549435e-38", "1.1754943508222875e-38", "1.4e-45",
		"1.401298464324817e-45", "7e-46", "7.006492321624085e-46", "7.006492321624086e-46", "7.1e-46", "16777216",
		"16777217", "16777218", "16777219", "33554433", "1e38", "1e39", "1e-45", "1e-46", "1e-65", "1e-64",
		// exactly halfway between doubles
		"1.00000000000000011102230246251565404236316680908203125",
		"1.00000000000000011102230246251565404236316680908203124",
		"1.00000000000000011102230246251565404236316680908203126",
		"9007199254740993.0000000000000000000000000000000001", "9007199254740992.9999999999999999999999999999999999",
		"0.500000000000000166533453693773481063544750213623046875", "4503599627370497.5", "4503599627370496.5",
		// huge exponent digit strings
		"1e99999999999999999999", "1e-99999999999999999999", "0e99999999999999999999", "-1e+99999999999999999999",
		"1.5e2147483648", "1.5e2147483647", "1e-2147483649", "123456789e-2147483648", "1e+00000000000000000000000001",
		"0.0000000000000000000000000000000000000000000000000000000000000000000001e70",
		"100000000000000000000000000000000000000000000000000000000000000000000e-68",
		// leading zeros and long digit strings
		"0000000000000000000000000000001.5", "0.0000000000000000000000000000000000000001234567890123",
		"-000000000000000000000000000000000000.000000000000000000000000000000000000e5",
		"000000000000000000000000000000000000012345678901234567890",
		"3.141592653589793238462643383279502884197169399375105820974944592307816406286",
		"2.718281828459045235360287471352662497757247093699959574966967627724076630353",
		"123456789012345678901234567890123456789012345678901234567890",
		"0.30000000000000000000000000000000000000000000000000001", "0.29999999999999998889776975374843459576368331909180",
		// special values (accepted case-insensitively by double.Parse)
		"Infinity", "-Infinity", "+Infinity", "infinity", "INFINITY", "-iNfInItY", "NaN", "nan", "-NaN", "+nan", "NAN",
	};

	public static FastFloatTheoryData<string> InvalidInputs { get; } = new()
	{
		"", " ", "  ", "\t", " 1", "1 ", "\t1", "1\t", "1\n", "+", "-", ".", "-.", "+.", "e", "e5", ".e5", "-e5", "1e",
		"1e+", "1e-", "1E", "1.2.3", "1..2", "1,000", "1,5", "1,000.5", "0x10", "0X1F", "1f", "1d", "1m", "abc", "--1",
		"+-1", "-+1", "++1", "1e5e5", "1e5.5", "1e++5", "1e--5", "1e+-5", "1_000", "1 000", "1'000", "١", "１",
		"inf", "Inf", "-inf", "+INF", "infinit", "infinityy", "nana", "na", "∞", "-∞", "1\0", "\01", "1-", "1+",
		"(1)", "$1", "1%", "1.0f", "e1", "E1", "1e1.", "1.e", ".e", "+.e1", "0x", "1.5E+", "1.5E-", "1..", "..1",
	};

	[TestMethod]
	[DynamicData(nameof(EdgeCases))]
	public void Double_EdgeCases_MatchDoubleParse(string s)
	{
		// CheckDouble also requires acceptance of every input with at most 19 significant digits (and Infinity/NaN).
		var failures = new Failures();
		CheckDouble(Encoding.UTF8.GetBytes(s), failures);
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DynamicData(nameof(EdgeCases))]
	public void Single_EdgeCases_MatchSingleParse(string s)
	{
		var failures = new Failures();
		CheckSingle(Encoding.UTF8.GetBytes(s), failures);
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DynamicData(nameof(InvalidInputs))]
	public void InvalidInputs_ReturnFalse(string s)
	{
		var utf8 = Encoding.UTF8.GetBytes(s);
		Assert.IsFalse(FastFloat.TryParseDouble(utf8, out _), $"TryParseDouble accepted \"{s}\"");
		Assert.IsFalse(FastFloat.TryParseSingle(utf8, out _), $"TryParseSingle accepted \"{s}\"");
		Assert.IsFalse(FastFloat.TryParseDouble(s.AsSpan(), out _), $"UTF-16 TryParseDouble accepted \"{s}\"");
		Assert.IsFalse(FastFloat.TryParseSingle(s.AsSpan(), out _), $"UTF-16 TryParseSingle accepted \"{s}\"");
	}

	[TestMethod]
	[DataRow("12,34567890")]
	[DataRow("-123456789,01234567e-12")]
	[DataRow(",000000001")]
	[DataRow("1,")]
	[DataRow("1,000000059604644776")]
	public void DecimalSeparator_BothEncodings(string s)
	{
		var culture = CultureInfo.GetCultureInfo("de-DE");
		var utf8 = Encoding.UTF8.GetBytes(s);
		var expectedDouble = double.Parse(s, Styles, culture);
		var expectedSingle = float.Parse(s, Styles, culture);
		Assert.IsTrue(FastFloat.TryParseDouble(utf8, out var d8, ','));
		Assert.IsTrue(FastFloat.TryParseDouble(s.AsSpan(), out var d16, ','));
		Assert.IsTrue(FastFloat.TryParseSingle(utf8, out var f8, ','));
		Assert.IsTrue(FastFloat.TryParseSingle(s.AsSpan(), out var f16, ','));
		Assert.AreEqual(BitConverter.DoubleToInt64Bits(expectedDouble), BitConverter.DoubleToInt64Bits(d8));
		Assert.AreEqual(BitConverter.DoubleToInt64Bits(expectedDouble), BitConverter.DoubleToInt64Bits(d16));
		Assert.AreEqual(BitConverter.SingleToInt32Bits(expectedSingle), BitConverter.SingleToInt32Bits(f8));
		Assert.AreEqual(BitConverter.SingleToInt32Bits(expectedSingle), BitConverter.SingleToInt32Bits(f16));
	}

	[TestMethod]
	public void DecimalComma_RandomizedMatchesBcl()
	{
		var culture = CultureInfo.GetCultureInfo("de-DE");
		var failures = new Failures();
		RunParallel(267, 200_000, failures, (rng, f) =>
		{
			var source = BitConverter.Int64BitsToDouble(rng.NextInt64(long.MinValue, long.MaxValue));
			if (!double.IsFinite(source)) return;
			Span<byte> bytes = stackalloc byte[128];
			Assert.IsTrue(source.TryFormat(bytes, out var length, rng.Next(2) == 0 ? "R" : "E16", Invariant));
			var utf8 = bytes.Slice(0, length);
			for (var i = 0; i < utf8.Length; i++)
				if (utf8[i] == '.') utf8[i] = (byte)',';
			Span<char> chars = stackalloc char[128];
			var utf16 = chars.Slice(0, Encoding.UTF8.GetChars(utf8, chars));

			var expectedDouble = double.Parse(utf16, Styles, culture);
			var expectedSingle = float.Parse(utf16, Styles, culture);
			var okDouble8 = FastFloat.TryParseDouble(utf8, out var double8, ',');
			var okDouble16 = FastFloat.TryParseDouble(utf16, out var double16, ',');
			var okSingle8 = FastFloat.TryParseSingle(utf8, out var single8, ',');
			var okSingle16 = FastFloat.TryParseSingle(utf16, out var single16, ',');
			if (!okDouble8 || !okDouble16 || !okSingle8 || !okSingle16 ||
				BitConverter.DoubleToInt64Bits(expectedDouble) != BitConverter.DoubleToInt64Bits(double8) ||
				BitConverter.DoubleToInt64Bits(expectedDouble) != BitConverter.DoubleToInt64Bits(double16) ||
				BitConverter.SingleToInt32Bits(expectedSingle) != BitConverter.SingleToInt32Bits(single8) ||
				BitConverter.SingleToInt32Bits(expectedSingle) != BitConverter.SingleToInt32Bits(single16))
				f.Add($"comma decimal mismatch: \"{Show(utf8)}\"");
			f.Checked++;
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow("1234567\u013012345678")]
	[DataRow("12345678\u013112345678")]
	[DataRow("123456789012345\u0139")]
	[DataRow("12345678\uFF1012345678")]
	[DataRow("12345678\uD80012345678")]
	[DataRow("0.1234567\u013012345678")]
	[DataRow("0.12345678\u013112345678")]
	[DataRow("0.123456789012345\u0139")]
	[DataRow("0.12345678\uFF1012345678")]
	[DataRow("0.12345678\uD80012345678")]
	public void Utf16_NonAsciiDigitsAreRejected(string s)
	{
		Assert.IsFalse(FastFloat.TryParseDouble(s.AsSpan(), out _));
		Assert.IsFalse(FastFloat.TryParseSingle(s.AsSpan(), out _));
	}

	[TestMethod]
	public void Utf16_ShortDigitRuns()
	{
		string[] suffixes = ["", "9", "98", "987", "9876", "e-2"];
		Span<char> buffer = stackalloc char[16];
		buffer[0] = '0';
		buffer[1] = '.';
		for (var i = 0; i < 10_000; i++)
		{
			Assert.IsTrue(i.TryFormat(buffer.Slice(2), out var written, "D4", Invariant));
			Assert.AreEqual(4, written);
			foreach (var suffix in suffixes)
			{
				suffix.AsSpan().CopyTo(buffer.Slice(6));
				var s = buffer.Slice(0, 6 + suffix.Length);
				var okDouble = FastFloat.TryParseDouble(s, out var d);
				var okSingle = FastFloat.TryParseSingle(s, out var f);
				var expectedDouble = BitConverter.DoubleToInt64Bits(double.Parse(s, Styles, Invariant));
				var expectedSingle = BitConverter.SingleToInt32Bits(float.Parse(s, Styles, Invariant));
				if (!okDouble || !okSingle ||
					expectedDouble != BitConverter.DoubleToInt64Bits(d) ||
					expectedSingle != BitConverter.SingleToInt32Bits(f))
					Assert.Fail($"Short digit run \"{s}\": double accepted={okDouble}, expected=0x{expectedDouble:X16}, actual=0x{BitConverter.DoubleToInt64Bits(d):X16}; " +
						$"float accepted={okSingle}, expected=0x{expectedSingle:X8}, actual=0x{BitConverter.SingleToInt32Bits(f):X8}");
			}
		}

		foreach (var invalid in new[] { '/', ':', '\u0130', '\uFF10', '\u8000', '\uFFFF', '\uD800', '\0' })
		{
			for (var position = 0; position < 4; position++)
			{
				var chars = buffer.Slice(0, 9);
				"0.1234e-2".AsSpan().CopyTo(chars);
				chars[2 + position] = invalid;
				if (FastFloat.TryParseDouble(chars, out _) || FastFloat.TryParseSingle(chars, out _))
					Assert.Fail($"Accepted invalid digit run: \"{chars}\"");
			}
		}
	}

	[TestMethod]
	[DataRow("NaN")]
	[DataRow("-Infinity")]
	public void SpecialValues_CanBeDeferredToTheCaller(string s)
	{
		Assert.IsFalse(FastFloat.TryParseDouble(s.AsSpan(), out _, allowSpecial: false));
		Assert.IsFalse(FastFloat.TryParseSingle(s.AsSpan(), out _, allowSpecial: false));
		Assert.IsFalse(FastFloat.TryParseDouble(Encoding.UTF8.GetBytes(s), out _, allowSpecial: false));
		Assert.IsFalse(FastFloat.TryParseSingle(Encoding.UTF8.GetBytes(s), out _, allowSpecial: false));
	}

	[TestMethod]
	[DataRow("  1.5")]
	[DataRow("\t-0")]
	[DataRow("\v\f+1.25e-2")]
	[DataRow("\r\n1.000000059604644776")]
	[DataRow(" \tNaN")]
	[DataRow("\t-Infinity")]
	public void LeadingWhitespace_BothEncodings(string s)
	{
		var bytes = Encoding.UTF8.GetBytes(s);
		var expectedDouble = BitConverter.DoubleToInt64Bits(double.Parse(s, Styles, Invariant));
		var expectedSingle = BitConverter.SingleToInt32Bits(float.Parse(s, Styles, Invariant));
		Assert.IsTrue(FastFloat.TryParseDouble(bytes, out var d8, allowLeadingWhitespace: true));
		Assert.IsTrue(FastFloat.TryParseDouble(s.AsSpan(), out var d16, allowLeadingWhitespace: true));
		Assert.IsTrue(FastFloat.TryParseSingle(bytes, out var f8, allowLeadingWhitespace: true));
		Assert.IsTrue(FastFloat.TryParseSingle(s.AsSpan(), out var f16, allowLeadingWhitespace: true));
		Assert.AreEqual(expectedDouble, BitConverter.DoubleToInt64Bits(d8));
		Assert.AreEqual(expectedDouble, BitConverter.DoubleToInt64Bits(d16));
		Assert.AreEqual(expectedSingle, BitConverter.SingleToInt32Bits(f8));
		Assert.AreEqual(expectedSingle, BitConverter.SingleToInt32Bits(f16));
	}

	[TestMethod]
	[DataRow(" ")]
	[DataRow("\t\r\n")]
	[DataRow(" 1.5 ")]
	[DataRow("\u00A01.5")]
	[DataRow("\u20031.5")]
	[DataRow(" \0")]
	public void LeadingWhitespace_RejectsUnsupportedInput(string s)
	{
		var bytes = Encoding.UTF8.GetBytes(s);
		Assert.IsFalse(FastFloat.TryParseDouble(bytes, out _, allowLeadingWhitespace: true));
		Assert.IsFalse(FastFloat.TryParseDouble(s.AsSpan(), out _, allowLeadingWhitespace: true));
		Assert.IsFalse(FastFloat.TryParseSingle(bytes, out _, allowLeadingWhitespace: true));
		Assert.IsFalse(FastFloat.TryParseSingle(s.AsSpan(), out _, allowLeadingWhitespace: true));
	}

	[TestMethod]
	[DataRow("9007199254740993.00000000000000000000", true)]
	[DataRow("9007199254740993.00000000000000000001", false)]
	[DataRow("0000009007199254740993.00000000000000000001e+0", false)]
	[DataRow("900719925474099300000000000000000000e-20", true)]
	[DataRow("900719925474099300000000000000000001e-20", false)]
	public void Double_LongHalfwaySuffixes(string s, bool expectedFast)
	{
		var bytes = Encoding.UTF8.GetBytes(s);
		var expected = BitConverter.DoubleToInt64Bits(double.Parse(s, Styles, Invariant));
		Assert.AreEqual(expectedFast, FastFloat.TryParseDouble(bytes, out var utf8));
		Assert.AreEqual(expectedFast, FastFloat.TryParseDouble(s.AsSpan(), out var utf16));
		if (expectedFast)
		{
			Assert.AreEqual(expected, BitConverter.DoubleToInt64Bits(utf8));
			Assert.AreEqual(expected, BitConverter.DoubleToInt64Bits(utf16));
		}
	}

	[TestMethod]
	[DataRow("16777217.00000000000000000000", true)]
	[DataRow("16777217.00000000000000000001", false)]
	[DataRow("0000016777217.00000000000000000001e+0", false)]
	public void Single_LongHalfwaySuffixes(string s, bool expectedFast)
	{
		var bytes = Encoding.UTF8.GetBytes(s);
		var expected = BitConverter.SingleToInt32Bits(float.Parse(s, Styles, Invariant));
		Assert.AreEqual(expectedFast, FastFloat.TryParseSingle(bytes, out var utf8));
		Assert.AreEqual(expectedFast, FastFloat.TryParseSingle(s.AsSpan(), out var utf16));
		if (expectedFast)
		{
			Assert.AreEqual(expected, BitConverter.SingleToInt32Bits(utf8));
			Assert.AreEqual(expected, BitConverter.SingleToInt32Bits(utf16));
		}
	}

	[TestMethod]
	[DataRow("0", 0x0000000000000000L)]
	[DataRow("-0", unchecked((long)0x8000000000000000UL))]
	[DataRow("-0e-400", unchecked((long)0x8000000000000000UL))]
	[DataRow("1", 0x3FF0000000000000L)]
	[DataRow("9007199254740993", 0x4340000000000000L)] // 2^53 + 1 is a tie: rounds to even (2^53)
	[DataRow("9007199254740995", 0x4340000000000002L)] // 2^53 + 3 is a tie: rounds to even (2^53 + 4)
	[DataRow("1e23", 0x44B52D02C7E14AF6L)]
	[DataRow("8.98846567431158e307", 0x7FE0000000000000L)]
	[DataRow("1.7976931348623157e308", 0x7FEFFFFFFFFFFFFFL)]
	[DataRow("1.7976931348623159e308", 0x7FF0000000000000L)]
	[DataRow("-1e400", unchecked((long)0xFFF0000000000000UL))]
	[DataRow("2.2250738585072014e-308", 0x0010000000000000L)]
	[DataRow("2.2250738585072011e-308", 0x000FFFFFFFFFFFFFL)]
	[DataRow("2.2250738585072012e-308", 0x0010000000000000L)]
	[DataRow("4.9406564584124654e-324", 0x0000000000000001L)]
	[DataRow("2.4703282292062327e-324", 0x0000000000000000L)]
	[DataRow("2.4703282292062328e-324", 0x0000000000000001L)]
	[DataRow("1.00000000000000011102230246251565404236316680908203125", 0x3FF0000000000000L)]
	[DataRow("0.1", 0x3FB999999999999AL)]
	[DataRow("-12.5", unchecked((long)0xC029000000000000UL))]
	public void Double_KnownBitPatterns(string s, long expectedBits)
	{
		var utf8 = Encoding.UTF8.GetBytes(s);
		Assert.AreEqual(expectedBits, BitConverter.DoubleToInt64Bits(double.Parse(utf8, Styles, Invariant)));
		if (FastFloat.TryParseDouble(utf8, out var d)) Assert.AreEqual(expectedBits, BitConverter.DoubleToInt64Bits(d));
		else Assert.IsTrue(s.Length > 20, $"\"{s}\" should take the fast path");
		if (FastFloat.TryParseDouble(s.AsSpan(), out var d16)) Assert.AreEqual(expectedBits, BitConverter.DoubleToInt64Bits(d16));
		else Assert.IsTrue(s.Length > 20, $"UTF-16 \"{s}\" should take the fast path");
	}

	[TestMethod]
	[DataRow("16777217", 0x4B800000)] // 2^24 + 1 is a tie: rounds to even
	[DataRow("16777219", 0x4B800002)]
	[DataRow("3.4028235e38", 0x7F7FFFFF)]
	[DataRow("3.4028236e38", 0x7F800000)]
	[DataRow("1.17549435e-38", 0x00800000)]
	[DataRow("1.4e-45", 0x00000001)]
	[DataRow("7e-46", 0x00000000)]
	[DataRow("7.1e-46", 0x00000001)]
	[DataRow("-0", unchecked((int)0x80000000))]
	[DataRow("0.1", 0x3DCCCCCD)]
	[DataRow("1.00000005960464477539062500", 0x3F800000)] // 1 + 2^-24 is a tie: rounds to even (1)
	[DataRow("1.00000017881393432617187500", 0x3F800002)] // 1 + 3 * 2^-24 is a tie: rounds to even
	[DataRow("1.000000059604644775", 0x3F800000)] // just below the tie
	[DataRow("1.000000059604644776", 0x3F800001)] // just above the tie
	public void Single_KnownBitPatterns(string s, int expectedBits)
	{
		var utf8 = Encoding.UTF8.GetBytes(s);
		Assert.AreEqual(expectedBits, BitConverter.SingleToInt32Bits(float.Parse(utf8, Styles, Invariant)));
		if (FastFloat.TryParseSingle(utf8, out var f)) Assert.AreEqual(expectedBits, BitConverter.SingleToInt32Bits(f));
		else Assert.IsTrue(s.Length > 20, $"\"{s}\" should take the fast path");
		if (FastFloat.TryParseSingle(s.AsSpan(), out var f16)) Assert.AreEqual(expectedBits, BitConverter.SingleToInt32Bits(f16));
		else Assert.IsTrue(s.Length > 20, $"UTF-16 \"{s}\" should take the fast path");
	}

	[TestMethod]
	[DataRow("0")]
	[DataRow("1")]
	[DataRow("-1")]
	[DataRow("0.5")]
	[DataRow("-12.5")]
	[DataRow("123456")]
	[DataRow("0.123456789")]
	[DataRow("1.2345678901234567E-05")]
	[DataRow("3.4028235E+38")]
	[DataRow("1.7976931348623157E+308")]
	[DataRow("2.2250738585072014E-308")]
	[DataRow("4.94065645841247E-324")]
	[DataRow("1E+16")]
	[DataRow("1E-05")]
	[DataRow("-0.000123")]
	[DataRow("98765.4321")]
	[DataRow("3.14159")]
	[DataRow("2.718281828459045")]
	[DataRow("42")]
	[DataRow("1234567890123")]
	[DataRow("-9.87654321e10")]
	[DataRow("6.02214076e23")]
	[DataRow("1.602176634e-19")]
	[DataRow("1.401298E-45")]
	[DataRow("1.17549435E-38")]
	[DataRow("100.00")]
	[DataRow(".25")]
	[DataRow("25.")]
	[DataRow("+7")]
	public void CommonInputs_AreHandledWithoutFallback(string s)
	{
		var utf8 = Encoding.UTF8.GetBytes(s);
		Assert.IsTrue(FastFloat.TryParseDouble(utf8, out var d), $"TryParseDouble fell back for \"{s}\"");
		Assert.AreEqual(BitConverter.DoubleToInt64Bits(double.Parse(utf8, Styles, Invariant)), BitConverter.DoubleToInt64Bits(d));
		Assert.IsTrue(FastFloat.TryParseSingle(utf8, out var f), $"TryParseSingle fell back for \"{s}\"");
		Assert.AreEqual(BitConverter.SingleToInt32Bits(float.Parse(utf8, Styles, Invariant)), BitConverter.SingleToInt32Bits(f));
	}

	[TestMethod]
	public void LongMantissas()
	{
		var failures = new Failures();
		var rng = new Random(800);
		string[] inputs =
		[
			"1" + new string('0', 800),
			"1" + new string('0', 800) + "e-800",
#if !NET8_0
			// 1E-300, which .NET 8's double.Parse returns as 0 (.NET 9 fixed it).
			"1" + new string('0', 800) + "e-1100",
#endif
			"0." + new string('9', 800),
			"9" + new string('9', 799),
			"0." + new string('0', 400) + "1e400",
			"0." + new string('0', 799) + "1",
			"1." + new string('0', 799) + "1",
			"1." + new string('0', 16) + "1" + new string('0', 783),
			new string('0', 800) + "1.5",
			"0." + new string('0', 800),
		];
		foreach (var s in inputs)
		{
			var utf8 = Encoding.UTF8.GetBytes(s);
			CheckDouble(utf8, failures);
			CheckSingle(utf8, failures);
		}

		Span<byte> buffer = stackalloc byte[832];
		for (var n = 0; n < 200; n++)
		{
			var len = rng.Next(20, 801);
			for (var k = 0; k < len; k++) buffer[k] = (byte)('0' + rng.Next(10));
			if (rng.Next(2) == 0)
			{
				var point = rng.Next(len + 1);
				buffer.Slice(point, len - point).CopyTo(buffer.Slice(point + 1));
				buffer[point] = (byte)'.';
				len++;
			}
			if (rng.Next(2) == 0)
			{
				buffer[len++] = (byte)'e';
				Assert.IsTrue(rng.Next(-1200, 400).TryFormat(buffer.Slice(len), out var written, default, Invariant));
				len += written;
			}
			CheckDouble(buffer.Slice(0, len), failures);
			CheckSingle(buffer.Slice(0, len), failures);
		}

		// Long but easily decided mantissas must not need the fallback.
		Assert.IsTrue(FastFloat.TryParseDouble(Encoding.UTF8.GetBytes("1" + new string('0', 800)), out var inf));
		Assert.AreEqual(double.PositiveInfinity, inf);
		Assert.IsTrue(FastFloat.TryParseDouble(Encoding.UTF8.GetBytes("0." + new string('9', 800)), out var one));
		Assert.AreEqual(1.0, one);
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow(800)]
	[DataRow(1200)]
	public void LongMantissas_RoundCorrectlyBeyondDotNet8DigitLimit(int zeroCount)
	{
		// .NET 8's double.Parse returns zero for these values; csFastFloat and later runtimes return 1e-300.
		var s = "1" + new string('0', zeroCount) + "e-" + (zeroCount + 300);
		var expected = BitConverter.DoubleToInt64Bits(1e-300);
		Assert.IsTrue(FastFloat.TryParseDouble(Encoding.UTF8.GetBytes(s), out var utf8));
		Assert.IsTrue(FastFloat.TryParseDouble(s.AsSpan(), out var utf16));
		Assert.AreEqual(expected, BitConverter.DoubleToInt64Bits(utf8));
		Assert.AreEqual(expected, BitConverter.DoubleToInt64Bits(utf16));
	}

	[TestMethod]
	public void NeverReadsOutsideTheSpan()
	{
		// Inputs placed flush against inaccessible pages: reading a single byte past either end would crash.
		if (!OperatingSystem.IsWindows()) return;
		const int Page = 4096;
		var block = VirtualAlloc(IntPtr.Zero, 3 * Page, MemCommit | MemReserve, PageReadWrite);
		Assert.AreNotEqual(IntPtr.Zero, block);
		try
		{
			Assert.IsTrue(VirtualProtect(block, Page, PageNoAccess, out _));
			Assert.IsTrue(VirtualProtect(block + 2 * Page, Page, PageNoAccess, out _));
			var usable = block + Page;

			var inputs = new List<byte[]>();
			foreach (var item in EdgeCases) inputs.Add(Encoding.UTF8.GetBytes((string)item[0]));
			foreach (var item in InvalidInputs) inputs.Add(Encoding.UTF8.GetBytes((string)item[0]));
			var rng = new Random(77);
			Span<byte> buffer = stackalloc byte[128];
			Span<char> charBuffer = stackalloc char[1024];
			for (var n = 0; n < 5000; n++) inputs.Add(buffer.Slice(0, RandomNumberString(rng, buffer, maxDigits: 30, maxExponent: 330)).ToArray());
			for (var n = 0; n < 2000; n++)
			{
				var d = BitConverter.Int64BitsToDouble(rng.NextInt64(long.MinValue, long.MaxValue));
				Assert.IsTrue(d.TryFormat(buffer, out var written, n % 2 == 0 ? "R" : "E16", Invariant));
				inputs.Add(buffer.Slice(0, written).ToArray());
				Assert.IsTrue(TypicalDouble(rng).TryFormat(buffer, out written, TypicalFormats[rng.Next(TypicalFormats.Length)], Invariant));
				inputs.Add(buffer.Slice(0, written).ToArray());
			}

			foreach (var input in inputs)
			{
				var okDouble = FastFloat.TryParseDouble(input, out var expectedDouble);
				var okSingle = FastFloat.TryParseSingle(input, out var expectedSingle);
				var okDoubleWhite = FastFloat.TryParseDouble(input, out var expectedDoubleWhite, allowLeadingWhitespace: true);
				var okSingleWhite = FastFloat.TryParseSingle(input, out var expectedSingleWhite, allowLeadingWhitespace: true);
				for (var side = 0; side < 2; side++)
				{
					var address = side == 0 ? usable + Page - input.Length : usable;
					var span = NativeSpan(address, input.Length);
					input.CopyTo(span);
					Assert.AreEqual(okDouble, FastFloat.TryParseDouble(span, out var d));
					Assert.AreEqual(BitConverter.DoubleToInt64Bits(expectedDouble), BitConverter.DoubleToInt64Bits(d));
					Assert.AreEqual(okSingle, FastFloat.TryParseSingle(span, out var f));
					Assert.AreEqual(BitConverter.SingleToInt32Bits(expectedSingle), BitConverter.SingleToInt32Bits(f));
					Assert.AreEqual(okDoubleWhite, FastFloat.TryParseDouble(span, out var dw, allowLeadingWhitespace: true));
					Assert.AreEqual(BitConverter.DoubleToInt64Bits(expectedDoubleWhite), BitConverter.DoubleToInt64Bits(dw));
					Assert.AreEqual(okSingleWhite, FastFloat.TryParseSingle(span, out var fw, allowLeadingWhitespace: true));
					Assert.AreEqual(BitConverter.SingleToInt32Bits(expectedSingleWhite), BitConverter.SingleToInt32Bits(fw));
				}

				var chars = charBuffer.Slice(0, Encoding.UTF8.GetChars(input, charBuffer));
				okDouble = FastFloat.TryParseDouble(chars, out expectedDouble);
				okSingle = FastFloat.TryParseSingle(chars, out expectedSingle);
				okDoubleWhite = FastFloat.TryParseDouble(chars, out expectedDoubleWhite, allowLeadingWhitespace: true);
				okSingleWhite = FastFloat.TryParseSingle(chars, out expectedSingleWhite, allowLeadingWhitespace: true);
				var byteLength = chars.Length * sizeof(char);
				for (var side = 0; side < 2; side++)
				{
					var address = side == 0 ? usable + Page - byteLength : usable;
					var span = MemoryMarshal.Cast<byte, char>(NativeSpan(address, byteLength));
					chars.CopyTo(span);
					Assert.AreEqual(okDouble, FastFloat.TryParseDouble(span, out var d));
					Assert.AreEqual(BitConverter.DoubleToInt64Bits(expectedDouble), BitConverter.DoubleToInt64Bits(d));
					Assert.AreEqual(okSingle, FastFloat.TryParseSingle(span, out var f));
					Assert.AreEqual(BitConverter.SingleToInt32Bits(expectedSingle), BitConverter.SingleToInt32Bits(f));
					Assert.AreEqual(okDoubleWhite, FastFloat.TryParseDouble(span, out var dw, allowLeadingWhitespace: true));
					Assert.AreEqual(BitConverter.DoubleToInt64Bits(expectedDoubleWhite), BitConverter.DoubleToInt64Bits(dw));
					Assert.AreEqual(okSingleWhite, FastFloat.TryParseSingle(span, out var fw, allowLeadingWhitespace: true));
					Assert.AreEqual(BitConverter.SingleToInt32Bits(expectedSingleWhite), BitConverter.SingleToInt32Bits(fw));
				}
			}
		}
		finally
		{
			VirtualFree(block, 0, MemRelease);
		}
	}

	private static Span<byte> NativeSpan(IntPtr address, int length) =>
		MemoryMarshal.CreateSpan(ref Unsafe.AddByteOffset(ref Unsafe.NullRef<byte>(), address), length);

	private const uint MemCommit = 0x1000, MemReserve = 0x2000, MemRelease = 0x8000, PageNoAccess = 0x01, PageReadWrite = 0x04;

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr VirtualAlloc(IntPtr address, nuint size, uint allocationType, uint protect);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool VirtualProtect(IntPtr address, nuint size, uint newProtect, out uint oldProtect);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool VirtualFree(IntPtr address, nuint size, uint freeType);

	[TestMethod]
	public void PowersOfTen()
	{
		var failures = new Failures();
		for (var k = -360; k <= 360; k++)
		{
			foreach (var s in new[]
			{
				$"1e{k}", $"1E{k}", $"5e{k}", $"9.999999999999999e{k}", $"1.0000000000000001e{k}", $"1.000000001e{k}",
				$"123456789e{k}", $"9999999999999999999e{k}", $"1000000000000000000e{k}", $"4.4501477170144023e{k}",
				k >= 0 ? "1" + new string('0', k) : "0." + new string('0', -k - 1) + "1",
			})
			{
				var utf8 = Encoding.UTF8.GetBytes(s);
				CheckDouble(utf8, failures);
				CheckSingle(utf8, failures);
			}
		}

		failures.AssertNone(TestContext);
	}

	// ------------------------------------------------------------------------------------------------------------
	// Randomized tests (fixed seeds)
	// ------------------------------------------------------------------------------------------------------------

	[TestMethod]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	[DataRow(4)]
	public void Double_RandomBitPatterns_FormattedRoundTrip(int seed)
	{
		// 4 seeds x 500,000 random doubles, each formatted with R, G17, E16 and (for moderate magnitudes) one of the
		// fixed formats: about 7 million parses.
		var failures = new Failures();
		RunParallel(seed, 500_000, failures, (rng, f) =>
		{
			Span<byte> buffer = stackalloc byte[512];
			var d = BitConverter.Int64BitsToDouble(rng.NextInt64(long.MinValue, long.MaxValue));
			if (!double.IsFinite(d)) return;
			foreach (var format in RoundTripDoubleFormats)
			{
				Assert.IsTrue(d.TryFormat(buffer, out var n, format, Invariant));
				CheckDouble(buffer.Slice(0, n), f, expected: RoundTrips(format) ? d : null);
			}

			var abs = Math.Abs(d);
			if (abs < 1e25 && abs > 1e-25)
			{
				Assert.IsTrue(d.TryFormat(buffer, out var n, FixedFormats[rng.Next(FixedFormats.Length)], Invariant));
				CheckDouble(buffer.Slice(0, n), f);
			}
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow(11)]
	[DataRow(12)]
	public void Double_RandomTypicalValues(int seed)
	{
		// Values as they appear in CSV files: modest magnitudes, a few decimals, assorted formats.
		var failures = new Failures();
		RunParallel(seed, 250_000, failures, (rng, f) =>
		{
			Span<byte> buffer = stackalloc byte[512];
			var d = TypicalDouble(rng);
			var format = TypicalFormats[rng.Next(TypicalFormats.Length)];
			Assert.IsTrue(d.TryFormat(buffer, out var n, format, Invariant));
			CheckDouble(buffer.Slice(0, n), f);
			Assert.IsTrue(d.TryFormat(buffer, out n, "R", Invariant));
			CheckDouble(buffer.Slice(0, n), f);
			Assert.IsTrue(d.TryFormat(buffer, out n, "G17", Invariant));
			CheckDouble(buffer.Slice(0, n), f, expected: d);
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow(21)]
	[DataRow(22)]
	public void Double_RandomDigitStrings(int seed)
	{
		var failures = new Failures();
		RunParallel(seed, 500_000, failures, (rng, f) =>
		{
			Span<byte> buffer = stackalloc byte[128];
			var n = RandomNumberString(rng, buffer, maxDigits: 40, maxExponent: 360);
			CheckDouble(buffer.Slice(0, n), f);
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow(31)]
	public void Double_RandomMutations(int seed)
	{
		// Valid numbers with one character inserted, replaced or deleted: the syntax oracle decides what is valid.
		var failures = new Failures();
		RunParallel(seed, 300_000, failures, (rng, f) =>
		{
			Span<byte> buffer = stackalloc byte[128];
			var n = RandomNumberString(rng, buffer, maxDigits: 22, maxExponent: 330);
			n = Mutate(rng, buffer, n);
			CheckDouble(buffer.Slice(0, n), f);
			CheckSingle(buffer.Slice(0, n), f);
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	public void Double_NearPowersOfTwo()
	{
		var failures = new Failures();
		Parallel.For(-1074, 1024, static () => new Failures(), (k, _, local) =>
		{
			Span<byte> buffer = stackalloc byte[512];
			var p = Math.ScaleB(1.0, k);
			ReadOnlySpan<double> nearby = [Math.BitDecrement(p), p, Math.BitIncrement(p)];
			foreach (var d in nearby)
			{
				if (!double.IsFinite(d) || d <= 0) continue;
				foreach (var format in RoundTripDoubleFormats)
				{
					for (var sign = 0; sign < 2; sign++)
					{
						var v = sign == 0 ? d : -d;
						Assert.IsTrue(v.TryFormat(buffer, out var n, format, Invariant));
						CheckDouble(buffer.Slice(0, n), local, expected: RoundTrips(format) ? v : null);
					}
				}
			}

			var rng = new Random(k + 5000);
			if (p > double.Epsilon) CheckDoubleHalfway(Math.BitDecrement(p), rng, local);
			CheckDoubleHalfway(p, rng, local);
			return local;
		}, failures.Merge);
		CheckDoubleHalfway(double.MaxValue, new Random(1), failures);
		CheckDoubleHalfway(0.0, new Random(3), failures);
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow(41)]
	[DataRow(42)]
	public void Double_NearHalfwayPoints(int seed)
	{
		var failures = new Failures();
		RunParallel(seed, 12_000, failures, (rng, f) =>
		{
			// Random positive finite doubles, biased towards the normal range.
			var bits = (ulong)rng.NextInt64(1, 0x7FEFFFFFFFFFFFFF);
			if (rng.Next(4) == 0) bits = (bits & 0x000FFFFFFFFFFFFF) | ((ulong)rng.Next(1000, 1100) << 52); // ~[1e-7, 1e23]
			CheckDoubleHalfway(BitConverter.UInt64BitsToDouble(bits), rng, f);
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	public void Double_ExactTiesRoundToEven()
	{
		// Ties w * 10^q with w < 10^19 exist only for small |q|; build them from (2m + 1) * 2^(e - 1).
		var failures = new Failures();
		var rng = new Random(51);
		var found = 0;
		Span<byte> buffer = stackalloc byte[256];
		for (var n = 0; n < 200_000; n++)
		{
			var m = (1UL << 52) | ((ulong)rng.NextInt64() & 0x000FFFFFFFFFFFFF);
			var e = rng.Next(-80, 80);
			if (rng.Next(2) == 0)
			{
				// Make 2m + 1 divisible by 5^j so that the tie has trailing decimal zeros (q > 0).
				var j = rng.Next(1, 24);
				var five = BigInteger.Pow(5, j);
				var odd = (BigInteger)(2 * m + 1);
				odd -= odd % (2 * five);
				odd += five;
				if (odd < (BigInteger)(1UL << 53) || odd >= (BigInteger)(1UL << 54)) continue;
				m = (ulong)((odd - 1) / 2);
			}

			(var digits, var exp10) = ExactDecimal(2 * (BigInteger)m + 1, e - 1);
			(digits, exp10) = StripTrailingZeros(digits, exp10);
			if (digits.Length > 19) continue;
			found++;
			var lo = Math.ScaleB((double)m, e);
			var hi = Math.ScaleB((double)(m + 1), e);
			if (!double.IsFinite(hi) || lo < 2.2250738585072014E-308) continue;
			var even = (m & 1) == 0 ? lo : hi;
			var written = Encoding.UTF8.GetBytes(FormatDecimal(digits, exp10, rng.Next(3)), buffer);
			var utf8 = buffer.Slice(0, written);
			if (!CheckDouble(utf8, failures)) Assert.Fail($"tie \"{Encoding.UTF8.GetString(utf8)}\" not accepted");
			Assert.IsTrue(FastFloat.TryParseDouble(utf8, out var actual));
			Assert.AreEqual(BitConverter.DoubleToInt64Bits(even), BitConverter.DoubleToInt64Bits(actual));
		}

		Assert.IsTrue(found > 10_000, $"only {found} ties generated");
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow(101)]
	[DataRow(102)]
	public void Single_RandomBitPatterns_FormattedRoundTrip(int seed)
	{
		var failures = new Failures();
		RunParallel(seed, 200_000, failures, (rng, f) =>
		{
			Span<byte> buffer = stackalloc byte[512];
			var x = BitConverter.Int32BitsToSingle(rng.Next(int.MinValue, int.MaxValue));
			if (!float.IsFinite(x)) return;
			foreach (var format in RoundTripSingleFormats)
			{
				Assert.IsTrue(x.TryFormat(buffer, out var n, format, Invariant));
				CheckSingle(buffer.Slice(0, n), f, expected: RoundTrips(format) ? x : null);
			}

			// Also the double formatting of the float, and of random doubles: forces real rounding to float.
			Assert.IsTrue(((double)x).TryFormat(buffer, out var m, "R", Invariant));
			CheckSingle(buffer.Slice(0, m), f, expected: x);
			var d = BitConverter.Int64BitsToDouble(rng.NextInt64(0x3600000000000000, 0x4800000000000000));
			Assert.IsTrue(d.TryFormat(buffer, out m, rng.Next(2) == 0 ? "R" : "E16", Invariant));
			CheckSingle(buffer.Slice(0, m), f);
			var abs = Math.Abs(x);
			if (abs < 1e20f && abs > 1e-20f)
			{
				Assert.IsTrue(x.TryFormat(buffer, out var n, FixedFormats[rng.Next(FixedFormats.Length)], Invariant));
				CheckSingle(buffer.Slice(0, n), f);
			}
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	public void Single_StridedSweepOfAllBitPatterns()
	{
		// Every 997th float bit pattern (about 4.3 million values) formatted shortest and with 9 digits.
		var failures = new Failures();
		const long Stride = 997;
		const int Chunks = 64;
		var total = (1L << 32) / Stride + 1;
		Parallel.For(0, Chunks, static () => new Failures(), (chunk, _, local) =>
		{
			Span<byte> buffer = stackalloc byte[128];
			for (long index = chunk; index < total; index += Chunks)
			{
				var x = BitConverter.UInt32BitsToSingle((uint)(index * Stride));
				if (!float.IsFinite(x)) continue;
				Assert.IsTrue(x.TryFormat(buffer, out var n, "R", Invariant));
				CheckSingle(buffer.Slice(0, n), local);
				Assert.IsTrue(x.TryFormat(buffer, out n, "E8", Invariant));
				CheckSingle(buffer.Slice(0, n), local, expected: x);
			}
			return local;
		}, failures.Merge);
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow(111)]
	public void Single_RandomTypicalValues(int seed)
	{
		var failures = new Failures();
		RunParallel(seed, 300_000, failures, (rng, f) =>
		{
			Span<byte> buffer = stackalloc byte[512];
			var d = TypicalDouble(rng);
			var format = TypicalFormats[rng.Next(TypicalFormats.Length)];
			Assert.IsTrue(d.TryFormat(buffer, out var n, format, Invariant));
			CheckSingle(buffer.Slice(0, n), f);
			var x = (float)d;
			Assert.IsTrue(x.TryFormat(buffer, out n, "R", Invariant));
			CheckSingle(buffer.Slice(0, n), f);
			Assert.IsTrue(x.TryFormat(buffer, out n, "G9", Invariant));
			CheckSingle(buffer.Slice(0, n), f, expected: x);
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow(121)]
	[DataRow(122)]
	public void Single_RandomDigitStrings(int seed)
	{
		var failures = new Failures();
		RunParallel(seed, 500_000, failures, (rng, f) =>
		{
			Span<byte> buffer = stackalloc byte[128];
			var n = RandomNumberString(rng, buffer, maxDigits: 40, maxExponent: 70);
			CheckSingle(buffer.Slice(0, n), f);
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	public void Single_NearPowersOfTwo()
	{
		var failures = new Failures();
		Parallel.For(-149, 128, static () => new Failures(), (k, _, local) =>
		{
			Span<byte> buffer = stackalloc byte[512];
			var p = MathF.ScaleB(1.0f, k);
			ReadOnlySpan<float> nearby = [MathF.BitDecrement(p), p, MathF.BitIncrement(p)];
			foreach (var x in nearby)
			{
				if (!float.IsFinite(x) || x <= 0) continue;
				foreach (var format in RoundTripSingleFormats)
				{
					for (var sign = 0; sign < 2; sign++)
					{
						var v = sign == 0 ? x : -x;
						Assert.IsTrue(v.TryFormat(buffer, out var n, format, Invariant));
						CheckSingle(buffer.Slice(0, n), local, expected: RoundTrips(format) ? v : null);
					}
				}
			}

			var rng = new Random(k + 7000);
			if (p > float.Epsilon) CheckSingleHalfway(MathF.BitDecrement(p), rng, local);
			CheckSingleHalfway(p, rng, local);
			return local;
		}, failures.Merge);
		CheckSingleHalfway(float.MaxValue, new Random(2), failures);
		CheckSingleHalfway(0.0f, new Random(4), failures);
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	[DataRow(141)]
	public void Single_NearHalfwayPoints(int seed)
	{
		var failures = new Failures();
		RunParallel(seed, 40_000, failures, (rng, f) =>
		{
			var bits = (uint)rng.Next(1, 0x7F7FFFFF);
			CheckSingleHalfway(BitConverter.UInt32BitsToSingle(bits), rng, f);
		});
		failures.AssertNone(TestContext);
	}

	[TestMethod]
	public void Single_ExactTiesRoundToEven()
	{
		var failures = new Failures();
		var rng = new Random(151);
		var found = 0;
		Span<byte> buffer = stackalloc byte[256];
		for (var n = 0; n < 200_000; n++)
		{
			var m = (1U << 23) | ((uint)rng.Next() & 0x7FFFFF);
			var e = rng.Next(-70, 40);
			if (rng.Next(2) == 0)
			{
				var j = rng.Next(1, 11);
				var five = BigInteger.Pow(5, j);
				var odd = (BigInteger)(2 * m + 1);
				odd -= odd % (2 * five);
				odd += five;
				if (odd < (1 << 24) || odd >= (1 << 25)) continue;
				m = (uint)((odd - 1) / 2);
			}

			(var digits, var exp10) = ExactDecimal(2 * (BigInteger)m + 1, e - 1);
			(digits, exp10) = StripTrailingZeros(digits, exp10);
			if (digits.Length > 19) continue;
			var lo = MathF.ScaleB(m, e);
			var hi = MathF.ScaleB(m + 1, e);
			if (!float.IsFinite(hi) || lo < 1.17549435E-38f) continue;
			found++;
			var even = (m & 1) == 0 ? lo : hi;
			var written = Encoding.UTF8.GetBytes(FormatDecimal(digits, exp10, rng.Next(3)), buffer);
			var utf8 = buffer.Slice(0, written);
			if (!CheckSingle(utf8, failures)) Assert.Fail($"tie \"{Encoding.UTF8.GetString(utf8)}\" not accepted");
			Assert.IsTrue(FastFloat.TryParseSingle(utf8, out var actual));
			Assert.AreEqual(BitConverter.SingleToInt32Bits(even), BitConverter.SingleToInt32Bits(actual));
		}

		Assert.IsTrue(found > 10_000, $"only {found} ties generated");
		failures.AssertNone(TestContext);
	}

	// ------------------------------------------------------------------------------------------------------------
	// Helpers
	// ------------------------------------------------------------------------------------------------------------

	[TestMethod]
	public void FailureStatistics_MergeCountsAndBoundMessages()
	{
		var aggregate = new Failures();
		Parallel.For(0, 32, chunk =>
		{
			var local = new Failures { Checked = 10, Accepted = 8 };
			for (var i = 0; i < 3; i++) local.Add($"failure-{chunk}-{i}");
			aggregate.Merge(local);
		});
		Assert.AreEqual(320L, aggregate.Checked);
		Assert.AreEqual(256L, aggregate.Accepted);
		var error = Assert.ThrowsExactly<AssertFailedException>(() => aggregate.AssertNone());
		StringAssert.Contains(error.Message, "96 failure(s)");
		Assert.AreEqual(20, error.Message.Split("failure-", StringSplitOptions.None).Length - 1);
	}

	private static readonly string[] RoundTripDoubleFormats = ["R", "G17", "E16"];
	private static readonly string[] RoundTripSingleFormats = ["R", "G9", "E8"];

	/// <summary>
	/// 17 (9) significant digits always round-trip. "R" is compared with double.Parse only: the .NET 10 shortest
	/// formatting is off by one ulp just at some powers of two (e.g. 2^-25 formats as 2.980232238769531E-08, which
	/// parses to the double below).
	/// </summary>
	private static bool RoundTrips(string format) => format != "R";
	private static readonly string[] FixedFormats = ["F0", "F1", "F2", "F3", "F5", "F8", "F10", "F15", "F17", "F20", "F25", "N2"];
	private static readonly string[] TypicalFormats =
	[
		"R", "G", "G3", "G5", "G6", "G7", "G8", "G9", "G10", "G12", "G15", "G17", "F0", "F1", "F2", "F3", "F4", "F6",
		"F9", "E", "E3", "E6", "E10", "E16", "0.###", "0.000000", "0.##########e+00", "#.#####",
	];

	/// <summary>Worker-local statistics and bounded failures, merged after parallel checks.</summary>
	private sealed class Failures
	{
		private readonly ConcurrentQueue<string> messages = new();
		private int count;
		public long Checked;
		public long Accepted;

		public void Add(string message)
		{
			if (Interlocked.Increment(ref count) <= 20) messages.Enqueue(message);
		}

		public void Merge(Failures other)
		{
			Interlocked.Add(ref Checked, other.Checked);
			Interlocked.Add(ref Accepted, other.Accepted);
			var previous = Interlocked.Add(ref count, other.count) - other.count;
			foreach (var message in other.messages)
			{
				if (previous++ >= 20) break;
				messages.Enqueue(message);
			}
		}

		public void AssertNone(TestContext? output = null)
		{
			output?.WriteLine($"{Checked:N0} inputs checked, {Accepted:N0} accepted by FastFloat ({100.0 * Accepted / Math.Max(1, Checked):F3}%), the rest left to the BCL");
			var count = Volatile.Read(ref this.count);
			if (count != 0)
				Assert.Fail($"{count} failure(s); the first ones:{Environment.NewLine}{string.Join(Environment.NewLine, messages)}");
		}
	}

	/// <summary>Runs <paramref name="body"/> <paramref name="iterations"/> times on deterministic per-chunk generators.</summary>
	private static void RunParallel(int seed, int iterations, Failures failures, Action<Random, Failures> body)
	{
		const int Chunks = 32;
		Parallel.For(0, Chunks, static () => new Failures(), (chunk, _, local) =>
		{
			var rng = new Random(seed * 1000 + chunk);
			var count = iterations / Chunks + (chunk < iterations % Chunks ? 1 : 0);
			for (var i = 0; i < count; i++) body(rng, local);
			return local;
		}, failures.Merge);
	}

	private static string Show(ReadOnlySpan<byte> s) =>
		s.Length <= 160 ? Encoding.UTF8.GetString(s)
		: $"{Encoding.UTF8.GetString(s.Slice(0, 80))}...{Encoding.UTF8.GetString(s.Slice(s.Length - 60))} ({s.Length} bytes)";

	/// <summary>
	/// Checks FastFloat.TryParseDouble on <paramref name="s"/>: when it returns true the bits must equal double.Parse
	/// (and <paramref name="expected"/> if given); it must accept every well-formed input with at most 19 significant
	/// digits and reject everything outside its grammar. Returns whether it accepted the input.
	/// </summary>
	private static bool CheckDouble(ReadOnlySpan<byte> s, Failures failures, double? expected = null)
	{
		var significant = SignificantDigits(s);
		var special = significant < 0 && IsSpecial(s);
		var ok = FastFloat.TryParseDouble(s, out var actual);
		Span<char> buffer = s.Length <= 1024 ? stackalloc char[s.Length] : new char[s.Length];
		var utf16 = buffer.Slice(0, Encoding.UTF8.GetChars(s, buffer));
		var ok16 = FastFloat.TryParseDouble(utf16, out var actual16);
		if (ok16 != ok || BitConverter.DoubleToInt64Bits(actual16) != BitConverter.DoubleToInt64Bits(actual))
			failures.Add($"double \"{Show(s)}\": UTF-8 and UTF-16 results differ");
		if (FastFloat.TryParseDouble(Padded(utf16), out var padded16) != ok16 || BitConverter.DoubleToInt64Bits(padded16) != BitConverter.DoubleToInt64Bits(actual16))
			failures.Add($"double \"{Show(s)}\": UTF-16 result differs when surrounded by digits");
		if (ok16 && (!double.TryParse(utf16, Styles, Invariant, out var bcl16) || BitConverter.DoubleToInt64Bits(actual16) != BitConverter.DoubleToInt64Bits(bcl16)))
			failures.Add($"double \"{Show(s)}\": UTF-16 result differs from double.Parse");
		// The same input surrounded by digits must give the same answer: nothing outside the span may be used.
		if (FastFloat.TryParseDouble(Padded(s), out var padded) != ok || BitConverter.DoubleToInt64Bits(padded) != BitConverter.DoubleToInt64Bits(actual))
			failures.Add($"double \"{Show(s)}\": different result when the input is surrounded by digits");
		failures.Checked++;
		if (ok)
		{
			failures.Accepted++;
			if (significant < 0 && !special)
				failures.Add($"double \"{Show(s)}\": accepted input outside the supported grammar");
			if (!double.TryParse(s, Styles, Invariant, out var bcl))
				failures.Add($"double \"{Show(s)}\": returned {actual:R} but double.Parse rejects the input");
			else if (BitConverter.DoubleToInt64Bits(actual) != BitConverter.DoubleToInt64Bits(bcl))
				failures.Add($"double \"{Show(s)}\": 0x{BitConverter.DoubleToInt64Bits(actual):X16} ({actual:R}) but double.Parse gives 0x{BitConverter.DoubleToInt64Bits(bcl):X16} ({bcl:R})");
			if (expected is double e && BitConverter.DoubleToInt64Bits(actual) != BitConverter.DoubleToInt64Bits(e))
				failures.Add($"double \"{Show(s)}\": {actual:R} but the formatted value was {e:R}");
		}
		else if ((significant >= 0 && significant <= 19) || special)
		{
			failures.Add($"double \"{Show(s)}\": returned false for a well-formed number with {significant} significant digits");
		}

		return ok;
	}

	/// <summary>Same as <see cref="CheckDouble"/> for FastFloat.TryParseSingle against float.Parse.</summary>
	private static bool CheckSingle(ReadOnlySpan<byte> s, Failures failures, float? expected = null)
	{
		var significant = SignificantDigits(s);
		var special = significant < 0 && IsSpecial(s);
		var ok = FastFloat.TryParseSingle(s, out var actual);
		Span<char> buffer = s.Length <= 1024 ? stackalloc char[s.Length] : new char[s.Length];
		var utf16 = buffer.Slice(0, Encoding.UTF8.GetChars(s, buffer));
		var ok16 = FastFloat.TryParseSingle(utf16, out var actual16);
		if (ok16 != ok || BitConverter.SingleToInt32Bits(actual16) != BitConverter.SingleToInt32Bits(actual))
			failures.Add($"float \"{Show(s)}\": UTF-8 and UTF-16 results differ");
		if (FastFloat.TryParseSingle(Padded(utf16), out var padded16) != ok16 || BitConverter.SingleToInt32Bits(padded16) != BitConverter.SingleToInt32Bits(actual16))
			failures.Add($"float \"{Show(s)}\": UTF-16 result differs when surrounded by digits");
		if (ok16 && (!float.TryParse(utf16, Styles, Invariant, out var bcl16) || BitConverter.SingleToInt32Bits(actual16) != BitConverter.SingleToInt32Bits(bcl16)))
			failures.Add($"float \"{Show(s)}\": UTF-16 result differs from float.Parse");
		if (FastFloat.TryParseSingle(Padded(s), out var padded) != ok || BitConverter.SingleToInt32Bits(padded) != BitConverter.SingleToInt32Bits(actual))
			failures.Add($"float \"{Show(s)}\": different result when the input is surrounded by digits");
		failures.Checked++;
		if (ok)
		{
			failures.Accepted++;
			if (significant < 0 && !special)
				failures.Add($"float \"{Show(s)}\": accepted input outside the supported grammar");
			if (!float.TryParse(s, Styles, Invariant, out var bcl))
				failures.Add($"float \"{Show(s)}\": returned {actual:R} but float.Parse rejects the input");
			else if (BitConverter.SingleToInt32Bits(actual) != BitConverter.SingleToInt32Bits(bcl))
				failures.Add($"float \"{Show(s)}\": 0x{BitConverter.SingleToInt32Bits(actual):X8} ({actual:R}) but float.Parse gives 0x{BitConverter.SingleToInt32Bits(bcl):X8} ({bcl:R})");
			if (expected is float e && BitConverter.SingleToInt32Bits(actual) != BitConverter.SingleToInt32Bits(e))
				failures.Add($"float \"{Show(s)}\": {actual:R} but the formatted value was {e:R}");
		}
		else if ((significant >= 0 && significant <= 19) || special)
		{
			failures.Add($"float \"{Show(s)}\": returned false for a well-formed number with {significant} significant digits");
		}

		return ok;
	}

	[ThreadStatic]
	private static byte[]? padded;
	[ThreadStatic]
	private static char[]? paddedChars;

	/// <summary>A copy of <paramref name="s"/> with sixteen '9' bytes on either side.</summary>
	private static ReadOnlySpan<byte> Padded(ReadOnlySpan<byte> s)
	{
		var buffer = padded ??= new byte[1024];
		if (buffer.Length < s.Length + 32) padded = buffer = new byte[s.Length + 32];
		buffer.AsSpan(0, s.Length + 32).Fill((byte)'9');
		s.CopyTo(buffer.AsSpan(16));

		return buffer.AsSpan(16, s.Length);
	}

	private static ReadOnlySpan<char> Padded(ReadOnlySpan<char> s)
	{
		var buffer = paddedChars ??= new char[1024];
		if (buffer.Length < s.Length + 32) paddedChars = buffer = new char[s.Length + 32];
		buffer.AsSpan(0, s.Length + 32).Fill('9');
		s.CopyTo(buffer.AsSpan(16));
		return buffer.AsSpan(16, s.Length);
	}

	/// <summary>
	/// Independent oracle for the grammar <c>[+-]digits[.digits][(e|E)[+-]digits]</c> with at least one mantissa
	/// digit: returns -1 if <paramref name="s"/> does not match, otherwise the number of mantissa digits from the
	/// first to the last non-zero one (0 for zero).
	/// </summary>
	private static int SignificantDigits(ReadOnlySpan<byte> s)
	{
		var i = 0;
		if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
		int digits = 0, firstNonZero = -1, lastNonZero = -1;
		var point = false;
		for (; i < s.Length; i++)
		{
			var c = s[i];
			if (c >= '0' && c <= '9')
			{
				if (c != '0')
				{
					if (firstNonZero < 0) firstNonZero = digits;
					lastNonZero = digits;
				}

				digits++;
			}
			else if (c == '.' && !point)
			{
				point = true;
			}
			else
			{
				break;
			}
		}

		if (digits == 0) return -1;
		if (i < s.Length)
		{
			if (s[i] != 'e' && s[i] != 'E') return -1;
			i++;
			if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
			var exponentDigits = 0;
			for (; i < s.Length && s[i] >= '0' && s[i] <= '9'; i++) exponentDigits++;
			if (exponentDigits == 0 || i != s.Length) return -1;
		}

		return firstNonZero < 0 ? 0 : lastNonZero - firstNonZero + 1;
	}

	/// <summary>The non-numeric inputs double.Parse accepts with the invariant culture.</summary>
	private static bool IsSpecial(ReadOnlySpan<byte> s)
	{
		if (s.Length != 0 && (s[0] == '+' || s[0] == '-')) s = s.Slice(1);

		return System.Text.Ascii.EqualsIgnoreCase(s, "infinity"u8) || System.Text.Ascii.EqualsIgnoreCase(s, "nan"u8);
	}

	/// <summary>
	/// A random well-formed number: optional sign, 1..maxDigits digits (sometimes biased towards 0, 9, 4/5 runs to
	/// sit near rounding boundaries), an optional decimal point anywhere, an optional exponent (sometimes huge or
	/// zero padded).
	/// </summary>
	private static int RandomNumberString(Random rng, Span<byte> buffer, int maxDigits, int maxExponent)
	{
		var n = 0;
		var r = rng.Next(8);
		if (r == 0) buffer[n++] = (byte)'-';
		else if (r == 1) buffer[n++] = (byte)'+';

		var digits = rng.Next(1, maxDigits + 1);
		var point = rng.Next(-1, digits + 1); // -1: no decimal point
		var mode = rng.Next(6);
		for (var k = 0; k < digits; k++)
		{
			if (k == point) buffer[n++] = (byte)'.';
			var d = mode switch
			{
				0 => rng.Next(4) == 0 ? rng.Next(10) : 0,       // mostly zeros
				1 => rng.Next(4) == 0 ? rng.Next(10) : 9,       // mostly nines
				2 => rng.Next(3) == 0 ? 5 : rng.Next(2) == 0 ? 4 : 9, // 4999.. / 5000.. patterns
				3 => k == 0 ? rng.Next(1, 10) : rng.Next(10),   // no leading zero
				_ => rng.Next(10),
			};
			buffer[n++] = (byte)('0' + d);
		}

		if (point == digits) buffer[n++] = (byte)'.';
		if (point >= 0 && digits == 0) buffer[n++] = (byte)'0';

		if (rng.Next(4) != 0)
		{
			buffer[n++] = rng.Next(2) == 0 ? (byte)'e' : (byte)'E';
			r = rng.Next(3);
			if (r == 0) buffer[n++] = (byte)'-';
			else if (r == 1) buffer[n++] = (byte)'+';
			var exponent = rng.Next(40) switch
			{
				0 => rng.NextInt64(0, long.MaxValue),
				1 => rng.Next(0, 3000),
				_ => rng.Next(0, maxExponent + 1),
			};
			var zeros = rng.Next(8) == 0 ? rng.Next(1, 20) : 0;
			for (var k = 0; k < zeros; k++) buffer[n++] = (byte)'0';
			Assert.IsTrue(exponent.TryFormat(buffer.Slice(n), out var written, default, Invariant));
			n += written;
		}

		return n;
	}

	private static ReadOnlySpan<byte> MutationCharacters => " ,.eE+-x\0_9015\t'"u8;

	/// <summary>Inserts, replaces or deletes one random character.</summary>
	private static int Mutate(Random rng, Span<byte> buffer, int n)
	{
		var at = rng.Next(n + 1);
		var c = MutationCharacters[rng.Next(MutationCharacters.Length)];
		switch (rng.Next(3))
		{
			case 0: // insert
				buffer.Slice(at, n - at).CopyTo(buffer.Slice(at + 1));
				buffer[at] = c;

				return n + 1;
			case 1: // replace
				if (at == n) return n;
				buffer[at] = c;

				return n;
			default: // delete
				if (at == n) return n;
				buffer.Slice(at + 1, n - at - 1).CopyTo(buffer.Slice(at));

				return n - 1;
		}
	}

	private static double TypicalDouble(Random rng)
	{
		var d = rng.Next(5) switch
		{
			0 => rng.NextDouble(),
			1 => rng.NextDouble() * Math.Pow(10, rng.Next(-10, 16)),
			2 => Math.Round(rng.NextDouble() * Math.Pow(10, rng.Next(0, 9)), rng.Next(0, 7)),
			3 => rng.Next(-1_000_000, 1_000_000) / Math.Pow(10, rng.Next(0, 8)),
			_ => (rng.NextDouble() - 0.5) * Math.Pow(10, rng.Next(-40, 40)),
		};

		return rng.Next(3) == 0 ? -d : d;
	}

	/// <summary>The exact decimal expansion of m * 2^e2 as digits * 10^exp10.</summary>
	private static (string Digits, int Exp10) ExactDecimal(BigInteger m, int e2) =>
		e2 >= 0
			? ((m << e2).ToString(CultureInfo.InvariantCulture), 0)
			: ((m * BigInteger.Pow(5, -e2)).ToString(CultureInfo.InvariantCulture), e2);

	private static (string Digits, int Exp10) StripTrailingZeros(string digits, int exp10)
	{
		var end = digits.Length;
		while (end > 1 && digits[end - 1] == '0') end--;

		return (digits.Substring(0, end), exp10 + digits.Length - end);
	}

	/// <summary>Formats digits * 10^exp10 as "digitsEexp" (style 0), scientific (1) or plain decimal (2).</summary>
	private static string FormatDecimal(string digits, int exp10, int style)
	{
		var len = digits.Length;
		switch (style)
		{
			case 0:
				return $"{digits}e{exp10}";
			case 1:
				return len == 1 ? $"{digits}E{exp10}" : $"{digits[0]}.{digits.AsSpan(1)}E{exp10 + len - 1:+0;-0}";
			default:
				if (exp10 >= 0) return digits + new string('0', exp10);
				if (-exp10 < len) return $"{digits.AsSpan(0, len + exp10)}.{digits.AsSpan(len + exp10)}";

				return $"0.{new string('0', -exp10 - len)}{digits}";
		}
	}

	/// <summary>
	/// Checks decimal strings at, just below and just above the midpoint between <paramref name="lo"/> and the next
	/// double up, exact and truncated to various lengths (and rounded up by one unit in the last digit).
	/// </summary>
	private static void CheckDoubleHalfway(double lo, Random rng, Failures failures)
	{
		// The longest exact binary64 midpoint needs fewer than 1,100 ASCII characters.
		Span<byte> buffer = stackalloc byte[2048];
		buffer[0] = (byte)'-';
		var bits = BitConverter.DoubleToUInt64Bits(lo);
		var biased = (int)(bits >> 52) & 0x7FF;
		var fraction = bits & 0x000FFFFFFFFFFFFF;
		BigInteger m = biased == 0 ? fraction : fraction | (1UL << 52);
		var e = biased == 0 ? -1074 : biased - 1075;
		foreach (var s in MidpointStrings(2 * m + 1, e - 1, rng))
		{
			var written = Encoding.UTF8.GetBytes(s, buffer.Slice(1));
			var utf8 = buffer.Slice(1, written);
			CheckDouble(utf8, failures);
			CheckDouble(buffer.Slice(0, written + 1), failures);
		}
	}

	private static void CheckSingleHalfway(float lo, Random rng, Failures failures)
	{
		Span<byte> buffer = stackalloc byte[512];
		buffer[0] = (byte)'-';
		var bits = BitConverter.SingleToUInt32Bits(lo);
		var biased = (int)(bits >> 23) & 0xFF;
		var fraction = bits & 0x7FFFFF;
		BigInteger m = biased == 0 ? fraction : fraction | (1U << 23);
		var e = biased == 0 ? -149 : biased - 150;
		foreach (var s in MidpointStrings(2 * m + 1, e - 1, rng))
		{
			var written = Encoding.UTF8.GetBytes(s, buffer.Slice(1));
			var utf8 = buffer.Slice(1, written);
			CheckSingle(utf8, failures);
			CheckSingle(buffer.Slice(0, written + 1), failures);
		}
	}

	private static IEnumerable<string> MidpointStrings(BigInteger odd, int e2, Random rng)
	{
		var exact = ExactDecimal(odd, e2);
		(var digits, var exp10) = StripTrailingZeros(exact.Digits, exact.Exp10);
		var len = digits.Length;
		yield return FormatDecimal(digits, exp10, rng.Next(3));
		yield return FormatDecimal(digits + "00001", exp10 - 5, rng.Next(3)); // just above
		var below = BigInteger.Parse(digits, CultureInfo.InvariantCulture) * 100000 - 1; // just below
		yield return FormatDecimal(below.ToString(CultureInfo.InvariantCulture), exp10 - 5, rng.Next(3));
		foreach (var n in MidpointDigitLengths)
		{
			if (n >= len) break;
			var prefix = digits.Substring(0, n);
			yield return FormatDecimal(prefix, exp10 + len - n, rng.Next(3));
			var up = (BigInteger.Parse(prefix, CultureInfo.InvariantCulture) + 1).ToString(CultureInfo.InvariantCulture);
			yield return FormatDecimal(up, exp10 + len - n, rng.Next(3));
		}
	}
	private static readonly int[] MidpointDigitLengths = [15, 16, 17, 18, 19, 20, 21, 25, 30];
}

public sealed class FastFloatTheoryData<T> : List<object[]>
	where T : notnull
{
	public void Add(T value) => base.Add([value]);
}
