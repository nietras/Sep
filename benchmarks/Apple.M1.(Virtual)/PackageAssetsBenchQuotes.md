```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.56 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  Error=NA  
RatioSD=?  

```
| Method       | Scope | Rows    | Mean | Ratio | MB  | MB/s | ns/row | Alloc Ratio |
|------------- |------ |-------- |-----:|------:|----:|-----:|-------:|------------:|
| Sep______    | Row   | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_Async    | Row   | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_Unescape | Row   | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sylvan___    | Row   | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| ReadLine_    | Row   | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| CsvHelper    | Row   | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
|              |       |         |      |       |     |      |        |             |
| Sep______    | Cols  | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_Unescape | Cols  | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sylvan___    | Cols  | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| ReadLine_    | Cols  | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| CsvHelper    | Cols  | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
|              |       |         |      |       |     |      |        |             |
| Sep______    | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_MT___    | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sylvan___    | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| ReadLine_    | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
| CsvHelper    | Asset | 50000   |   NA |     ? | n/a |  n/a |    n/a |           ? |
|              |       |         |      |       |     |      |        |             |
| Sep______    | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sep_MT___    | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| Sylvan___    | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| ReadLine_    | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |
| CsvHelper    | Asset | 1000000 |   NA |     ? | n/a |  n/a |    n/a |           ? |

Benchmarks with issues:
  QuotesRowPackageAssetsBench.Sep______: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Row, Quotes=True, Reader=String, Rows=50000]
  QuotesRowPackageAssetsBench.Sep_Async: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Row, Quotes=True, Reader=String, Rows=50000]
  QuotesRowPackageAssetsBench.Sep_Unescape: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Row, Quotes=True, Reader=String, Rows=50000]
  QuotesRowPackageAssetsBench.Sylvan___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Row, Quotes=True, Reader=String, Rows=50000]
  QuotesRowPackageAssetsBench.ReadLine_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Row, Quotes=True, Reader=String, Rows=50000]
  QuotesRowPackageAssetsBench.CsvHelper: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Row, Quotes=True, Reader=String, Rows=50000]
  QuotesColsPackageAssetsBench.Sep______: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  QuotesColsPackageAssetsBench.Sep_Unescape: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  QuotesColsPackageAssetsBench.Sylvan___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  QuotesColsPackageAssetsBench.ReadLine_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  QuotesColsPackageAssetsBench.CsvHelper: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Cols, Quotes=True, Reader=String, Rows=50000]
  QuotesAssetPackageAssetsBench.Sep______: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  QuotesAssetPackageAssetsBench.Sep_MT___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  QuotesAssetPackageAssetsBench.Sylvan___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  QuotesAssetPackageAssetsBench.ReadLine_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  QuotesAssetPackageAssetsBench.CsvHelper: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=50000]
  LongQuotesAssetPackageAssetsBench.Sep______: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
  LongQuotesAssetPackageAssetsBench.Sep_MT___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
  LongQuotesAssetPackageAssetsBench.Sylvan___: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
  LongQuotesAssetPackageAssetsBench.ReadLine_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
  LongQuotesAssetPackageAssetsBench.CsvHelper: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Scope=Asset, Quotes=True, Reader=String, Rows=1000000]
