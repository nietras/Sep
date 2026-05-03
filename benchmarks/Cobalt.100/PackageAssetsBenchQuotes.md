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
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     9.985 ms |  1.00 |  33 | 3342.6 |  199.7 |        968 B |        1.00 |
| Sep_Async    | Row   | 50000   |     9.966 ms |  1.00 |  33 | 3349.0 |  199.3 |        968 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.846 ms |  0.99 |  33 | 3389.9 |  196.9 |        969 B |        1.00 |
| Sylvan___    | Row   | 50000   |    22.040 ms |  2.21 |  33 | 1514.4 |  440.8 |       7478 B |        7.73 |
| ReadLine_    | Row   | 50000   |    23.077 ms |  2.31 |  33 | 1446.4 |  461.5 |  111389419 B |  115,071.71 |
| CsvHelper    | Row   | 50000   |    62.900 ms |  6.30 |  33 |  530.6 | 1258.0 |      20424 B |       21.10 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.192 ms |  1.00 |  33 | 2982.3 |  223.8 |        968 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.495 ms |  1.12 |  33 | 2671.3 |  249.9 |        968 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    26.411 ms |  2.36 |  33 | 1263.8 |  528.2 |       7481 B |        7.73 |
| ReadLine_    | Cols  | 50000   |    23.830 ms |  2.13 |  33 | 1400.6 |  476.6 |  111389416 B |  115,071.71 |
| CsvHelper    | Cols  | 50000   |    96.575 ms |  8.63 |  33 |  345.6 | 1931.5 |     456374 B |      471.46 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    36.256 ms |  1.00 |  33 |  920.6 |  725.1 |   14133008 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    13.841 ms |  0.38 |  33 | 2411.5 |  276.8 |   14191304 B |        1.00 |
| Sylvan___    | Asset | 50000   |    57.851 ms |  1.60 |  33 |  577.0 | 1157.0 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   107.774 ms |  2.97 |  33 |  309.7 | 2155.5 |  125238788 B |        8.86 |
| CsvHelper    | Asset | 50000   |   105.403 ms |  2.91 |  33 |  316.7 | 2108.1 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   889.639 ms |  1.00 | 667 |  750.5 |  889.6 |  273063656 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   367.391 ms |  0.41 | 667 | 1817.4 |  367.4 |  276236648 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,276.644 ms |  1.44 | 667 |  523.0 | 1276.6 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,824.136 ms |  3.17 | 667 |  236.4 | 2824.1 | 2500932992 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,255.135 ms |  2.53 | 667 |  296.1 | 2255.1 |  273234848 B |        1.00 |
