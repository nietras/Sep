```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-GAKWOE : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-GAKWOE  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.534 ms |  1.00 |  29 | 8230.8 |   70.7 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.208 ms |  1.19 |  29 | 6911.2 |   84.2 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.486 ms |  0.99 |  29 | 8343.7 |   69.7 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.441 ms |  1.26 |  29 | 6548.8 |   88.8 |       7.67 KB |        7.48 |
| ReadLine_    | Row   | 50000   |    21.936 ms |  6.21 |  29 | 1325.9 |  438.7 |    88608.3 KB |   86,496.56 |
| CsvHelper    | Row   | 50000   |    63.529 ms | 17.98 |  29 |  457.8 | 1270.6 |      20.12 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.834 ms |  1.00 |  29 | 6016.6 |   96.7 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.628 ms |  1.16 |  29 | 5168.0 |  112.6 |       1.04 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.516 ms |  1.76 |  29 | 3415.3 |  170.3 |       7.68 KB |        7.44 |
| ReadLine_    | Cols  | 50000   |    22.506 ms |  4.66 |  29 | 1292.4 |  450.1 |   88608.29 KB |   85,841.90 |
| CsvHelper    | Cols  | 50000   |   110.650 ms | 22.89 |  29 |  262.9 | 2213.0 |     445.93 KB |      432.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    40.183 ms |  1.00 |  29 |  723.8 |  803.7 |   13803.91 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    24.942 ms |  0.62 |  29 | 1166.1 |  498.8 |   13852.97 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    50.632 ms |  1.26 |  29 |  574.5 | 1012.6 |   13962.27 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   125.355 ms |  3.12 |  29 |  232.0 | 2507.1 |  102134.79 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   126.403 ms |  3.15 |  29 |  230.1 | 2528.1 |      13971 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   826.839 ms |  1.00 | 581 |  703.7 |  826.8 |  266670.12 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   508.723 ms |  0.62 | 581 | 1143.8 |  508.7 |   277744.1 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,021.667 ms |  1.24 | 581 |  569.5 | 1021.7 |  266838.41 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,526.808 ms |  3.06 | 581 |  230.3 | 2526.8 | 2038849.63 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,575.298 ms |  3.11 | 581 |  225.9 | 2575.3 |  266846.34 KB |        1.00 |
