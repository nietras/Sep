using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
#if DEBUG
using Pos = nietras.SeparatedValues.SepCharPosition;
#else
using Pos = System.Int32;
#endif

namespace nietras.SeparatedValues.Benchmarks;

[HideColumns("Method")]
public class SepCharsFinderBench
{
    public delegate void Fill(Span<char> chars);

    public record FillerSpec(string Name, char Separator, Fill Fill)
    {
        public override string ToString() => Name;
    }

    public record FinderSpec(string Name, Func<Sep, object> CreateParser)
    {
        public override string ToString() => Name;
    }

#pragma warning disable CA1823 // Avoid unused private fields
    const string PackageAssetsText = @"75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,RuntimeAssemblies,,,net5.0,,,,,,lib/net5.0/BlazorGrid.dll,BlazorGrid.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,CompileLibAssemblies,,,net5.0,,,,,,lib/net5.0/BlazorGrid.dll,BlazorGrid.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ResourceAssemblies,,,net5.0,,,,,,lib/net5.0/de/BlazorGrid.resources.dll,BlazorGrid.resources.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
5a09800b-a7ff-4440-9d52-403c7bb39a73,2020-11-28T01:49:52.6008922+00:00,Brahma.FSharp.OpenCL.Printer,2.0.0-alpha3,2020-11-27T21:33:22.1200000+00:00,AvailableAssets,CompileLibAssemblies,,,net461,,,,,,lib/net461/Brahma.FSharp.OpenCL.Printer.dll,Brahma.FSharp.OpenCL.Printer.dll,.dll,lib,net461,.NETFramework,4.6.1.0,,,0.0.0.0
5a09800b-a7ff-4440-9d52-403c7bb39a73,2020-11-28T01:49:52.6008922+00:00,Brahma.FSharp.OpenCL.Printer,2.0.0-alpha3,2020-11-27T21:33:22.1200000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/Brahma.FSharp.OpenCL.Printer.dll,Brahma.FSharp.OpenCL.Printer.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
5a4075f8-c0b5-4458-aa6f-0937912389c2,2020-11-28T01:49:22.0669525+00:00,Bruteflow,2.1.1,2020-11-27T21:00:45.0200000+00:00,AvailableAssets,RuntimeAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Bruteflow.dll,Bruteflow.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
5a4075f8-c0b5-4458-aa6f-0937912389c2,2020-11-28T01:49:22.0669525+00:00,Bruteflow,2.1.1,2020-11-27T21:00:45.0200000+00:00,AvailableAssets,CompileLibAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/Bruteflow.dll,Bruteflow.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
6abd9023-8925-44fd-8290-fddfd85ebadf,2020-11-28T01:48:53.3506710+00:00,DNT.Deskly.FluentValidation,1.1.2,2020-11-27T20:29:21.7430000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/DNT.Deskly.FluentValidation.dll,DNT.Deskly.FluentValidation.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
6abd9023-8925-44fd-8290-fddfd85ebadf,2020-11-28T01:48:53.3506710+00:00,DNT.Deskly.FluentValidation,1.1.2,2020-11-27T20:29:21.7430000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/DNT.Deskly.FluentValidation.dll,DNT.Deskly.FluentValidation.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
39d151a6-ae83-432f-a88d-993f5a92d5a8,2020-11-28T01:45:41.6716781+00:00,DNT.Deskly.Web,1.1.0,2020-11-27T19:36:41.5530000+00:00,AvailableAssets,RuntimeAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/DNT.Deskly.Web.dll,DNT.Deskly.Web.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
39d151a6-ae83-432f-a88d-993f5a92d5a8,2020-11-28T01:45:41.6716781+00:00,DNT.Deskly.Web,1.1.0,2020-11-27T19:36:41.5530000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/DNT.Deskly.Web.dll,DNT.Deskly.Web.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
a00e8166-6f7d-4c8c-a6f3-f0f6a8202b26,2020-11-28T01:50:41.8598561+00:00,IbanNet,4.3.0,2020-11-27T22:43:43.2900000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/IbanNet.dll,IbanNet.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
a00e8166-6f7d-4c8c-a6f3-f0f6a8202b26,2020-11-28T01:50:41.8598561+00:00,IbanNet,4.3.0,2020-11-27T22:43:43.2900000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/IbanNet.dll,IbanNet.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
82db9bff-c066-4fec-8783-a495be2cc5e9,2020-11-28T01:50:43.3448103+00:00,IbanNet.DataAnnotations,4.3.0,2020-11-27T22:43:43.8030000+00:00,AvailableAssets,RuntimeAssemblies,,,net45,,,,,,lib/net45/IbanNet.DataAnnotations.dll,IbanNet.DataAnnotations.dll,.dll,lib,net45,.NETFramework,4.5.0.0,,,0.0.0.0
82db9bff-c066-4fec-8783-a495be2cc5e9,2020-11-28T01:50:43.3448103+00:00,IbanNet.DataAnnotations,4.3.0,2020-11-27T22:43:43.8030000+00:00,AvailableAssets,RuntimeAssemblies,,,net47,,,,,,lib/net47/IbanNet.DataAnnotations.dll,IbanNet.DataAnnotations.dll,.dll,lib,net47,.NETFramework,4.7.0.0,,,0.0.0.0
7b19e292-f78c-4b0c-bc56-f9fdc454099c,2020-11-28T01:50:44.8656299+00:00,IbanNet.DependencyInjection.ServiceProvider,4.3.0,2020-11-27T22:43:45.1970000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.0,,,,,,lib/netstandard2.0/IbanNet.DependencyInjection.ServiceProvider.dll,IbanNet.DependencyInjection.ServiceProvider.dll,.dll,lib,netstandard2.0,.NETStandard,2.0.0.0,,,0.0.0.0
7b19e292-f78c-4b0c-bc56-f9fdc454099c,2020-11-28T01:50:44.8656299+00:00,IbanNet.DependencyInjection.ServiceProvider,4.3.0,2020-11-27T22:43:45.1970000+00:00,AvailableAssets,CompileLibAssemblies,,,netstandard2.1,,,,,,lib/netstandard2.1/IbanNet.DependencyInjection.ServiceProvider.dll,IbanNet.DependencyInjection.ServiceProvider.dll,.dll,lib,netstandard2.1,.NETStandard,2.1.0.0,,,0.0.0.0
097e8769-5154-461f-9473-deb18892e3f5,2020-11-28T01:50:44.2346927+00:00,IbanNet.FluentValidation,4.3.0,2020-11-27T22:43:44.2730000+00:00,AvailableAssets,RuntimeAssemblies,,,net461,,,,,,lib/net461/IbanNet.FluentValidation.dll,IbanNet.FluentValidation.dll,.dll,lib,net461,.NETFramework,4.6.1.0,,,0.0.0.0
097e8769-5154-461f-9473-deb18892e3f5,2020-11-28T01:50:44.2346927+00:00,IbanNet.FluentValidation,4.3.0,2020-11-27T22:43:44.2730000+00:00,AvailableAssets,RuntimeAssemblies,,,net5.0,,,,,,lib/net5.0/IbanNet.FluentValidation.dll,IbanNet.FluentValidation.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0
67f6b493-983b-4c83-a6f7-87091e9ff3d0,2020-11-28T01:46:09.0933270+00:00,pi-top.MakerArchitecture.FoundationKit,1.0.352,2020-11-27T20:10:00.6970000+00:00,AvailableAssets,RuntimeAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/PiTop.MakerArchitecture.Foundation.dll,PiTop.MakerArchitecture.Foundation.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
67f6b493-983b-4c83-a6f7-87091e9ff3d0,2020-11-28T01:46:09.0933270+00:00,pi-top.MakerArchitecture.FoundationKit,1.0.352,2020-11-27T20:10:00.6970000+00:00,AvailableAssets,CompileLibAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/PiTop.MakerArchitecture.Foundation.dll,PiTop.MakerArchitecture.Foundation.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
3a094a18-801d-4b32-a941-b19f3a6d4fc6,2020-11-28T01:49:00.8054122+00:00,PlainHttp,1.2.0,2020-11-27T20:41:41.3630000+00:00,AvailableAssets,RuntimeAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/PlainHttp.dll,PlainHttp.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
3a094a18-801d-4b32-a941-b19f3a6d4fc6,2020-11-28T01:49:00.8054122+00:00,PlainHttp,1.2.0,2020-11-27T20:41:41.3630000+00:00,AvailableAssets,CompileLibAssemblies,,,netcoreapp3.1,,,,,,lib/netcoreapp3.1/PlainHttp.dll,PlainHttp.dll,.dll,lib,netcoreapp3.1,.NETCoreApp,3.1.0.0,,,0.0.0.0
";
#pragma warning restore CA1823 // Avoid unused private fields

    readonly FinderSpec[] _finders;
    readonly FillerSpec[] _fillers;
    ISepCharsFinder? _finder;
    readonly int _finderPaddingLengthMax = 32;
    char[]? _chars;
    int _charsEnd;
    Pos[]? _positions;

    public SepCharsFinderBench()
    {
#pragma warning disable CA1307 // Specify StringComparison for clarity
        _finders = SepCharsFinderFactory.CreateAcceleratedFactories().Select(p =>
            new FinderSpec(p.Key.Name.Replace("SepCharsFinder", "").Replace("SepCharsFinderVector", ""), p.Value)).ToArray();
#pragma warning restore CA1307 // Specify StringComparison for clarity
        _finderPaddingLengthMax = Math.Max(_finderPaddingLengthMax,
            _finders.Select(p => ((ISepCharsFinder)p.CreateParser(Sep.Default)).PaddingLength).Max());
#if false //DEBUG
        _fillers = new FillerSpec[]
        {
            new("Sequence", ';', FillSequence)
        };
#else
        _fillers = new FillerSpec[]
        {
            new("0_Assets___", ',', FillFor(PackageAssetsText)),
            new("1_ManyChars", ';', FillFor(";\n\r\"____;___\n___" + "\r___\"___####ˉˉˉˉ")),
            new("2_FewSep___", ';', FillFor("ˉˉ;ˉˉˉˉˉˉˉˉˉˉˉˉ;ˉˉˉˉˉˉˉˉˉˉˉ;ˉˉˉˉˉˉˉˉˉ;ˉˉˉˉˉ")),
            new("3_NoChars__", ';', FillFor("ˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉ")),
            new("4_FullSeps_", ';', FillFor(";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;")),
        };
#endif
        Finder = _finders.First();
        Filler = _fillers.First();
        Length = 64;
        GlobalSetup();
    }

    [ParamsSource(nameof(Parsers))]
    public FinderSpec Finder { get; set; }
    public IEnumerable<FinderSpec> Parsers() => _finders;

    [ParamsSource(nameof(Fillers))]
    public FillerSpec Filler { get; set; }
    public IEnumerable<FillerSpec> Fillers() => _fillers;

    [Params(16 * 1024)]
    public int Length { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _finder = (ISepCharsFinder)Finder.CreateParser(new(Filler.Separator));
        _chars = new char[Length + _finderPaddingLengthMax];
        Filler.Fill(_chars.AsSpan(0, Length));
        _charsEnd = Length;
        _positions = new Pos[_chars.Length];
    }

    [Benchmark(Baseline = true)]
    public void Find()
    {
        var positionsEnd = 0;
        _finder!.Find(_chars!, 0, _charsEnd, _positions!, 0, ref positionsEnd);
    }

    static Fill FillFor(string text) => chars =>
    {
        for (var i = 0; i < chars.Length; i += text.Length)
        {
            var span = text.AsSpan().Slice(0, Math.Min(text.Length, chars.Length - i));
            span.CopyTo(chars.Slice(i, span.Length));
        }
    };

    static void FillSequence(Span<char> chars)
    {
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)i;
        }
    }
}
