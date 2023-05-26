using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace nietras.SeparatedValues;

static class SepCharsFinderFactory
{
    static readonly ConcurrentDictionary<Sep, ISepCharsFinder> s_sepToBestFinder = new();

    [ExcludeFromCodeCoverage]
    internal static ISepCharsFinder GetBest(Sep sep)
    {
        if (!s_sepToBestFinder.TryGetValue(sep, out var finder))
        {
            finder = CreateBestInternal(sep);
            s_sepToBestFinder.TryAdd(sep, finder);
        }
        return finder;
    }

    [ExcludeFromCodeCoverage]
    static ISepCharsFinder CreateBestInternal(Sep sep)
    {
        if (Avx2.IsSupported) { return new SepCharsFinderAvx2PackCmpOrMoveMaskTzcnt(sep); }
        if (Vector256.IsHardwareAccelerated) { return new SepCharsFinderVector256NrwCmpExtMsbTzcnt(sep); }
        if (Sse2.IsSupported) { return new SepCharsFinderSse2PackCmpOrMoveMaskTzcnt(sep); }
        if (Vector128.IsHardwareAccelerated) { return new SepCharsFinderVector128NrwCmpExtMsbTzcnt(sep); }
        if (Vector64.IsHardwareAccelerated) { return new SepCharsFinderVector64NrwCmpExtMsbTzcnt(sep); }
        if (Vector.IsHardwareAccelerated) { return new SepCharsFinderVectorNrwCmpULongTzcnt(sep); }
        return new SepCharsFinderIndexOfAny(sep);
    }

    internal static IReadOnlyDictionary<Type, Func<Sep, ISepCharsFinder>> CreateAcceleratedFactories()
        => CreateFactories(createUnaccelerated: false);

    internal static IReadOnlyDictionary<Type, Func<Sep, ISepCharsFinder>> CreateFactories(bool createUnaccelerated = true)
    {
        var finders = new Dictionary<Type, Func<Sep, ISepCharsFinder>>();
        if (Avx2.IsSupported)
        {
            Add(finders, static sep => new SepCharsFinderAvx2PackCmpOrMoveMaskTzcnt(sep));
            Add(finders, static sep => new SepCharsFinderAvx2PackShuffleMoveMaskTzcnt(sep));
            Add(finders, static sep => new SepCharsFinderAvx2PackCmpOrMoveMaskLeftPack(sep));
        }
        if (Sse2.IsSupported)
        {
            Add(finders, static sep => new SepCharsFinderSse2PackCmpOrMoveMaskTzcnt(sep));
        }
        if (createUnaccelerated || Vector256.IsHardwareAccelerated)
        { Add(finders, static sep => new SepCharsFinderVector256NrwCmpExtMsbTzcnt(sep)); }
        if (createUnaccelerated || Vector128.IsHardwareAccelerated)
        { Add(finders, static sep => new SepCharsFinderVector128NrwCmpExtMsbTzcnt(sep)); }
        if (createUnaccelerated || Vector64.IsHardwareAccelerated)
        { Add(finders, static sep => new SepCharsFinderVector64NrwCmpExtMsbTzcnt(sep)); }
        if (createUnaccelerated || Vector.IsHardwareAccelerated)
        { Add(finders, static sep => new SepCharsFinderVectorNrwCmpULongTzcnt(sep)); }
        Add(finders, static sep => new SepCharsFinderIndexOfAny(sep));
        return finders;
    }

    static void Add<TFinder>(Dictionary<Type, Func<Sep, ISepCharsFinder>> finders, Func<Sep, TFinder> create)
        where TFinder : ISepCharsFinder
    {
        finders.Add(typeof(TFinder), sep => create(sep));
    }
}
