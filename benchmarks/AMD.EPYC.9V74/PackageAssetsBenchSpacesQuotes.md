```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.46 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 |  7.508 ms |  1.00 | 41 | 5550.6 |  150.2 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 11.848 ms |  1.58 | 41 | 3517.4 |  237.0 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.629 ms |  1.82 | 41 | 3057.8 |  272.6 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 15.719 ms |  2.09 | 41 | 2651.3 |  314.4 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 63.309 ms |  8.43 | 41 |  658.3 | 1266.2 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 66.318 ms |  8.83 | 41 |  628.4 | 1326.4 |    1264 B |        4.39 |
