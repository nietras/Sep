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
| Sep______    | Row   | 50000   |     3.840 ms |  1.00 |  29 | 7600.0 |   76.8 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.292 ms |  1.12 |  29 | 6798.4 |   85.8 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.894 ms |  1.01 |  29 | 7494.5 |   77.9 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.865 ms |  1.27 |  29 | 5998.7 |   97.3 |       8.46 KB |        8.32 |
| ReadLine_    | Row   | 50000   |    20.106 ms |  5.24 |  29 | 1451.4 |  402.1 |   88608.23 KB |   87,161.21 |
| CsvHelper    | Row   | 50000   |    65.612 ms | 17.09 |  29 |  444.8 | 1312.2 |      19.95 KB |       19.62 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.163 ms |  1.00 |  29 | 5652.5 |  103.3 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.143 ms |  1.19 |  29 | 4750.3 |  122.9 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     9.378 ms |  1.82 |  29 | 3111.8 |  187.6 |       8.46 KB |        8.32 |
| ReadLine_    | Cols  | 50000   |    22.825 ms |  4.42 |  29 | 1278.5 |  456.5 |   88608.23 KB |   87,161.21 |
| CsvHelper    | Cols  | 50000   |   104.528 ms | 20.25 |  29 |  279.2 | 2090.6 |     445.67 KB |      438.39 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    41.384 ms |  1.00 |  29 |  705.1 |  827.7 |   13802.24 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.326 ms |  0.76 |  29 |  931.5 |  626.5 |   13855.17 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    46.888 ms |  1.13 |  29 |  622.4 |  937.8 |   13961.91 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   154.748 ms |  3.74 |  29 |  188.6 | 3095.0 |  102134.07 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   128.513 ms |  3.11 |  29 |  227.1 | 2570.3 |   13970.18 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   944.784 ms |  1.00 | 583 |  617.9 |  944.8 |  266667.11 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   548.976 ms |  0.58 | 583 | 1063.4 |  549.0 |  277782.22 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,113.786 ms |  1.18 | 583 |  524.1 | 1113.8 |  266824.28 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,448.241 ms |  3.65 | 583 |  169.3 | 3448.2 | 2038834.05 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,628.657 ms |  2.78 | 583 |  222.1 | 2628.7 |  266834.04 KB |        1.00 |
