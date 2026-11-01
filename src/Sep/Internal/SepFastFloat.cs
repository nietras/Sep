// Derived from CsvHelper's FastFloat implementation and fast_float.
// Modified for Sep; CsvHelper-derived portions use Apache-2.0.
// Original licenses are in src/Sep/ThirdParty.
// CsvHelper source: https://github.com/JoshClose/CsvHelper/tree/dffe5e9a634e1894d334b6a69599d8b489c4fc78
// fast_float: https://github.com/fastfloat/fast_float
using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace nietras.SeparatedValues;

/// <summary>
/// Fast, exactly rounded parsing of decimal floating point numbers from UTF-8 or UTF-16: a port of Daniel Lemire's fast_float
/// (Clinger's fast path followed by the Eisel-Lemire algorithm, see "Number Parsing at a Gigabyte per Second" and
/// "Fast Number Parsing Without Fallback"). Results are rounded to nearest, ties to even, including -0, subnormals,
/// overflow to infinity, and underflow to zero.
/// It returns false, leaving the input to the BCL, for anything outside <c>[+-]digits[.digits][(e|E)[+-]digits]</c>
/// (whitespace, thousands separators, hex, "inf", ...) other than Infinity/NaN and optional leading ASCII whitespace,
/// and for the rare inputs with more than
/// 19 significant digits whose truncation to 19 digits sits right on a rounding boundary.
/// </summary>
internal static partial class FastFloat
{
	// binary64 parameters (see fast_float's binary_format<double>).
	private const int DoubleMantissaBits = 52;
	private const int DoubleMinimumExponent = -1023;
	private const int DoubleMinExponentRoundToEven = -4;
	private const int DoubleMaxExponentRoundToEven = 23;
	private const int DoubleSmallestPowerOfTen = -342;
	private const int DoubleLargestPowerOfTen = 308;
	private const ulong DoubleInfinityBits = 0x7FF0000000000000UL;

	// binary32 parameters (see fast_float's binary_format<float>).
	private const int SingleMantissaBits = 23;
	private const int SingleMinimumExponent = -127;
	private const int SingleMinExponentRoundToEven = -17;
	private const int SingleMaxExponentRoundToEven = 10;
	private const int SingleSmallestPowerOfTen = -64;
	private const int SingleLargestPowerOfTen = 38;
	private const ulong SingleInfinityBits = 0x7F800000UL;

	/// <summary>Returned by the Eisel-Lemire routines when the result cannot be decided (never a valid result).</summary>
	private const ulong Undecided = ulong.MaxValue;

	/// <summary>Exactly representable powers of ten for Clinger's fast path: 10^0..10^22.</summary>
	private static ReadOnlySpan<double> DoublePowersOfTen =>
	[
		1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9, 1e10, 1e11,
		1e12, 1e13, 1e14, 1e15, 1e16, 1e17, 1e18, 1e19, 1e20, 1e21, 1e22,
	];

	private static ReadOnlySpan<ulong> UInt64PowersOfTen => [1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseDouble(ReadOnlySpan<byte> s, out double value, char decimalSeparator = '.',
		bool allowSpecial = true, bool allowLeadingWhitespace = false) =>
		TryParse<byte, double>(s, out value, decimalSeparator, allowSpecial, allowLeadingWhitespace);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseDouble(ReadOnlySpan<char> s, out double value, char decimalSeparator = '.',
		bool allowSpecial = true, bool allowLeadingWhitespace = false) =>
		TryParse<char, double>(s, out value, decimalSeparator, allowSpecial, allowLeadingWhitespace);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseSingle(ReadOnlySpan<byte> s, out float value, char decimalSeparator = '.',
		bool allowSpecial = true, bool allowLeadingWhitespace = false) =>
		TryParse<byte, float>(s, out value, decimalSeparator, allowSpecial, allowLeadingWhitespace);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseSingle(ReadOnlySpan<char> s, out float value, char decimalSeparator = '.',
		bool allowSpecial = true, bool allowLeadingWhitespace = false) =>
		TryParse<char, float>(s, out value, decimalSeparator, allowSpecial, allowLeadingWhitespace);

	/// <summary>
	/// The hot path, specialized for byte/char input and double/float output. Parses
	/// <c>[+-]digits[.digits][(e|E)[+-]digits]</c> with at most 19 mantissa digits; everything else (invalid input,
	/// Infinity/NaN, longer mantissas) is handled separately.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryParse<TChar, T>(ReadOnlySpan<TChar> s, out T value, char decimalSeparator,
		bool allowSpecial, bool allowLeadingWhitespace)
		where TChar : unmanaged
		where T : struct
	{
		ref var p = ref MemoryMarshal.GetReference(s);
		nint len = s.Length;
		if (len == 0) goto Invalid;

		nint i = 0;
		var negative = false;
		uint c = CodeUnit(ref p, i);
		if (allowLeadingWhitespace && IsAsciiWhitespace(c))
		{
			do
			{
				if (++i == len) goto Invalid;
				c = CodeUnit(ref p, i);
			}
			while (IsAsciiWhitespace(c));
			s = s.Slice((int)i);
			p = ref MemoryMarshal.GetReference(s);
			len = s.Length;
			i = 0;
		}

		if (c == '-')
		{
			negative = true;
			i = 1;
		}
		else if (c == '+')
		{
			i = 1;
		}

		// Integer part (usually short: a plain loop is cheapest).
		var start = i;
		var w = 0UL;
		while (i < len)
		{
			var d = CodeUnit(ref p, i) - (ulong)'0';
			if (d > 9) break;
			w = w * 10 + d; // may wrap with more than 19 digits, which go to the slow path
			i++;
		}

		var digits = i - start;

		// Fraction: process digit runs in blocks where possible.
		var q = 0L;
		if (i < len && CodeUnit(ref p, i) == decimalSeparator)
		{
			i++;
			var fractionStart = i;
			if (len >= 8)
			{
				i = ParseDigitRun(ref p, i, len, ref w);
			}
			else
			{
				while (i < len)
				{
					var d = CodeUnit(ref p, i) - (ulong)'0';
					if (d > 9) break;
					w = w * 10 + d;
					i++;
				}
			}

			digits += i - fractionStart;
			q = fractionStart - i;
		}

		if (digits == 0)
		{
			if (allowSpecial) return TryParseSpecial(s, out value);
			goto Invalid;
		}

		if (i < len)
		{
			if ((CodeUnit(ref p, i) | 0x20) != 'e') goto Invalid;
			i++;
			var negativeExponent = false;
			if (i < len)
			{
				c = CodeUnit(ref p, i);
				if (c == '-')
				{
					negativeExponent = true;
					i++;
				}
				else if (c == '+')
				{
					i++;
				}
			}

			if (i >= len) goto Invalid;
			var e = 0L;
			do
			{
				var d = CodeUnit(ref p, i) - (ulong)'0';
				if (d > 9) goto Invalid;
				if (e < 0x10000000) e = e * 10 + (long)d; // saturate: anything this large is zero or infinity anyway
				i++;
			}
			while (i < len);
			q += negativeExponent ? -e : e;
		}

		if (digits > 19) return TryParseLong(s, start, digits, q, decimalSeparator, negative, out value);

		// Clinger's fast path: w and 10^|q| are exact doubles, so a single correctly rounded operation gives the
		// correctly rounded double.
		if (w <= (1UL << 53) && (ulong)(q + 22) <= 44)
		{
			double d = (long)w;
			ref var pow10 = ref MemoryMarshal.GetReference(DoublePowersOfTen);
			if (q < 0) d /= Unsafe.Add(ref pow10, (nint)(-q));
			else d *= Unsafe.Add(ref pow10, (nint)q);
			if (typeof(T) == typeof(double))
			{
				value = Unsafe.BitCast<double, T>(negative ? -d : d);

				return true;
			}

			// For float, rounding the double again gives the correctly rounded float unless the double landed
			// exactly on a midpoint between two floats (the exact value may lie on either side): leave those to
			// Eisel-Lemire. The result is always within the normal float range here.
			if ((BitConverter.DoubleToUInt64Bits(d) & 0x1FFFFFFF) != 0x10000000)
			{
				var f = (float)d;
				value = Unsafe.BitCast<float, T>(negative ? -f : f);

				return true;
			}
		}

		return Finish(typeof(T) == typeof(double) ? ComputeDouble(q, w) : ComputeSingle(q, w), negative, out value);

	Invalid:
		value = default;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint CodeUnit<TChar>(ref TChar p, nint i) where TChar : unmanaged
	{
		ref var c = ref Unsafe.Add(ref p, i);
		return typeof(TChar) == typeof(byte) ? Unsafe.As<TChar, byte>(ref c) : Unsafe.As<TChar, char>(ref c);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsAsciiWhitespace(uint c) => c == ' ' || c - '\t' <= '\r' - '\t';

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static nint ParseDigitRun<TChar>(ref TChar p, nint i, nint len, ref ulong m) where TChar : unmanaged =>
		typeof(TChar) == typeof(byte)
			? ParseDigitRun(ref Unsafe.As<TChar, byte>(ref p), i, len, ref m)
			: ParseDigitRun(ref Unsafe.As<TChar, char>(ref p), i, len, ref m);

	/// <summary>
	/// Accumulates the run of ASCII digits starting at <paramref name="i"/> into <paramref name="m"/> eight bytes at a
	/// time, without a branch per digit, and returns the index after the run. Requires len &gt;= 8; never reads
	/// outside [0, len).
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static nint ParseDigitRun(ref byte p, nint i, nint len, ref ulong m)
	{
		while (true)
		{
			var remaining = len - i;
			ulong v;
			if (remaining >= 8) v = ReadEightBytes(ref Unsafe.Add(ref p, i));
			// Near the end: the last eight bytes, shifted so that byte i comes first and the bytes past the end are 0.
			else if (remaining > 0) v = ReadEightBytes(ref Unsafe.Add(ref p, len - 8)) >> (int)(64 - 8 * remaining);
			else return i;
			// Digit values; a byte is a digit if its value is at most 9. Borrows and carries only travel upwards, so
			// the lowest non-digit byte is always detected correctly, which is all that is needed.
			var d = v - 0x3030303030303030UL;
			var nonDigits = ((d + 0x7676767676767676UL) | d) & 0x8080808080808080UL;
			if (nonDigits == 0)
			{
				m = m * 100000000 + ConvertEightDigits(d);
				i += 8;
				continue;
			}

			// n leading digits (0..7): shift them to the top bytes (the zeros below act as leading zeros).
			var n = BitOperations.TrailingZeroCount(nonDigits) >> 3;
			if (n != 0)
			{
				m = m * Unsafe.Add(ref MemoryMarshal.GetReference(UInt64PowersOfTen), n) + ConvertEightDigits(d << (64 - 8 * n));
				i += n;
			}

			return i;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static nint ParseDigitRun(ref char p, nint i, nint len, ref ulong m)
	{
		if (Vector128.IsHardwareAccelerated && BitConverter.IsLittleEndian)
		{
			while (len - i >= 8)
			{
				ref var first = ref Unsafe.As<char, ushort>(ref Unsafe.Add(ref p, i));
				var digits = Vector128.LoadUnsafe(ref first) - Vector128.Create((ushort)'0');
				if (Vector128.GreaterThanAny(digits, Vector128.Create((ushort)9))) break;
				var packed = Vector128.Narrow(digits, Vector128<ushort>.Zero).AsUInt64().GetElement(0);
				m = m * 100000000 + ConvertEightDigits(packed);
				i += 8;
			}
		}

		while (i < len)
		{
			var d = Unsafe.Add(ref p, i) - (ulong)'0';
			if (d > 9) break;
			m = m * 10 + d;
			i++;
		}

		return i;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong ReadEightBytes(ref byte p)
	{
		var v = Unsafe.ReadUnaligned<ulong>(ref p);

		return BitConverter.IsLittleEndian ? v : BinaryPrimitives.ReverseEndianness(v);
	}

	/// <summary>Converts eight digit values (0..9, first digit in the lowest byte) to their value (credit: aqrit).</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ConvertEightDigits(ulong d)
	{
		d = d * 10 + (d >> 8);
		d = ((d & 0x000000FF000000FFUL) * 0x000F424000000064UL + ((d >> 16) & 0x000000FF000000FFUL) * 0x0000271000000001UL) >> 32;

		return (uint)d;
	}

	/// <summary>Applies the sign to the unsigned IEEE bits of the result, or fails for <see cref="Undecided"/>.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool Finish<T>(ulong bits, bool negative, out T value) where T : struct
	{
		if (bits == Undecided)
		{
			value = default;

			return false;
		}

		if (typeof(T) == typeof(double))
		{
			if (negative) bits |= 1UL << 63;
			value = Unsafe.BitCast<ulong, T>(bits);
		}
		else
		{
			var b = (uint)bits;
			if (negative) b |= 1U << 31;
			value = Unsafe.BitCast<uint, T>(b);
		}

		return true;
	}

	/// <summary>
	/// The validated mantissa is truncated to 19 significant digits w; the result lies in [w, w + 1) * 10^q.
	/// When the bounds round differently, only an all-zero discarded suffix can be decided without the BCL.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool TryParseLong<TChar, T>(ReadOnlySpan<TChar> s, nint start, nint digits, long q,
		char decimalSeparator, bool negative, out T value)
		where TChar : unmanaged
		where T : struct
	{
		ref var p = ref MemoryMarshal.GetReference(s);
		var i = start;
		nint leading = 0;
		while (leading < digits)
		{
			var c = CodeUnit(ref p, i++);
			if (c == decimalSeparator) continue;
			if (c != '0') { i--; break; }
			leading++;
		}

		if (leading == digits) return Finish(0, negative, out value);

		var significant = digits - leading;
		var wanted = significant < 19 ? significant : 19;
		nint taken = 0;
		var w = 0UL;
		while (taken < wanted)
		{
			var end = i + wanted - taken;
			var next = ParseDigitRun(ref p, i, end, ref w);
			taken += next - i;
			i = next;
			if (taken < wanted) i++; // skip the already validated decimal separator
		}

		q += significant - taken;
		var bits = typeof(T) == typeof(double) ? ComputeDouble(q, w) : ComputeSingle(q, w);
		if (significant > taken &&
			bits != (typeof(T) == typeof(double) ? ComputeDouble(q, w + 1) : ComputeSingle(q, w + 1)))
		{
			for (; i < s.Length; i++)
			{
				var c = CodeUnit(ref p, i);
				if (c == decimalSeparator || c == '0') continue;
				if (c - '0' > 9) break; // exponent
				bits = Undecided;
				break;
			}
		}

		return Finish(bits, negative, out value);
	}

	/// <summary>
	/// The non-numeric values double.Parse and float.Parse accept with the invariant culture, case-insensitively and
	/// with an optional sign: Infinity and NaN (but not "inf").
	/// </summary>
	private static bool TryParseSpecial<TChar, T>(ReadOnlySpan<TChar> s, out T value)
		where TChar : unmanaged
		where T : struct
	{
		var negative = false;
		ref var p = ref MemoryMarshal.GetReference(s);
		if (s.Length != 0 && (CodeUnit(ref p, 0) == '-' || CodeUnit(ref p, 0) == '+'))
		{
			negative = CodeUnit(ref p, 0) == '-';
			s = s.Slice(1);
		}

		if (EqualsAsciiIgnoreCase(s, "Infinity"u8))
		{
			value = typeof(T) == typeof(double)
				? Unsafe.BitCast<double, T>(negative ? double.NegativeInfinity : double.PositiveInfinity)
				: Unsafe.BitCast<float, T>(negative ? float.NegativeInfinity : float.PositiveInfinity);

			return true;
		}

		if (EqualsAsciiIgnoreCase(s, "NaN"u8))
		{
			value = typeof(T) == typeof(double) ? Unsafe.BitCast<double, T>(double.NaN) : Unsafe.BitCast<float, T>(float.NaN);

			return true;
		}

		value = default;

		return false;
	}

	private static bool EqualsAsciiIgnoreCase<TChar>(ReadOnlySpan<TChar> s, ReadOnlySpan<byte> ascii)
		where TChar : unmanaged
	{
		if (s.Length != ascii.Length) return false;
		ref var p = ref MemoryMarshal.GetReference(s);
		for (var i = 0; i < ascii.Length; i++)
			if ((CodeUnit(ref p, i) | 0x20) != (ascii[i] | 0x20)) return false;
		return true;
	}

	/// <summary>
	/// Eisel-Lemire for binary64: the IEEE bits (without sign) of the correctly rounded w * 10^q, or
	/// <see cref="Undecided"/>. Port of fast_float's compute_float.
	/// </summary>
	private static ulong ComputeDouble(long q, ulong w)
	{
		if (w == 0 || q < DoubleSmallestPowerOfTen) return 0;
		if (q > DoubleLargestPowerOfTen) return DoubleInfinityBits;

		var lz = BitOperations.LeadingZeroCount(w);
		w <<= lz;
		ref var entry = ref Unsafe.Add(ref MemoryMarshal.GetReference(PowersOfFive), (nint)(q - SmallestPowerOfFive) * 2);
		// We need the top mantissa + 3 bits: implicit bit, rounding bit, and one that may be lost to normalization.
		var hi = MultiplyHigh(w, entry, out var lo);
		const ulong PrecisionMask = ulong.MaxValue >> (DoubleMantissaBits + 3);
		if ((hi & PrecisionMask) == PrecisionMask)
		{
			var hi2 = MultiplyHigh(w, Unsafe.Add(ref entry, 1));
			lo += hi2;
			if (hi2 > lo) hi++;
			// Proven unnecessary (Mushtak and Lemire, "Fast number parsing without fallback"); kept as a cheap guard.
			if (lo == ulong.MaxValue && (q < -27 || q > 55)) return Undecided;
		}

		var upperBit = (int)(hi >> 63);
		var shift = upperBit + 64 - DoubleMantissaBits - 3;
		var mantissa = hi >> shift;
		var power2 = Power((int)q) + upperBit - lz - DoubleMinimumExponent;
		if (power2 <= 0)
		{
			// Subnormal (or zero). Halfway ties cannot occur this far from 10^0, so round half up.
			if (-power2 + 1 >= 64) return 0;
			mantissa >>= -power2 + 1;
			mantissa += mantissa & 1;
			mantissa >>= 1;

			// Rounding up may carry into the exponent field and produce the smallest normal, which is fine.
			return mantissa;
		}

		// Exactly halfway between two doubles (only possible for small |q|): round to even.
		if (lo <= 1 && (ulong)(q - DoubleMinExponentRoundToEven) <= DoubleMaxExponentRoundToEven - DoubleMinExponentRoundToEven &&
			(mantissa & 3) == 1 && (mantissa << shift) == hi)
		{
			mantissa &= ~1UL;
		}

		mantissa += mantissa & 1;
		mantissa >>= 1;
		// mantissa is in [2^52, 2^53]: adding it carries the implicit bit (or a rounding overflow) into the exponent.
		var bits = mantissa + ((ulong)(power2 - 1) << DoubleMantissaBits);

		return bits >= DoubleInfinityBits ? DoubleInfinityBits : bits;
	}

	/// <summary>Eisel-Lemire for binary32: same as <see cref="ComputeDouble"/> with the float parameters.</summary>
	private static ulong ComputeSingle(long q, ulong w)
	{
		if (w == 0 || q < SingleSmallestPowerOfTen) return 0;
		if (q > SingleLargestPowerOfTen) return SingleInfinityBits;

		var lz = BitOperations.LeadingZeroCount(w);
		w <<= lz;
		ref var entry = ref Unsafe.Add(ref MemoryMarshal.GetReference(PowersOfFive), (nint)(q - SmallestPowerOfFive) * 2);
		var hi = MultiplyHigh(w, entry, out var lo);
		const ulong PrecisionMask = ulong.MaxValue >> (SingleMantissaBits + 3);
		if ((hi & PrecisionMask) == PrecisionMask)
		{
			var hi2 = MultiplyHigh(w, Unsafe.Add(ref entry, 1));
			lo += hi2;
			if (hi2 > lo) hi++;
			if (lo == ulong.MaxValue && (q < -27 || q > 55)) return Undecided;
		}

		var upperBit = (int)(hi >> 63);
		var shift = upperBit + 64 - SingleMantissaBits - 3;
		var mantissa = hi >> shift;
		var power2 = Power((int)q) + upperBit - lz - SingleMinimumExponent;
		if (power2 <= 0)
		{
			if (-power2 + 1 >= 64) return 0;
			mantissa >>= -power2 + 1;
			mantissa += mantissa & 1;
			mantissa >>= 1;

			return mantissa;
		}

		if (lo <= 1 && (ulong)(q - SingleMinExponentRoundToEven) <= SingleMaxExponentRoundToEven - SingleMinExponentRoundToEven &&
			(mantissa & 3) == 1 && (mantissa << shift) == hi)
		{
			mantissa &= ~1UL;
		}

		mantissa += mantissa & 1;
		mantissa >>= 1;
		var bits = mantissa + ((ulong)(power2 - 1) << SingleMantissaBits);

		return bits >= SingleInfinityBits ? SingleInfinityBits : bits;
	}

	/// <summary>
	/// The 128-bit product of two 64-bit values. Math.BigMul passes the low half through memory (it takes its
	/// address for mulx), so with BMI2 compute the halves separately, keeping both in registers.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong MultiplyHigh(ulong a, ulong b, out ulong low)
	{
		if (Bmi2.X64.IsSupported)
		{
			low = a * b;

			return Bmi2.X64.MultiplyNoFlags(a, b);
		}

		return Math.BigMul(a, b, out low);
	}

	/// <summary>The high 64 bits of the 128-bit product of two 64-bit values.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong MultiplyHigh(ulong a, ulong b) => Bmi2.X64.IsSupported ? Bmi2.X64.MultiplyNoFlags(a, b) : Math.BigMul(a, b, out _);

	/// <summary>floor(log2(5^q)) + q + 63 for q in (-400, 350), i.e. the binary exponent of 10^q plus 63.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Power(int q) => (((152170 + 65536) * q) >> 16) + 63;

}
