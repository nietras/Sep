```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.82GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    37.55 ms |  1.00 |  33 |  886.4 |  750.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    21.54 ms |  0.57 |  33 | 1545.1 |  430.8 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    59.86 ms |  1.59 |  33 |  556.0 | 1197.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    65.15 ms |  1.74 |  33 |  510.8 | 1303.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.24 ms |  3.15 |  33 |  281.5 | 2364.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   763.92 ms |  1.00 | 665 |  871.6 |  763.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   425.42 ms |  0.56 | 665 | 1565.1 |  425.4 |  263.06 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,221.27 ms |  1.60 | 665 |  545.2 | 1221.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,362.44 ms |  1.78 | 665 |  488.7 | 1362.4 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,359.41 ms |  3.09 | 665 |  282.2 | 2359.4 |  260.58 MB |        1.00 |
