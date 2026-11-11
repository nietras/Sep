```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V45 4.06GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
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
| Sep_                       | Cols  | 50000 |  5.251 ms |  1.00 | 41 | 7935.7 |  105.0 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 |  8.441 ms |  1.61 | 41 | 4937.0 |  168.8 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 |  9.859 ms |  1.88 | 41 | 4227.0 |  197.2 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 10.643 ms |  2.03 | 41 | 3915.5 |  212.9 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 46.891 ms |  8.93 | 41 |  888.8 |  937.8 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 51.000 ms |  9.71 | 41 |  817.1 | 1020.0 |    1264 B |        4.39 |
