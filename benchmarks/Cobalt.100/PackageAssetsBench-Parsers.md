```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9725/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.96 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method | Parser                                          | Scope | Rows  | Mean      | MB | MB/s   | ns/row | Allocated |
|------- |------------------------------------------------ |------ |------ |----------:|---:|-------:|-------:|----------:|
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt       | Row   | 50000 |  5.969 ms | 29 | 4889.1 |  119.4 |     288 B |
| Sep_   | SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  6.667 ms | 29 | 4376.8 |  133.3 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt             | Row   | 50000 |  8.278 ms | 29 | 3525.2 |  165.6 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt              | Row   | 50000 |  9.947 ms | 29 | 2933.8 |  198.9 |     288 B |
| Sep_   | SepParserIndexOfAny                             | Row   | 50000 | 18.565 ms | 29 | 1571.9 |  371.3 |     288 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt             | Row   | 50000 | 19.162 ms | 29 | 1522.9 |  383.2 |     288 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt             | Row   | 50000 | 19.347 ms | 29 | 1508.3 |  386.9 |     288 B |
