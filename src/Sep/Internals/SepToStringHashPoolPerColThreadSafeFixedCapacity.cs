using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nietras.SeparatedValues;

sealed class SepToStringHashPoolPerColThreadSafeFixedCapacity : SepToString
{
    readonly SepStringHashPoolThreadSafeFixedCapacity[] _pools;

    public SepToStringHashPoolPerColThreadSafeFixedCapacity(int colCount,
        int maximumStringLength, int capacity)
    {
        _pools = new SepStringHashPoolThreadSafeFixedCapacity[colCount];
        for (var i = 0; i < colCount; i++)
        {
            _pools[i] = new(maximumStringLength, capacity);
        }
    }

    public override bool IsThreadSafe => true;

    public override string ToString(ReadOnlySpan<char> colSpan, int colIndex)
    {
        if (colIndex < _pools.Length)
        {
            var pool = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_pools), colIndex);
            return pool.ToStringThreadSafe(colSpan);
        }
        else
        {
            return new(colSpan);
        }
    }

    internal override void DisposeManaged()
    {
        foreach (var pool in _pools)
        {
            pool.Dispose();
        }
    }
}
