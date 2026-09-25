```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.44 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.683 ms |  1.00 |  29 | 6210.5 |   93.7 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.885 ms |  1.04 |  29 | 5953.6 |   97.7 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.651 ms |  0.99 |  29 | 6254.3 |   93.0 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.601 ms |  4.19 |  29 | 1483.9 |  392.0 |       7470 B |        7.78 |
| ReadLine_    | Row   | 50000   |    21.947 ms |  4.69 |  29 | 1325.3 |  438.9 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    56.225 ms | 12.01 |  29 |  517.3 | 1124.5 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.119 ms |  1.00 |  29 | 4753.2 |  122.4 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.859 ms |  1.12 |  29 | 4240.8 |  137.2 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    22.465 ms |  3.67 |  29 | 1294.7 |  449.3 |       7477 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    22.459 ms |  3.67 |  29 | 1295.0 |  449.2 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    86.763 ms | 14.18 |  29 |  335.2 | 1735.3 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.128 ms |  1.00 |  29 |  828.0 |  702.6 |   14132928 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    18.971 ms |  0.54 |  29 | 1533.2 |  379.4 |   14205842 B |        1.01 |
| Sylvan___    | Asset | 50000   |    57.145 ms |  1.63 |  29 |  509.0 | 1142.9 |   14295531 B |        1.01 |
| ReadLine_    | Asset | 50000   |   128.714 ms |  3.66 |  29 |  226.0 | 2574.3 |  104584256 B |        7.40 |
| CsvHelper    | Asset | 50000   |   102.671 ms |  2.92 |  29 |  283.3 | 2053.4 |   14305232 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   784.447 ms |  1.00 | 581 |  741.8 |  784.4 |  273063928 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   394.643 ms |  0.50 | 581 | 1474.4 |  394.6 |  283205560 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,202.935 ms |  1.53 | 581 |  483.7 | 1202.9 |  273225400 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,886.913 ms |  3.68 | 581 |  201.6 | 2886.9 | 2087766056 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,158.069 ms |  2.75 | 581 |  269.6 | 2158.1 |  273235184 B |        1.00 |
