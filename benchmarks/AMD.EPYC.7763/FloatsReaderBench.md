```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-XDFYGT : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.913 ms |  1.00 | 20 | 6958.5 |  116.5 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.566 ms |  1.22 | 20 | 5685.3 |  142.6 |    10.71 KB |        8.51 |
| ReadLine_ | Row    | 25000 |  18.192 ms |  6.24 | 20 | 1114.3 |  727.7 |  73489.7 KB |   58,426.60 |
| CsvHelper | Row    | 25000 |  38.233 ms | 13.12 | 20 |  530.2 | 1529.3 |    20.06 KB |       15.95 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.950 ms |  1.00 | 20 | 5131.9 |  158.0 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.911 ms |  1.50 | 20 | 3429.6 |  236.4 |    10.72 KB |        8.48 |
| ReadLine_ | Cols   | 25000 |  19.574 ms |  4.96 | 20 | 1035.6 |  783.0 | 73489.68 KB |   58,155.66 |
| CsvHelper | Cols   | 25000 |  41.031 ms | 10.39 | 20 |  494.1 | 1641.3 | 21340.29 KB |   16,887.53 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.469 ms |  1.00 | 20 |  644.2 | 1258.7 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  12.639 ms |  0.40 | 20 | 1604.0 |  505.5 |    67.81 KB |        8.40 |
| Sylvan___ | Floats | 25000 |  84.199 ms |  2.68 | 20 |  240.8 | 3368.0 |    19.89 KB |        2.46 |
| ReadLine_ | Floats | 25000 | 112.934 ms |  3.59 | 20 |  179.5 | 4517.4 |  73493.2 KB |    9,101.10 |
| CsvHelper | Floats | 25000 | 161.035 ms |  5.12 | 20 |  125.9 | 6441.4 | 22062.53 KB |    2,732.14 |
