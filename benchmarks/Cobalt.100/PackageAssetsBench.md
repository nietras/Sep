```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.6899) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     5.786 ms |  1.00 |  29 | 5043.2 |  115.7 |        952 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.919 ms |  1.02 |  29 | 4929.9 |  118.4 |        952 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.527 ms |  0.96 |  29 | 5280.2 |  110.5 |        952 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.097 ms |  3.65 |  29 | 1383.2 |  421.9 |       6653 B |        6.99 |
| ReadLine_    | Row   | 50000   |    20.407 ms |  3.53 |  29 | 1430.0 |  408.1 |   90734824 B |   95,309.69 |
| CsvHelper    | Row   | 50000   |    54.870 ms |  9.49 |  29 |  531.8 | 1097.4 |      20424 B |       21.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     7.355 ms |  1.00 |  29 | 3967.3 |  147.1 |        952 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.875 ms |  1.07 |  29 | 3705.7 |  157.5 |        952 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.954 ms |  3.39 |  29 | 1169.4 |  499.1 |       6656 B |        6.99 |
| ReadLine_    | Cols  | 50000   |    20.826 ms |  2.83 |  29 | 1401.2 |  416.5 |   90734827 B |   95,309.69 |
| CsvHelper    | Cols  | 50000   |    88.817 ms | 12.08 |  29 |  328.6 | 1776.3 |     456368 B |      479.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    31.305 ms |  1.00 |  29 |  932.2 |  626.1 |   14132992 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    13.853 ms |  0.44 |  29 | 2106.5 |  277.1 |   14184144 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.747 ms |  1.81 |  29 |  514.2 | 1134.9 |   14295595 B |        1.01 |
| ReadLine_    | Asset | 50000   |   127.959 ms |  4.09 |  29 |  228.1 | 2559.2 |  104584112 B |        7.40 |
| CsvHelper    | Asset | 50000   |    98.423 ms |  3.14 |  29 |  296.5 | 1968.5 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   790.081 ms |  1.00 | 583 |  738.9 |  790.1 |  273063584 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   376.771 ms |  0.48 | 583 | 1549.4 |  376.8 |  282250016 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,234.977 ms |  1.56 | 583 |  472.7 | 1235.0 |  273225232 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,851.876 ms |  3.61 | 583 |  204.7 | 2851.9 | 2087764592 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,133.327 ms |  2.70 | 583 |  273.6 | 2133.3 |  273234944 B |        1.00 |
