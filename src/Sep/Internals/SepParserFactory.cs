using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace nietras.SeparatedValues;

readonly record struct SepParserConfig(char Separator, char QuotesOrSeparatorIfDisabled)
{
    internal SepParserConfig(Sep sep)
        : this(sep.Separator, SepDefaults.Quote)
    { }
    internal SepParserConfig(Sep sep, bool disableQuotesParsing)
        : this(sep.Separator, disableQuotesParsing ? sep.Separator : SepDefaults.Quote)
    { }
}

static class SepParserFactory
{
    const string SepForceParserEnvName = "SEPFORCEPARSER";

    static SepParserFactory()
    {
        CreateBest = CreateBestFunc();
    }

    [ExcludeFromCodeCoverage]
    internal static Func<SepParserConfig, ISepParser> CreateBest { get; }

    [ExcludeFromCodeCoverage]
    internal static Func<SepParserConfig, ISepParser> CreateBestFunc()
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

    internal static IReadOnlyDictionary<string, Func<SepParserConfig, ISepParser>> AcceleratedFactories { get; } = CreateFactories(createUnaccelerated: false);
    internal static IReadOnlyDictionary<string, Func<SepParserConfig, ISepParser>> AvailableFactories { get; } = CreateFactories(createUnaccelerated: true);

    static IReadOnlyDictionary<string, Func<SepParserConfig, ISepParser>> CreateFactories(bool createUnaccelerated)
    {
        var parsers = new Dictionary<string, Func<SepParserConfig, ISepParser>>();
        AddFactories(parsers, createUnaccelerated);
        return parsers;
    }

    static void AddFactories<TCollection>(TCollection parsers, bool createUnaccelerated)
        where TCollection : ICollection<KeyValuePair<string, Func<SepParserConfig, ISepParser>>>
    {
#if NET8_0_OR_GREATER
        if (Environment.Is64BitProcess && Avx512BW.IsSupported)
        { Add(parsers, static sep => new SepParserAvx512PackCmpOrMoveMaskTzcnt(sep)); }
        if (Environment.Is64BitProcess && (createUnaccelerated || Vector512.IsHardwareAccelerated))
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
    }

    static void Add<TCollection, TParser>(TCollection parsers, Func<SepParserConfig, TParser> create)
        where TParser : ISepParser
        where TCollection : ICollection<KeyValuePair<string, Func<SepParserConfig, ISepParser>>>
    {
        parsers.Add(new(typeof(TParser).Name, sep => create(sep)));
    }
}
