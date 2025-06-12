```

BenchmarkDotNet v0.15.1, Linux Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-BFPPER : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-BFPPER  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.586 ms |  1.00 |  29 | 8110.7 |   71.7 |       1.03 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.646 ms |  1.02 |  29 | 7978.2 |   72.9 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.564 ms |  0.99 |  29 | 8161.3 |   71.3 |       1.15 KB |        1.12 |
| Sylvan___    | Row   | 50000   |     4.421 ms |  1.23 |  29 | 6578.6 |   88.4 |       7.66 KB |        7.47 |
| ReadLine_    | Row   | 50000   |    22.687 ms |  6.33 |  29 | 1282.0 |  453.7 |    88608.3 KB |   86,414.19 |
| CsvHelper    | Row   | 50000   |    63.425 ms | 17.69 |  29 |  458.6 | 1268.5 |      20.12 KB |       19.62 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.788 ms |  1.00 |  29 | 6075.3 |   95.8 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.757 ms |  1.20 |  29 | 5052.2 |  115.1 |       1.04 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.431 ms |  1.76 |  29 | 3450.1 |  168.6 |       8.03 KB |        7.78 |
| ReadLine_    | Cols  | 50000   |    24.198 ms |  5.05 |  29 | 1202.0 |  484.0 |   88608.32 KB |   85,841.93 |
| CsvHelper    | Cols  | 50000   |   110.904 ms | 23.17 |  29 |  262.3 | 2218.1 |     445.93 KB |      432.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    40.065 ms |  1.00 |  29 |  726.0 |  801.3 |   13802.85 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.673 ms |  0.72 |  29 | 1014.4 |  573.5 |   13858.63 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    51.127 ms |  1.28 |  29 |  568.9 | 1022.5 |   13962.26 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   122.110 ms |  3.05 |  29 |  238.2 | 2442.2 |  102134.64 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   127.405 ms |  3.18 |  29 |  228.3 | 2548.1 |   13974.95 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   860.730 ms |  1.00 | 581 |  676.0 |  860.7 |  266670.27 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   500.834 ms |  0.58 | 581 | 1161.8 |  500.8 |  274965.85 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,039.885 ms |  1.21 | 581 |  559.6 | 1039.9 |   266826.6 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,666.458 ms |  3.10 | 581 |  218.2 | 2666.5 | 2038838.18 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,589.531 ms |  3.01 | 581 |  224.7 | 2589.5 |  266841.23 KB |        1.00 |
