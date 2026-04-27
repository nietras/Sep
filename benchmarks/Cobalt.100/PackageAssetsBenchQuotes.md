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
| Sep______    | Row   | 50000   |    10.21 ms |  1.00 |  33 | 3270.0 |  204.1 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |    11.00 ms |  1.08 |  33 | 3035.4 |  219.9 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.99 ms |  1.08 |  33 | 3037.7 |  219.8 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.10 ms |  2.26 |  33 | 1445.1 |  462.0 |       7478 B |        7.79 |
| ReadLine_    | Row   | 50000   |    24.51 ms |  2.40 |  33 | 1361.6 |  490.3 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    63.23 ms |  6.20 |  33 |  527.9 | 1264.6 |      20424 B |       21.27 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.48 ms |  1.00 |  33 | 2674.4 |  249.6 |        962 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.75 ms |  1.10 |  33 | 2428.2 |  274.9 |        962 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.38 ms |  2.19 |  33 | 1219.1 |  547.6 |       7481 B |        7.78 |
| ReadLine_    | Cols  | 50000   |    25.04 ms |  2.01 |  33 | 1332.9 |  500.8 |  111389416 B |  115,789.41 |
| CsvHelper    | Cols  | 50000   |    96.05 ms |  7.70 |  33 |  347.5 | 1920.9 |     456368 B |      474.40 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    37.84 ms |  1.00 |  33 |  882.1 |  756.8 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.43 ms |  0.38 |  33 | 2312.7 |  288.6 |   14191294 B |        1.00 |
| Sylvan___    | Asset | 50000   |    58.66 ms |  1.55 |  33 |  569.0 | 1173.2 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   118.84 ms |  3.14 |  33 |  280.9 | 2376.8 |  125238788 B |        8.86 |
| CsvHelper    | Asset | 50000   |   105.32 ms |  2.78 |  33 |  316.9 | 2106.3 |   14308394 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   878.24 ms |  1.00 | 667 |  760.3 |  878.2 |  273063648 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   372.51 ms |  0.42 | 667 | 1792.4 |  372.5 |  280388504 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,286.70 ms |  1.47 | 667 |  518.9 | 1286.7 |  273225312 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,117.43 ms |  3.55 | 667 |  214.2 | 3117.4 | 2500931688 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,229.00 ms |  2.54 | 667 |  299.6 | 2229.0 |  273238320 B |        1.00 |
