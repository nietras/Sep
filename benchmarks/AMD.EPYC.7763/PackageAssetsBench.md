```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-NMHWMW : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-NMHWMW  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.670 ms |  1.00 |  29 | 7925.0 |   73.4 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.722 ms |  1.01 |  29 | 7815.2 |   74.4 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.570 ms |  1.25 |  29 | 6364.0 |   91.4 |       7.67 KB |        7.48 |
| ReadLine_    | Row   | 50000   |    23.906 ms |  6.52 |  29 | 1216.7 |  478.1 |   88608.32 KB |   86,414.21 |
| CsvHelper    | Row   | 50000   |    65.482 ms | 17.85 |  29 |  444.2 | 1309.6 |      20.28 KB |       19.78 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.958 ms |  1.00 |  29 | 5866.0 |   99.2 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.832 ms |  1.18 |  29 | 4987.3 |  116.6 |       1.04 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.311 ms |  1.68 |  29 | 3499.7 |  166.2 |       7.68 KB |        7.44 |
| ReadLine_    | Cols  | 50000   |    25.227 ms |  5.09 |  29 | 1153.0 |  504.5 |   88608.31 KB |   85,760.78 |
| CsvHelper    | Cols  | 50000   |   109.375 ms | 22.06 |  29 |  265.9 | 2187.5 |     445.86 KB |      431.53 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    31.267 ms |  1.00 |  29 |  930.2 |  625.3 |   13802.34 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.237 ms |  0.71 |  29 | 1308.0 |  444.7 |   13868.55 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    48.208 ms |  1.54 |  29 |  603.3 |  964.2 |   13961.85 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   118.179 ms |  3.78 |  29 |  246.1 | 2363.6 |  102133.09 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   120.474 ms |  3.85 |  29 |  241.4 | 2409.5 |    13970.3 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   787.504 ms |  1.00 | 581 |  738.9 |  787.5 |  266669.05 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   514.091 ms |  0.65 | 581 | 1131.9 |  514.1 |  276642.65 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,007.588 ms |  1.28 | 581 |  577.5 | 1007.6 |  266827.02 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,312.541 ms |  4.21 | 581 |  175.7 | 3312.5 | 2038835.09 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,528.824 ms |  3.21 | 581 |  230.1 | 2528.8 |  266834.74 KB |        1.00 |
