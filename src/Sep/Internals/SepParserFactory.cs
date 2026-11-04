using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace nietras.SeparatedValues;

static class SepParserFactory
{
    internal const string SepForceParserEnvName = "SEPFORCEPARSER";

    internal static ISepParser Best { get; } = GetBest();

    [ExcludeFromCodeCoverage]
    internal static ISepParser GetBest() => GetBest(GetForceParserName());

    [ExcludeFromCodeCoverage]
    internal static ISepParser GetBest(string forceParserName)
    {
        var selection = new SelectParser(forceParserName);
        if (forceParserName.Length != 0)
        {
            AddParsers(ref selection, createUnaccelerated: true);
            if (selection.Parser is { } forced)
            {
                Trace.WriteLine($"Forcing use of parser '{forceParserName}'");
                return forced;
            }
        }
        selection = new SelectParser(string.Empty);
        AddParsers(ref selection, createUnaccelerated: false);
        return selection.Parser!;
    }

    internal static string GetForceParserName() => Environment.GetEnvironmentVariable(SepForceParserEnvName) ?? string.Empty;

    internal static IReadOnlyDictionary<string, ISepParser> CreateAvailableParsers() => CreateParsers(true);
    internal static IReadOnlyDictionary<string, ISepParser> CreateAcceleratedParsers() => CreateParsers(false);

    static Dictionary<string, ISepParser> CreateParsers(bool createUnaccelerated)
    {
        var collection = new CollectParsers();
        AddParsers(ref collection, createUnaccelerated);
        return collection.Parsers;
    }

    static void AddParsers<TCollector>(ref TCollector parsers, bool createUnaccelerated)
        where TCollector : struct, IParserCollector
    {
#if NET8_0_OR_GREATER
        // AVX-512 to 256 avoids mask register code gen issues
        if (Avx512BW.IsSupported)
        { if (!parsers.TryAdd<SepParserAvx512To256CmpOrMoveMaskTzcnt>(nameof(SepParserAvx512To256CmpOrMoveMaskTzcnt))) { return; } }
#endif
        // Avx2 and Vector256 are faster than most AVX-512 due to mask register code gen issues
        if (Avx2.IsSupported)
        { if (!parsers.TryAdd<SepParserAvx2PackCmpOrMoveMaskTzcnt>(nameof(SepParserAvx2PackCmpOrMoveMaskTzcnt))) { return; } }
        if (createUnaccelerated || Vector256.IsHardwareAccelerated)
        { if (!parsers.TryAdd<SepParserVector256NrwCmpExtMsbTzcnt>(nameof(SepParserVector256NrwCmpExtMsbTzcnt))) { return; } }
#if NET8_0_OR_GREATER
        if (Environment.Is64BitProcess && Avx512BW.IsSupported)
        { if (!parsers.TryAdd<SepParserAvx512PackCmpOrMoveMaskTzcnt>(nameof(SepParserAvx512PackCmpOrMoveMaskTzcnt))) { return; } }
        if (Avx512BW.VL.IsSupported)
        { if (!parsers.TryAdd<SepParserAvx256To128CmpOrMoveMaskTzcnt>(nameof(SepParserAvx256To128CmpOrMoveMaskTzcnt))) { return; } }
        if (Environment.Is64BitProcess && (createUnaccelerated || Vector512.IsHardwareAccelerated))
        { if (!parsers.TryAdd<SepParserVector512NrwCmpExtMsbTzcnt>(nameof(SepParserVector512NrwCmpExtMsbTzcnt))) { return; } }
        if (Environment.Is64BitProcess && AdvSimd.Arm64.IsSupported)
        { if (!parsers.TryAdd<SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt>(nameof(SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt))) { return; } }
#endif
#if NET9_0_OR_GREATER
        if (Environment.Is64BitProcess && AdvSimd.Arm64.IsSupported)
        { if (!parsers.TryAdd<SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt>(nameof(SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt))) { return; } }
#endif
        if (Sse2.IsSupported)
        { if (!parsers.TryAdd<SepParserSse2PackCmpOrMoveMaskTzcnt>(nameof(SepParserSse2PackCmpOrMoveMaskTzcnt))) { return; } }
        if (createUnaccelerated || Vector128.IsHardwareAccelerated)
        { if (!parsers.TryAdd<SepParserVector128NrwCmpExtMsbTzcnt>(nameof(SepParserVector128NrwCmpExtMsbTzcnt))) { return; } }
        if (createUnaccelerated || Vector64.IsHardwareAccelerated)
        { if (!parsers.TryAdd<SepParserVector64NrwCmpExtMsbTzcnt>(nameof(SepParserVector64NrwCmpExtMsbTzcnt))) { return; } }
        parsers.TryAdd<SepParserIndexOfAny>(nameof(SepParserIndexOfAny));
    }

    interface IParserCollector
    {
        bool TryAdd<TParser>(string name) where TParser : ISepParser, new();
    }

    struct SelectParser(string name) : IParserCollector
    {
        internal ISepParser? Parser;

        public bool TryAdd<TParser>(string parserName) where TParser : ISepParser, new()
        {
            if (name.Length != 0 && name != parserName) { return true; }
            Parser = new TParser();
            return false;
        }
    }

    readonly struct CollectParsers() : IParserCollector
    {
        internal Dictionary<string, ISepParser> Parsers { get; } = [];

        public bool TryAdd<TParser>(string name) where TParser : ISepParser, new()
        {
            Parsers.Add(name, new TParser());
            return true;
        }
    }
}
