using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace nietras.SeparatedValues;

static class SepParserFactory
{
    static SepParserFactory()
    {
        CreateBest = CreateBestFunc();
    }

    [ExcludeFromCodeCoverage]
    internal static Func<Sep, ISepParser> CreateBest { get; }

    [ExcludeFromCodeCoverage]
    internal static Func<Sep, ISepParser> CreateBestFunc()
    {
        var forceParserName = Environment.GetEnvironmentVariable("SEPFORCEPARSER") ?? string.Empty;
        if (AvailableFactories.TryGetValue(forceParserName, out var createParser))
        {
            Trace.WriteLine($"Forcing use of parser '{forceParserName}'");
            return createParser;
        }
#if NET8_0_OR_GREATER
        if (Environment.Is64BitProcess && Avx512BW.IsSupported)
        { return static sep => new SepParserAvx512PackCmpOrMoveMaskTzcnt(sep); }
        if (Environment.Is64BitProcess && Vector512.IsHardwareAccelerated)
        { return static sep => new SepParserVector512NrwCmpExtMsbTzcnt(sep); }
#endif
        if (Avx2.IsSupported) { return static sep => new SepParserAvx2PackCmpOrMoveMaskTzcnt(sep); }
        if (Sse2.IsSupported) { return static sep => new SepParserSse2PackCmpOrMoveMaskTzcnt(sep); }
        if (Vector256.IsHardwareAccelerated) { return static sep => new SepParserVector256NrwCmpExtMsbTzcnt(sep); }
        if (Vector128.IsHardwareAccelerated) { return static sep => new SepParserVector128NrwCmpExtMsbTzcnt(sep); }
        if (Vector64.IsHardwareAccelerated) { return static sep => new SepParserVector64NrwCmpExtMsbTzcnt(sep); }
        return static sep => new SepParserIndexOfAny(sep);
        //throw new NotImplementedException();
    }

    internal static IReadOnlyDictionary<string, Func<Sep, ISepParser>> AcceleratedFactories { get; } = CreateFactories(createUnaccelerated: false);
    internal static IReadOnlyDictionary<string, Func<Sep, ISepParser>> AvailableFactories { get; } = CreateFactories(createUnaccelerated: true);

    static IReadOnlyDictionary<string, Func<Sep, ISepParser>> CreateFactories(bool createUnaccelerated)
    {
        var parsers = new Dictionary<string, Func<Sep, ISepParser>>();
#if NET8_0_OR_GREATER
        if (Environment.Is64BitProcess && Avx512BW.IsSupported)
        { Add(parsers, static sep => new SepParserAvx512PackCmpOrMoveMaskTzcnt(sep)); }
        if (Environment.Is64BitProcess)
        { Add(parsers, static sep => new SepParserVector512NrwCmpExtMsbTzcnt(sep)); }
#endif
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

    static void Add<TParser>(Dictionary<string, Func<Sep, ISepParser>> parsers, Func<Sep, TParser> create)
        where TParser : ISepParser
    {
        parsers.Add(typeof(TParser).Name, sep => create(sep));
    }
}
