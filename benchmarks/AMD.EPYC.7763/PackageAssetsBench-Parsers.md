```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.61GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.83 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  Error=NA  

```
| Method | Parser                              | Scope | Rows  | Mean | MB  | MB/s | ns/row |
|------- |------------------------------------ |------ |------ |-----:|----:|-----:|-------:|
| Sep_   | SepParserAvx2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   NA | n/a |  n/a |    n/a |
| Sep_   | SepParserIndexOfAny                 | Row   | 50000 |   NA | n/a |  n/a |    n/a |
| Sep_   | SepParserSse2PackCmpOrMoveMaskTzcnt | Row   | 50000 |   NA | n/a |  n/a |    n/a |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt | Row   | 50000 |   NA | n/a |  n/a |    n/a |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt | Row   | 50000 |   NA | n/a |  n/a |    n/a |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt | Row   | 50000 |   NA | n/a |  n/a |    n/a |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt  | Row   | 50000 |   NA | n/a |  n/a |    n/a |

Benchmarks with issues:
  ParsersRowPackageAssetsBench.Sep_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Parser=SepParserAvx2PackCmpOrMoveMaskTzcnt, Scope=Row, Quotes=False, Reader=String, Rows=50000]
  ParsersRowPackageAssetsBench.Sep_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Parser=SepParserIndexOfAny, Scope=Row, Quotes=False, Reader=String, Rows=50000]
  ParsersRowPackageAssetsBench.Sep_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Parser=SepParserSse2PackCmpOrMoveMaskTzcnt, Scope=Row, Quotes=False, Reader=String, Rows=50000]
  ParsersRowPackageAssetsBench.Sep_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Parser=SepParserVector128NrwCmpExtMsbTzcnt, Scope=Row, Quotes=False, Reader=String, Rows=50000]
  ParsersRowPackageAssetsBench.Sep_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Parser=SepParserVector256NrwCmpExtMsbTzcnt, Scope=Row, Quotes=False, Reader=String, Rows=50000]
  ParsersRowPackageAssetsBench.Sep_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Parser=SepParserVector512NrwCmpExtMsbTzcnt, Scope=Row, Quotes=False, Reader=String, Rows=50000]
  ParsersRowPackageAssetsBench.Sep_: .NET 11.0(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 11.0, Toolchain=CsProjCore, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Parser=SepParserVector64NrwCmpExtMsbTzcnt, Scope=Row, Quotes=False, Reader=String, Rows=50000]
