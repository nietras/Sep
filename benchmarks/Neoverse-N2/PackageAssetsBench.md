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
| Sep______    | Row   | 50000   |     4.681 ms |  1.00 |  29 | 6213.7 |   93.6 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.867 ms |  1.04 |  29 | 5976.5 |   97.3 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.730 ms |  1.01 |  29 | 6149.4 |   94.6 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.039 ms |  4.28 |  29 | 1451.4 |  400.8 |       7476 B |        7.79 |
| ReadLine_    | Row   | 50000   |    22.577 ms |  4.82 |  29 | 1288.3 |  451.5 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    54.168 ms | 11.57 |  29 |  537.0 | 1083.4 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     6.228 ms |  1.00 |  29 | 4669.8 |  124.6 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.432 ms |  1.19 |  29 | 3913.6 |  148.6 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.261 ms |  4.06 |  29 | 1151.4 |  505.2 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    22.844 ms |  3.67 |  29 | 1273.2 |  456.9 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    87.385 ms | 14.03 |  29 |  332.8 | 1747.7 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    31.687 ms |  1.00 |  29 |  917.9 |  633.7 |   14133000 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    14.791 ms |  0.47 |  29 | 1966.4 |  295.8 |   14185100 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.908 ms |  1.80 |  29 |  511.1 | 1138.2 |   14295523 B |        1.01 |
| ReadLine_    | Asset | 50000   |   104.634 ms |  3.30 |  29 |  278.0 | 2092.7 |  104584100 B |        7.40 |
| CsvHelper    | Asset | 50000   |    99.370 ms |  3.14 |  29 |  292.7 | 1987.4 |   14305310 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   781.331 ms |  1.00 | 581 |  744.7 |  781.3 |  273063624 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   343.212 ms |  0.44 | 581 | 1695.4 |  343.2 |  283104160 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,239.876 ms |  1.59 | 581 |  469.3 | 1239.9 |  273225272 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,552.345 ms |  3.27 | 581 |  228.0 | 2552.3 | 2087764840 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,225.529 ms |  2.85 | 581 |  261.5 | 2225.5 |  273234840 B |        1.00 |
