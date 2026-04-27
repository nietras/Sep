```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.56GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.46 ms |  1.00 |  29 |  869.3 |  669.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.76 ms |  0.56 |  29 | 1550.6 |  375.2 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.30 ms |  1.23 |  29 |  704.3 |  825.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    51.05 ms |  1.53 |  29 |  569.7 | 1021.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   118.75 ms |  3.55 |  29 |  244.9 | 2375.0 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   705.66 ms |  1.00 | 581 |  824.6 |  705.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   370.95 ms |  0.53 | 581 | 1568.6 |  370.9 |  269.32 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   848.85 ms |  1.20 | 581 |  685.5 |  848.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,192.11 ms |  1.69 | 581 |  488.1 | 1192.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,399.65 ms |  3.40 | 581 |  242.5 | 2399.6 |  260.58 MB |        1.00 |
