```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26100.6899/24H2/2024Update/HudsonValley)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    11.228 ms |  1.00 |  33 | 2972.8 |  224.6 |      1.01 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     9.916 ms |  0.88 |  33 | 3365.9 |  198.3 |      1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.953 ms |  0.89 |  33 | 3353.4 |  199.1 |      1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    23.829 ms |  2.12 |  33 | 1400.7 |  476.6 |      7.66 KB |        7.60 |
| ReadLine_    | Row   | 50000   |    21.909 ms |  1.95 |  33 | 1523.4 |  438.2 | 108778.73 KB |  107,935.48 |
| CsvHelper    | Row   | 50000   |    71.571 ms |  6.38 |  33 |  466.4 | 1431.4 |     19.95 KB |       19.79 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.798 ms |  1.00 |  33 | 2608.0 |  256.0 |      1.01 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.689 ms |  1.07 |  33 | 2438.2 |  273.8 |      1.01 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.287 ms |  2.13 |  33 | 1223.2 |  545.7 |      7.67 KB |        7.61 |
| ReadLine_    | Cols  | 50000   |    23.201 ms |  1.81 |  33 | 1438.6 |  464.0 | 108778.73 KB |  107,935.48 |
| CsvHelper    | Cols  | 50000   |   104.298 ms |  8.15 |  33 |  320.0 | 2086.0 |     445.6 KB |      442.15 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    55.957 ms |  1.00 |  33 |  596.5 | 1119.1 |  13802.68 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    44.668 ms |  0.80 |  33 |  747.2 |  893.4 |  13934.89 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    68.353 ms |  1.22 |  33 |  488.3 | 1367.1 |  13961.88 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   260.030 ms |  4.65 |  33 |  128.4 | 5200.6 | 122304.52 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   127.992 ms |  2.29 |  33 |  260.8 | 2559.8 |  13971.32 KB |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,145.307 ms |  1.00 | 667 |  583.0 | 1145.3 | 266673.49 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   696.277 ms |  0.61 | 667 |  959.0 |  696.3 | 267979.84 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,499.974 ms |  1.31 | 667 |  445.1 | 1500.0 | 266827.47 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,060.701 ms |  3.55 | 667 |  164.4 | 4060.7 |   2442318 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,601.519 ms |  2.27 | 667 |  256.7 | 2601.5 | 266839.95 KB |        1.00 |
