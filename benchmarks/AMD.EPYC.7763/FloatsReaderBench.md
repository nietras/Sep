```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-GAKWOE : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.907 ms |  1.00 | 20 | 6973.1 |  116.3 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.429 ms |  1.18 | 20 | 5911.7 |  137.2 |    10.71 KB |        8.51 |
| ReadLine_ | Row    | 25000 |  18.876 ms |  6.49 | 20 | 1074.0 |  755.0 | 73489.67 KB |   58,426.57 |
| CsvHelper | Row    | 25000 |  37.064 ms | 12.75 | 20 |  546.9 | 1482.6 |    20.06 KB |       15.95 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.901 ms |  1.00 | 20 | 5196.8 |  156.0 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.854 ms |  1.50 | 20 | 3463.1 |  234.2 |    10.72 KB |        8.51 |
| ReadLine_ | Cols   | 25000 |  19.260 ms |  4.94 | 20 | 1052.5 |  770.4 | 73489.62 KB |   58,335.95 |
| CsvHelper | Cols   | 25000 |  40.847 ms | 10.47 | 20 |  496.3 | 1633.9 | 21340.29 KB |   16,939.89 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.152 ms |  1.00 | 20 |  650.7 | 1246.1 |     8.12 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  12.662 ms |  0.41 | 20 | 1601.1 |  506.5 |    68.38 KB |        8.43 |
| Sylvan___ | Floats | 25000 |  83.543 ms |  2.68 | 20 |  242.7 | 3341.7 |    19.89 KB |        2.45 |
| ReadLine_ | Floats | 25000 | 112.030 ms |  3.60 | 20 |  181.0 | 4481.2 | 73496.21 KB |    9,055.48 |
| CsvHelper | Floats | 25000 | 161.410 ms |  5.18 | 20 |  125.6 | 6456.4 | 22062.53 KB |    2,718.33 |
