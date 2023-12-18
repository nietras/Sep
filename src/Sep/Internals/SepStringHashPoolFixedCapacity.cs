#define SEPSTRINGPOOL_CACHE_LAST
using System;
using System.Buffers;
#if SEPSTRINGPOOLUSAGE
using System.Collections.Generic;
#endif
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if SEPSTRINGPOOL_CACHE_LAST
using System.Threading;
#endif

namespace nietras.SeparatedValues;

// Based on https://github.com/MarkPflug/Sylvan/blob/main/source/Sylvan.Common/StringPool.cs
// MIT License, Copyright(c) 2022 Mark Pflug
// Highly optimized and greatly-simplified HashSet<string> that only allows additions.
sealed class SepStringHashPoolFixedCapacity : ISepStringHashPool
{
    internal const int CapacityDefault = 2048;
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
    int[] _buckets; // contains index into entries offset by -1. So that 0 (default) means empty bucket.
    Entry[] _entries;
    volatile int _count;
#if SEPSTRINGPOOL_CACHE_LAST
    uint _lastHashCode = SepHash.Default(string.Empty);
    string _lastString = string.Empty;
    readonly ThreadLocal<(uint, string)> _last = new(() => (SepHash.Default(string.Empty), string.Empty));
#endif

    /// <summary>
    /// Simple string pool based on a very simple, fast, but poor hash.
    /// </summary>
    /// <param name="maximumStringLength">The string length beyond which strings will not be pooled.</param>
    /// <param name="capacity">Initial requested capacity of pool. May be rounded up to nearest power of two.</param>
    /// <remarks>
    /// The <paramref name="maximumStringLength"/> prevents pooling strings beyond a certain length. 
    /// Longer strings are typically less likely to be duplicated, and carry extra cost for identifying uniqueness.
    /// </remarks>
    public SepStringHashPoolFixedCapacity(int maximumStringLength = MaximumStringLengthDefault,
        int capacity = CapacityDefault)
    {
        _maximumStringLength = maximumStringLength;
        var size = GetSize(Math.Max(2, capacity));
        _buckets = ArrayPool<int>.Shared.Rent(size);
        Array.Clear(_buckets);
        _entries = ArrayPool<Entry>.Shared.Rent(size);
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
        var count = _count;
        while ((uint)i < count)
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
        var index = count;
        if (index < entriesLength)
        {
            ref var entry = ref Unsafe.Add(ref entriesRef, index);
            entry.HashCode = hashCode;
#if SEPSTRINGPOOLUSAGE
        entry.Count = 1;
#endif
            entry.Next = bucket - 1;
            entry.String = stringValue;

            bucket = index + 1; // bucket is an int ref
            _count = index + 1;
        }
        return stringValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public string ToStringThreadSafe(ReadOnlySpan<char> chars)
    {
        var length = chars.Length;
        if (length == 0) { return string.Empty; }
        if (length > _maximumStringLength) { return new(chars); }

        var hashCode = SepHash.Default(chars);

#if SEPSTRINGPOOL_CACHE_LAST
        var (lastHashCode, lastString) = _last.Value;
        if (lastHashCode == hashCode && MemoryExtensions.SequenceEqual(chars, lastString))
        {
            return lastString;
        }
#endif
        ref var bucket = ref GetBucket(hashCode);

        var entries = _entries;
        ref var entriesRef = ref MemoryMarshal.GetArrayDataReference(entries);
        var entriesLength = entries.Length;

        var i = bucket - 1;
        uint collisionCount = 0;
        while ((uint)i < (uint)_count)
        {
            ref var e = ref Unsafe.Add(ref entriesRef, i);
            if (e.HashCode == hashCode && MemoryExtensions.SequenceEqual(chars, e.String.AsSpan()))
            {
#if SEPSTRINGPOOLUSAGE
                e.Count++;
#endif
#if SEPSTRINGPOOL_CACHE_LAST
                _last.Value = (hashCode, e.String);
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
        _last.Value = (hashCode, stringValue);
#endif

        // Add only if more room
        if (_count < entriesLength)
        {
            // Lock during add of new entry
            lock (this)
            {
                var index = _count;
                if (index < entriesLength)
                {
                    ref var entry = ref Unsafe.Add(ref entriesRef, index);
                    entry.HashCode = hashCode;
#if SEPSTRINGPOOLUSAGE
                    entry.Count = 1;
#endif
                    entry.Next = bucket - 1;
                    entry.String = stringValue;

                    bucket = index + 1; // bucket is an int ref
                    _count = index + 1;
                }
            }
        }

        return stringValue;
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
        ArrayPool<int>.Shared.Return(_buckets);
        ArrayPool<Entry>.Shared.Return(_entries);
#if SEPSTRINGPOOL_CACHE_LAST
        _last.Dispose();
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
