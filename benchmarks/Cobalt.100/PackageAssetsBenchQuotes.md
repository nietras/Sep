```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9725/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.96 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     7.688 ms |  1.00 |  33 | 4341.4 |  153.8 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     8.028 ms |  1.04 |  33 | 4157.6 |  160.6 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.955 ms |  1.30 |  33 | 3353.0 |  199.1 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.869 ms |  2.85 |  33 | 1526.3 |  437.4 |      7477 B |       25.96 |
| CsvHelper    | Row   | 50000   |    19.739 ms |  2.57 |  33 | 1690.9 |  394.8 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     9.402 ms |  1.00 |  33 | 3549.9 |  188.0 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.629 ms |  1.34 |  33 | 2642.8 |  252.6 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    26.124 ms |  2.78 |  33 | 1277.7 |  522.5 |      7480 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    44.097 ms |  4.69 |  33 |  756.9 |  881.9 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    50.523 ms |  1.00 |  33 |  660.6 | 1010.5 |  14133297 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.947 ms |  0.63 |  33 | 1044.8 |  638.9 |  14252720 B |        1.01 |
| Sylvan___    | Asset | 50000   |    64.791 ms |  1.28 |  33 |  515.2 | 1295.8 |  14296306 B |        1.01 |
| CsvHelper    | Asset | 50000   |    66.115 ms |  1.31 |  33 |  504.8 | 1322.3 |  15112274 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   994.238 ms |  1.00 | 667 |  671.6 |  994.2 | 273069304 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   488.136 ms |  0.49 | 667 | 1367.9 |  488.1 | 279729544 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,359.151 ms |  1.37 | 667 |  491.3 | 1359.2 | 273228664 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,386.817 ms |  1.39 | 667 |  481.5 | 1386.8 | 274047496 B |        1.00 |
