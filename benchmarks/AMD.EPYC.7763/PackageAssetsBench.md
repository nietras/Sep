```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.61 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.669 ms |  1.00 |  29 | 7953.2 |   73.4 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.936 ms |  1.07 |  29 | 7414.8 |   78.7 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.828 ms |  1.04 |  29 | 7623.8 |   76.6 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.856 ms |  1.32 |  29 | 6008.8 |   97.1 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    19.459 ms |  5.30 |  29 | 1499.6 |  389.2 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    65.411 ms | 17.83 |  29 |  446.1 | 1308.2 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.089 ms |  1.00 |  29 | 5734.2 |  101.8 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.068 ms |  1.19 |  29 | 4809.2 |  121.4 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.564 ms |  1.68 |  29 | 3407.4 |  171.3 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    19.688 ms |  3.87 |  29 | 1482.2 |  393.8 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |   105.588 ms | 20.75 |  29 |  276.4 | 2111.8 |      445.6 KB |      438.75 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    46.318 ms |  1.00 |  29 |  630.0 |  926.4 |   13802.56 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    29.129 ms |  0.63 |  29 | 1001.8 |  582.6 |   13889.79 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    50.915 ms |  1.10 |  29 |  573.1 | 1018.3 |      13962 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   138.688 ms |  3.00 |  29 |  210.4 | 2773.8 |  102134.04 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   124.920 ms |  2.71 |  29 |  233.6 | 2498.4 |   13969.95 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   936.403 ms |  1.00 | 583 |  623.4 |  936.4 |  266667.99 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   556.936 ms |  0.60 | 583 | 1048.2 |  556.9 |  277394.99 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,063.710 ms |  1.14 | 583 |  548.8 | 1063.7 |  266824.95 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,887.414 ms |  3.09 | 583 |  202.2 | 2887.4 | 2038835.94 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,620.897 ms |  2.80 | 583 |  222.7 | 2620.9 |  266836.18 KB |        1.00 |
