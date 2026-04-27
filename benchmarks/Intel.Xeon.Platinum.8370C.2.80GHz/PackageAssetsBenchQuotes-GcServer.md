```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.56GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.52 ms |  1.00 |  33 |  821.4 |  810.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    23.15 ms |  0.57 |  33 | 1438.0 |  462.9 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    60.49 ms |  1.49 |  33 |  550.2 | 1209.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    57.11 ms |  1.41 |  33 |  582.8 | 1142.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.36 ms |  2.92 |  33 |  281.2 | 2367.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   845.91 ms |  1.00 | 665 |  787.1 |  845.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   456.17 ms |  0.54 | 665 | 1459.5 |  456.2 |  265.27 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,248.91 ms |  1.48 | 665 |  533.1 | 1248.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,374.55 ms |  1.63 | 665 |  484.4 | 1374.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,375.53 ms |  2.81 | 665 |  280.3 | 2375.5 |  260.58 MB |        1.00 |
