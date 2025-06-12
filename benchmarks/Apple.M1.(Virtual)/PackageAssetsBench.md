```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-AKIMDM : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-AKIMDM  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.049 ms |  1.00 |  29 | 9539.7 |   61.0 |       1780 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.090 ms |  1.01 |  29 | 9412.3 |   61.8 |        968 B |        0.54 |
| Sep_Unescape | Row   | 50000   |     2.995 ms |  0.98 |  29 | 9711.6 |   59.9 |        967 B |        0.54 |
| Sylvan___    | Row   | 50000   |    19.494 ms |  6.39 |  29 | 1492.1 |  389.9 |       6958 B |        3.91 |
| ReadLine_    | Row   | 50000   |    18.381 ms |  6.03 |  29 | 1582.4 |  367.6 |   90734884 B |   50,974.65 |
| CsvHelper    | Row   | 50000   |    43.358 ms | 14.22 |  29 |  670.8 |  867.2 |      20692 B |       11.62 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.070 ms |  1.00 |  29 | 7146.0 |   81.4 |        974 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.508 ms |  1.11 |  29 | 6452.2 |   90.2 |        975 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.949 ms |  5.39 |  29 | 1325.1 |  439.0 |       6725 B |        6.90 |
| ReadLine_    | Cols  | 50000   |    19.604 ms |  4.82 |  29 | 1483.7 |  392.1 |   90734891 B |   93,156.97 |
| CsvHelper    | Cols  | 50000   |    71.862 ms | 17.66 |  29 |  404.7 | 1437.2 |     457440 B |      469.65 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    31.638 ms |  1.00 |  29 |  919.3 |  632.8 |   14134042 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.132 ms |  0.64 |  29 | 1444.7 |  402.6 |   14312402 B |        1.01 |
| Sylvan___    | Asset | 50000   |    52.805 ms |  1.67 |  29 |  550.8 | 1056.1 |   14296822 B |        1.01 |
| ReadLine_    | Asset | 50000   |    93.011 ms |  2.95 |  29 |  312.7 | 1860.2 |  104585942 B |        7.40 |
| CsvHelper    | Asset | 50000   |    77.498 ms |  2.46 |  29 |  375.3 | 1550.0 |   14306376 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   563.469 ms |  1.00 | 581 | 1032.7 |  563.5 |  273070312 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   454.372 ms |  0.81 | 581 | 1280.6 |  454.4 |  283171600 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,066.748 ms |  1.89 | 581 |  545.5 | 1066.7 |  273235400 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,778.023 ms |  3.16 | 581 |  327.3 | 1778.0 | 2087769280 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,593.827 ms |  2.83 | 581 |  365.1 | 1593.8 |  273247744 B |        1.00 |
