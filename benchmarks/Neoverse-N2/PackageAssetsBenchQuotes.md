```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.36 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     9.424 ms |  1.00 |  33 | 3531.8 |  188.5 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     9.887 ms |  1.05 |  33 | 3366.1 |  197.7 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.438 ms |  1.00 |  33 | 3526.3 |  188.8 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.513 ms |  2.28 |  33 | 1547.0 |  430.3 |       7477 B |        7.79 |
| ReadLine_    | Row   | 50000   |    27.079 ms |  2.87 |  33 | 1229.1 |  541.6 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    63.050 ms |  6.69 |  33 |  527.9 | 1261.0 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.707 ms |  1.00 |  33 | 2842.9 |  234.1 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.657 ms |  1.08 |  33 | 2629.6 |  253.1 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.723 ms |  2.20 |  33 | 1293.9 |  514.5 |       7479 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    27.790 ms |  2.37 |  33 | 1197.6 |  555.8 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    95.050 ms |  8.12 |  33 |  350.2 | 1901.0 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    40.207 ms |  1.00 |  33 |  827.8 |  804.1 |   14132928 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.933 ms |  0.55 |  33 | 1517.4 |  438.7 |   14213074 B |        1.01 |
| Sylvan___    | Asset | 50000   |    58.833 ms |  1.46 |  33 |  565.7 | 1176.7 |   14295507 B |        1.01 |
| ReadLine_    | Asset | 50000   |   131.310 ms |  3.27 |  33 |  253.5 | 2626.2 |  125239040 B |        8.86 |
| CsvHelper    | Asset | 50000   |   106.367 ms |  2.65 |  33 |  312.9 | 2127.3 |   14305232 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   880.177 ms |  1.00 | 665 |  756.4 |  880.2 |  273063736 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   393.705 ms |  0.45 | 665 | 1691.1 |  393.7 |  280592520 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,247.294 ms |  1.42 | 665 |  533.8 | 1247.3 |  273225488 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,295.165 ms |  3.74 | 665 |  202.1 | 3295.2 | 2500932824 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,245.528 ms |  2.55 | 665 |  296.5 | 2245.5 |  273235120 B |        1.00 |
