```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-GLYBTL : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-GLYBTL  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.598 ms |  1.00 |  29 | 8082.8 |   72.0 |       1.03 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.806 ms |  1.06 |  29 | 7641.2 |   76.1 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.656 ms |  1.02 |  29 | 7954.7 |   73.1 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.523 ms |  1.26 |  29 | 6430.7 |   90.5 |       7.66 KB |        7.47 |
| ReadLine_    | Row   | 50000   |    22.847 ms |  6.35 |  29 | 1273.1 |  456.9 |    88608.3 KB |   86,414.19 |
| CsvHelper    | Row   | 50000   |    63.704 ms | 17.70 |  29 |  456.6 | 1274.1 |      20.12 KB |       19.62 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.730 ms |  1.00 |  29 | 6149.0 |   94.6 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.772 ms |  1.22 |  29 | 5039.5 |  115.4 |       1.04 KB |        1.01 |
| Sylvan___    | Cols  | 50000   |     8.419 ms |  1.78 |  29 | 3454.7 |  168.4 |       7.68 KB |        7.44 |
| ReadLine_    | Cols  | 50000   |    23.577 ms |  4.98 |  29 | 1233.7 |  471.5 |   88608.31 KB |   85,841.92 |
| CsvHelper    | Cols  | 50000   |   108.022 ms | 22.84 |  29 |  269.3 | 2160.4 |     445.93 KB |      432.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.855 ms |  1.00 |  29 |  729.8 |  797.1 |   13802.86 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.540 ms |  0.72 |  29 | 1019.1 |  570.8 |   13863.52 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    50.855 ms |  1.28 |  29 |  571.9 | 1017.1 |   13962.26 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   128.364 ms |  3.22 |  29 |  226.6 | 2567.3 |   102134.8 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   126.913 ms |  3.19 |  29 |  229.2 | 2538.3 |   13973.58 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   826.932 ms |  1.00 | 581 |  703.7 |  826.9 |   266674.5 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   492.812 ms |  0.60 | 581 | 1180.7 |  492.8 |  274558.77 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,043.982 ms |  1.26 | 581 |  557.4 | 1044.0 |  266827.59 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,537.937 ms |  3.07 | 581 |  229.3 | 2537.9 | 2038837.51 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,580.468 ms |  3.12 | 581 |  225.5 | 2580.5 |  266838.77 KB |        1.00 |
