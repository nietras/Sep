```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 3.08GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.38 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.617 ms |  1.00 |  29 | 8041.5 |   72.3 |      1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.641 ms |  1.01 |  29 | 7988.4 |   72.8 |      1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.676 ms |  1.02 |  29 | 7912.1 |   73.5 |      1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.410 ms |  1.22 |  29 | 6595.7 |   88.2 |      8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    21.442 ms |  5.93 |  29 | 1356.5 |  428.8 |  88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    65.534 ms | 18.12 |  29 |  443.8 | 1310.7 |     19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.877 ms |  1.00 |  29 | 5964.1 |   97.5 |      1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.770 ms |  1.18 |  29 | 5040.8 |  115.4 |      1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     7.851 ms |  1.61 |  29 | 3704.8 |  157.0 |      8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    22.279 ms |  4.57 |  29 | 1305.5 |  445.6 |  88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |   101.868 ms | 20.89 |  29 |  285.5 | 2037.4 |     445.6 KB |      438.75 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    43.596 ms |  1.00 |  29 |  667.2 |  871.9 |  13802.26 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    27.804 ms |  0.64 |  29 | 1046.1 |  556.1 |   13876.7 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    51.345 ms |  1.18 |  29 |  566.5 | 1026.9 |     13962 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   134.200 ms |  3.08 |  29 |  216.7 | 2684.0 | 102134.62 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   117.689 ms |  2.70 |  29 |  247.1 | 2353.8 |  13969.95 KB |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   859.772 ms |  1.00 | 581 |  676.8 |  859.8 | 266668.13 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   535.349 ms |  0.62 | 581 | 1086.9 |  535.3 | 277348.68 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,012.417 ms |  1.18 | 581 |  574.7 | 1012.4 | 266825.01 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,719.289 ms |  3.16 | 581 |  214.0 | 2719.3 | 2038836.1 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,495.583 ms |  2.90 | 581 |  233.2 | 2495.6 | 266840.95 KB |        1.00 |
