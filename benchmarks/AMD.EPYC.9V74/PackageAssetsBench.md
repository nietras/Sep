```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______    | Row   | 50000   |     3.272 ms |  1.00 |  29 | 8888.2 |   65.4 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.502 ms |  1.07 |  29 | 8305.7 |   70.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.235 ms |  0.99 |  29 | 8991.8 |   64.7 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.147 ms |  1.27 |  29 | 7013.3 |   82.9 |       8.46 KB |        8.26 |
| ReadLine_    | Row   | 50000   |    21.520 ms |  6.58 |  29 | 1351.5 |  430.4 |   88608.23 KB |   86,579.03 |
| CsvHelper    | Row   | 50000   |    63.680 ms | 19.48 |  29 |  456.7 | 1273.6 |      23.03 KB |       22.51 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.666 ms |  1.00 |  29 | 6233.3 |   93.3 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.723 ms |  1.23 |  29 | 5082.5 |  114.5 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     7.869 ms |  1.69 |  29 | 3696.3 |  157.4 |       8.46 KB |        8.26 |
| ReadLine_    | Cols  | 50000   |    23.264 ms |  4.99 |  29 | 1250.2 |  465.3 |   88608.23 KB |   86,579.03 |
| CsvHelper    | Cols  | 50000   |   107.592 ms | 23.06 |  29 |  270.3 | 2151.8 |     445.67 KB |      435.47 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    40.967 ms |  1.00 |  29 |  710.0 |  819.3 |   13802.24 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.882 ms |  0.75 |  29 |  941.8 |  617.6 |   13858.79 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    49.626 ms |  1.21 |  29 |  586.1 |  992.5 |    13961.9 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   132.531 ms |  3.24 |  29 |  219.5 | 2650.6 |  102133.29 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   129.305 ms |  3.16 |  29 |  224.9 | 2586.1 |   13970.67 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   847.704 ms |  1.00 | 581 |  686.4 |  847.7 |  266667.34 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   532.704 ms |  0.63 | 581 | 1092.3 |  532.7 |  275913.77 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,037.713 ms |  1.22 | 581 |  560.7 | 1037.7 |  266824.09 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,894.495 ms |  3.41 | 581 |  201.0 | 2894.5 | 2038835.09 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,616.898 ms |  3.09 | 581 |  222.4 | 2616.9 |  266836.73 KB |        1.00 |
