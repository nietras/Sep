```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-GLYBTL : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.999 ms |  1.00 | 20 | 6758.8 |  120.0 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.414 ms |  1.14 | 20 | 5937.6 |  136.6 |    10.71 KB |        8.53 |
| ReadLine_ | Row    | 25000 |  18.635 ms |  6.21 | 20 | 1087.9 |  745.4 | 73489.67 KB |   58,517.44 |
| CsvHelper | Row    | 25000 |  38.884 ms | 12.96 | 20 |  521.3 | 1555.4 |    20.06 KB |       15.97 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.037 ms |  1.00 | 20 | 5021.8 |  161.5 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.342 ms |  1.57 | 20 | 3196.7 |  253.7 |    10.72 KB |        8.51 |
| ReadLine_ | Cols   | 25000 |  20.070 ms |  4.97 | 20 | 1010.0 |  802.8 | 73489.68 KB |   58,335.99 |
| CsvHelper | Cols   | 25000 |  40.105 ms |  9.94 | 20 |  505.5 | 1604.2 | 21340.29 KB |   16,939.89 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.272 ms |  1.00 | 20 |  648.2 | 1250.9 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  13.463 ms |  0.43 | 20 | 1505.8 |  538.5 |     71.1 KB |        8.81 |
| Sylvan___ | Floats | 25000 |  85.354 ms |  2.73 | 20 |  237.5 | 3414.1 |    18.96 KB |        2.35 |
| ReadLine_ | Floats | 25000 | 114.060 ms |  3.65 | 20 |  177.7 | 4562.4 | 73496.21 KB |    9,101.48 |
| CsvHelper | Floats | 25000 | 162.109 ms |  5.18 | 20 |  125.1 | 6484.4 | 22062.48 KB |    2,732.13 |
