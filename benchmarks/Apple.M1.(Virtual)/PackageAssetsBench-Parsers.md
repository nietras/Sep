```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-DUYDYG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-DUYDYG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                    | Scope | Rows  | Mean      | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------------ |------ |------ |----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  3.440 ms | 29 | 8454.7 |   68.8 |     969 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt       | Row   | 50000 |  4.369 ms | 29 | 6656.8 |   87.4 |    1071 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt       | Row   | 50000 |  4.898 ms | 29 | 5938.5 |   98.0 |     992 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt       | Row   | 50000 |  4.920 ms | 29 | 5912.2 |   98.4 |    1232 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt        | Row   | 50000 |  5.298 ms | 29 | 5489.6 |  106.0 |    1007 B |
| Sep_   | SepParserIndexOfAny                       | Row   | 50000 | 20.125 ms | 29 | 1445.3 |  402.5 |    1025 B |
