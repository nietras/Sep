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
| Sep______    | Row   | 50000   |    10.93 ms |  1.00 |  33 | 3054.7 |  218.5 |        962 B |        1.00 |
| Sep_Async    | Row   | 50000   |    11.29 ms |  1.03 |  33 | 2955.7 |  225.9 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.99 ms |  1.01 |  33 | 3035.8 |  219.9 |        962 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.04 ms |  2.11 |  33 | 1448.8 |  460.8 |       7478 B |        7.77 |
| ReadLine_    | Row   | 50000   |    24.59 ms |  2.25 |  33 | 1357.2 |  491.8 |  111389416 B |  115,789.41 |
| CsvHelper    | Row   | 50000   |    62.63 ms |  5.73 |  33 |  532.9 | 1252.6 |      20424 B |       21.23 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.58 ms |  1.00 |  33 | 2654.0 |  251.5 |        962 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.81 ms |  1.10 |  33 | 2417.5 |  276.1 |        962 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.37 ms |  2.18 |  33 | 1219.4 |  547.4 |       7481 B |        7.78 |
| ReadLine_    | Cols  | 50000   |    24.86 ms |  1.98 |  33 | 1342.5 |  497.2 |  111389416 B |  115,789.41 |
| CsvHelper    | Cols  | 50000   |    96.36 ms |  7.66 |  33 |  346.4 | 1927.2 |     456368 B |      474.40 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    36.86 ms |  1.00 |  33 |  905.6 |  737.2 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.75 ms |  0.40 |  33 | 2262.9 |  295.0 |   14192686 B |        1.00 |
| Sylvan___    | Asset | 50000   |    58.62 ms |  1.59 |  33 |  569.4 | 1172.4 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   129.38 ms |  3.51 |  33 |  258.0 | 2587.7 |  125239048 B |        8.86 |
| CsvHelper    | Asset | 50000   |   106.55 ms |  2.89 |  33 |  313.3 | 2130.9 |   14305310 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   912.17 ms |  1.00 | 667 |  732.0 |  912.2 |  273063624 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   383.66 ms |  0.42 | 667 | 1740.4 |  383.7 |  275511208 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,282.69 ms |  1.41 | 667 |  520.6 | 1282.7 |  273225152 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,360.52 ms |  3.68 | 667 |  198.7 | 3360.5 | 2500939640 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,391.60 ms |  2.62 | 667 |  279.2 | 2391.6 |  273234800 B |        1.00 |
