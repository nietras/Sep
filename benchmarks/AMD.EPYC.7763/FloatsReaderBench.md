```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-RAOLFZ : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.913 ms |  1.00 | 20 | 6958.2 |  116.5 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.511 ms |  1.21 | 20 | 5774.0 |  140.4 |    10.71 KB |        8.51 |
| ReadLine_ | Row    | 25000 |  19.754 ms |  6.78 | 20 | 1026.2 |  790.2 | 73489.68 KB |   58,426.57 |
| CsvHelper | Row    | 25000 |  38.285 ms | 13.14 | 20 |  529.5 | 1531.4 |    20.06 KB |       15.95 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.876 ms |  1.00 | 20 | 5230.0 |  155.0 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.891 ms |  1.52 | 20 | 3441.4 |  235.6 |    10.72 KB |        8.51 |
| ReadLine_ | Cols   | 25000 |  21.160 ms |  5.46 | 20 |  958.0 |  846.4 | 73489.68 KB |   58,335.99 |
| CsvHelper | Cols   | 25000 |  40.117 ms | 10.35 | 20 |  505.3 | 1604.7 | 21340.29 KB |   16,939.89 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.349 ms |  1.00 | 20 |  646.7 | 1254.0 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  12.954 ms |  0.41 | 20 | 1565.0 |  518.1 |    67.81 KB |        8.40 |
| Sylvan___ | Floats | 25000 |  90.425 ms |  2.88 | 20 |  224.2 | 3617.0 |    18.96 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 112.571 ms |  3.59 | 20 |  180.1 | 4502.9 | 73493.98 KB |    9,101.20 |
| CsvHelper | Floats | 25000 | 160.313 ms |  5.11 | 20 |  126.5 | 6412.5 | 22062.29 KB |    2,732.11 |
