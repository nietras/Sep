```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-MPBGVI : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-MPBGVI  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.355 ms |  1.00 |  29 | 8669.9 |   67.1 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.476 ms |  1.04 |  29 | 8368.5 |   69.5 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.456 ms |  1.03 |  29 | 8416.4 |   69.1 |       1.14 KB |        1.12 |
| Sylvan___    | Row   | 50000   |     4.286 ms |  1.28 |  29 | 6785.5 |   85.7 |       7.66 KB |        7.48 |
| ReadLine_    | Row   | 50000   |    21.730 ms |  6.48 |  29 | 1338.5 |  434.6 |    88608.3 KB |   86,496.56 |
| CsvHelper    | Row   | 50000   |    66.216 ms | 19.74 |  29 |  439.3 | 1324.3 |      20.28 KB |       19.79 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.733 ms |  1.00 |  29 | 6144.8 |   94.7 |       1.04 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.540 ms |  1.17 |  29 | 5250.4 |  110.8 |       1.04 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.399 ms |  1.77 |  29 | 3463.0 |  168.0 |       7.68 KB |        7.40 |
| ReadLine_    | Cols  | 50000   |    23.671 ms |  5.00 |  29 | 1228.8 |  473.4 |   88608.31 KB |   85,357.39 |
| CsvHelper    | Cols  | 50000   |   109.777 ms | 23.19 |  29 |  265.0 | 2195.5 |     445.93 KB |      429.57 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.283 ms |  1.00 |  29 |  740.4 |  785.7 |   13802.84 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.198 ms |  0.72 |  29 | 1031.5 |  564.0 |   13855.79 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    50.355 ms |  1.28 |  29 |  577.6 | 1007.1 |    13962.8 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   120.015 ms |  3.06 |  29 |  242.4 | 2400.3 |  102136.55 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   129.372 ms |  3.30 |  29 |  224.8 | 2587.4 |   13971.49 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   842.923 ms |  1.00 | 581 |  690.3 |  842.9 |  266670.18 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   488.854 ms |  0.58 | 581 | 1190.3 |  488.9 |  275834.52 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,033.466 ms |  1.23 | 581 |  563.0 | 1033.5 |  266826.28 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,486.079 ms |  2.95 | 581 |  234.1 | 2486.1 | 2038845.42 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,576.812 ms |  3.06 | 581 |  225.8 | 2576.8 |   266840.7 KB |        1.00 |
