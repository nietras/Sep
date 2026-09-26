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
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.74 ms |  1.00 |  33 | 3107.1 |  214.8 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.68 ms |  0.99 |  33 | 3125.7 |  213.6 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.40 ms |  0.97 |  33 | 3210.8 |  207.9 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.94 ms |  2.04 |  33 | 1521.0 |  438.9 |       7477 B |        7.79 |
| ReadLine_    | Row   | 50000   |    26.19 ms |  2.44 |  33 | 1274.6 |  523.7 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    63.14 ms |  5.88 |  33 |  528.6 | 1262.8 |      20424 B |       21.27 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.84 ms |  1.00 |  33 | 2818.5 |  236.8 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.19 ms |  1.11 |  33 | 2531.1 |  263.7 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.77 ms |  2.18 |  33 | 1295.2 |  515.4 |       7479 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    26.57 ms |  2.24 |  33 | 1256.2 |  531.4 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    94.19 ms |  7.95 |  33 |  354.3 | 1883.9 |     456296 B |      475.31 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    50.97 ms |  1.00 |  33 |  654.9 | 1019.4 |   14134348 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.89 ms |  0.63 |  33 | 1046.6 |  637.8 |   14233711 B |        1.01 |
| Sylvan___    | Asset | 50000   |    66.22 ms |  1.30 |  33 |  504.0 | 1324.4 |   14296106 B |        1.01 |
| ReadLine_    | Asset | 50000   |   154.95 ms |  3.04 |  33 |  215.4 | 3099.1 |  125240848 B |        8.86 |
| CsvHelper    | Asset | 50000   |   114.48 ms |  2.25 |  33 |  291.6 | 2289.6 |   14307160 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   986.81 ms |  1.00 | 667 |  676.6 |  986.8 |  273069960 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   462.51 ms |  0.47 | 667 | 1443.6 |  462.5 |  280451160 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,351.26 ms |  1.37 | 667 |  494.1 | 1351.3 |  273228792 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,757.15 ms |  2.79 | 667 |  242.2 | 2757.2 | 2500936272 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,369.09 ms |  2.40 | 667 |  281.8 | 2369.1 |  273244200 B |        1.00 |
