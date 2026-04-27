```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.56GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     4.812 ms |  1.00 |  29 | 6044.6 |   96.2 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.970 ms |  1.03 |  29 | 5852.1 |   99.4 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.083 ms |  1.06 |  29 | 5721.6 |  101.7 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     5.035 ms |  1.05 |  29 | 5776.3 |  100.7 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    25.317 ms |  5.26 |  29 | 1148.9 |  506.3 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    69.099 ms | 14.36 |  29 |  420.9 | 1382.0 |      20.02 KB |       19.71 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     6.477 ms |  1.00 |  29 | 4490.6 |  129.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.366 ms |  1.14 |  29 | 3948.8 |  147.3 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.930 ms |  1.38 |  29 | 3256.9 |  178.6 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |    24.168 ms |  3.73 |  29 | 1203.5 |  483.4 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |   107.413 ms | 16.59 |  29 |  270.8 | 2148.3 |     445.68 KB |      438.82 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    40.063 ms |  1.00 |  29 |  726.0 |  801.3 |   13801.77 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    26.933 ms |  0.67 |  29 | 1079.9 |  538.7 |   13859.91 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    48.392 ms |  1.21 |  29 |  601.0 |  967.8 |    13961.7 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   138.800 ms |  3.47 |  29 |  209.6 | 2776.0 |   102133.7 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   124.608 ms |  3.11 |  29 |  233.4 | 2492.2 |   13970.03 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   896.558 ms |  1.00 | 581 |  649.0 |  896.6 |  266665.55 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   566.004 ms |  0.63 | 581 | 1028.0 |  566.0 |  275264.31 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,066.547 ms |  1.19 | 581 |  545.6 | 1066.5 |  266824.34 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,097.547 ms |  3.46 | 581 |  187.9 | 3097.5 | 2038833.29 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,620.696 ms |  2.92 | 581 |  222.0 | 2620.7 |  266838.74 KB |        1.00 |
