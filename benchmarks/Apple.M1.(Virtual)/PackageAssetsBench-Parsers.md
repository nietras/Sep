```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-SMYKWG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-SMYKWG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                    | Scope | Rows  | Mean      | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------------ |------ |------ |----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  3.174 ms | 29 | 9164.3 |   63.5 |    1055 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt       | Row   | 50000 |  4.001 ms | 29 | 7269.1 |   80.0 |    1068 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt       | Row   | 50000 |  4.286 ms | 29 | 6786.7 |   85.7 |    1230 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt       | Row   | 50000 |  4.461 ms | 29 | 6519.4 |   89.2 |     996 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt        | Row   | 50000 |  4.992 ms | 29 | 5826.8 |   99.8 |     953 B |
| Sep_   | SepParserIndexOfAny                       | Row   | 50000 | 16.242 ms | 29 | 1790.8 |  324.8 |    1015 B |
