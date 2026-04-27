```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    11.75 ms |  1.00 |  33 | 2839.9 |  235.1 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.30 ms |  0.96 |  33 | 2954.0 |  226.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.21 ms |  0.95 |  33 | 2977.9 |  224.2 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    23.94 ms |  2.04 |  33 | 1394.0 |  478.9 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    24.39 ms |  2.08 |  33 | 1368.5 |  487.8 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    81.92 ms |  6.97 |  33 |  407.5 | 1638.3 |      19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    14.59 ms |  1.00 |  33 | 2288.1 |  291.8 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    15.13 ms |  1.04 |  33 | 2205.4 |  302.7 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    33.16 ms |  2.27 |  33 | 1006.5 |  663.2 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    25.59 ms |  1.75 |  33 | 1304.4 |  511.7 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   108.72 ms |  7.46 |  33 |  307.0 | 2174.3 |     445.67 KB |      438.82 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    51.79 ms |  1.00 |  33 |  644.5 | 1035.8 |   13802.23 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    34.29 ms |  0.66 |  33 |  973.5 |  685.7 |   13860.16 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    72.69 ms |  1.41 |  33 |  459.2 | 1453.8 |    13962.3 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   157.69 ms |  3.06 |  33 |  211.7 | 3153.8 |  122305.17 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.06 ms |  2.44 |  33 |  264.8 | 2521.2 |   13970.46 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,312.16 ms |  1.07 | 667 |  508.9 | 1312.2 |  266668.02 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   648.57 ms |  0.53 | 667 | 1029.5 |  648.6 |  272884.59 KB |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,491.84 ms |  1.21 | 667 |  447.6 | 1491.8 |  266826.51 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,872.98 ms |  3.15 | 667 |  172.4 | 3873.0 | 2442318.23 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,680.68 ms |  2.18 | 667 |  249.1 | 2680.7 |  266835.39 KB |        1.00 |
