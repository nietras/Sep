```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
Cobalt 100, 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-MYYDFG : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-MYYDFG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                    | Scope | Rows  | Mean      | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------------ |------ |------ |----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  5.200 ms | 29 | 5611.5 |  104.0 |     953 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt       | Row   | 50000 |  7.833 ms | 29 | 3725.3 |  156.7 |    1082 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt       | Row   | 50000 |  7.966 ms | 29 | 3663.3 |  159.3 |     970 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt       | Row   | 50000 |  8.505 ms | 29 | 3431.2 |  170.1 |    1235 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt        | Row   | 50000 |  9.581 ms | 29 | 3045.9 |  191.6 |     961 B |
| Sep_   | SepParserIndexOfAny                       | Row   | 50000 | 18.261 ms | 29 | 1598.0 |  365.2 |     993 B |
