﻿using System;
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
        if (Avx2.IsSupported) { return new SepParserAvx2PackCmpOrMoveMaskTzcnt(sep); }
        if (Sse2.IsSupported) { return new SepParserSse2PackCmpOrMoveMaskTzcnt(sep); }
        if (Vector256.IsHardwareAccelerated) { return new SepParserVector256NrwCmpExtMsbTzcnt(sep); }
        if (Vector128.IsHardwareAccelerated) { return new SepParserVector128NrwCmpExtMsbTzcnt(sep); }
        if (Vector64.IsHardwareAccelerated) { return new SepParserVector64NrwCmpExtMsbTzcnt(sep); }
        return new SepParserIndexOfAny(sep);
    }

    internal static IReadOnlyDictionary<Type, Func<Sep, ISepParser>> CreateAcceleratedFactories()
        => CreateFactories(createUnaccelerated: false);

    internal static IReadOnlyDictionary<Type, Func<Sep, ISepParser>> CreateFactories(bool createUnaccelerated = true)
    {
        var parsers = new Dictionary<Type, Func<Sep, ISepParser>>();
        if (Avx2.IsSupported)
        { Add(parsers, static sep => new SepParserAvx2PackCmpOrMoveMaskTzcnt(sep)); }
        if (Sse2.IsSupported)
        { Add(parsers, static sep => new SepParserSse2PackCmpOrMoveMaskTzcnt(sep)); }
        if (createUnaccelerated || Vector256.IsHardwareAccelerated)
        { Add(parsers, static sep => new SepParserVector256NrwCmpExtMsbTzcnt(sep)); }
        if (createUnaccelerated || Vector128.IsHardwareAccelerated)
        { Add(parsers, static sep => new SepParserVector128NrwCmpExtMsbTzcnt(sep)); }
        if (createUnaccelerated || Vector64.IsHardwareAccelerated)
        { Add(parsers, static sep => new SepParserVector64NrwCmpExtMsbTzcnt(sep)); }
        Add(parsers, static sep => new SepParserIndexOfAny(sep));
        return parsers;
    }

    static void Add<TParser>(Dictionary<Type, Func<Sep, ISepParser>> parsers, Func<Sep, TParser> create)
        where TParser : ISepParser
    {
        parsers.Add(typeof(TParser), sep => create(sep));
    }
}
