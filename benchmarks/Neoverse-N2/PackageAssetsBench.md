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
| Sep______    | Row   | 50000   |     5.687 ms |  1.00 |  29 | 5114.7 |  113.7 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.784 ms |  1.02 |  29 | 5029.0 |  115.7 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.616 ms |  0.99 |  29 | 5179.0 |  112.3 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.742 ms |  3.65 |  29 | 1402.2 |  414.8 |       7516 B |        7.83 |
| ReadLine_    | Row   | 50000   |    22.939 ms |  4.04 |  29 | 1268.0 |  458.8 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    54.158 ms |  9.53 |  29 |  537.1 | 1083.2 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     7.196 ms |  1.00 |  29 | 4042.1 |  143.9 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.224 ms |  1.14 |  29 | 3536.5 |  164.5 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.523 ms |  3.55 |  29 | 1139.6 |  510.5 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    23.283 ms |  3.24 |  29 | 1249.2 |  465.7 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    87.643 ms | 12.18 |  29 |  331.9 | 1752.9 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    32.082 ms |  1.00 |  29 |  906.6 |  641.6 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    15.177 ms |  0.47 |  29 | 1916.5 |  303.5 |   14184269 B |        1.00 |
| Sylvan___    | Asset | 50000   |    57.174 ms |  1.78 |  29 |  508.7 | 1143.5 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   126.667 ms |  3.95 |  29 |  229.6 | 2533.3 |  104588292 B |        7.40 |
| CsvHelper    | Asset | 50000   |    99.831 ms |  3.11 |  29 |  291.4 | 1996.6 |   14308394 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   794.343 ms |  1.00 | 581 |  732.5 |  794.3 |  273063616 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   377.160 ms |  0.47 | 581 | 1542.8 |  377.2 |  281918352 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,236.394 ms |  1.56 | 581 |  470.6 | 1236.4 |  273225288 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,889.816 ms |  3.64 | 581 |  201.4 | 2889.8 | 2087772976 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,139.100 ms |  2.69 | 581 |  272.0 | 2139.1 |  273237080 B |        1.00 |
