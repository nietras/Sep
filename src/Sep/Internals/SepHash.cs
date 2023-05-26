using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace nietras.SeparatedValues;

// Extremely fast for small e.g. <= 128 chars, but no doubt horrible char/string
// hashes. Used only for string pooling so hash quality is not that important.
static class SepHash
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint Default(ReadOnlySpan<char> chars) => SumMultiplyPrimesNUInt(chars);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe static uint SumMultiplyPrimesNUInt(ReadOnlySpan<char> chars)
    {
        // Four completely random prime numbers (probably horrible hash):
        // 31 = 0x001F, 127 = 0x007F, 191 = 0x00BF, 251 = 0x00FB
        const nuint singleMultiplier = 31;
        nuint nativeSizeMultiplier = unchecked((nuint)0x001F_007F_00BF_00FB);
        // Dumb hack to get constant enregistered instead of mov per loop.
        //nativeSizeMultiplier = nativeSizeMultiplier | (nuint)(byteLength ^ byteLength);

        ref var byteRef = ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(chars));

        nint byteIndex = 0;
        nint byteLength = chars.Length * sizeof(char);
        nint nativeSize = Unsafe.SizeOf<nuint>();

        nuint hash = 0;
        while (byteIndex <= (byteLength - nativeSize))
        {
            hash = (hash * nativeSizeMultiplier) + Unsafe.As<byte, nuint>(ref Unsafe.Add(ref byteRef, byteIndex));
            byteIndex += nativeSize;
        }
        while (byteIndex < byteLength)
        {
            hash = (hash * singleMultiplier) + Unsafe.As<byte, ushort>(ref Unsafe.Add(ref byteRef, byteIndex));
            byteIndex += sizeof(ushort);
        }
        return (uint)(hash + (hash >> 32));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe static uint UnrollSumMultiply31(ReadOnlySpan<char> chars)
    {
        ref var charsCurrent = ref Unsafe.As<char, ushort>(ref MemoryMarshal.GetReference(chars));
        ref var charsEnd = ref Unsafe.Add(ref charsCurrent, chars.Length);
        ref var charsEndBlock = ref Unsafe.Subtract(ref charsEnd, 3);
        uint hash = 0;
        for (; Unsafe.IsAddressLessThan(ref charsCurrent, ref charsEndBlock);
             charsCurrent = ref Unsafe.Add(ref charsCurrent, 4))
        {
            //hash = (((hash * 31 + chars[i - 3]) * 31 + chars[i - 2]) * 31 + chars[i - 1]) * 31 + chars[i + 0];
            //hash = (uint)(hash * 31 * 31 * 31 * 31 + chars[i] * 31 * 31 * 31 + chars[i + 1] * 31 * 31 + chars[i + 2] * 31 + chars[i + 3]);
            // 31 = 1F,  31 * 31 = 961 = 3C1, 31 * 31 * 31 = 29791 = 745F, 31 * 31 * 31 * 31 = 923521 = E1781
            hash = (uint)(hash * 31 * 31 * 31 * 31 +
                Unsafe.Add(ref charsCurrent, 0) * 31 * 31 * 31 +
                Unsafe.Add(ref charsCurrent, 1) * 31 * 31 +
                Unsafe.Add(ref charsCurrent, 2) * 31 +
                Unsafe.Add(ref charsCurrent, 3));
        }
        for (; Unsafe.IsAddressLessThan(ref charsCurrent, ref charsEnd);
             charsCurrent = ref Unsafe.Add(ref charsCurrent, 1))
        {
            hash = hash * 31 + charsCurrent;
        }
        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SkipLocalsInit]
    internal unsafe static uint Sse2SumMultiply31(ReadOnlySpan<char> chars)
    {
        ref var charsCurrent = ref Unsafe.As<char, ushort>(ref MemoryMarshal.GetReference(chars));
        var multiplier = Vector128.Create(31 * 31 * 31u, 31 * 31u, 31u, 1u);
        multiplier = Vector128.BitwiseOr(multiplier, Vector128<uint>.Zero); // Hack to put constant in register.
        fixed (ushort* ptr = &charsCurrent)
        {
            var ptrCurrent = ptr;
            var ptrEnd = ptrCurrent + chars.Length;
            var ptrEndBlock = ptrEnd - 4;
            uint hash = 0;
            var store = stackalloc uint[4];
            for (; ptrCurrent < ptrEndBlock; ptrCurrent += 4)
            {
                var vectorChars = Sse2.LoadScalarVector128((ulong*)ptrCurrent).AsUInt16();
                var (lower, _) = Vector128.Widen(vectorChars);
                var hashes = multiplier * lower;
                Sse2.Store(store, hashes);
                hash *= 31 * 31 * 31 * 31u;
                hash += store[0];
                hash += store[1];
                hash += store[2];
                hash += store[3];
            }
            for (; ptrCurrent < ptrEnd; ++ptrCurrent)
            {
                hash = hash * 31 + *ptrCurrent;
            }
            return hash;
        }
    }

    internal static uint SumMultiply31(ReadOnlySpan<char> chars)
    {
        uint hash = 0;
        for (var i = 0; i < chars.Length; i++)
        {
            hash = hash * 31 + chars[i];
        }
        return hash;
    }

    // https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
    internal static uint FNV1aNUInt(ReadOnlySpan<char> chars)
    {
        const nuint fnvPrime = 16777619;
        const nuint fnvOffsetBasis = 2166136261;

        ref var charsCurrent = ref Unsafe.As<char, byte>(ref MemoryMarshal.GetReference(chars));

        nint byteIndex = 0;
        nint byteLength = chars.Length * sizeof(char);
        var nativeSize = Unsafe.SizeOf<nuint>();

        nuint hash = fnvOffsetBasis;
        while (byteIndex <= (byteLength - nativeSize))
        {
            hash = (Unsafe.As<byte, nuint>(ref Unsafe.Add(ref charsCurrent, byteIndex)) ^ hash) * fnvPrime;
            byteIndex += nativeSize;
        }
        while (byteIndex < byteLength)
        {
            hash = (Unsafe.As<byte, ushort>(ref Unsafe.Add(ref charsCurrent, byteIndex)) ^ hash) * fnvPrime;
            byteIndex += sizeof(ushort);
        }
        return (uint)(hash + (hash >> 32));
    }
}
