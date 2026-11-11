```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 3.49GHz), 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method                     | Scope | Rows  | Mean     | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|--------------------------- |------ |------ |---------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep_                       | Cols  | 50000 | 10.31 ms |  1.00 | 41 | 4040.3 |  206.3 |     288 B |        1.00 |
| Sep_Trim                   | Cols  | 50000 | 15.19 ms |  1.47 | 41 | 2744.4 |  303.7 |     288 B |        1.00 |
| Sep_TrimUnescape           | Cols  | 50000 | 17.18 ms |  1.67 | 41 | 2426.2 |  343.5 |     288 B |        1.00 |
| Sep_TrimUnescapeTrim       | Cols  | 50000 | 19.36 ms |  1.88 | 41 | 2152.8 |  387.2 |     288 B |        1.00 |
| CsvHelper_TrimUnescape     | Cols  | 50000 | 76.52 ms |  7.42 | 41 |  544.7 | 1530.3 |    1264 B |        4.39 |
| CsvHelper_TrimUnescapeTrim | Cols  | 50000 | 81.28 ms |  7.88 | 41 |  512.7 | 1625.6 |    1264 B |        4.39 |
