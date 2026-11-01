```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 7763 3.21GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.77 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  Error=NA  
RatioSD=?  

```
| Method                     | Scope | Rows  | Mean | Ratio | MB  | MB/s | ns/row | Alloc Ratio |
|--------------------------- |------ |------ |-----:|------:|----:|-----:|-------:|------------:|
| Sep_                       | Cols  | 50000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_Trim                   | Cols  | 50000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_TrimUnescape           | Cols  | 50000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_TrimUnescapeTrim       | Cols  | 50000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| CsvHelper_TrimUnescape     | Cols  | 50000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 |   NA |     ? | n/a |  n/a |    n/a |           ? |

Benchmarks with issues:
  SpacesQuotesColsPackageAssetsBench.Sep_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  SpacesQuotesColsPackageAssetsBench.Sep_Trim: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  SpacesQuotesColsPackageAssetsBench.Sep_TrimUnescape: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  SpacesQuotesColsPackageAssetsBench.Sep_TrimUnescapeTrim: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  SpacesQuotesColsPackageAssetsBench.CsvHelper_TrimUnescape: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  SpacesQuotesColsPackageAssetsBench.CsvHelper_TrimUnescapeTrim: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
