```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-YYWBKJ : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-YYWBKJ  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.22 ms |  1.00 |  29 |  875.4 |  664.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.78 ms |  0.51 |  29 | 1733.3 |  335.6 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    43.96 ms |  1.33 |  29 |  661.7 |  879.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.14 ms |  1.78 |  29 |  491.8 | 1182.7 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   119.47 ms |  3.60 |  29 |  243.5 | 2389.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   677.09 ms |  1.00 | 581 |  859.4 |  677.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   346.13 ms |  0.51 | 581 | 1681.1 |  346.1 |  271.12 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   877.32 ms |  1.30 | 581 |  663.2 |  877.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,223.19 ms |  1.81 | 581 |  475.7 | 1223.2 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,459.94 ms |  3.63 | 581 |  236.5 | 2459.9 |  260.58 MB |        1.00 |
