```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.4 (23H420) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-HYWXRS : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-HYWXRS  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.38 ms |  1.00 |  33 | 3206.3 |  207.6 |       1022 B |        1.00 |
| Sep_Async    | Row   | 50000   |    10.45 ms |  1.01 |  33 | 3186.0 |  208.9 |       1021 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    10.14 ms |  0.98 |  33 | 3282.8 |  202.8 |       1021 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.48 ms |  2.07 |  33 | 1549.7 |  429.5 |       6958 B |        6.81 |
| ReadLine_    | Row   | 50000   |    21.00 ms |  2.02 |  33 | 1584.9 |  420.0 |  111389487 B |  108,991.67 |
| CsvHelper    | Row   | 50000   |    46.86 ms |  4.51 |  33 |  710.2 |  937.3 |      20764 B |       20.32 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.11 ms |  1.00 |  33 | 2994.6 |  222.3 |       1102 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.11 ms |  1.09 |  33 | 2747.4 |  242.3 |       1102 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.78 ms |  2.23 |  33 | 1343.1 |  495.6 |       6731 B |        6.11 |
| ReadLine_    | Cols  | 50000   |    22.19 ms |  2.00 |  33 | 1500.2 |  443.7 |  111389493 B |  101,079.39 |
| CsvHelper    | Cols  | 50000   |    71.44 ms |  6.43 |  33 |  465.9 | 1428.8 |     459732 B |      417.18 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.79 ms |  1.00 |  33 |  956.6 |  695.8 |   14135314 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    26.01 ms |  0.75 |  33 | 1279.6 |  520.2 |   14209301 B |        1.01 |
| Sylvan___    | Asset | 50000   |    55.38 ms |  1.59 |  33 |  601.0 | 1107.5 |   14297549 B |        1.01 |
| ReadLine_    | Asset | 50000   |   124.17 ms |  3.57 |  33 |  268.0 | 2483.5 |  125240894 B |        8.86 |
| CsvHelper    | Asset | 50000   |    80.01 ms |  2.30 |  33 |  416.0 | 1600.2 |   14306376 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   790.14 ms |  1.00 | 665 |  842.6 |  790.1 |  273070232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   509.62 ms |  0.65 | 665 | 1306.5 |  509.6 |  280622856 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,180.82 ms |  1.49 | 665 |  563.8 | 1180.8 |  273228792 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,406.08 ms |  3.05 | 665 |  276.7 | 2406.1 | 2500937896 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,711.27 ms |  2.17 | 665 |  389.1 | 1711.3 |  273242512 B |        1.00 |
