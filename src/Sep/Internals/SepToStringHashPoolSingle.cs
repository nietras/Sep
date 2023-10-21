using System;

namespace nietras.SeparatedValues;

sealed class SepToStringHashPoolSingle : SepToString
{
#pragma warning disable CA2213 // Disposable fields should be disposed
    readonly SepStringHashPool _pool;
#pragma warning restore CA2213 // Disposable fields should be disposed

    public SepToStringHashPoolSingle(int maximumStringLength, int initialCapacity, int maximumCapacity)
    {
        _pool = new(maximumStringLength, initialCapacity, maximumCapacity);
    }

    public override string ToString(ReadOnlySpan<char> colSpan, int colIndex) => _pool.ToString(colSpan);

    internal override void DisposeManaged() => _pool.Dispose();
}
