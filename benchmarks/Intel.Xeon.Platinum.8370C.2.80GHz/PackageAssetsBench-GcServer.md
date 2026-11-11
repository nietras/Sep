```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 3.49GHz), 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  34.33 ms |  1.00 |  29 |  847.2 |  686.6 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  17.86 ms |  0.52 |  29 | 1629.0 |  357.1 |  13.52 MB |        1.00 |
| Sylvan___ | Asset | 50000   |  41.92 ms |  1.22 |  29 |  693.9 |  838.4 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  34.86 ms |  1.02 |  29 |  834.3 |  697.3 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 683.95 ms |  1.00 | 581 |  850.8 |  684.0 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 360.96 ms |  0.53 | 581 | 1612.0 |  361.0 | 269.94 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 894.33 ms |  1.31 | 581 |  650.6 |  894.3 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 678.55 ms |  0.99 | 581 |  857.5 |  678.5 | 261.35 MB |        1.00 |
