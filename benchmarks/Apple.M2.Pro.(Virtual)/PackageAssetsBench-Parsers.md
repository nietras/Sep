```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
Apple M2 Pro (Virtual), 1 CPU, 5 logical and 5 physical cores
Memory: 14 GB Total, 0.8 GB Available
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
| Sep_   | SepParserAdvSimdLoad4xNrwCmpOrBulkMoveMaskTzcnt | Row   | 50000 |  4.063 ms | 29 | 7158.0 |   81.3 |     288 B |
| Sep_   | SepParserAdvSimdNrwCmpOrBulkMoveMaskTzcnt       | Row   | 50000 |  4.096 ms | 29 | 7100.4 |   81.9 |     288 B |
| Sep_   | SepParserVector64NrwCmpExtMsbTzcnt              | Row   | 50000 |  5.412 ms | 29 | 5373.8 |  108.2 |     288 B |
| Sep_   | SepParserVector128NrwCmpExtMsbTzcnt             | Row   | 50000 |  5.780 ms | 29 | 5031.9 |  115.6 |     288 B |
| Sep_   | SepParserVector256NrwCmpExtMsbTzcnt             | Row   | 50000 | 14.008 ms | 29 | 2076.4 |  280.2 |     288 B |
| Sep_   | SepParserVector512NrwCmpExtMsbTzcnt             | Row   | 50000 | 14.272 ms | 29 | 2038.0 |  285.4 |     288 B |
| Sep_   | SepParserIndexOfAny                             | Row   | 50000 | 19.020 ms | 29 | 1529.2 |  380.4 |     288 B |
