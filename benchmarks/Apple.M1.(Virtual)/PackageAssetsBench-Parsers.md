```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-AKIMDM : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-AKIMDM  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                    | Scope | Rows  | Mean      | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------------ |------ |------ |----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  3.086 ms | 29 | 9425.3 |   61.7 |     969 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt       | Row   | 50000 |  3.937 ms | 29 | 7387.9 |   78.7 |     988 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt       | Row   | 50000 |  4.180 ms | 29 | 6958.0 |   83.6 |    1069 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt       | Row   | 50000 |  4.365 ms | 29 | 6664.1 |   87.3 |    1230 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt        | Row   | 50000 |  5.584 ms | 29 | 5208.9 |  111.7 |     955 B |
| Sep_   | SepParserIndexOfAny                       | Row   | 50000 | 16.196 ms | 29 | 1795.9 |  323.9 |    1011 B |
