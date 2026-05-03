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
| Sep______    | Row   | 50000   |     7.085 ms |  1.00 |  33 | 4697.3 |  141.7 |        969 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.082 ms |  1.00 |  33 | 4699.8 |  141.6 |        968 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.553 ms |  1.07 |  33 | 4406.6 |  151.1 |        968 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.891 ms |  3.09 |  33 | 1520.4 |  437.8 |       7516 B |        7.76 |
| ReadLine_    | Row   | 50000   |    19.929 ms |  2.81 |  33 | 1670.0 |  398.6 |  111389416 B |  114,952.96 |
| CsvHelper    | Row   | 50000   |    46.336 ms |  6.54 |  33 |  718.3 |  926.7 |      20496 B |       21.15 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.380 ms |  1.00 |  33 | 3971.7 |  167.6 |        968 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.974 ms |  1.07 |  33 | 3708.5 |  179.5 |        968 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.207 ms |  2.77 |  33 | 1434.2 |  464.1 |       7478 B |        7.73 |
| ReadLine_    | Cols  | 50000   |    19.778 ms |  2.36 |  33 | 1682.8 |  395.6 |  111389416 B |  115,071.71 |
| CsvHelper    | Cols  | 50000   |    72.186 ms |  8.62 |  33 |  461.1 | 1443.7 |     456368 B |      471.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.815 ms |  1.00 |  33 |  929.3 |  716.3 |   14133384 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.413 ms |  0.57 |  33 | 1630.5 |  408.3 |   14264421 B |        1.01 |
| Sylvan___    | Asset | 50000   |    52.592 ms |  1.47 |  33 |  632.8 | 1051.8 |   14295924 B |        1.01 |
| ReadLine_    | Asset | 50000   |   116.780 ms |  3.27 |  33 |  285.0 | 2335.6 |  125239974 B |        8.86 |
| CsvHelper    | Asset | 50000   |    79.485 ms |  2.23 |  33 |  418.7 | 1589.7 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   728.883 ms |  1.00 | 665 |  913.5 |  728.9 |  273067048 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   483.202 ms |  0.66 | 665 | 1377.9 |  483.2 |  286605008 B |        1.05 |
| Sylvan___    | Asset | 1000000 | 1,108.336 ms |  1.52 | 665 |  600.7 | 1108.3 |  273226904 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,301.458 ms |  3.16 | 665 |  289.3 | 2301.5 | 2500933480 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,704.264 ms |  2.34 | 665 |  390.7 | 1704.3 |  273243168 B |        1.00 |
