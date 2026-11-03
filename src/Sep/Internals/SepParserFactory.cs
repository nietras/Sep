using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace nietras.SeparatedValues;

static class SepParserFactory
{
    internal const int MaxPaddingLength = 128;
    internal const string SepForceParserEnvName = "SEPFORCEPARSER";

    static readonly Lazy<ISepParser> _best = new(GetBest);

    [ExcludeFromCodeCoverage]
    internal static ISepParser Best => _best.Value;

    [ExcludeFromCodeCoverage]
    internal static ISepParser GetBest()
    {
        var forceParserName = GetForceParserName();
        if (AvailableParsers.TryGetValue(forceParserName, out var parser))
        {
            Trace.WriteLine($"Forcing use of parser '{forceParserName}'");
            return parser;
        }
        return AcceleratedParsers.Values.First();
    }

    internal static string GetForceParserName() => Environment.GetEnvironmentVariable(SepForceParserEnvName) ?? string.Empty;

    internal static IReadOnlyDictionary<string, ISepParser> AvailableParsers { get; } =
        CreateFactories(createUnaccelerated: true).ToDictionary(p => p.Key, p => p.Value());
    internal static IReadOnlyDictionary<string, ISepParser> AcceleratedParsers { get; } =
        CreateFactories(createUnaccelerated: false).Keys.ToDictionary(name => name, name => AvailableParsers[name]);

    static IReadOnlyDictionary<string, Func<ISepParser>> CreateFactories(bool createUnaccelerated)
    {
        var parsers = new Dictionary<string, Func<ISepParser>>();
        AddFactories(parsers, createUnaccelerated);
        return parsers;
    }

    static void AddFactories<TCollection>(TCollection parsers, bool createUnaccelerated)
        where TCollection : ICollection<KeyValuePair<string, Func<ISepParser>>>
    {
#if NET8_0_OR_GREATER
        // AVX-512 to 256 avoids mask register code gen issues
        if (Avx512BW.IsSupported)
        { Add(parsers, nameof(SepParserAvx512To256CmpOrMoveMaskTzcnt), static () => new SepParserAvx512To256CmpOrMoveMaskTzcnt()); }
#endif
        // Avx2 and Vector256 are faster than most AVX-512 due to mask register code gen issues
        if (Avx2.IsSupported)
        { Add(parsers, nameof(SepParserAvx2PackCmpOrMoveMaskTzcnt), static () => new SepParserAvx2PackCmpOrMoveMaskTzcnt()); }
        if (createUnaccelerated || Vector256.IsHardwareAccelerated)
        { Add(parsers, nameof(SepParserVector256NrwCmpExtMsbTzcnt), static () => new SepParserVector256NrwCmpExtMsbTzcnt()); }
#if NET8_0_OR_GREATER
        if (Environment.Is64BitProcess && Avx512BW.IsSupported)
        { Add(parsers, nameof(SepParserAvx512PackCmpOrMoveMaskTzcnt), static () => new SepParserAvx512PackCmpOrMoveMaskTzcnt()); }
        if (Avx512BW.VL.IsSupported)
        { Add(parsers, nameof(SepParserAvx256To128CmpOrMoveMaskTzcnt), static () => new SepParserAvx256To128CmpOrMoveMaskTzcnt()); }
        if (Environment.Is64BitProcess && (createUnaccelerated || Vector512.IsHardwareAccelerated))
        { Add(parsers, nameof(SepParserVector512NrwCmpExtMsbTzcnt), static () => new SepParserVector512NrwCmpExtMsbTzcnt()); }
        if (Environment.Is64BitProcess && AdvSimd.Arm64.IsSupported)
        { Add(parsers, nameof(SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt), static () => new SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt()); }
#endif
#if NET9_0_OR_GREATER
        if (Environment.Is64BitProcess && AdvSimd.Arm64.IsSupported)
        { Add(parsers, nameof(SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt), static () => new SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt()); }
#endif
        if (Sse2.IsSupported)
        { Add(parsers, nameof(SepParserSse2PackCmpOrMoveMaskTzcnt), static () => new SepParserSse2PackCmpOrMoveMaskTzcnt()); }
        if (createUnaccelerated || Vector128.IsHardwareAccelerated)
        { Add(parsers, nameof(SepParserVector128NrwCmpExtMsbTzcnt), static () => new SepParserVector128NrwCmpExtMsbTzcnt()); }
        if (createUnaccelerated || Vector64.IsHardwareAccelerated)
        { Add(parsers, nameof(SepParserVector64NrwCmpExtMsbTzcnt), static () => new SepParserVector64NrwCmpExtMsbTzcnt()); }
        Add(parsers, nameof(SepParserIndexOfAny), static () => new SepParserIndexOfAny());
    }

    static void Add<TCollection, TParser>(TCollection parsers, string name, Func<TParser> create)
        where TParser : ISepParser
        where TCollection : ICollection<KeyValuePair<string, Func<ISepParser>>>
    {
        parsers.Add(new(name, () => create()));
    }
}
