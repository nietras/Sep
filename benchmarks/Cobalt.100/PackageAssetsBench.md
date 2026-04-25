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
| Sep______    | Row   | 50000   |     5.340 ms |  1.00 |  29 | 5464.7 |  106.8 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.597 ms |  1.05 |  29 | 5213.3 |  111.9 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.640 ms |  1.06 |  29 | 5174.2 |  112.8 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.682 ms |  3.87 |  29 | 1410.9 |  413.6 |       7476 B |        7.79 |
| ReadLine_    | Row   | 50000   |    20.551 ms |  3.85 |  29 | 1419.9 |  411.0 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    54.913 ms | 10.29 |  29 |  531.4 | 1098.3 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     7.362 ms |  1.00 |  29 | 3963.7 |  147.2 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.858 ms |  1.07 |  29 | 3713.3 |  157.2 |        961 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.559 ms |  3.47 |  29 | 1141.7 |  511.2 |       7480 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    20.873 ms |  2.84 |  29 | 1398.0 |  417.5 |   90734827 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    87.639 ms | 11.90 |  29 |  333.0 | 1752.8 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    31.249 ms |  1.00 |  29 |  933.8 |  625.0 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    18.210 ms |  0.58 |  29 | 1602.5 |  364.2 |   14202296 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.699 ms |  1.81 |  29 |  514.7 | 1134.0 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   122.579 ms |  3.92 |  29 |  238.1 | 2451.6 |  104588220 B |        7.40 |
| CsvHelper    | Asset | 50000   |    98.133 ms |  3.14 |  29 |  297.4 | 1962.7 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   780.554 ms |  1.00 | 583 |  747.9 |  780.6 |  273063648 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   357.693 ms |  0.46 | 583 | 1632.1 |  357.7 |  281281184 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,235.402 ms |  1.58 | 583 |  472.5 | 1235.4 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,752.618 ms |  3.53 | 583 |  212.1 | 2752.6 | 2087772960 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,146.407 ms |  2.75 | 583 |  272.0 | 2146.4 |  273234776 B |        1.00 |
