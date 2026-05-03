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
| Sep______    | Row   | 50000   |     4.870 ms |  1.00 |  29 | 5991.7 |   97.4 |        968 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.892 ms |  1.00 |  29 | 5964.8 |   97.8 |        968 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.741 ms |  0.97 |  29 | 6155.2 |   94.8 |        968 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.150 ms |  4.14 |  29 | 1448.2 |  403.0 |       7476 B |        7.72 |
| ReadLine_    | Row   | 50000   |    20.025 ms |  4.11 |  29 | 1457.2 |  400.5 |   90734824 B |   93,734.32 |
| CsvHelper    | Row   | 50000   |    54.100 ms | 11.11 |  29 |  539.4 | 1082.0 |      20424 B |       21.10 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.347 ms |  1.00 |  29 | 4597.5 |  126.9 |        968 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.044 ms |  1.11 |  29 | 4142.7 |  140.9 |        968 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.847 ms |  3.76 |  29 | 1223.7 |  476.9 |       7479 B |        7.73 |
| ReadLine_    | Cols  | 50000   |    20.141 ms |  3.17 |  29 | 1448.9 |  402.8 |   90734824 B |   93,734.32 |
| CsvHelper    | Cols  | 50000   |    87.215 ms | 13.74 |  29 |  334.6 | 1744.3 |     456368 B |      471.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    31.897 ms |  1.00 |  29 |  914.9 |  637.9 |   14133008 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    16.561 ms |  0.52 |  29 | 1762.1 |  331.2 |   14203273 B |        1.00 |
| Sylvan___    | Asset | 50000   |    55.753 ms |  1.75 |  29 |  523.4 | 1115.1 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |    97.629 ms |  3.06 |  29 |  298.9 | 1952.6 |  104584230 B |        7.40 |
| CsvHelper    | Asset | 50000   |    97.872 ms |  3.07 |  29 |  298.2 | 1957.4 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   757.472 ms |  1.00 | 583 |  770.7 |  757.5 |  273063656 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   349.563 ms |  0.46 | 583 | 1670.0 |  349.6 |  280665352 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,218.216 ms |  1.61 | 583 |  479.2 | 1218.2 |  273225192 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,227.798 ms |  2.94 | 583 |  262.0 | 2227.8 | 2087765608 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,195.699 ms |  2.90 | 583 |  265.9 | 2195.7 |  273234944 B |        1.00 |
