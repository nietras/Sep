```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  Job-XZYDSN : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2

Job=Job-XZYDSN  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     2.278 ms |  1.00 |  29 | 12811.2 |   45.6 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.222 ms |  0.98 |  29 | 13132.2 |   44.4 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     2.823 ms |  1.24 |  29 | 10338.6 |   56.5 |       7.21 KB |        7.10 |
| ReadLine_    | Row   | 50000   |    12.414 ms |  5.43 |  29 |  2350.6 |  248.3 |   88608.24 KB |   87,329.01 |
| CsvHelper    | Row   | 50000   |    46.178 ms | 20.27 |  29 |   631.9 |  923.6 |      19.99 KB |       19.71 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     3.099 ms |  1.00 |  29 |  9415.3 |   62.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     3.664 ms |  1.18 |  29 |  7963.8 |   73.3 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     5.039 ms |  1.63 |  29 |  5791.0 |  100.8 |       7.21 KB |        7.09 |
| ReadLine_    | Cols  | 50000   |    12.515 ms |  4.01 |  29 |  2331.6 |  250.3 |   88608.24 KB |   87,161.23 |
| CsvHelper    | Cols  | 50000   |    69.842 ms | 22.56 |  29 |   417.8 | 1396.8 |     445.76 KB |      438.48 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    35.134 ms |  1.00 |  29 |   830.6 |  702.7 |   13802.48 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.891 ms |  0.62 |  29 |  1333.0 |  437.8 |   13996.06 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    37.722 ms |  1.07 |  29 |   773.6 |  754.4 |   13962.33 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    95.572 ms |  2.69 |  29 |   305.3 | 1911.4 |  102133.25 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    84.280 ms |  2.40 |  29 |   346.2 | 1685.6 |   13970.38 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   627.708 ms |  1.00 | 583 |   930.0 |  627.7 |  266667.65 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   255.784 ms |  0.41 | 583 |  2282.3 |  255.8 |  267978.42 KB |        1.00 |
| Sylvan___    | Asset | 1000000 |   782.747 ms |  1.25 | 583 |   745.8 |  782.7 |  266825.97 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,619.584 ms |  2.58 | 583 |   360.5 | 1619.6 | 2038833.09 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,741.880 ms |  2.77 | 583 |   335.1 | 1741.9 |  266844.68 KB |        1.00 |
