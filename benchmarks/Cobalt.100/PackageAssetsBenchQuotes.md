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
| Sep______    | Row   | 50000   |    10.83 ms |  1.00 |  33 | 3081.7 |  216.6 |        962 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.59 ms |  0.98 |  33 | 3152.0 |  211.8 |        961 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.36 ms |  0.96 |  33 | 3221.0 |  207.2 |        961 B |        1.00 |
| Sylvan___    | Row   | 50000   |    22.95 ms |  2.12 |  33 | 1454.6 |  458.9 |       7478 B |        7.77 |
| ReadLine_    | Row   | 50000   |    24.15 ms |  2.23 |  33 | 1382.3 |  482.9 |  111389416 B |  115,789.41 |
| CsvHelper    | Row   | 50000   |    62.64 ms |  5.78 |  33 |  532.9 | 1252.7 |      20424 B |       21.23 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.03 ms |  1.00 |  33 | 2774.5 |  240.6 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.62 ms |  1.13 |  33 | 2449.9 |  272.5 |        962 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.37 ms |  2.28 |  33 | 1219.3 |  547.5 |       7481 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    24.65 ms |  2.05 |  33 | 1354.2 |  492.9 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    96.29 ms |  8.01 |  33 |  346.6 | 1925.8 |     456368 B |      475.38 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    37.80 ms |  1.00 |  33 |  883.1 |  755.9 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.31 ms |  0.38 |  33 | 2333.2 |  286.1 |   14191098 B |        1.00 |
| Sylvan___    | Asset | 50000   |    58.65 ms |  1.55 |  33 |  569.1 | 1172.9 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   122.74 ms |  3.25 |  33 |  271.9 | 2454.8 |  125238788 B |        8.86 |
| CsvHelper    | Asset | 50000   |   105.22 ms |  2.78 |  33 |  317.2 | 2104.3 |   14305310 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   883.01 ms |  1.00 | 667 |  756.2 |  883.0 |  273063648 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   376.30 ms |  0.43 | 667 | 1774.4 |  376.3 |  279760744 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,289.54 ms |  1.46 | 667 |  517.8 | 1289.5 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,169.20 ms |  3.59 | 667 |  210.7 | 3169.2 | 2500931888 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,278.99 ms |  2.58 | 667 |  293.0 | 2279.0 |  273237328 B |        1.00 |
