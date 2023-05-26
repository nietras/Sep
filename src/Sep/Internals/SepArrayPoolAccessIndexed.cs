using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace nietras.SeparatedValues;

internal sealed class SepArrayPoolAccessIndexed : IDisposable
{
    [ExcludeFromCodeCoverage]
    readonly record struct Key(int Index, Type Type);
    [ExcludeFromCodeCoverage]
    readonly record struct Value(object Array, Action<object> Return);

    readonly Dictionary<Key, Value> _pool = new();
    int _poolIndex = 0;

    internal Span<T> RentUniqueArrayAsSpan<T>(int length)
    {
        Key key = new(_poolIndex, typeof(T));
        var array = _pool.TryGetValue(key, out var value) ? Unsafe.As<T[]>(value.Array) : null;
        if (array is null || array.Length < length)
        {
            if (array is not null) { ArrayPool<T>.Shared.Return(array); }
            array = ArrayPool<T>.Shared.Rent(length);
            _pool[key] = new(array, ArrayPoolHelper<T>.Return);
        }
        ++_poolIndex;
        return new Span<T>(array, 0, length);
    }

    internal void Reset() => _poolIndex = 0;

    static class ArrayPoolHelper<T>
    {
        internal static readonly Action<object> Return =
            obj => ArrayPool<T>.Shared.Return(Unsafe.As<T[]>(obj));
    }

    void DisposeManaged()
    {
        foreach (var (_, value) in _pool)
        {
            value.Return(value.Array);
        }
    }

    #region Dispose
    bool _disposed;
    void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DisposeManaged();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
