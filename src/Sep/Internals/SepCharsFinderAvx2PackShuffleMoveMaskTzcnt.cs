using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static nietras.SeparatedValues.SepDefaults;
using static nietras.SeparatedValues.SepCharsFinderHelper;

namespace nietras.SeparatedValues;

sealed class SepCharsFinderAvx2PackShuffleMoveMaskTzcnt : ISepCharsFinder
{
    readonly byte _separator;
    // SIMDized check which bytes are in a set
    // Special Case 1 -- small sets
    // http://0x80.pl/articles/simd-byte-lookup.html
    // Up to 8 different bytes can be found by defining 8 bit flags
    readonly Vector256<byte> _loNibblesLookupVector;
    readonly Vector256<byte> _hiNibblesLookupVector;
    readonly Vector256<byte> _nibbleMask = Vector256.Create<byte>(0x0F);
    readonly Vector256<byte> _zero = Vector256.Create<byte>(0x00);
    readonly Vector256<byte> _separatorFlags = Vector256.Create<byte>(SeparatorFlag);

    public unsafe SepCharsFinderAvx2PackShuffleMoveMaskTzcnt(Sep sep)
    {
        _separator = (byte)sep.Separator;

        const int specialCharsCount = 4;
        var specialBytes = stackalloc byte[specialCharsCount]
            { LineFeedByte, CarriageReturnByte, QuoteByte, (byte)sep.Separator };
        var flags = stackalloc byte[specialCharsCount]
            { LineFeedFlag, CarriageReturnFlag, QuoteFlag, SeparatorFlag };

        // _mm256_shuffle_epi8 works like two times an _mm_shuffle_epi8 side-by-side
        // so below we repeat the nibble LUTs so in both low and high 128-bit
        const int nibblesCount = 16;
        Span<byte> loNibblesLookup = stackalloc byte[nibblesCount * 2];
        for (var c = 0; c < specialCharsCount; c++)
        {
            var nibble = specialBytes[c] & 0x0F;
            loNibblesLookup[nibble] |= flags[c];
            loNibblesLookup[nibble + 16] = loNibblesLookup[nibble];
        }
        Span<byte> hiNibblesLookup = stackalloc byte[nibblesCount * 2];
        for (var c = 0; c < specialCharsCount; c++)
        {
            var nibble = specialBytes[c] >> 4;
            hiNibblesLookup[nibble] |= flags[c];
            hiNibblesLookup[nibble + 16] = hiNibblesLookup[nibble];
        }
        _loNibblesLookupVector = Vector256.Create<byte>(loNibblesLookup);
        _hiNibblesLookupVector = Vector256.Create<byte>(hiNibblesLookup);
    }

    public int PaddingLength => Vector256<byte>.Count; // Parses 2 x char vectors e.g. 1 byte vector
    public int RequestedPositionsFreeLength => PaddingLength * 32;

    [SkipLocalsInit]
    public int Find(char[] _chars, int charsStart, int charsEnd,
                    Pos[] positions, int positionsStart, ref int positionsEnd)
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        var chars = _chars;
        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        // Absolute minimum, prefer RequestedPositionsFreeLength for free
        positions.CheckPadding(positionsEnd, PaddingLength);

        A.Assert(charsStart <= charsEnd);
        A.Assert(charsEnd <= (_chars.Length - PaddingLength));
        var dataStart = charsStart;
        var dataEnd = charsEnd;
        ref var charsRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars), dataStart);

        ref var positionsRef = ref Unsafe.As<Pos, int>(ref MemoryMarshal.GetArrayDataReference(positions));
        ref var positionsRefCurrent = ref Unsafe.Add(ref positionsRef, positionsEnd);
        ref var positionsRefStop = ref Unsafe.Add(ref positionsRef, positions.Length - Vector256<byte>.Count);

        // Try to get vectors into registers by making local
        var loNibblesLookupVector = _loNibblesLookupVector;
        var hiNibblesLookupVector = _hiNibblesLookupVector;
        // Only way to get these "constants" into registers is to assign them to
        // fields and then to local variables. Also avoids extra xorps for zero.
        var nibbleMask = _nibbleMask; //Vector256.Create<byte>(0x0F);
        var zero = _zero; //Vector256.Create<byte>(0x00);
        var separatorFlags = _separatorFlags;

        var separatorShifted = _separator << SepCharPosition.CharShift;

        var dataIndex = dataStart;
        for (; dataIndex < dataEnd; dataIndex += Vector256<byte>.Count,
             charsRef = ref Unsafe.Add(ref charsRef, Vector256<byte>.Count))
        {
            var vector0 = Unsafe.ReadUnaligned<Vector256<short>>(
                ref Unsafe.As<char, byte>(ref charsRef));
            var vector1 = Unsafe.ReadUnaligned<Vector256<short>>(
                ref Unsafe.As<char, byte>(ref Unsafe.Add(ref charsRef, Vector256<short>.Count)));

            var packed = Avx2.PackUnsignedSaturate(vector0, vector1);
            // Pack interleaves the two vectors need to permute them back
            var bytes = Avx2.Permute4x64(packed.AsInt64(), 0b_11_01_10_00).AsByte();

            var loNibbles = Avx2.And(bytes, nibbleMask);
            var loTranslated = Avx2.Shuffle(loNibblesLookupVector, loNibbles);

            var shift = Avx2.ShiftRightLogical(bytes.AsInt16(), (byte)4).AsByte();
            var hiNibbles = Avx2.And(shift, nibbleMask);
            var hiTranslated = Avx2.Shuffle(hiNibblesLookupVector, hiNibbles);

            var intersection = Avx2.And(loTranslated, hiTranslated);

            var specialCharsNot = Avx2.CompareEqual(intersection, zero);
            var specialChars = Avx2.CompareEqual(specialCharsNot, zero); // Could invert mask instead ~, but not faster

            // Optimize for the case of no special character
            var specialCharMask = Avx2.MoveMask(specialChars);
            if (specialCharMask != 0)
            {
                var sepsEq = Vector256.Equals(intersection, separatorFlags);
                var sepsMask = Avx2.MoveMask(sepsEq);
                // Optimize for case of only separators i.e. no endings or quotes
                if (sepsMask == specialCharMask)
                {
                    SepAssert.AssertMaxPosition(dataIndex, Vector256<byte>.Count);
                    positionsRefCurrent = ref PackSeparatorPositions(sepsMask,
                        separatorShifted, dataIndex, ref positionsRefCurrent);
                }
                else
                {
                    positionsRefCurrent = ref PackSpecialCharPositions(specialCharMask,
                        ref charsRef, dataIndex, ref positionsRefCurrent);
                }
                // If current is greater than or equal than "stop", then break.
                // There is no longer guaranteed space enough for next Vector256<byte>.Count.
                if (Unsafe.IsAddressLessThan(ref positionsRefStop, ref positionsRefCurrent))
                {
                    // Move data index so next find starts correctly
                    dataIndex += Vector256<byte>.Count;
                    break;
                }
            }
        }
        positionsEnd = (int)(Unsafe.ByteOffset(ref positionsRef, ref positionsRefCurrent) >> 2); // / sizeof(int)); // CQ: Weird with div sizeof
        // Step is Vector256<byte>.Count so may go past end, ensure limited
        dataIndex = Math.Min(charsEnd, dataIndex);
        return dataIndex;
    }
}
