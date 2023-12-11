using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace nietras.SeparatedValues.Benchmarks;

public class SepParserBench
{
    public record FillerSpec(string Name, char Separator, string Text)
    {
        public override string ToString() => $"{Name} {Text.Length:D8}";
    }

    public record ParserSpec(string Name, Func<Sep, object> CreateParser)
    {
        public override string ToString() => Name;
    }

#pragma warning disable CA1823 // Avoid unused private fields
    const string PackageAssetsText = @"75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,RuntimeAssemblies,,,net5.0,,,,,,lib/net5.0/BlazorGrid.dll,BlazorGrid.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0\r\n";
    const string FloatsText = @"SetCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC;wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww.png;Train;0.52276427;0.16843422;0.26259267;0.7244084;0.51292276;0.17365117;0.76125056;0.23458846;0.2573214;0.50560355;0.3202332;0.3809696;0.26024464;0.5174511;0.035318818;0.8141374;0.57719684;0.3974705;0.15219308;0.09011261;0.70515215;0.81618196;0.5399706;0.044147138;0.7111546;0.14776127;0.90621275;0.6925897;0.5164137;0.18637845;0.041509967;0.30819967;0.5831603;0.8210651;0.003954861;0.535722;0.8051845;0.7483589;0.3845737;0.14911908\r\n";
#pragma warning restore CA1823 // Avoid unused private fields

    readonly ParserSpec[] _parsers;
    readonly FillerSpec[] _fillers;
    ISepParser? _parser;
    readonly int _parserPaddingLengthMax = 32;
    SepReaderState? _state;

    public SepParserBench()
    {
#pragma warning disable CA1307 // Specify StringComparison for clarity
        _parsers = SepParserFactory.AcceleratedFactories.Select(p =>
            new ParserSpec(p.Key.Replace("SepParser", "").Replace("SepParserVector", ""), p.Value)).ToArray();
#pragma warning restore CA1307 // Specify StringComparison for clarity
        _parserPaddingLengthMax = Math.Max(_parserPaddingLengthMax,
            _parsers.Select(p => ((ISepParser)p.CreateParser(Sep.Default)).PaddingLength).Max());
#if false //DEBUG
        _fillers = new FillerSpec[]
        {
            new("Sequence", ';', FillSequence)
        };
#else
        _fillers = new FillerSpec[]
        {
            new("0_Assets___", ',', PackageAssetsText),
            //new("1_Floats___", ';', FloatsText),
            //new("1_ManyChars", ';', FillFor(";\n\r\"____;___\n___" + "\r___\"___####ˉˉˉˉ")),
            //new("2_FewSep___", ';', FillFor("ˉˉ;ˉˉˉˉˉˉˉˉˉˉˉˉ;ˉˉˉˉˉˉˉˉˉˉˉ;ˉˉˉˉˉˉˉˉˉ;ˉˉˉˉˉ")),
            //new("3_NoChars__", ';', FillFor("ˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉ")),
            //new("4_FullSeps_", ';', FillFor(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;")),
        };
#endif
        Parser = _parsers.First();
        Filler = _fillers.First();
        GlobalSetup();
    }

    [ParamsSource(nameof(Parsers))]
    public ParserSpec Parser { get; set; }
    public IEnumerable<ParserSpec> Parsers() => _parsers;

    [ParamsSource(nameof(Fillers))]
    public FillerSpec Filler { get; set; }
    public IEnumerable<FillerSpec> Fillers() => _fillers;

    public int Length => Filler.Text.Length;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _parser = (ISepParser)Parser.CreateParser(new(Filler.Separator));
        var text = Filler.Text;
        _state = new SepReaderState();
        _state._chars = ArrayPool<char>.Shared.Rent(text.Length + _parser.PaddingLength);
        text.AsSpan().CopyTo(_state._chars.AsSpan().Slice(0, text.Length));
        _state._charsDataEnd = text.Length;
        _state._colEndsOrColInfos = ArrayPool<int>.Shared.Rent(SepReaderState.ColEndsInitialLength);
    }

    [Benchmark(Baseline = true)]
    public void ParseColEnds()
    {
        _state!._currentRowColCount = 0;
        _state!._parsingLineNumber = 0;
        _state!._charsParseStart = 0;
        _parser!.ParseColEnds(_state!);
    }

    //[Benchmark]
    public void Copy()
    {
        Filler.Text.CopyTo(_state!._chars.AsSpan(0, _state._charsDataEnd));
    }
}
