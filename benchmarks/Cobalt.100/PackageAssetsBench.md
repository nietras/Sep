```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.774 ms |  1.00 |  29 | 6112.8 |   95.5 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.943 ms |  1.04 |  29 | 5904.0 |   98.9 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.783 ms |  1.00 |  29 | 6100.6 |   95.7 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.510 ms |  4.30 |  29 | 1422.8 |  410.2 |       7477 B |        7.79 |
| ReadLine_    | Row   | 50000   |    19.760 ms |  4.14 |  29 | 1476.8 |  395.2 |   90734827 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    54.339 ms | 11.39 |  29 |  537.0 | 1086.8 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.348 ms |  1.00 |  29 | 4597.1 |  127.0 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.089 ms |  1.12 |  29 | 4116.4 |  141.8 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.203 ms |  3.81 |  29 | 1205.7 |  484.1 |       7480 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    20.224 ms |  3.19 |  29 | 1442.9 |  404.5 |   90734827 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    88.599 ms | 13.96 |  29 |  329.4 | 1772.0 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    31.068 ms |  1.00 |  29 |  939.3 |  621.4 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.075 ms |  0.45 |  29 | 2073.3 |  281.5 |   14192592 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.204 ms |  1.81 |  29 |  519.2 | 1124.1 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   103.516 ms |  3.33 |  29 |  281.9 | 2070.3 |  104584180 B |        7.40 |
| CsvHelper    | Asset | 50000   |    97.607 ms |  3.14 |  29 |  299.0 | 1952.1 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   785.609 ms |  1.00 | 583 |  743.1 |  785.6 |  273063600 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   320.289 ms |  0.41 | 583 | 1822.7 |  320.3 |  281198680 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,241.650 ms |  1.58 | 583 |  470.2 | 1241.7 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,433.449 ms |  3.10 | 583 |  239.9 | 2433.4 | 2087765328 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,100.199 ms |  2.67 | 583 |  278.0 | 2100.2 |  273234848 B |        1.00 |
