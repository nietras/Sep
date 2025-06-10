```

BenchmarkDotNet v0.15.1, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-FXNRMG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-FXNRMG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                    | Scope | Rows  | Mean      | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------------ |------ |------ |----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  4.840 ms | 29 | 6029.0 |   96.8 |     953 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt       | Row   | 50000 |  7.205 ms | 29 | 4050.2 |  144.1 |    1346 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt       | Row   | 50000 |  7.414 ms | 29 | 3935.8 |  148.3 |     993 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt       | Row   | 50000 |  7.981 ms | 29 | 3656.5 |  159.6 |    1219 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt        | Row   | 50000 |  9.033 ms | 29 | 3230.4 |  180.7 |     940 B |
| Sep_   | SepParserIndexOfAny                       | Row   | 50000 | 17.397 ms | 29 | 1677.4 |  347.9 |     934 B |
