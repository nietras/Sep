```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9711/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.88 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  
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
  GcServerAssetPackageAssetsBench.Sep______: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=50000]
  GcServerAssetPackageAssetsBench.Sep_MT___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=50000]
  GcServerAssetPackageAssetsBench.Sylvan___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=50000]
  GcServerAssetPackageAssetsBench.ReadLine_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=50000]
  GcServerAssetPackageAssetsBench.CsvHelper: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=50000]
  GcServerLongAssetPackageAssetsBench.Sep______: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=1000000]
  GcServerLongAssetPackageAssetsBench.Sep_MT___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=1000000]
  GcServerLongAssetPackageAssetsBench.Sylvan___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=1000000]
  GcServerLongAssetPackageAssetsBench.ReadLine_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=1000000]
  GcServerLongAssetPackageAssetsBench.CsvHelper: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Server=True, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=False, Reader=String, Rows=1000000]
