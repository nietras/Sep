```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
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
| Sep______    | Row   | 50000   |     3.304 ms |  1.00 |  29 | 8803.3 |   66.1 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.553 ms |  1.08 |  29 | 8186.6 |   71.1 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.784 ms |  1.45 |  29 | 6080.5 |   95.7 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.542 ms |  5.92 |  29 | 1488.4 |  390.8 |       7516 B |        7.83 |
| ReadLine_    | Row   | 50000   |    21.335 ms |  6.46 |  29 | 1363.3 |  426.7 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    47.321 ms | 14.33 |  29 |  614.7 |  946.4 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.276 ms |  1.00 |  29 | 6802.0 |   85.5 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.646 ms |  1.09 |  29 | 6260.4 |   92.9 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.785 ms |  5.10 |  29 | 1335.1 |  435.7 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    17.661 ms |  4.13 |  29 | 1646.9 |  353.2 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    68.030 ms | 15.92 |  29 |  427.5 | 1360.6 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.033 ms |  1.01 |  29 |  830.2 |  700.7 |   14133404 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    26.981 ms |  0.78 |  29 | 1078.0 |  539.6 |   14225666 B |        1.01 |
| Sylvan___    | Asset | 50000   |    67.002 ms |  1.93 |  29 |  434.1 | 1340.0 |   14295908 B |        1.01 |
| ReadLine_    | Asset | 50000   |   118.546 ms |  3.41 |  29 |  245.4 | 2370.9 |  104585348 B |        7.40 |
| CsvHelper    | Asset | 50000   |    90.727 ms |  2.61 |  29 |  320.6 | 1814.5 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   736.818 ms |  1.01 | 581 |  789.7 |  736.8 |  273067184 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   487.099 ms |  0.67 | 581 | 1194.6 |  487.1 |  283038968 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,080.644 ms |  1.48 | 581 |  538.5 | 1080.6 |  273226832 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,808.111 ms |  2.47 | 581 |  321.8 | 1808.1 | 2087767136 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,617.042 ms |  2.21 | 581 |  359.8 | 1617.0 |  273236504 B |        1.00 |
