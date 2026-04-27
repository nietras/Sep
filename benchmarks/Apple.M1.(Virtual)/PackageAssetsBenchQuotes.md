```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     7.182 ms |  1.00 |  33 | 4634.3 |  143.6 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.174 ms |  1.00 |  33 | 4639.2 |  143.5 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.016 ms |  0.98 |  33 | 4743.4 |  140.3 |        961 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.347 ms |  2.84 |  33 | 1635.7 |  406.9 |       7516 B |        7.83 |
| ReadLine_    | Row   | 50000   |    19.388 ms |  2.70 |  33 | 1716.6 |  387.8 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    46.241 ms |  6.45 |  33 |  719.8 |  924.8 |      20496 B |       21.35 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.389 ms |  1.00 |  33 | 3967.4 |  167.8 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.010 ms |  1.07 |  33 | 3694.0 |  180.2 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.468 ms |  2.80 |  33 | 1418.2 |  469.4 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    19.878 ms |  2.37 |  33 | 1674.3 |  397.6 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    70.359 ms |  8.39 |  33 |  473.0 | 1407.2 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.977 ms |  1.00 |  33 |  925.1 |  719.5 |   14133368 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    24.169 ms |  0.67 |  33 | 1377.1 |  483.4 |   14363550 B |        1.02 |
| Sylvan___    | Asset | 50000   |    51.132 ms |  1.43 |  33 |  650.9 | 1022.6 |   14295890 B |        1.01 |
| ReadLine_    | Asset | 50000   |   118.811 ms |  3.31 |  33 |  280.1 | 2376.2 |  125239786 B |        8.86 |
| CsvHelper    | Asset | 50000   |    78.883 ms |  2.20 |  33 |  421.9 | 1577.7 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   704.743 ms |  1.00 | 665 |  944.7 |  704.7 |  273067136 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   456.755 ms |  0.65 | 665 | 1457.7 |  456.8 |  282264200 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,121.438 ms |  1.59 | 665 |  593.7 | 1121.4 |  273239256 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,253.913 ms |  3.20 | 665 |  295.4 | 2253.9 | 2500933912 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,722.556 ms |  2.45 | 665 |  386.5 | 1722.6 |  273242152 B |        1.00 |
