```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.92 ms |  1.00 |  33 | 3057.5 |  218.3 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |    11.08 ms |  1.02 |  33 | 3012.0 |  221.6 |        962 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.98 ms |  1.01 |  33 | 3040.5 |  219.6 |        962 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.03 ms |  2.11 |  33 | 1449.2 |  460.6 |       7478 B |        7.79 |
| ReadLine_    | Row   | 50000   |    24.21 ms |  2.22 |  33 | 1378.4 |  484.3 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    62.88 ms |  5.76 |  33 |  530.8 | 1257.7 |      20424 B |       21.27 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.47 ms |  1.00 |  33 | 2677.2 |  249.3 |        962 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.73 ms |  1.10 |  33 | 2430.4 |  274.7 |        962 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.41 ms |  2.20 |  33 | 1217.5 |  548.3 |       7481 B |        7.78 |
| ReadLine_    | Cols  | 50000   |    25.01 ms |  2.01 |  33 | 1334.7 |  500.2 |  111389416 B |  115,789.41 |
| CsvHelper    | Cols  | 50000   |    95.34 ms |  7.65 |  33 |  350.1 | 1906.8 |     456368 B |      474.40 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    36.81 ms |  1.00 |  33 |  906.7 |  736.3 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.89 ms |  0.40 |  33 | 2241.8 |  297.8 |   14190284 B |        1.00 |
| Sylvan___    | Asset | 50000   |    58.64 ms |  1.59 |  33 |  569.2 | 1172.9 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   126.40 ms |  3.43 |  33 |  264.1 | 2528.0 |  125238788 B |        8.86 |
| CsvHelper    | Asset | 50000   |   106.21 ms |  2.89 |  33 |  314.3 | 2124.2 |   14305310 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   912.09 ms |  1.00 | 667 |  732.1 |  912.1 |  273063648 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   389.94 ms |  0.43 | 667 | 1712.3 |  389.9 |  282220464 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,276.04 ms |  1.40 | 667 |  523.3 | 1276.0 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,123.47 ms |  3.42 | 667 |  213.8 | 3123.5 | 2500931904 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,292.40 ms |  2.51 | 667 |  291.3 | 2292.4 |  273234848 B |        1.00 |
