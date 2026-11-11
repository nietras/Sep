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
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|-------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  3.012 ms |  1.00 | 20 | 6731.2 |  120.5 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  4.000 ms |  1.33 | 20 | 5067.5 |  160.0 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  3.182 ms |  1.06 | 20 | 6370.4 |  127.3 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Cols   | 25000 |  3.998 ms |  1.00 | 20 | 5070.5 |  159.9 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  6.545 ms |  1.64 | 20 | 3097.4 |  261.8 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  4.694 ms |  1.17 | 20 | 4318.4 |  187.8 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |        |        |           |             |
| Sep______ | Floats | 25000 | 33.305 ms |  1.00 | 20 |  608.7 | 1332.2 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 | 16.664 ms |  0.50 | 20 | 1216.5 |  666.6 |  50.62 KB |        8.65 |
| Sylvan___ | Floats | 25000 | 74.364 ms |  2.23 | 20 |  272.6 | 2974.6 |  18.62 KB |        3.18 |
| CsvHelper | Floats | 25000 | 39.907 ms |  1.20 | 20 |  508.0 | 1596.3 |   6.09 KB |        1.04 |
