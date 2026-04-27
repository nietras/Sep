```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
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
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt       | Row   | 50000 |  3.120 ms | 29 | 9321.4 |   62.4 |     960 B |
| Sep_   | SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  3.202 ms | 29 | 9084.6 |   64.0 |     960 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt             | Row   | 50000 |  4.539 ms | 29 | 6407.9 |   90.8 |     976 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt              | Row   | 50000 |  5.003 ms | 29 | 5813.2 |  100.1 |     936 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt             | Row   | 50000 | 14.559 ms | 29 | 1997.8 |  291.2 |    1216 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt             | Row   | 50000 | 14.811 ms | 29 | 1963.8 |  296.2 |    1056 B |
| Sep_   | SepParserIndexOfAny                             | Row   | 50000 | 18.410 ms | 29 | 1579.9 |  368.2 |     936 B |
