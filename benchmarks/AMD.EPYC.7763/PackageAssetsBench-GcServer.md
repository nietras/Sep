```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-ZOWZUS : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-ZOWZUS  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Server=True  Toolchain=net90  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.70 ms |  1.00 |  29 |  889.5 |  653.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.64 ms |  0.51 |  29 | 1748.0 |  332.8 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    44.32 ms |  1.36 |  29 |  656.2 |  886.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    58.67 ms |  1.79 |  29 |  495.7 | 1173.4 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   120.06 ms |  3.67 |  29 |  242.3 | 2401.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   667.19 ms |  1.00 | 581 |  872.1 |  667.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   343.62 ms |  0.52 | 581 | 1693.4 |  343.6 |  269.07 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   884.52 ms |  1.33 | 581 |  657.8 |  884.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,238.66 ms |  1.86 | 581 |  469.8 | 1238.7 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,422.78 ms |  3.63 | 581 |  240.2 | 2422.8 |  260.58 MB |        1.00 |
