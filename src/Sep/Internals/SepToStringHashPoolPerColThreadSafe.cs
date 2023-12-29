using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nietras.SeparatedValues;

sealed class SepToStringHashPoolPerColThreadSafe : SepToString
{
    readonly SepStringHashPool[] _pools;

    public SepToStringHashPoolPerColThreadSafe(int colCount,
        int maximumStringLength, int initialCapacity, int maximumCapacity)
    {
        _pools = new SepStringHashPool[colCount];
        for (var i = 0; i < colCount; i++)
        {
            _pools[i] = new(maximumStringLength, initialCapacity, maximumCapacity);
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
