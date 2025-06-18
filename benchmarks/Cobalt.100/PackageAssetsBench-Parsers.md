```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-DSGOZQ : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-DSGOZQ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                    | Scope | Rows  | Mean      | MB  | MB/s   | ns/row | Allocated |
|------- |------------------------------------------ |------ |------ |----------:|----:|-------:|-------:|----------:|
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |        NA | n/a |    n/a |    n/a |        NA |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt       | Row   | 50000 |  7.373 ms |  29 | 3957.9 |  147.5 |    1049 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt       | Row   | 50000 |  7.513 ms |  29 | 3884.0 |  150.3 |     992 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt       | Row   | 50000 |  8.258 ms |  29 | 3533.9 |  165.2 |    1238 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt        | Row   | 50000 |  9.317 ms |  29 | 3131.9 |  186.3 |     960 B |
| Sep_   | SepParserIndexOfAny                       | Row   | 50000 | 17.676 ms |  29 | 1650.9 |  353.5 |     987 B |

Benchmarks with issues:
  ParsersRowPackageAssetsBench.Sep_: Job-DSGOZQ(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 9.0, Toolchain=net90, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Parser=SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt, Scope=Row, Quotes=False, Reader=String, Rows=50000]
