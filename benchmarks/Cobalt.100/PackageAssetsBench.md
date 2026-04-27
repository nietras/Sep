```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
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
| Sep______    | Row   | 50000   |     5.195 ms |  1.00 |  29 | 5616.8 |  103.9 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.715 ms |  1.10 |  29 | 5106.0 |  114.3 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.432 ms |  1.05 |  29 | 5372.4 |  108.6 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.667 ms |  3.99 |  29 | 1412.0 |  413.3 |       7476 B |        7.79 |
| ReadLine_    | Row   | 50000   |    20.739 ms |  4.00 |  29 | 1407.1 |  414.8 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    54.592 ms | 10.53 |  29 |  534.5 | 1091.8 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.963 ms |  1.00 |  29 | 4190.9 |  139.3 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.811 ms |  1.12 |  29 | 3735.7 |  156.2 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.814 ms |  3.56 |  29 | 1176.0 |  496.3 |       7480 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    20.856 ms |  3.00 |  29 | 1399.2 |  417.1 |   90734827 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    88.627 ms | 12.73 |  29 |  329.3 | 1772.5 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    31.201 ms |  1.00 |  29 |  935.3 |  624.0 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.267 ms |  0.46 |  29 | 2045.3 |  285.3 |   14193488 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.442 ms |  1.81 |  29 |  517.0 | 1128.8 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   117.311 ms |  3.76 |  29 |  248.8 | 2346.2 |  104584230 B |        7.40 |
| CsvHelper    | Asset | 50000   |    97.873 ms |  3.14 |  29 |  298.2 | 1957.5 |   14308394 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   775.808 ms |  1.00 | 583 |  752.5 |  775.8 |  273063600 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   367.674 ms |  0.47 | 583 | 1587.8 |  367.7 |  281452296 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,237.548 ms |  1.60 | 583 |  471.7 | 1237.5 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,741.695 ms |  3.53 | 583 |  212.9 | 2741.7 | 2087765224 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,130.007 ms |  2.75 | 583 |  274.1 | 2130.0 |  273247280 B |        1.00 |
