```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.13GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______    | Row   | 50000   |     3.719 ms |  1.00 |  29 | 7821.1 |   74.4 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.892 ms |  1.05 |  29 | 7473.2 |   77.8 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.736 ms |  1.00 |  29 | 7784.6 |   74.7 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.506 ms |  1.21 |  29 | 6455.0 |   90.1 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |    23.211 ms |  6.24 |  29 | 1253.1 |  464.2 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    63.954 ms | 17.20 |  29 |  454.8 | 1279.1 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.025 ms |  1.00 |  29 | 5788.3 |  100.5 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.123 ms |  1.22 |  29 | 4750.6 |  122.5 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.353 ms |  1.66 |  29 | 3482.3 |  167.1 |       8.46 KB |        8.32 |
| ReadLine_    | Cols  | 50000   |    23.620 ms |  4.70 |  29 | 1231.4 |  472.4 |   88608.23 KB |   87,161.21 |
| CsvHelper    | Cols  | 50000   |   102.724 ms | 20.44 |  29 |  283.1 | 2054.5 |     445.67 KB |      438.39 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.861 ms |  1.00 |  29 |  729.7 |  797.2 |    13802.2 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    27.808 ms |  0.70 |  29 | 1045.9 |  556.2 |   13856.53 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    49.111 ms |  1.23 |  29 |  592.3 |  982.2 |   13961.98 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   137.023 ms |  3.44 |  29 |  212.3 | 2740.5 |  102133.82 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   123.544 ms |  3.10 |  29 |  235.4 | 2470.9 |   13970.29 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   833.499 ms |  1.00 | 581 |  698.1 |  833.5 |  266667.34 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   512.527 ms |  0.61 | 581 | 1135.3 |  512.5 |  276532.04 KB |        1.04 |
| Sylvan___    | Asset | 1000000 |   996.368 ms |  1.20 | 581 |  584.0 |  996.4 |   266824.3 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,766.663 ms |  3.32 | 581 |  210.3 | 2766.7 | 2038835.15 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,483.517 ms |  2.98 | 581 |  234.3 | 2483.5 |  266840.48 KB |        1.00 |
