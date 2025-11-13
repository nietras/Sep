```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.6899) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    11.06 ms |  1.00 |  33 | 3017.0 |  221.3 |        953 B |        1.00 |
| Sep_Async    | Row   | 50000   |    11.38 ms |  1.03 |  33 | 2934.3 |  227.5 |        954 B |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.28 ms |  1.02 |  33 | 2958.1 |  225.7 |        954 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.17 ms |  2.09 |  33 | 1440.4 |  463.4 |       6654 B |        6.98 |
| ReadLine_    | Row   | 50000   |    24.39 ms |  2.20 |  33 | 1368.6 |  487.8 |  111389416 B |  116,882.91 |
| CsvHelper    | Row   | 50000   |    63.45 ms |  5.74 |  33 |  526.0 | 1269.0 |      20424 B |       21.43 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    12.67 ms |  1.00 |  33 | 2634.1 |  253.4 |        952 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.86 ms |  1.09 |  33 | 2408.9 |  277.1 |        954 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.54 ms |  2.17 |  33 | 1211.8 |  550.9 |       6657 B |        6.99 |
| ReadLine_    | Cols  | 50000   |    24.94 ms |  1.97 |  33 | 1338.1 |  498.9 |  111389419 B |  117,005.69 |
| CsvHelper    | Cols  | 50000   |    97.05 ms |  7.66 |  33 |  343.9 | 1941.0 |     456368 B |      479.38 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    38.12 ms |  1.00 |  33 |  875.5 |  762.4 |   14132992 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    15.19 ms |  0.40 |  33 | 2198.0 |  303.7 |   14190126 B |        1.00 |
| Sylvan___    | Asset | 50000   |    58.84 ms |  1.54 |  33 |  567.3 | 1176.8 |   14295595 B |        1.01 |
| ReadLine_    | Asset | 50000   |   135.98 ms |  3.57 |  33 |  245.5 | 2719.6 |  125238788 B |        8.86 |
| CsvHelper    | Asset | 50000   |   106.50 ms |  2.79 |  33 |  313.4 | 2130.1 |   14305310 B |        1.01 |
|              |       |         |             |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   919.60 ms |  1.00 | 667 |  726.1 |  919.6 |  273063640 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   392.61 ms |  0.43 | 667 | 1700.7 |  392.6 |  275587208 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,303.94 ms |  1.42 | 667 |  512.1 | 1303.9 |  273225360 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,503.86 ms |  3.81 | 667 |  190.6 | 3503.9 | 2500932192 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,309.99 ms |  2.51 | 667 |  289.1 | 2310.0 |  273236680 B |        1.00 |
