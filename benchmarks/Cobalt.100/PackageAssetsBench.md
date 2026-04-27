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
| Sep______    | Row   | 50000   |     4.784 ms |  1.00 |  29 | 6099.5 |   95.7 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.944 ms |  1.03 |  29 | 5902.7 |   98.9 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.758 ms |  1.00 |  29 | 6132.7 |   95.2 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.093 ms |  4.20 |  29 | 1452.3 |  401.9 |       7476 B |        7.79 |
| ReadLine_    | Row   | 50000   |    19.793 ms |  4.14 |  29 | 1474.3 |  395.9 |   90734827 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    54.257 ms | 11.35 |  29 |  537.8 | 1085.1 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.425 ms |  1.00 |  29 | 4541.5 |  128.5 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.060 ms |  1.10 |  29 | 4133.2 |  141.2 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.583 ms |  3.83 |  29 | 1187.0 |  491.7 |       7480 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    20.435 ms |  3.18 |  29 | 1428.0 |  408.7 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    88.547 ms | 13.78 |  29 |  329.6 | 1770.9 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    30.831 ms |  1.00 |  29 |  946.5 |  616.6 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    13.289 ms |  0.43 |  29 | 2195.9 |  265.8 |   14199299 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.379 ms |  1.83 |  29 |  517.6 | 1127.6 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   106.335 ms |  3.45 |  29 |  274.4 | 2126.7 |  104584184 B |        7.40 |
| CsvHelper    | Asset | 50000   |    97.313 ms |  3.16 |  29 |  299.9 | 1946.3 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   760.076 ms |  1.00 | 583 |  768.1 |  760.1 |  273063480 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   350.264 ms |  0.46 | 583 | 1666.7 |  350.3 |  283553488 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,228.423 ms |  1.62 | 583 |  475.2 | 1228.4 |  273225184 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,635.811 ms |  3.47 | 583 |  221.5 | 2635.8 | 2087765264 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,204.081 ms |  2.90 | 583 |  264.9 | 2204.1 |  273234944 B |        1.00 |
