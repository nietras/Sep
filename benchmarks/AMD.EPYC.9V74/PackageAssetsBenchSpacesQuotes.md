```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.68GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.45 GB Available
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
| Sep_                       | Cols  | 50000 |  7.740 ms |  1.00 | 41 | 5384.0 |  154.8 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 12.141 ms |  1.57 | 41 | 3432.5 |  242.8 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 13.991 ms |  1.81 | 41 | 2978.6 |  279.8 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 15.986 ms |  2.07 | 41 | 2607.0 |  319.7 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 62.895 ms |  8.13 | 41 |  662.6 | 1257.9 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 68.435 ms |  8.84 | 41 |  609.0 | 1368.7 |    1264 B |        4.39 |
