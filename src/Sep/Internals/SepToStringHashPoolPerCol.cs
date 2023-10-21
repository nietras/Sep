using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nietras.SeparatedValues;

sealed class SepToStringHashPoolPerCol : SepToString
{
    readonly SepStringHashPool[] _colStringHashPools;

    public SepToStringHashPoolPerCol(int colCount,
        int maximumStringLength, int initialCapacity, int maximumCapacity)
    {
        _colStringHashPools = new SepStringHashPool[colCount];
        for (var i = 0; i < colCount; i++)
        {
            _colStringHashPools[i] = new(maximumStringLength, initialCapacity, maximumCapacity);
        }
    }

    public override string ToString(ReadOnlySpan<char> colSpan, int colIndex)
    {
        if (colIndex < _colStringHashPools.Length)
        {
            var spanToString = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_colStringHashPools), colIndex);
            return spanToString.ToString(colSpan);
        }
        else
        {
            return new(colSpan);
        }
    }

    internal override void DisposeManaged()
    {
        foreach (var pool in _colStringHashPools)
        {
            pool.Dispose();
        }
    }
}
