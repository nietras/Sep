```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                          | Scope | Rows  | Mean      | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------------------ |------ |------ |----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt       | Row   | 50000 |  5.013 ms | 29 | 5801.6 |  100.3 |     968 B |
| Sep_   | SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  5.822 ms | 29 | 4995.8 |  116.4 |     968 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt             | Row   | 50000 |  7.558 ms | 29 | 3848.5 |  151.2 |     984 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt              | Row   | 50000 |  9.154 ms | 29 | 3177.3 |  183.1 |     944 B |
| Sep_   | SepParserIndexOfAny                             | Row   | 50000 | 19.846 ms | 29 | 1465.6 |  396.9 |     944 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt             | Row   | 50000 | 21.478 ms | 29 | 1354.2 |  429.6 |    1064 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt             | Row   | 50000 | 21.788 ms | 29 | 1335.0 |  435.8 |    1224 B |
