```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  14.152 ms |  1.00 |  29 | 2062.0 |  283.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   3.460 ms |  0.24 |  29 | 8434.1 |   69.2 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  18.113 ms |  1.28 |  29 | 1611.1 |  362.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  18.960 ms |  1.34 |  29 | 1539.1 |  379.2 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |  48.971 ms |  3.46 |  29 |  595.9 |  979.4 |   13.64 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 289.503 ms |  1.00 | 583 | 2016.5 |  289.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  58.678 ms |  0.20 | 583 | 9949.0 |   58.7 |  261.63 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 375.349 ms |  1.30 | 583 | 1555.3 |  375.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 364.651 ms |  1.26 | 583 | 1600.9 |  364.7 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 999.550 ms |  3.45 | 583 |  584.0 |  999.5 |  260.58 MB |        1.00 |
