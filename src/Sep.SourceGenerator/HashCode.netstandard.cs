#if !NET
// System.HashCode does not exist on netstandard2.0, which is the target framework the analyzer
// actually ships as. Polyfilling it here rather than taking a dependency on Microsoft.Bcl.HashCode
// keeps the analyzer a single assembly, since an analyzer must ship every assembly it loads.
//
// Only the members the generator uses are implemented. The algorithm does not have to match the
// runtime implementation since these hash codes are never persisted, they are only used for
// in-process incremental generator caching.
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System;

internal struct HashCode
{
    const int Factor = 397;

    // Starting from the default zero makes the first Add yield the value's own hash code.
    int _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add<T>(T value) => AddHash(value?.GetHashCode() ?? 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add<T>(T value, IEqualityComparer<T> comparer) =>
        AddHash(value is null ? 0 : comparer.GetHashCode(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void AddHash(int hash) => _value = unchecked((_value * Factor) ^ hash);

    public readonly int ToHashCode() => _value;

    public static int Combine<T1, T2>(T1 value1, T2 value2)
    {
        var hashCode = new HashCode();
        hashCode.Add(value1);
        hashCode.Add(value2);
        return hashCode.ToHashCode();
    }

    public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
    {
        var hashCode = new HashCode();
        hashCode.Add(value1);
        hashCode.Add(value2);
        hashCode.Add(value3);
        return hashCode.ToHashCode();
    }
}
#endif
