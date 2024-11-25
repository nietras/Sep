```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-UGLWRK : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-UGLWRK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.87 ms |  1.00 |  29 |  884.8 |  657.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.55 ms |  0.47 |  29 | 1870.2 |  311.0 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    42.88 ms |  1.30 |  29 |  678.3 |  857.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    38.57 ms |  1.17 |  29 |  754.1 |  771.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   120.27 ms |  3.66 |  29 |  241.8 | 2405.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   611.99 ms |  1.00 | 581 |  950.8 |  612.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   290.50 ms |  0.47 | 581 | 2003.0 |  290.5 |  268.75 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   841.41 ms |  1.38 | 581 |  691.6 |  841.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,308.78 ms |  2.14 | 581 |  444.6 | 1308.8 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,362.27 ms |  3.86 | 581 |  246.3 | 2362.3 |  260.58 MB |        1.00 |
