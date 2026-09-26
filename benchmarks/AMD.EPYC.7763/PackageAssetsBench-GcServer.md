```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 3.08GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.38 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.79 ms |  1.00 |  29 |  887.0 |  655.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.78 ms |  0.51 |  29 | 1733.7 |  335.5 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    42.30 ms |  1.29 |  29 |  687.7 |  845.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    55.22 ms |  1.69 |  29 |  526.7 | 1104.4 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   113.68 ms |  3.48 |  29 |  255.9 | 2273.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   665.15 ms |  1.00 | 581 |  874.8 |  665.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   343.16 ms |  0.52 | 581 | 1695.6 |  343.2 |   269.9 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   841.83 ms |  1.27 | 581 |  691.2 |  841.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,187.90 ms |  1.79 | 581 |  489.8 | 1187.9 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,433.82 ms |  3.66 | 581 |  239.1 | 2433.8 |  260.58 MB |        1.00 |
