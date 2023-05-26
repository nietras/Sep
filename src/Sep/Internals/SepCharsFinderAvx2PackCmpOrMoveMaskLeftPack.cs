using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static nietras.SeparatedValues.SepDefaults;

namespace nietras.SeparatedValues;

#pragma warning disable CA1823 // Avoid unused private fields

// Left packing inspired by descaping code at:
// https://github.com/InstLatx64/AVX512_VPCOMPRESSB_Emu
// Results at:
// https://github.com/InstLatx64/AVX512_VPCOMPRESSB_Emu/tree/master/Results
// https://raw.githubusercontent.com/InstLatx64/AVX512_VPCOMPRESSB_Emu/master/Results/AVX512BW_VPCOMPRESSB_Emu_Zen3_AMD_Ryzen_9_5900X_CPUIDA20F10.txt
// Links:
// https://stackoverflow.com/questions/45506309/efficient-sse-shuffle-mask-generation-for-left-packing-byte-elements/
// https://github.com/InstLatx64/AVX512_VPCOMPRESSB_Emu/blob/master/AVX512BW_VPCOMPRESSB_Emu.cpp
// https://github.com/aqrit/despacer/blob/master/include/despacer.h
// https://gcc.godbolt.org/#z:OYLghAFBqd5QCxAYwPYBMCmBRdBLAF1QCcAaPECAKxAEZSBnVAV2OUxAHIBSAJgGY8AO2QAbZlgDU3fgGEGBfKgB0CGdm4AGAIJ9BI8VJnzFovACNV6rboHCxEzNLl4Atq%2BEFiwq/w069e0MnYwV8IQJff10dBjwAL0wAfQJJLAYABwBDdiSGfMx%2BJPECJNoAa3MISQA3VDx0ACo0hSS6htJa%2BqbJBjY27s64xJTJUUwhYAIESQBKG24AdgAhBc0ATiSk11peAA48SWa%2B5GcAEUkILZ39w8bZk4GGmVWAjeZPPZTm9AVzy4%2BES%2BBHuv1K7XQLzWMXWCiyBDwpzQQj%2BgIIADYACyjAhZczjbgAVmWuz2RIuW3hXgszAIySSUCyZmAQkw6AgWNmXP%2BS1eMK0600AA9NItNOjNITNJjNPxNLxNLRNJpOsLlaLxZLpbL5YrVSKlWKJVKZXKFSrJGqlcqjVrTfLSGt1gK1QrbSaddb9bqbZqPXL6JaRQrDX7tXKLa7Fer3eHHW9nRs1XLYzKQ8rvRGNcbtSHM9HUz789bs3aHU6XSKzaWPV6gzrzTXtYHk7rQzmZZGq22Y2HMfH%2BUmRTLY9W62rO02x96Ze2tdWu83ezmzQOBZXc1Po4u08v53r67u51KIzOe76V2uNhv7bH0zudRetXnD56m%2BazwG91LeFfE4LhwbJtx0AhdgM/HtYwfMdvz/DcpVte1729KUnyQg81SlOd0MXLC9yQuCh2fUt7RA4i0M9FDIM1e1cPPEjZUIgD90Q88qKzViX0wt9OIzesT23BjfwrIiBLdGjt3Yn0GJbEUxILCS6JgtCmO4kdNWUqjJ1tad%2BKXHSiz02c91XETmI9cSJS/OjdyfAMtOoqy%2BLU%2BidNUuTw1LZCjMfLyMI80i/Jst8n2EhN4M8yzJJ8sDbVkssYNtYLlOzdzNTFf02PrCVfUyriRQlUM8ucgrBIyyK0olcTMrIqrexq706qKzzFya%2Bq4zMtUnPKo8So09qj0aniyw/bKQo1CzKqAnroq6oDcubIbErLVqgJLHqpvU%2BTau1BbdLm4zyoXIbjL28twqIiVdyO2bStsibKLG3qbtW3q9s2nVyu8g7fK%2B/yJK/P7XvGr6Purcqdpwh74qutaHuBlKMqmhDItusNfXQmGtVDHChrwjGG2R3UxUC1af17Ui8eo2iqfvDVTSJlNNx2gSCfysNAc3MmQdzRniaXbnYsWsbtvpvqV1c6UifUzSRd2jV9tK2sFcMubld9UyLuYjt%2Besqn7rFey5eex99cl/hpc%2BrK1ZajV2Z1znRpt0m7ct8G0Yd6SxSx4rQ25zGYyWM4oTeGFkT%2Ba5SUOVwsgYcokkVf5tlcJI8QYJJMAyPA9iudwkmQVwMkwABHDOs5z5O8kwAhEmIVA8jwUkIFmUhK4Yava/ruIm65eZ%2BD5AVw9SSPbkkGO46SBUk7zhhRDMMu8CxCBx/jg9CT7geNiHyQR4OMfY/jsVp5T9uCFoBec6DI%2BN6dbfd8OEp84iYhRGPquvAviBOpFRdf%2B9P/6wALVCqb%2BJUABiYD9QQP/jAoM0DAH/xvqHAUAAzEg1QXhjAmFMGY1h%2BAXFoOiTB4xJjTAALQyDOIQuYvIzIAHo6FjFQFkSEWt761EwMgIgxBNBv1EMw9AzAG5NxOHwVYvBlhIMHJsbYUcOFcJILQN%2BzAhDZGQOUBAeAF5LxqJw7hFpdEKJ4VI%2BhjCsB0i4b0NRmAGA2AYeuDY7DzAAE86Qr14ZQneecSDCP2LnFOBci6l0ztnZeB9J4GL0SQTQLc7F0Icf%2BNuCBmAoJQeMT%2Bj9kDP1EJ0SuLDfhl2YDnFeSQxSdEMfo3uIdpFONcZgdxb9Akl0/iUhU5SonGOqaY3oJBUgQDQIXYgNiGDzC1mifgvBRjmEIA0zxldXCoF0SUkJOcXFuIPjErp14NgVOiW/BgyTUnpJWRAXZPDcl534Sw0Q2jMRQHvvcaQvB0S4nxJgIkyxpkEFmc8q%2BYDyRci2f%2BM5Si5kz0OWk5IJyQUXJTlc9ANyQlLyuLI24jy%2BAvLxASYkEAvmzL8OoSQexZhPPRH8gFJitYMMkGgFRqQhCoCEGQzIOQbFmXYQgBgzBXD7JYYUiued8mfzWfUjZnQSmKhbl4k%2BHdMB118TnQF/doQJOpQoEg7y2Ez24ZgRFWdkUPNmGCdpRjNnKq1mCJ5qx8EAhRHgFkbJZiV2QDUAg3d9gNwmRATl3KN6SGpc6117qvhxCXj61wlLEnao1XqxedyUU3AOKCBQJruG0EjQKS1YjPEQBUXEB16AnV5xdW6xuHq4hesriotRGitFIrueGzo4alWb2dIsYOyCNjUvQKgSQQyY7CGEMASQIrbEJgYVmiRnj0jWKSOYYgWQRAIHGPkCAxrehsE6CQnBlKYRDIIKwIQlxhjJAILMNdfwyGXFzZ8b4RrWgQhbQsdtnAW6iC4ISTgpAhBcE0F%2B1AXBZDZokT01g7Ann8FoF%2Bggv7X0t3KCAfghJlDoj2LwdYCp1i0FoHsTE6J9ikHfZwTEX6f2cD/aQADnAv0MBACqGD5HX2kDgLAJAAys7jDIBQfpqBC54E4yAYA6ILYoP43SYgtGUUJ0oC3cwsHSADNcBMAgAB5IQohnHyfwEMrheBdG0cY%2BQCIur5NUGLswOVmnOD8GULwZQUHCPYOmHQUgGQhk1BU%2BYKgejKBbGkxAYY8Rxi0B/AIBUmJ1iLExHsSgKisCidZOgSg4xgATCS5AQkpAVScCg73N9TmkD0Dc5gDzXmfNSc0JQKgABFCzxBnETKVISaLEp0SLH4NFyUvAcPoloJQfAmRRBZCs0xwLwXQsTOlJF6LsWhDxeEGyZLmBUtzfgJl7LuXe6OdIYVrbqAMgIkZQZshAB1Jkr9JDMsUJQlkzBgMkiUWQlT/BLsxzYGofBrIEBMnqZIV7DKyFZBqEKGjLA2AcHTYRj9pH5NUaFHsdEZCsSSGAMgU46JlAvYgLgQgJAIOBlkLxjjcr8ezGg7BrkpAEOQeUHh3geHFi8EJOsQk7XpS/iIyR0grg6AZjIxRqjNG6OkAY3%2BluLHEAoCJ/xuV5BKDsZl8QFAQ3JidlIKJ0Q4nJN%2Bcq83UgcnDOKeU2pjTWm8A6YRPp%2BTngTOGbM3VqzNm7MOe3c5or7nPPea4b5rYuuxuYBC/KMLU2osxdzXNzACXFsQBS2l%2BASosukBy3BynrvCuuY92V73FWqu1cs41yULXxTtc64SbrCO%2BtrrwIN4bXBRsJCCwHib4Xpth7i5HhbSWY/Lbj5ABPG2U/5Z23QPbB28BHa4FdhFFhKFmHMMge7l2ztz0u2EG7Qg7tTokdhy7z3XtZHe5Qr7P3XCg7AxDt90Pv2w64PDxHyOxCLuHZiZQmhX%2BXBx9w/HnRCd8c4xB3gMnEXCneDRDOzPYZrXrHDfYQkPYUUKHYjGHQzQXRgYXUXODBA3gL9HnL0fnf9OvYAxjSnXRCTcfH9TEIAA%3D
// Benchmarks:
// ~4x slower than simple tzcnt approach for real-world data like PackageAssets
sealed class SepCharsFinderAvx2PackCmpOrMoveMaskLeftPack : ISepCharsFinder
{
    readonly byte _separator;
    readonly Vector256<byte> _nls = Vector256.Create(LineFeedByte);
    readonly Vector256<byte> _crs = Vector256.Create(CarriageReturnByte);
    readonly Vector256<byte> _qts = Vector256.Create(QuoteByte);
    readonly Vector256<byte> _sps;

    //readonly Vector256<byte> _zero = Vector256<byte>.Zero;
    readonly Vector256<byte> _permutationMask = Vector256.Create(0x0020100884828180L).AsByte();
    readonly Vector256<byte> _invertMask = Vector256.Create(0x0020100880808080L).AsByte();
    readonly Vector256<byte> _fixup = Vector256.Create(
        0x07070707, 0x00000000, 0x0F0F0F0F, 0x08080808,
        0x07070707, 0x00000000, 0x0F0F0F0F, 0x08080808
    ).AsByte();
    // _mm256_set_epi32 has reverse order 7..0
    //0x08080808, 0x0F0F0F0F, 0x00000000, 0x07070707,
    //0x08080808, 0x0F0F0F0F, 0x00000000, 0x07070707
    readonly Vector256<int> _lut = Vector256.Create(
        0x07060504, // 0x00'000000, 0x'07060504
        0x06070605, // 0x0100'0000, 0x00'070605
        0x05070604, // 0x0200'0000, 0x00'070604
        0x05060706, // 0x020100'00, 0x0000'0706
        0x04070504, // 0x0300'0000, 0x00'070504
        0x04060705, // 0x030100'00, 0x0000'0705
        0x04050704, // 0x030200'00, 0x0000'0704
        0x04050607  // 0x03020100', 0x000000'07
    );
    // hi bits are ignored by pshufb, used to reject movement of low qword bytes
    readonly Vector256<byte> _shuffleA = Vector256.Create((byte)
        0x70, 0x61, 0x52, 0x43, 0x34, 0x25, 0x16, 0x07, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,
        0x70, 0x61, 0x52, 0x43, 0x34, 0x25, 0x16, 0x07, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F
    );
    // broadcast 0x08 then blendd...
    readonly Vector256<byte> _shuffleB = Vector256.Create(
        0x00000000, 0x00000000, 0x08080808, 0x08080808,
        0x00000000, 0x00000000, 0x08080808, 0x08080808
    ).AsByte();
    readonly Vector256<byte> _sequence = Vector256.Create((byte)
        00, 01, 02, 03, 04, 05, 06, 07, 08, 09,
        10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
        20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
        30, 31
    );

    public unsafe SepCharsFinderAvx2PackCmpOrMoveMaskLeftPack(Sep sep)
    {
        _separator = (byte)sep.Separator;
        _sps = Vector256.Create(_separator);
    }

    public int PaddingLength => Vector256<byte>.Count; // Parses 2 x char vectors e.g. 1 byte vector
    public int RequestedPositionsFreeLength => PaddingLength * 32;

    [SkipLocalsInit]
    public unsafe int Find(char[] _chars, int charsStart, int charsEnd,
                    Pos[] positions, int positionsStart, ref int positionsEnd)
    {
        // Method should **not** call other non-inlined methods, since this
        // impacts code-generation severely.

        var chars = _chars;
        chars.CheckPaddingAndIsZero(charsEnd, PaddingLength);
        // Absolute minimum, prefer RequestedPositionsFreeLength for free
        positions.CheckPadding(positionsEnd, PaddingLength);

        A.Assert(charsStart <= charsEnd);
        A.Assert(charsEnd <= (chars.Length - PaddingLength));
        var dataStart = charsStart;
        var dataEnd = charsEnd;
        ref var charsRef = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(chars), dataStart);

        ref var positionsRef = ref Unsafe.As<Pos, int>(ref MemoryMarshal.GetArrayDataReference(positions));
        ref var positionsRefCurrent = ref Unsafe.Add(ref positionsRef, positionsEnd);
        ref var positionsRefStop = ref Unsafe.Add(ref positionsRef, positions.Length - Vector256<byte>.Count);

        var nls = _nls; //Vector256.Create(LineFeedByte);
        var crs = _crs; //Vector256.Create(CarriageReturnByte);
        var qts = _qts; //Vector256.Create(QuoteByte);
        var sps = _sps; //Vector256.Create(_separator);

        var permutationMask = _permutationMask;
        var invertMask = _invertMask;
        //var zero = _zero;
        var fixup = _fixup;
        var lut = _lut;
        var shuffleA = _shuffleA;
        var shuffleB = _shuffleB;
        var sequence = _sequence;

        //var permutationMask = Vector256.Create(0x0020100884828180L).AsByte();
        //var invertMask = Vector256.Create(0x0020100880808080L).AsByte();
        var zero = Vector256<byte>.Zero;
        //var fixup = Vector256.Create(0x07070707, 0x00000000, 0x0F0F0F0F, 0x08080808, 0x07070707, 0x00000000, 0x0F0F0F0F, 0x08080808).AsByte();
        //var lut = Vector256.Create(
        //    0x07060504, // 0x00'000000, 0x'07060504
        //    0x06070605, // 0x0100'0000, 0x00'070605
        //    0x05070604, // 0x0200'0000, 0x00'070604
        //    0x05060706, // 0x020100'00, 0x0000'0706
        //    0x04070504, // 0x0300'0000, 0x00'070504
        //    0x04060705, // 0x030100'00, 0x0000'0705
        //    0x04050704, // 0x030200'00, 0x0000'0704
        //    0x04050607  // 0x03020100', 0x000000'07
        //);
        // hi bits are ignored by pshufb, used to reject movement of low qword bytes
        //var shuffleA = Vector256.Create((sbyte)
        //    0x70, 0x61, 0x52, 0x43, 0x34, 0x25, 0x16, 0x07, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,
        //    0x70, 0x61, 0x52, 0x43, 0x34, 0x25, 0x16, 0x07, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F
        //).AsByte();
        // broadcast 0x08 then blendd...
        //var shuffleB = Vector256.Create(
        //    0x00000000, 0x00000000, 0x08080808, 0x08080808,
        //    0x00000000, 0x00000000, 0x08080808, 0x08080808
        //).AsByte();

        //var separatorShifted = _separator << SepCharPosition.CharShift;

        var packedBytes = stackalloc byte[1024];

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

            var nlsEq = Vector256.Equals(bytes, nls);
            var crsEq = Vector256.Equals(bytes, crs);
            var qtsEq = Vector256.Equals(bytes, qts);
            var spsEq = Vector256.Equals(bytes, sps);

            var lineEndings = nlsEq | crsEq;
            var endingsAndQuotes = lineEndings | qtsEq;
            var specialChars = endingsAndQuotes | spsEq;

            // Optimize for the case of no special character
            var specialCharMask = Avx2.MoveMask(specialChars);
            if (specialCharMask != 0)
            {
                var r0 = bytes;
                // r1 is inverted, i.e. chars not special, we want to keep/left pack special chars
                var r1 = Avx2.CompareEqual(specialChars, zero);

                var r2 = Avx2.SumAbsoluteDifferences(zero, r1).AsByte();
                r1 = Avx2.AndNot(r1, permutationMask);

                r1 = Avx2.SumAbsoluteDifferences(r1, invertMask).AsByte(); // index_bitmap[0:5], low32_spaces_count[7:15]

                r2 = Avx2.Shuffle(r2, zero);

                r2 = Avx2.Subtract(shuffleA, r2); // add space cnt of low qword
                var r3 = Avx2.ShiftLeftLogical(r1.AsUInt64(), 29).AsByte(); // move top part of index_bitmap to high DWORD
                var r4 = Avx2.ShiftRightLogical(r1.AsUInt64(), 7).AsByte(); // number of spaces in low DWORD
                                                                            //var r3 = Avx2.ShiftLeftLogical128BitLane(r1, 2); // move top part of index_bitmap to high DWORD
                                                                            //var r4 = Avx2.ShiftRightLogical128BitLane(r1, 7); // number of spaces in low DWORD

                r4 = Avx2.Shuffle(r4, shuffleB);
                r1 = Avx2.Or(r1, r3);
                r1 = Avx2.PermuteVar8x32(lut, r1.AsInt32()).AsByte();
                r4 = Avx2.Add(r4, shuffleA);
                r2 = Avx2.Shuffle(r4, r2);
                r2 = Avx2.Max(r2, r4); // pin low QWORD bytes
                r1 = Avx2.Xor(r1, fixup);
                r1 = Avx2.Shuffle(r1, r2); // complete shuffle mask

                // left pack
                var pack = Avx2.Shuffle(r0, r1);
                var packSeq = Avx2.Shuffle(sequence, r1);

                var s0 = specialCharMask;
                var s1 = BitOperations.PopCount((uint)s0);
                s0 &= 0xFFFF; // isolate low OWORD
                s0 = BitOperations.PopCount((uint)s0);

                // TODO: Change CharPosition to keep char at first 8-bits,
                //       and move position to left most 24-bits.
                //       This means those two can be combined at 16-bit.

                var (packUShort0, packUShort1) = Vector256.Widen(pack);
                var (packUInt0, packUInt1) = Vector256.Widen(packUShort0);
                var (packUInt2, packUInt3) = Vector256.Widen(packUShort1);
                packUInt0 = Vector256.ShiftLeft(packUInt0, SepCharPosition.CharShift);
                packUInt1 = Vector256.ShiftLeft(packUInt1, SepCharPosition.CharShift);
                packUInt2 = Vector256.ShiftLeft(packUInt2, SepCharPosition.CharShift);
                packUInt3 = Vector256.ShiftLeft(packUInt3, SepCharPosition.CharShift);

                var (packSeqUShort0, packSeqUShort1) = Vector256.Widen(packSeq);
                var (packSeqUInt0, packSeqUInt1) = Vector256.Widen(packSeqUShort0);
                var (packSeqUInt2, packSeqUInt3) = Vector256.Widen(packSeqUShort1);

                var offset = Vector256.Create((uint)dataIndex);
                var combined0 = Vector256.BitwiseOr(packUInt0, packSeqUInt0) + offset;
                var combined1 = Vector256.BitwiseOr(packUInt1, packSeqUInt1) + offset;
                var combined2 = Vector256.BitwiseOr(packUInt2, packSeqUInt2) + offset;
                var combined3 = Vector256.BitwiseOr(packUInt3, packSeqUInt3) + offset;

                Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref positionsRefCurrent), combined0.AsByte());
                Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref Unsafe.Add(ref positionsRefCurrent, Min(s0, 8))), combined1.AsByte());
                Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref Unsafe.Add(ref positionsRefCurrent, s0)), combined2.AsByte());
                Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref Unsafe.Add(ref positionsRefCurrent, s0 + Min(s1 - s0, 8))), combined3.AsByte());

                positionsRefCurrent = ref Unsafe.Add(ref positionsRefCurrent, s1);

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

    static int Min(int a, int b)
    {
        var diff = a - b;
        return b + (diff & diff >> 31);
    }

    //static int Min(int x, int y)
    //{
    //    return y + ((x - y) & ((x - y) >> (sizeof(int) * 8 - 1))); // min(x, y)
    //}
}
