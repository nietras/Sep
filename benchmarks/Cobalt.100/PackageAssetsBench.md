```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 12.01 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     5.256 ms |  1.00 |  29 | 5552.0 |  105.1 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.540 ms |  1.06 |  29 | 5267.2 |  110.8 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.223 ms |  1.00 |  29 | 5587.0 |  104.5 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.664 ms |  3.76 |  29 | 1484.0 |  393.3 |       7470 B |        7.78 |
| ReadLine_    | Row   | 50000   |    21.891 ms |  4.18 |  29 | 1333.0 |  437.8 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    56.933 ms | 10.88 |  29 |  512.6 | 1138.7 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     7.250 ms |  1.00 |  29 | 4024.8 |  145.0 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.533 ms |  1.04 |  29 | 3873.9 |  150.7 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.561 ms |  3.25 |  29 | 1238.5 |  471.2 |       7478 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    22.138 ms |  3.05 |  29 | 1318.2 |  442.8 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    87.863 ms | 12.12 |  29 |  332.1 | 1757.3 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    47.535 ms |  1.00 |  29 |  613.9 |  950.7 |   14133817 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.961 ms |  0.65 |  29 |  942.5 |  619.2 |   14250390 B |        1.01 |
| Sylvan___    | Asset | 50000   |    63.919 ms |  1.35 |  29 |  456.5 | 1278.4 |   14296236 B |        1.01 |
| ReadLine_    | Asset | 50000   |   113.735 ms |  2.40 |  29 |  256.6 | 2274.7 |  104585584 B |        7.40 |
| CsvHelper    | Asset | 50000   |   107.577 ms |  2.27 |  29 |  271.3 | 2151.5 |   14305756 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   898.594 ms |  1.00 | 583 |  649.7 |  898.6 |  273070128 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   438.261 ms |  0.49 | 583 | 1332.0 |  438.3 |  283016344 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,299.266 ms |  1.45 | 583 |  449.3 | 1299.3 |  273228480 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,218.690 ms |  2.47 | 583 |  263.1 | 2218.7 | 2087768344 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,212.577 ms |  2.46 | 583 |  263.8 | 2212.6 |  273238560 B |        1.00 |
