```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-BDDETC : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-BDDETC  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                    | Scope | Rows  | Mean      | MB  | MB/s   | ns/row | Allocated |
|------- |------------------------------------------ |------ |------ |----------:|----:|-------:|-------:|----------:|
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |        NA | n/a |    n/a |    n/a |        NA |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt       | Row   | 50000 |  5.188 ms |  29 | 5606.3 |  103.8 |     998 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt       | Row   | 50000 |  6.254 ms |  29 | 4650.8 |  125.1 |    1076 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt       | Row   | 50000 |  6.447 ms |  29 | 4511.7 |  128.9 |    1644 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt        | Row   | 50000 |  7.111 ms |  29 | 4090.1 |  142.2 |     959 B |
| Sep_   | SepParserIndexOfAny                       | Row   | 50000 | 20.213 ms |  29 | 1439.0 |  404.3 |    1037 B |

Benchmarks with issues:
  ParsersRowPackageAssetsBench.Sep_: Job-BDDETC(EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0, Runtime=.NET 9.0, Toolchain=net90, IterationTime=350ms, MaxIterationCount=15, MinIterationCount=5, WarmupCount=6) [Parser=SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt, Scope=Row, Quotes=False, Reader=String, Rows=50000]
