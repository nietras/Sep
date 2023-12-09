#define SEPSTRINGPOOL_USE_ARRAYPOOL
#define SEPSTRINGPOOL_CACHE_LAST
using System;
using System.Buffers;
#if SEPSTRINGPOOLUSAGE
using System.Collections.Generic;
#endif
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nietras.SeparatedValues;

// Based on https://github.com/MarkPflug/Sylvan/blob/main/source/Sylvan.Common/StringPool.cs
// MIT License, Copyright(c) 2022 Mark Pflug
// Highly optimized and greatly-simplified HashSet<string> that only allows additions.
sealed class SepStringHashPool : IDisposable
{
    internal const int InitialCapacityDefault = 64;
    internal const int MaximumCapacityDefault = 4096;
    internal const int MaximumStringLengthDefault = 32;
    const int CollisionLimit = 8;

    struct Entry
    {
        public uint HashCode;
        public int Next;
#if SEPSTRINGPOOLUSAGE
        public int Count;
#endif
        public string String;
    }

    readonly int _maximumStringLength;
    readonly int _maximumCapacity;
    int[] _buckets; // contains index into entries offset by -1. So that 0 (default) means empty bucket.
    Entry[] _entries;
    int _count;
#if SEPSTRINGPOOL_CACHE_LAST
    uint _lastHashCode = SepHash.Default(string.Empty);
    string _lastString = string.Empty;
#endif
    /// <summary>
    /// Simple string pool based on a very simple, fast, but poor hash.
    /// </summary>
    /// <param name="maximumStringLength">The string length beyond which strings will not be pooled.</param>
    /// <param name="initialCapacity">Initial requested capacity of pool. May be rounded up to nearest power of two.</param>
    /// <param name="maximumCapacity">Maximum capacity of pool. If pool full all new spans result in new strings.</param>
    /// <remarks>
    /// The <paramref name="maximumStringLength"/> prevents pooling strings beyond a certain length. 
    /// Longer strings are typically less likely to be duplicated, and carry extra cost for identifying uniqueness.
    /// </remarks>
    public SepStringHashPool(int maximumStringLength = MaximumStringLengthDefault,
        int initialCapacity = InitialCapacityDefault, int maximumCapacity = MaximumCapacityDefault)
    {
        if (initialCapacity > maximumCapacity)
        {
            throw new ArgumentException($"{nameof(initialCapacity)}:{initialCapacity} must be less than or equal to {nameof(maximumCapacity)}:{maximumCapacity}");
        }
        _maximumStringLength = maximumStringLength;
        _maximumCapacity = maximumCapacity;
        var size = GetSize(Math.Max(2, initialCapacity));
#if SEPSTRINGPOOL_USE_ARRAYPOOL
        _buckets = ArrayPool<int>.Shared.Rent(size);
        Array.Clear(_buckets);
        _entries = ArrayPool<Entry>.Shared.Rent(size);
#else
        _buckets = new int[size];
        _entries = new Entry[size];
#endif
    }

    public int Count => _count;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public string ToString(ReadOnlySpan<char> chars)
    {
        var length = chars.Length;
        if (length == 0) { return string.Empty; }
        if (length > _maximumStringLength) { return new(chars); }

        var hashCode = SepHash.Default(chars);

#if SEPSTRINGPOOL_CACHE_LAST
        ref var lastHashCode = ref _lastHashCode;
        ref var lastString = ref _lastString;
        if (lastHashCode == hashCode && MemoryExtensions.SequenceEqual(chars, lastString))
        {
            return lastString;
        }
#endif
        ref var bucket = ref GetBucket(hashCode);

        var entries = _entries;
        ref var entriesRef = ref MemoryMarshal.GetArrayDataReference(entries);
        var entriesLength = (uint)entries.Length;

        var i = bucket - 1;
        uint collisionCount = 0;
        while ((uint)i < entriesLength)
        {
            ref var e = ref Unsafe.Add(ref entriesRef, i);
            if (e.HashCode == hashCode && MemoryExtensions.SequenceEqual(chars, e.String.AsSpan()))
            {
#if SEPSTRINGPOOLUSAGE
                e.Count++;
#endif
#if SEPSTRINGPOOL_CACHE_LAST
                lastHashCode = hashCode;
                lastString = e.String;
#endif
                return e.String;
            }

            i = e.Next;

            if (++collisionCount > CollisionLimit)
            {
                // protects against malicious inputs
                // too many collisions give up and create the string.
                return new(chars);
            }
        }

        string stringValue = new(chars);

#if SEPSTRINGPOOL_CACHE_LAST
        lastHashCode = hashCode;
        lastString = stringValue;
#endif
        var index = _count;
        if (index == entriesLength)
        {
            if (index >= _maximumCapacity)
            {
                return stringValue;
            }
            entries = Resize();
            entriesRef = ref MemoryMarshal.GetArrayDataReference(entries);
            bucket = ref GetBucket(hashCode);
        }

        ref var entry = ref Unsafe.Add(ref entriesRef, index);
        entry.HashCode = hashCode;
#if SEPSTRINGPOOLUSAGE
        entry.Count = 1;
#endif
        entry.Next = bucket - 1;
        entry.String = stringValue;

        bucket = index + 1; // bucket is an int ref
        _count = index + 1;

        return stringValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal string ToStringThreadSafe(ReadOnlySpan<char> chars)
    {
        var length = chars.Length;
        if (length == 0) { return string.Empty; }
        if (length > _maximumStringLength) { return new(chars); }

        var hashCode = SepHash.Default(chars);

        lock (this)
        {
            ref var bucket = ref GetBucket(hashCode);
            var entries = _entries;
            ref var entriesRef = ref MemoryMarshal.GetArrayDataReference(entries);
            var entriesLength = (uint)entries.Length;

            var i = bucket - 1;
            uint collisionCount = 0;
            while ((uint)i < entriesLength)
            {
                ref var e = ref Unsafe.Add(ref entriesRef, i);
                if (e.HashCode == hashCode && MemoryExtensions.SequenceEqual(chars, e.String.AsSpan()))
                {
                    return e.String;
                }

                i = e.Next;

                if (++collisionCount > CollisionLimit)
                {
                    // protects against malicious inputs
                    // too many collisions give up and create the string.
                    return new(chars);
                }
            }

            string stringValue = new(chars);

            var index = _count;
            if (index == entriesLength)
            {
                if (index >= _maximumCapacity)
                {
                    return stringValue;
                }
                entries = Resize();
                entriesRef = ref MemoryMarshal.GetArrayDataReference(entries);
                bucket = ref GetBucket(hashCode);
            }

            ref var entry = ref Unsafe.Add(ref entriesRef, index);
            entry.HashCode = hashCode;
            entry.Next = bucket - 1;
            entry.String = stringValue;

            bucket = index + 1; // bucket is an int ref
            _count = index + 1;

            return stringValue;
        }
    }

    Entry[] Resize()
    {
        var newSize = GetSize(_count + 1);

#if SEPSTRINGPOOL_USE_ARRAYPOOL
        ArrayPool<int>.Shared.Return(_buckets);

        var entries = ArrayPool<Entry>.Shared.Rent(newSize);
        _buckets = ArrayPool<int>.Shared.Rent(newSize);
        Array.Clear(_buckets);
#else
        var entries = new Entry[newSize];
        _buckets = new int[newSize];
#endif
        var count = _count;
        Array.Copy(_entries, entries, count);

#if SEPSTRINGPOOL_USE_ARRAYPOOL
        ArrayPool<Entry>.Shared.Return(_entries);
#endif

        ref var entriesRef = ref MemoryMarshal.GetArrayDataReference(entries);
        for (var i = 0; i < count; i++)
        {
            ref var entry = ref Unsafe.Add(ref entriesRef, i);
            if (entry.Next >= -1)
            {
                ref var bucket = ref GetBucket(entry.HashCode);
                entry.Next = bucket - 1;
                bucket = i + 1;
            }
        }

        _entries = entries;
        return entries;
    }

    static int GetSize(int capacity) => (int)BitOperations.RoundUpToPowerOf2((uint)capacity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ref int GetBucket(uint hashCode)
    {
        var buckets = _buckets;
        var index = hashCode & ((uint)buckets.Length - 1);
        return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(buckets), index);
    }

#if SEPSTRINGPOOLUSAGE
    internal IEnumerable<(string str, int count)> GetUsage()
    {
        for (int i = 0; i < _buckets.Length; i++)
        {
            var b = _buckets[i];
            if (b != 0)
            {
                var idx = b - 1;
                while ((uint)idx < _entries.Length)
                {
                    var e = _entries[idx];
                    yield return (e.String, e.Count);
                    idx = e.Next;
                }
            }
        }
    }
#endif

    void DisposeManaged()
    {
#if SEPSTRINGPOOL_USE_ARRAYPOOL
        ArrayPool<int>.Shared.Return(_buckets);
        ArrayPool<Entry>.Shared.Return(_entries);
#endif
        _buckets = default!;
        _entries = default!;
    }

    #region Dispose
    public void Dispose()
    {
        Dispose(true);
    }

    // Dispose(bool disposing) executes in two distinct scenarios.
    // If disposing equals true, the method has been called directly
    // or indirectly by a user's code. Managed and unmanaged resources
    // can be disposed.
    // If disposing equals false, the method has been called by the 
    // runtime from inside the finalizer and you should not reference 
    // other objects. Only unmanaged resources can be disposed.
    void Dispose(bool disposing)
    {
        // Dispose only if we have not already disposed.
        if (!_disposed)
        {
            // If disposing equals true, dispose all managed and unmanaged resources.
            // I.e. dispose managed resources only if true, unmanaged always.
            if (disposing)
            {
                DisposeManaged();
            }

            // Call the appropriate methods to clean up unmanaged resources here.
            // If disposing is false, only the following code is executed.
        }
        _disposed = true;
    }

    volatile bool _disposed = false;
    #endregion
}
