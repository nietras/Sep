using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;


namespace nietras.SeparatedValues;

static class SepCharsFinderHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int PackSeparatorPositions(int mask, int separatorShifted, int dataIndex, ref int positionsRefCurrent)
    {
        SepAssert.AssertMaxPosition(dataIndex, Vector256<byte>.Count);
        var dataIndexWithSeparatorShifted = separatorShifted | dataIndex;
        do
        {
            var sepRelativeIndex = BitOperations.TrailingZeroCount((int)mask);
            mask &= (mask - 1); // Or Bmi1.ResetLowestSetBit/JIT seems to do it fine

            // Accumulate index + character found, bit pack char with index
            // Using ctor and Pos type is too slow due to bad code gen
            var sepIndex = dataIndexWithSeparatorShifted + sepRelativeIndex;
            positionsRefCurrent = sepIndex;
            positionsRefCurrent = ref Unsafe.Add(ref positionsRefCurrent, 1);
        }
        while (mask != 0);
        return ref positionsRefCurrent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref int PackSpecialCharPositions(int mask, ref char charsRef, int dataIndex, ref int positionsRefCurrent)
    {
        do
        {
            var sepRelativeIndex = BitOperations.TrailingZeroCount((int)mask);
            mask &= (mask - 1); // Or Bmi1.ResetLowestSetBit/JIT seems to do it fine

            // Accumulate index + character found, bit pack char with index
            // Getting from Vector is slow and incurs bounds check
            // Instead code generation better if just get as char from buffer
            var charFound = Unsafe.Add(ref charsRef, sepRelativeIndex);
            var sepIndex = dataIndex + sepRelativeIndex;
            positionsRefCurrent = SepCharPosition.PackRaw(charFound, sepIndex);
            positionsRefCurrent = ref Unsafe.Add(ref positionsRefCurrent, 1);
        }
        while (mask != 0);
        return ref positionsRefCurrent;
    }
}
