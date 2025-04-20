```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.2 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  Job-DRDGJI : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2

Job=Job-DRDGJI  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     3.465 ms |  1.00 |  29 | 8393.2 |   69.3 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     3.631 ms |  1.05 |  29 | 8011.5 |   72.6 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.551 ms |  1.02 |  29 | 8191.8 |   71.0 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     4.356 ms |  1.26 |  29 | 6676.9 |   87.1 |       7.66 KB |        7.48 |
| ReadLine_    | Row   | 50000   |    21.994 ms |  6.35 |  29 | 1322.4 |  439.9 |    88608.3 KB |   86,496.56 |
| CsvHelper    | Row   | 50000   |    63.463 ms | 18.31 |  29 |  458.3 | 1269.3 |      20.12 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     4.859 ms |  1.00 |  29 | 5985.5 |   97.2 |       1.04 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.693 ms |  1.17 |  29 | 5109.5 |  113.9 |       1.23 KB |        1.19 |
| Sylvan___    | Cols  | 50000   |     8.515 ms |  1.75 |  29 | 3415.7 |  170.3 |       7.68 KB |        7.41 |
| ReadLine_    | Cols  | 50000   |    26.891 ms |  5.53 |  29 | 1081.6 |  537.8 |   88608.31 KB |   85,437.77 |
| CsvHelper    | Cols  | 50000   |   107.289 ms | 22.08 |  29 |  271.1 | 2145.8 |     448.95 KB |      432.89 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    40.048 ms |  1.00 |  29 |  726.3 |  801.0 |   13802.76 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.007 ms |  0.70 |  29 | 1038.5 |  560.1 |   13868.88 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    50.537 ms |  1.26 |  29 |  575.5 | 1010.7 |   13962.28 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   124.559 ms |  3.11 |  29 |  233.5 | 2491.2 |  102134.83 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   126.945 ms |  3.17 |  29 |  229.1 | 2538.9 |      13971 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   844.001 ms |  1.00 | 581 |  689.4 |  844.0 |  266670.18 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   506.846 ms |  0.60 | 581 | 1148.0 |  506.8 |  276119.66 KB |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,027.096 ms |  1.22 | 581 |  566.5 | 1027.1 |  266831.32 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,565.816 ms |  3.04 | 581 |  226.8 | 2565.8 | 2038838.04 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,628.958 ms |  3.12 | 581 |  221.3 | 2629.0 |  266847.05 KB |        1.00 |
