```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.90GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    37.82 ms |  1.00 |  33 |  880.1 |  756.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    19.95 ms |  0.53 |  33 | 1668.2 |  399.0 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    60.86 ms |  1.61 |  33 |  546.8 | 1217.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    65.12 ms |  1.72 |  33 |  511.1 | 1302.3 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   117.65 ms |  3.11 |  33 |  282.9 | 2353.0 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   814.18 ms |  1.00 | 665 |  817.8 |  814.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   420.01 ms |  0.52 | 665 | 1585.2 |  420.0 |  262.55 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,275.65 ms |  1.57 | 665 |  521.9 | 1275.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,362.68 ms |  1.67 | 665 |  488.6 | 1362.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,362.46 ms |  2.90 | 665 |  281.8 | 2362.5 |  260.58 MB |        1.00 |
