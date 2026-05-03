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
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt       | Row   | 50000 |  3.552 ms | 29 | 8189.3 |   71.0 |     968 B |
| Sep_   | SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  3.986 ms | 29 | 7297.0 |   79.7 |     968 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt             | Row   | 50000 |  5.103 ms | 29 | 5700.2 |  102.1 |     984 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt              | Row   | 50000 |  5.675 ms | 29 | 5125.5 |  113.5 |     944 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt             | Row   | 50000 | 15.968 ms | 29 | 1821.5 |  319.4 |    1224 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt             | Row   | 50000 | 18.211 ms | 29 | 1597.2 |  364.2 |    1064 B |
| Sep_   | SepParserIndexOfAny                             | Row   | 50000 | 20.683 ms | 29 | 1406.3 |  413.7 |     944 B |
