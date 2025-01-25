```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-GAKWOE : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-GAKWOE  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.61 ms |  1.00 |  33 | 3137.7 |  212.1 |      1.06 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.00 ms |  1.04 |  33 | 3026.4 |  219.9 |      1.06 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.90 ms |  1.03 |  33 | 3052.2 |  218.1 |      1.06 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    25.26 ms |  2.38 |  33 | 1317.4 |  505.3 |      7.74 KB |        7.30 |
| ReadLine_    | Row   | 50000   |    25.66 ms |  2.42 |  33 | 1296.8 |  513.3 | 108778.82 KB |  102,568.61 |
| CsvHelper    | Row   | 50000   |    77.61 ms |  7.32 |  33 |  428.9 | 1552.1 |     20.21 KB |       19.05 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.69 ms |  1.00 |  33 | 2848.2 |  233.7 |      1.07 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.68 ms |  1.17 |  33 | 2433.5 |  273.5 |      1.08 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |    29.22 ms |  2.50 |  33 | 1138.9 |  584.5 |      7.76 KB |        7.25 |
| ReadLine_    | Cols  | 50000   |    26.97 ms |  2.31 |  33 | 1234.1 |  539.4 | 108778.81 KB |  101,632.75 |
| CsvHelper    | Cols  | 50000   |   105.38 ms |  9.02 |  33 |  315.8 | 2107.6 |    445.94 KB |      416.64 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    47.22 ms |  1.00 |  33 |  704.8 |  944.4 |  13802.89 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    33.06 ms |  0.70 |  33 | 1006.7 |  661.2 |  13864.26 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    72.35 ms |  1.53 |  33 |  460.0 | 1447.0 |  13962.38 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   148.41 ms |  3.14 |  33 |  224.3 | 2968.1 | 122305.47 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   124.68 ms |  2.64 |  33 |  266.9 | 2493.6 |  13970.82 KB |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   969.90 ms |  1.00 | 665 |  686.5 |  969.9 | 266670.22 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   576.11 ms |  0.59 | 665 | 1155.7 |  576.1 | 271866.89 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,479.69 ms |  1.53 | 665 |  450.0 | 1479.7 | 266827.16 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,075.64 ms |  3.17 | 665 |  216.5 | 3075.6 | 2442321.2 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,550.09 ms |  2.63 | 665 |  261.1 | 2550.1 | 266840.09 KB |        1.00 |
