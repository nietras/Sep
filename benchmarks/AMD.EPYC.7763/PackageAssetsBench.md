```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.71GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______    | Row   | 50000   |     3.703 ms |  1.00 |  29 | 7855.6 |   74.1 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.897 ms |  1.05 |  29 | 7463.4 |   77.9 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.694 ms |  1.00 |  29 | 7874.8 |   73.9 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     5.143 ms |  1.39 |  29 | 5656.0 |  102.9 |       8.46 KB |        8.26 |
| ReadLine_    | Row   | 50000   |    23.498 ms |  6.35 |  29 | 1237.8 |  470.0 |   88608.23 KB |   86,579.03 |
| CsvHelper    | Row   | 50000   |    63.669 ms | 17.20 |  29 |  456.8 | 1273.4 |      20.02 KB |       19.56 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.007 ms |  1.00 |  29 | 5809.6 |  100.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.922 ms |  1.18 |  29 | 4911.8 |  118.4 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.359 ms |  1.67 |  29 | 3479.7 |  167.2 |       8.46 KB |        8.26 |
| ReadLine_    | Cols  | 50000   |    24.083 ms |  4.81 |  29 | 1207.8 |  481.7 |   88608.23 KB |   86,496.50 |
| CsvHelper    | Cols  | 50000   |   102.354 ms | 20.44 |  29 |  284.2 | 2047.1 |     445.68 KB |      435.06 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    40.449 ms |  1.00 |  29 |  719.1 |  809.0 |   13802.25 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.268 ms |  0.75 |  29 |  960.9 |  605.4 |   13879.29 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    48.315 ms |  1.20 |  29 |  602.0 |  966.3 |    13961.9 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   151.282 ms |  3.74 |  29 |  192.3 | 3025.6 |  102133.76 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   124.271 ms |  3.07 |  29 |  234.1 | 2485.4 |   13971.31 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   860.244 ms |  1.00 | 581 |  676.4 |  860.2 |  266667.15 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   530.927 ms |  0.62 | 581 | 1096.0 |  530.9 |  277121.87 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,007.552 ms |  1.17 | 581 |  577.5 | 1007.6 |  266824.02 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,067.927 ms |  3.57 | 581 |  189.7 | 3067.9 | 2038834.41 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,539.028 ms |  2.95 | 581 |  229.2 | 2539.0 |  266842.54 KB |        1.00 |
