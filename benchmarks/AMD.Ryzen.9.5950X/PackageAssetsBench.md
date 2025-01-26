```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2
  Job-WRHRFC : .NET 9.0.1 (9.0.124.61010), X64 RyuJIT AVX2

Job=Job-WRHRFC  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     2.257 ms |  1.00 |  29 | 12929.6 |   45.1 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     2.727 ms |  1.21 |  29 | 10699.8 |   54.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.391 ms |  1.06 |  29 | 12204.9 |   47.8 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     2.993 ms |  1.33 |  29 |  9750.2 |   59.9 |       7.65 KB |        7.52 |
| ReadLine_    | Row   | 50000   |    12.106 ms |  5.36 |  29 |  2410.5 |  242.1 |   88608.25 KB |   87,077.59 |
| CsvHelper    | Row   | 50000   |    43.313 ms | 19.19 |  29 |   673.7 |  866.3 |      20.04 KB |       19.69 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     3.211 ms |  1.00 |  29 |  9089.3 |   64.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     3.845 ms |  1.20 |  29 |  7589.1 |   76.9 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     5.065 ms |  1.58 |  29 |  5760.9 |  101.3 |       7.66 KB |        7.52 |
| ReadLine_    | Cols  | 50000   |    12.850 ms |  4.00 |  29 |  2270.9 |  257.0 |   88608.25 KB |   86,910.78 |
| CsvHelper    | Cols  | 50000   |    68.999 ms | 21.49 |  29 |   422.9 | 1380.0 |     445.85 KB |      437.31 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    33.615 ms |  1.00 |  29 |   868.1 |  672.3 |   13802.47 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.231 ms |  0.60 |  29 |  1442.4 |  404.6 |    13992.1 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    34.762 ms |  1.03 |  29 |   839.5 |  695.2 |    13962.2 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    97.204 ms |  2.89 |  29 |   300.2 | 1944.1 |   102133.9 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    83.550 ms |  2.49 |  29 |   349.3 | 1671.0 |   13970.66 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   629.552 ms |  1.00 | 583 |   927.3 |  629.6 |  266669.13 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   261.089 ms |  0.41 | 583 |  2236.0 |  261.1 |  267793.45 KB |        1.00 |
| Sylvan___    | Asset | 1000000 |   761.171 ms |  1.21 | 583 |   767.0 |  761.2 |  266825.09 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,636.526 ms |  2.60 | 583 |   356.7 | 1636.5 | 2038835.59 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,754.461 ms |  2.79 | 583 |   332.7 | 1754.5 |  266833.16 KB |        1.00 |
