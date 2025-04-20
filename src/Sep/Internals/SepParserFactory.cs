using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace nietras.SeparatedValues;

static class SepParserFactory
{
    const string SepForceParserEnvName = "SEPFORCEPARSER";

    static SepParserFactory()
    {
        CreateBest = CreateBestFunc();
    }

    [ExcludeFromCodeCoverage]
    internal static Func<SepParserOptions, ISepParser> CreateBest { get; }

    [ExcludeFromCodeCoverage]
    internal static Func<SepParserOptions, ISepParser> CreateBestFunc()
    {
        var forceParserName = GetForceParserName();
        if (AvailableFactories.TryGetValue(forceParserName, out var createParser))
        {
            Trace.WriteLine($"Forcing use of parser '{forceParserName}'");
            return createParser;
        }
        return AcceleratedFactories.Values.First();
    }

    internal static string GetForceParserName() => Environment.GetEnvironmentVariable(SepForceParserEnvName) ?? string.Empty;

    internal static IReadOnlyDictionary<string, Func<SepParserOptions, ISepParser>> AcceleratedFactories { get; } = CreateFactories(createUnaccelerated: false);
    internal static IReadOnlyDictionary<string, Func<SepParserOptions, ISepParser>> AvailableFactories { get; } = CreateFactories(createUnaccelerated: true);

    static IReadOnlyDictionary<string, Func<SepParserOptions, ISepParser>> CreateFactories(bool createUnaccelerated)
    {
        var parsers = new Dictionary<string, Func<SepParserOptions, ISepParser>>();
        AddFactories(parsers, createUnaccelerated);
        return parsers;
    }

    static void AddFactories<TCollection>(TCollection parsers, bool createUnaccelerated)
        where TCollection : ICollection<KeyValuePair<string, Func<SepParserOptions, ISepParser>>>
    {
#if NET8_0_OR_GREATER
        if (Avx512BW.VL.IsSupported)
        { Add(parsers, nameof(SepParserAvx256To128PackCmpOrMoveMaskTzcnt), static sep => new SepParserAvx256To128PackCmpOrMoveMaskTzcnt(sep)); }
        if (Avx512BW.IsSupported)
        { Add(parsers, nameof(SepParserAvx512To256CmpOrMoveMaskTzcnt), static sep => new SepParserAvx512To256CmpOrMoveMaskTzcnt(sep)); }
        if (Environment.Is64BitProcess && Avx512BW.IsSupported)
        { Add(parsers, nameof(SepParserAvx512PackCmpOrMoveMaskTzcnt), static sep => new SepParserAvx512PackCmpOrMoveMaskTzcnt(sep)); }
        if (Environment.Is64BitProcess && (createUnaccelerated || Vector512.IsHardwareAccelerated))
        { Add(parsers, nameof(SepParserVector512NrwCmpExtMsbTzcnt), static sep => new SepParserVector512NrwCmpExtMsbTzcnt(sep)); }
#endif
        if (Avx2.IsSupported)
        { Add(parsers, nameof(SepParserAvx2PackCmpOrMoveMaskTzcnt), static sep => new SepParserAvx2PackCmpOrMoveMaskTzcnt(sep)); }
        if (Sse2.IsSupported)
        { Add(parsers, nameof(SepParserSse2PackCmpOrMoveMaskTzcnt), static sep => new SepParserSse2PackCmpOrMoveMaskTzcnt(sep)); }
        if (createUnaccelerated || Vector256.IsHardwareAccelerated)
        { Add(parsers, nameof(SepParserVector256NrwCmpExtMsbTzcnt), static sep => new SepParserVector256NrwCmpExtMsbTzcnt(sep)); }
        if (createUnaccelerated || Vector128.IsHardwareAccelerated)
        { Add(parsers, nameof(SepParserVector128NrwCmpExtMsbTzcnt), static sep => new SepParserVector128NrwCmpExtMsbTzcnt(sep)); }
        if (createUnaccelerated || Vector64.IsHardwareAccelerated)
        { Add(parsers, nameof(SepParserVector64NrwCmpExtMsbTzcnt), static sep => new SepParserVector64NrwCmpExtMsbTzcnt(sep)); }
        Add(parsers, nameof(SepParserIndexOfAny), static sep => new SepParserIndexOfAny(sep));
    }

    static void Add<TCollection, TParser>(TCollection parsers, string name, Func<SepParserOptions, TParser> create)
        where TParser : ISepParser
        where TCollection : ICollection<KeyValuePair<string, Func<SepParserOptions, ISepParser>>>
    {
        parsers.Add(new(name, sep => create(sep)));
    }
}
