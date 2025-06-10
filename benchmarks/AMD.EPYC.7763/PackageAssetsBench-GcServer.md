```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-GIXJJC : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-GIXJJC  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.34 ms |  1.00 |  29 |  899.4 |  646.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.01 ms |  0.53 |  29 | 1709.8 |  340.2 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    45.22 ms |  1.40 |  29 |  643.1 |  904.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.04 ms |  1.86 |  29 |  484.5 | 1200.7 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   119.47 ms |  3.69 |  29 |  243.5 | 2389.4 |   13.65 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   671.51 ms |  1.00 | 581 |  866.5 |  671.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   342.25 ms |  0.51 | 581 | 1700.1 |  342.3 |  268.32 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   881.19 ms |  1.31 | 581 |  660.3 |  881.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,231.90 ms |  1.83 | 581 |  472.3 | 1231.9 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,404.40 ms |  3.58 | 581 |  242.0 | 2404.4 |  260.58 MB |        1.00 |
