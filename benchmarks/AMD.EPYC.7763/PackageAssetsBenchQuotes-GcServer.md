```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 7763 3.21GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.77 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  
Error=NA  RatioSD=?  

```
| Method    | Scope | Rows    | Mean | Ratio | MB  | MB/s | ns/row | Alloc Ratio |
|---------- |------ |-------- |-----:|------:|----:|-----:|-------:|------------:|
| Sep______ | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_MT___ | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sylvan___ | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| ReadLine_ | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| CsvHelper | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
|           |       |         |      |       |     |      |        |             |
| Sep______ | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_MT___ | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sylvan___ | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| ReadLine_ | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| CsvHelper | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |

Benchmarks with issues:
  GcServerQuotesAssetPackageAssetsBench.Sep______: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  GcServerQuotesAssetPackageAssetsBench.Sep_MT___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  GcServerQuotesAssetPackageAssetsBench.Sylvan___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  GcServerQuotesAssetPackageAssetsBench.ReadLine_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  GcServerQuotesAssetPackageAssetsBench.CsvHelper: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  GcServerLongQuotesAssetPackageAssetsBench.Sep______: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
  GcServerLongQuotesAssetPackageAssetsBench.Sep_MT___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
  GcServerLongQuotesAssetPackageAssetsBench.Sylvan___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
  GcServerLongQuotesAssetPackageAssetsBench.ReadLine_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
  GcServerLongQuotesAssetPackageAssetsBench.CsvHelper: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
