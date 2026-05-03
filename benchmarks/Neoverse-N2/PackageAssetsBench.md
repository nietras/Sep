```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.889 ms |  1.00 |  29 | 5949.3 |   97.8 |        968 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.277 ms |  1.08 |  29 | 5512.2 |  105.5 |        968 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.987 ms |  1.02 |  29 | 5831.9 |   99.7 |       1147 B |        1.18 |
| Sylvan___    | Row   | 50000   |    20.569 ms |  4.21 |  29 | 1414.0 |  411.4 |       7476 B |        7.72 |
| ReadLine_    | Row   | 50000   |    22.541 ms |  4.61 |  29 | 1290.3 |  450.8 |   90734824 B |   93,734.32 |
| CsvHelper    | Row   | 50000   |    54.679 ms | 11.19 |  29 |  531.9 | 1093.6 |      20424 B |       21.10 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.688 ms |  1.00 |  29 | 4349.1 |  133.8 |        968 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.663 ms |  1.15 |  29 | 3795.5 |  153.3 |        968 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.445 ms |  3.81 |  29 | 1143.1 |  508.9 |       7516 B |        7.76 |
| ReadLine_    | Cols  | 50000   |    23.114 ms |  3.46 |  29 | 1258.3 |  462.3 |   90734824 B |   93,734.32 |
| CsvHelper    | Cols  | 50000   |    89.462 ms | 13.38 |  29 |  325.1 | 1789.2 |     456368 B |      471.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    32.005 ms |  1.00 |  29 |  908.8 |  640.1 |   14133008 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.061 ms |  0.44 |  29 | 2068.6 |  281.2 |   14185127 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.848 ms |  1.78 |  29 |  511.6 | 1137.0 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   117.917 ms |  3.68 |  29 |  246.7 | 2358.3 |  104584100 B |        7.40 |
| CsvHelper    | Asset | 50000   |    99.535 ms |  3.11 |  29 |  292.2 | 1990.7 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   785.815 ms |  1.00 | 581 |  740.5 |  785.8 |  273063720 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   380.306 ms |  0.48 | 581 | 1530.0 |  380.3 |  281466968 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,243.325 ms |  1.58 | 581 |  468.0 | 1243.3 |  273225264 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,738.606 ms |  3.49 | 581 |  212.5 | 2738.6 | 2087765016 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,225.696 ms |  2.83 | 581 |  261.4 | 2225.7 |  273234984 B |        1.00 |
