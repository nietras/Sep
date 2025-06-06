```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-BLMCOC : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-BLMCOC  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    31.96 ms |  1.00 |  29 |  910.1 |  639.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.92 ms |  0.53 |  29 | 1718.9 |  338.4 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    44.42 ms |  1.39 |  29 |  654.8 |  888.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.74 ms |  1.87 |  29 |  486.9 | 1194.8 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   123.07 ms |  3.85 |  29 |  236.3 | 2461.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   675.73 ms |  1.00 | 581 |  861.1 |  675.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   354.15 ms |  0.52 | 581 | 1643.0 |  354.1 |  268.29 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   885.37 ms |  1.31 | 581 |  657.2 |  885.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,234.77 ms |  1.83 | 581 |  471.2 | 1234.8 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,411.05 ms |  3.57 | 581 |  241.3 | 2411.1 |  260.58 MB |        1.00 |
