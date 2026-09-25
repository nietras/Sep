```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.59 GB Available
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
| Sep______    | Row   | 50000   |     3.754 ms |  1.00 |  29 | 7773.0 |   75.1 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.972 ms |  1.06 |  29 | 7347.6 |   79.4 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.862 ms |  1.03 |  29 | 7555.7 |   77.2 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.895 ms |  1.30 |  29 | 5961.4 |   97.9 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    18.569 ms |  4.95 |  29 | 1571.5 |  371.4 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    64.916 ms | 17.29 |  29 |  449.5 | 1298.3 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.168 ms |  1.00 |  29 | 5646.9 |  103.4 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.038 ms |  1.17 |  29 | 4833.0 |  120.8 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.433 ms |  1.63 |  29 | 3460.2 |  168.7 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    19.308 ms |  3.74 |  29 | 1511.3 |  386.2 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |   105.042 ms | 20.33 |  29 |  277.8 | 2100.8 |      445.6 KB |      438.75 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    44.722 ms |  1.00 |  29 |  652.5 |  894.4 |    13802.9 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.288 ms |  0.68 |  29 |  963.4 |  605.8 |   13873.03 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    52.673 ms |  1.18 |  29 |  554.0 | 1053.5 |   13962.29 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   168.019 ms |  3.76 |  29 |  173.7 | 3360.4 |  102134.05 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   127.737 ms |  2.86 |  29 |  228.4 | 2554.7 |   13969.95 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   953.363 ms |  1.00 | 583 |  612.3 |  953.4 |  266668.09 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   575.590 ms |  0.60 | 583 | 1014.2 |  575.6 |  278269.43 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,132.052 ms |  1.19 | 583 |  515.7 | 1132.1 |  266825.09 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,081.790 ms |  3.23 | 583 |  189.4 | 3081.8 | 2038836.01 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,650.721 ms |  2.78 | 583 |  220.2 | 2650.7 |  266839.68 KB |        1.00 |
