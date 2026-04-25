```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.418 ms |  1.00 |  29 | 8509.1 |   68.4 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.701 ms |  1.08 |  29 | 7859.8 |   74.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.506 ms |  1.03 |  29 | 8295.3 |   70.1 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.392 ms |  1.28 |  29 | 6622.7 |   87.8 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    22.643 ms |  6.62 |  29 | 1284.5 |  452.9 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    64.123 ms | 18.76 |  29 |  453.6 | 1282.5 |      20.02 KB |       19.71 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.968 ms |  1.00 |  29 | 5854.8 |   99.4 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.009 ms |  1.21 |  29 | 4840.4 |  120.2 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.068 ms |  1.62 |  29 | 3605.3 |  161.4 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    22.807 ms |  4.59 |  29 | 1275.3 |  456.1 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |   107.810 ms | 21.70 |  29 |  269.8 | 2156.2 |     445.67 KB |      438.82 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    41.897 ms |  1.00 |  29 |  694.2 |  837.9 |   13802.24 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.066 ms |  0.74 |  29 |  936.3 |  621.3 |   13862.25 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    52.952 ms |  1.26 |  29 |  549.3 | 1059.0 |   13961.94 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   172.474 ms |  4.12 |  29 |  168.6 | 3449.5 |  102133.29 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   130.126 ms |  3.11 |  29 |  223.5 | 2602.5 |   13970.36 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   858.383 ms |  1.00 | 581 |  677.9 |  858.4 |  266667.14 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   529.937 ms |  0.62 | 581 | 1098.0 |  529.9 |  276053.33 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,029.931 ms |  1.20 | 581 |  565.0 | 1029.9 |  266824.18 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,966.931 ms |  3.46 | 581 |  196.1 | 2966.9 | 2038834.69 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,624.677 ms |  3.06 | 581 |  221.7 | 2624.7 |     266833 KB |        1.00 |
