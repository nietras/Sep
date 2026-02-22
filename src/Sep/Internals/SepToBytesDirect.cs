using System;

namespace nietras.SeparatedValues;

sealed class SepToBytesDirect : SepToBytes
{
    SepToBytesDirect() { }
    public static SepToBytesDirect Instance { get; } = new();
    public override ReadOnlyMemory<byte> ToBytes(ReadOnlySpan<byte> colSpan, int colIndex) =>
        colSpan.ToArray();
    public override bool IsThreadSafe => true;
}
