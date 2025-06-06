```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-SMYKWG : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-SMYKWG  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     7.682 ms |  1.00 |  33 | 4332.6 |  153.6 |       1170 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.610 ms |  0.99 |  33 | 4373.7 |  152.2 |        992 B |        0.85 |
| Sep_Unescape | Row   | 50000   |     7.838 ms |  1.02 |  33 | 4246.4 |  156.8 |       1170 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.671 ms |  2.56 |  33 | 1691.9 |  393.4 |       6958 B |        5.95 |
| ReadLine_    | Row   | 50000   |    20.758 ms |  2.70 |  33 | 1603.3 |  415.2 |  111389487 B |   95,204.69 |
| CsvHelper    | Row   | 50000   |    46.904 ms |  6.11 |  33 |  709.6 |  938.1 |      20764 B |       17.75 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.699 ms |  1.00 |  33 | 3825.8 |  174.0 |        996 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.566 ms |  1.10 |  33 | 3479.3 |  191.3 |       1002 B |        1.01 |
| Sylvan___    | Cols  | 50000   |    23.190 ms |  2.67 |  33 | 1435.2 |  463.8 |       6958 B |        6.99 |
| ReadLine_    | Cols  | 50000   |    22.234 ms |  2.56 |  33 | 1496.9 |  444.7 |  111389493 B |  111,836.84 |
| CsvHelper    | Cols  | 50000   |    71.456 ms |  8.21 |  33 |  465.8 | 1429.1 |     457440 B |      459.28 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.324 ms |  1.00 |  33 |  942.2 |  706.5 |   14134026 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    23.104 ms |  0.66 |  33 | 1440.5 |  462.1 |   14288219 B |        1.01 |
| Sylvan___    | Asset | 50000   |    57.742 ms |  1.64 |  33 |  576.4 | 1154.8 |   14296830 B |        1.01 |
| ReadLine_    | Asset | 50000   |   129.407 ms |  3.68 |  33 |  257.2 | 2588.1 |  125241272 B |        8.86 |
| CsvHelper    | Asset | 50000   |    83.887 ms |  2.38 |  33 |  396.7 | 1677.7 |   14307000 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   730.671 ms |  1.00 | 665 |  911.2 |  730.7 |  273070152 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   478.347 ms |  0.66 | 665 | 1391.9 |  478.3 |  284454376 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,151.159 ms |  1.58 | 665 |  578.4 | 1151.2 |  273228920 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,536.257 ms |  3.47 | 665 |  262.5 | 2536.3 | 2500937864 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,001.062 ms |  2.74 | 665 |  332.7 | 2001.1 |  273239312 B |        1.00 |
