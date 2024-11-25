```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  Job-LKXTKX : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=Job-LKXTKX  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     2.188 ms |  1.00 |  29 | 13339.9 |   43.8 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.214 ms |  1.01 |  29 | 13181.4 |   44.3 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     2.825 ms |  1.29 |  29 | 10328.7 |   56.5 |       7.66 KB |        7.53 |
| ReadLine_    | Row   | 50000   |    11.707 ms |  5.35 |  29 |  2492.7 |  234.1 |   88608.25 KB |   87,161.24 |
| CsvHelper    | Row   | 50000   |    43.779 ms | 20.01 |  29 |   666.6 |  875.6 |      20.04 KB |       19.71 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     3.210 ms |  1.00 |  29 |  9091.8 |   64.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     3.856 ms |  1.20 |  29 |  7567.4 |   77.1 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     5.072 ms |  1.58 |  29 |  5753.7 |  101.4 |       7.66 KB |        7.51 |
| ReadLine_    | Cols  | 50000   |    13.080 ms |  4.08 |  29 |  2230.9 |  261.6 |   88608.25 KB |   86,827.61 |
| CsvHelper    | Cols  | 50000   |    71.032 ms | 22.13 |  29 |   410.8 | 1420.6 |     445.86 KB |      436.90 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    32.075 ms |  1.00 |  29 |   909.8 |  641.5 |   13802.45 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    19.243 ms |  0.60 |  29 |  1516.5 |  384.9 |   13993.52 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    34.429 ms |  1.07 |  29 |   847.6 |  688.6 |   13963.52 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    95.729 ms |  2.99 |  29 |   304.8 | 1914.6 |   102133.9 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    82.784 ms |  2.58 |  29 |   352.5 | 1655.7 |   13970.93 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   627.827 ms |  1.00 | 583 |   929.8 |  627.8 |  266680.78 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   254.075 ms |  0.40 | 583 |  2297.7 |  254.1 |   267692.5 KB |        1.00 |
| Sylvan___    | Asset | 1000000 |   736.667 ms |  1.17 | 583 |   792.5 |  736.7 |  266825.22 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,585.514 ms |  2.53 | 583 |   368.2 | 1585.5 | 2038834.79 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,661.220 ms |  2.65 | 583 |   351.4 | 1661.2 |   266833.3 KB |        1.00 |
