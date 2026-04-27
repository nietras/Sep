```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______    | Row   | 50000   |     3.816 ms |  1.00 |  29 | 7646.5 |   76.3 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.155 ms |  1.09 |  29 | 7024.0 |   83.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.828 ms |  1.00 |  29 | 7622.3 |   76.6 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.796 ms |  1.26 |  29 | 6083.9 |   95.9 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    20.861 ms |  5.47 |  29 | 1398.9 |  417.2 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    68.778 ms | 18.03 |  29 |  424.3 | 1375.6 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.230 ms |  1.00 |  29 | 5580.1 |  104.6 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.239 ms |  1.19 |  29 | 4677.2 |  124.8 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.571 ms |  1.64 |  29 | 3404.7 |  171.4 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    20.786 ms |  3.98 |  29 | 1403.9 |  415.7 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |   106.194 ms | 20.31 |  29 |  274.8 | 2123.9 |      445.6 KB |      438.75 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    42.105 ms |  1.00 |  29 |  693.1 |  842.1 |   13803.67 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.978 ms |  0.78 |  29 |  884.9 |  659.6 |   13854.65 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    50.942 ms |  1.21 |  29 |  572.8 | 1018.8 |   13961.91 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   161.331 ms |  3.83 |  29 |  180.9 | 3226.6 |  102133.68 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   131.742 ms |  3.13 |  29 |  221.5 | 2634.8 |   13971.01 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   956.809 ms |  1.00 | 583 |  610.1 |  956.8 |  266667.21 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   578.522 ms |  0.61 | 583 | 1009.1 |  578.5 |  276322.59 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,208.966 ms |  1.26 | 583 |  482.9 | 1209.0 |  266824.63 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,165.517 ms |  3.31 | 583 |  184.4 | 3165.5 | 2038834.43 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,611.847 ms |  2.73 | 583 |  223.5 | 2611.8 |  266843.61 KB |        1.00 |
