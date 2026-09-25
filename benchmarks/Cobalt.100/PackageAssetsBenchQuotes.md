```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.72 GB Available
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
| Sep______    | Row   | 50000   |    10.56 ms |  1.00 |  33 | 3159.4 |  211.3 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.34 ms |  0.98 |  33 | 3228.1 |  206.8 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.41 ms |  0.99 |  33 | 3207.0 |  208.2 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.83 ms |  2.07 |  33 | 1529.0 |  436.6 |       7477 B |        7.79 |
| ReadLine_    | Row   | 50000   |    24.49 ms |  2.32 |  33 | 1362.9 |  489.8 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    64.80 ms |  6.13 |  33 |  515.1 | 1296.1 |      20424 B |       21.27 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.81 ms |  1.00 |  33 | 2826.7 |  236.2 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.34 ms |  1.13 |  33 | 2502.0 |  266.8 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.94 ms |  2.20 |  33 | 1287.0 |  518.7 |       7479 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    24.90 ms |  2.11 |  33 | 1340.4 |  498.0 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    94.71 ms |  8.02 |  33 |  352.4 | 1894.3 |     456296 B |      475.31 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    39.77 ms |  1.00 |  33 |  839.2 |  795.4 |   14132928 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.07 ms |  0.55 |  33 | 1512.3 |  441.4 |   14207943 B |        1.01 |
| Sylvan___    | Asset | 50000   |    57.88 ms |  1.46 |  33 |  576.6 | 1157.6 |   14295541 B |        1.01 |
| ReadLine_    | Asset | 50000   |   132.02 ms |  3.32 |  33 |  252.8 | 2640.3 |  125238912 B |        8.86 |
| CsvHelper    | Asset | 50000   |   106.59 ms |  2.68 |  33 |  313.1 | 2131.9 |   14305232 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   883.69 ms |  1.00 | 667 |  755.6 |  883.7 |  273063872 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   376.93 ms |  0.43 | 667 | 1771.4 |  376.9 |  275769184 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,262.71 ms |  1.43 | 667 |  528.8 | 1262.7 |  273225560 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,346.77 ms |  3.79 | 667 |  199.5 | 3346.8 | 2500932928 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,246.99 ms |  2.54 | 667 |  297.2 | 2247.0 |  273235400 B |        1.00 |
