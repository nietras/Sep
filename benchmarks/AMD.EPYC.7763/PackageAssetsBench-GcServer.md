```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-OIQWSK : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-OIQWSK  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.42 ms |  1.00 |  29 |  870.2 |  668.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.68 ms |  0.50 |  29 | 1743.4 |  333.7 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    44.87 ms |  1.34 |  29 |  648.2 |  897.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.76 ms |  1.79 |  29 |  486.7 | 1195.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   120.26 ms |  3.60 |  29 |  241.9 | 2405.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   660.38 ms |  1.00 | 581 |  881.1 |  660.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   344.48 ms |  0.52 | 581 | 1689.1 |  344.5 |  268.84 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   886.35 ms |  1.34 | 581 |  656.5 |  886.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,282.45 ms |  1.94 | 581 |  453.7 | 1282.5 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,464.45 ms |  3.73 | 581 |  236.1 | 2464.4 |  260.58 MB |        1.00 |
