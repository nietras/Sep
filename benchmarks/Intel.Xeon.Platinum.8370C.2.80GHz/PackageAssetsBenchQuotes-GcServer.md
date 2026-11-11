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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    38.64 ms |  1.00 |  33 |  861.3 |  772.9 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    19.91 ms |  0.52 |  33 | 1671.3 |  398.3 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.13 ms |  1.58 |  33 |  544.5 | 1222.5 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    40.33 ms |  1.05 |  33 |  825.1 |  806.7 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   773.68 ms |  1.00 | 665 |  860.6 |  773.7 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   408.30 ms |  0.53 | 665 | 1630.7 |  408.3 | 267.39 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,285.82 ms |  1.66 | 665 |  517.8 | 1285.8 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   797.26 ms |  1.03 | 665 |  835.1 |  797.3 | 261.35 MB |        1.00 |
