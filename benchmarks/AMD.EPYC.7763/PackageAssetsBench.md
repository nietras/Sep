```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.1 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  Job-XDFYGT : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-XDFYGT  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.427 ms |  1.00 |  29 | 8487.1 |   68.5 |      1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.656 ms |  1.07 |  29 | 7954.9 |   73.1 |      1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.473 ms |  1.01 |  29 | 8376.0 |   69.5 |      1.15 KB |        1.12 |
| Sylvan___    | Row   | 50000   |     4.429 ms |  1.29 |  29 | 6567.1 |   88.6 |      7.67 KB |        7.48 |
| ReadLine_    | Row   | 50000   |    21.587 ms |  6.30 |  29 | 1347.4 |  431.7 |   88608.3 KB |   86,496.57 |
| CsvHelper    | Row   | 50000   |    63.743 ms | 18.60 |  29 |  456.3 | 1274.9 |     20.12 KB |       19.64 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.758 ms |  1.00 |  29 | 6112.5 |   95.2 |      1.04 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.691 ms |  1.20 |  29 | 5110.7 |  113.8 |      1.04 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.204 ms |  1.72 |  29 | 3545.5 |  164.1 |      7.68 KB |        7.42 |
| ReadLine_    | Cols  | 50000   |    22.823 ms |  4.80 |  29 | 1274.4 |  456.5 |  88608.31 KB |   85,518.30 |
| CsvHelper    | Cols  | 50000   |   110.312 ms | 23.18 |  29 |  263.7 | 2206.2 |    445.93 KB |      430.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    39.731 ms |  1.00 |  29 |  732.1 |  794.6 |  13803.91 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.730 ms |  0.72 |  29 | 1012.4 |  574.6 |  13858.99 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    50.605 ms |  1.28 |  29 |  574.8 | 1012.1 |  13963.34 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   125.231 ms |  3.16 |  29 |  232.3 | 2504.6 |    102135 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   127.145 ms |  3.20 |  29 |  228.8 | 2542.9 |  13971.75 KB |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   850.145 ms |  1.00 | 581 |  684.4 |  850.1 | 266670.16 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   505.005 ms |  0.59 | 581 | 1152.2 |  505.0 | 276118.02 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,035.263 ms |  1.22 | 581 |  562.1 | 1035.3 |  266828.4 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,597.222 ms |  3.06 | 581 |  224.0 | 2597.2 | 2038837.9 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,649.300 ms |  3.12 | 581 |  219.6 | 2649.3 | 266845.35 KB |        1.00 |
