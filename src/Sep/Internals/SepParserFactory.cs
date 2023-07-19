using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace nietras.SeparatedValues;

static class SepParserFactory
{
    [ExcludeFromCodeCoverage]
    internal static ISepParser CreateBest(Sep sep)
    {
        if (Avx2.IsSupported) { return new SepCharsFinderAvx2PackCmpOrMoveMaskTzcnt(sep); }
        if (Sse2.IsSupported) { return new SepCharsFinderSse2PackCmpOrMoveMaskTzcnt(sep); }
        if (Vector256.IsHardwareAccelerated) { return new SepCharsFinderVector256NrwCmpExtMsbTzcnt(sep); }
        if (Vector128.IsHardwareAccelerated) { return new SepCharsFinderVector128NrwCmpExtMsbTzcnt(sep); }
        if (Vector64.IsHardwareAccelerated) { return new SepCharsFinderVector64NrwCmpExtMsbTzcnt(sep); }
        return new SepCharsFinderIndexOfAny(sep);
    }

    internal static IReadOnlyDictionary<Type, Func<Sep, ISepParser>> CreateAcceleratedFactories()
        => CreateFactories(createUnaccelerated: false);

    internal static IReadOnlyDictionary<Type, Func<Sep, ISepParser>> CreateFactories(bool createUnaccelerated = true)
    {
        var parsers = new Dictionary<Type, Func<Sep, ISepParser>>();
        if (Avx2.IsSupported)
        { Add(parsers, static sep => new SepCharsFinderAvx2PackCmpOrMoveMaskTzcnt(sep)); }
        if (Sse2.IsSupported)
        { Add(parsers, static sep => new SepCharsFinderSse2PackCmpOrMoveMaskTzcnt(sep)); }
        if (createUnaccelerated || Vector256.IsHardwareAccelerated)
        { Add(parsers, static sep => new SepCharsFinderVector256NrwCmpExtMsbTzcnt(sep)); }
        if (createUnaccelerated || Vector128.IsHardwareAccelerated)
        { Add(parsers, static sep => new SepCharsFinderVector128NrwCmpExtMsbTzcnt(sep)); }
        if (createUnaccelerated || Vector64.IsHardwareAccelerated)
        { Add(parsers, static sep => new SepCharsFinderVector64NrwCmpExtMsbTzcnt(sep)); }
        Add(parsers, static sep => new SepCharsFinderIndexOfAny(sep));
        return parsers;
    }

    static void Add<TParser>(Dictionary<Type, Func<Sep, ISepParser>> parsers, Func<Sep, TParser> create)
        where TParser : ISepParser
    {
        parsers.Add(typeof(TParser), sep => create(sep));
    }
}
