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
| Sep______    | Row   | 50000   |     5.375 ms |  1.00 |  29 | 5429.1 |  107.5 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.605 ms |  1.04 |  29 | 5206.6 |  112.1 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.445 ms |  1.01 |  29 | 5359.3 |  108.9 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.722 ms |  3.86 |  29 | 1408.2 |  414.4 |       7476 B |        7.79 |
| ReadLine_    | Row   | 50000   |    20.449 ms |  3.81 |  29 | 1427.0 |  409.0 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    54.550 ms | 10.15 |  29 |  534.9 | 1091.0 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     7.241 ms |  1.00 |  29 | 4030.2 |  144.8 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.929 ms |  1.10 |  29 | 3680.1 |  158.6 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.992 ms |  3.45 |  29 | 1167.6 |  499.8 |       7480 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    20.860 ms |  2.88 |  29 | 1398.9 |  417.2 |   90734827 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    88.625 ms | 12.24 |  29 |  329.3 | 1772.5 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    32.394 ms |  1.00 |  29 |  900.8 |  647.9 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    17.705 ms |  0.55 |  29 | 1648.2 |  354.1 |   14205154 B |        1.01 |
| Sylvan___    | Asset | 50000   |    56.638 ms |  1.75 |  29 |  515.2 | 1132.8 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   122.235 ms |  3.77 |  29 |  238.7 | 2444.7 |  104584120 B |        7.40 |
| CsvHelper    | Asset | 50000   |    98.252 ms |  3.03 |  29 |  297.0 | 1965.0 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   777.687 ms |  1.00 | 583 |  750.7 |  777.7 |  273063600 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   364.900 ms |  0.47 | 583 | 1599.8 |  364.9 |  281136472 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,227.866 ms |  1.58 | 583 |  475.4 | 1227.9 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,642.603 ms |  3.40 | 583 |  220.9 | 2642.6 | 2087764912 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,129.342 ms |  2.74 | 583 |  274.2 | 2129.3 |  273234776 B |        1.00 |
