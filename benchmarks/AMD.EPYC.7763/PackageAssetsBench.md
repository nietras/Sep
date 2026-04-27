```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.582 ms |  1.00 |  29 | 8147.7 |   71.6 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.137 ms |  1.16 |  29 | 7054.1 |   82.7 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.611 ms |  1.01 |  29 | 8081.2 |   72.2 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.566 ms |  1.27 |  29 | 6391.3 |   91.3 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    18.389 ms |  5.13 |  29 | 1586.9 |  367.8 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    65.810 ms | 18.38 |  29 |  443.4 | 1316.2 |      20.02 KB |       19.71 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.925 ms |  1.00 |  29 | 5924.6 |   98.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.815 ms |  1.18 |  29 | 5018.3 |  116.3 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.247 ms |  1.67 |  29 | 3538.6 |  164.9 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    20.493 ms |  4.16 |  29 | 1424.0 |  409.9 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |   109.164 ms | 22.16 |  29 |  267.3 | 2183.3 |      445.6 KB |      438.75 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    43.518 ms |  1.00 |  29 |  670.5 |  870.4 |   13802.22 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    35.287 ms |  0.81 |  29 |  827.0 |  705.7 |   13868.82 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    45.878 ms |  1.06 |  29 |  636.1 |  917.6 |   13962.16 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   146.523 ms |  3.37 |  29 |  199.2 | 2930.5 |  102133.92 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   128.638 ms |  2.96 |  29 |  226.8 | 2572.8 |   13970.02 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   979.535 ms |  1.00 | 583 |  596.0 |  979.5 |  266673.58 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   535.046 ms |  0.55 | 583 | 1091.1 |  535.0 |  277963.93 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,056.026 ms |  1.08 | 583 |  552.8 | 1056.0 |  266824.24 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,828.514 ms |  2.90 | 583 |  206.4 | 2828.5 | 2038834.58 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,574.995 ms |  2.64 | 583 |  226.7 | 2575.0 |  266835.56 KB |        1.00 |
