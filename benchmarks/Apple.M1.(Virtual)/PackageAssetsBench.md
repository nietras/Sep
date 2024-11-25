```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-PJJVEM : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-PJJVEM  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.803 ms |  1.00 |  29 | 7648.4 |   76.1 |       1028 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.746 ms |  0.99 |  29 | 7765.1 |   74.9 |       1033 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.426 ms |  5.11 |  29 | 1497.3 |  388.5 |       6711 B |        6.53 |
| ReadLine_    | Row   | 50000   |    18.294 ms |  4.81 |  29 | 1589.9 |  365.9 |   90734887 B |   88,263.51 |
| CsvHelper    | Row   | 50000   |    43.002 ms | 11.31 |  29 |  676.4 |  860.0 |      20764 B |       20.20 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.721 ms |  1.00 |  29 | 5083.9 |  114.4 |       1035 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.967 ms |  1.04 |  29 | 4874.2 |  119.3 |        998 B |        0.96 |
| Sylvan___    | Cols  | 50000   |    22.434 ms |  3.93 |  29 | 1296.5 |  448.7 |       6958 B |        6.72 |
| ReadLine_    | Cols  | 50000   |    19.595 ms |  3.43 |  29 | 1484.3 |  391.9 |   90734891 B |   87,666.56 |
| CsvHelper    | Cols  | 50000   |    68.046 ms | 11.91 |  29 |  427.4 | 1360.9 |     456564 B |      441.12 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    33.600 ms |  1.00 |  29 |  865.6 |  672.0 |   14134044 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.317 ms |  0.67 |  29 | 1303.3 |  446.3 |   14244739 B |        1.01 |
| Sylvan___    | Asset | 50000   |    52.428 ms |  1.57 |  29 |  554.8 | 1048.6 |   14296318 B |        1.01 |
| ReadLine_    | Asset | 50000   |    91.200 ms |  2.72 |  29 |  318.9 | 1824.0 |  104586022 B |        7.40 |
| CsvHelper    | Asset | 50000   |   127.821 ms |  3.82 |  29 |  227.6 | 2556.4 |   14306376 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   592.214 ms |  1.00 | 581 |  982.5 |  592.2 |  273070232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   483.872 ms |  0.82 | 581 | 1202.5 |  483.9 |  281065624 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,065.275 ms |  1.80 | 581 |  546.2 | 1065.3 |  273233680 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,852.391 ms |  3.13 | 581 |  314.1 | 1852.4 | 2087769440 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,581.550 ms |  2.67 | 581 |  367.9 | 1581.5 |  273241784 B |        1.00 |
