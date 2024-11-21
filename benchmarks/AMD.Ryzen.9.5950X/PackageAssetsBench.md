```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-NGWSCM : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

Job=Job-NGWSCM  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     2.229 ms |  1.00 |  29 | 13093.3 |   44.6 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.226 ms |  1.00 |  29 | 13110.2 |   44.5 |       1.01 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     2.852 ms |  1.28 |  29 | 10232.2 |   57.0 |       7.65 KB |        7.54 |
| ReadLine_    | Row   | 50000   |    12.592 ms |  5.65 |  29 |  2317.4 |  251.8 |   88608.24 KB |   87,329.01 |
| CsvHelper    | Row   | 50000   |    44.620 ms | 20.02 |  29 |   654.0 |  892.4 |      19.99 KB |       19.71 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     3.560 ms |  1.00 |  29 |  8197.3 |   71.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     3.875 ms |  1.09 |  29 |  7529.9 |   77.5 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     5.383 ms |  1.51 |  29 |  5421.1 |  107.7 |       7.66 KB |        7.52 |
| ReadLine_    | Cols  | 50000   |    12.631 ms |  3.55 |  29 |  2310.3 |  252.6 |   88608.24 KB |   86,994.09 |
| CsvHelper    | Cols  | 50000   |    70.466 ms | 19.79 |  29 |   414.1 | 1409.3 |     445.76 KB |      437.64 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    35.106 ms |  1.00 |  29 |   831.2 |  702.1 |   13802.42 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.532 ms |  0.58 |  29 |  1421.3 |  410.6 |   13998.13 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    38.506 ms |  1.10 |  29 |   757.8 |  770.1 |    13962.7 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    92.881 ms |  2.65 |  29 |   314.2 | 1857.6 |  102133.31 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    83.947 ms |  2.39 |  29 |   347.6 | 1678.9 |   13973.52 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   629.698 ms |  1.00 | 583 |   927.1 |  629.7 |  266667.52 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   251.239 ms |  0.40 | 583 |  2323.6 |  251.2 |  269814.91 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   777.948 ms |  1.24 | 583 |   750.4 |  777.9 |  266824.96 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,624.295 ms |  2.58 | 583 |   359.4 | 1624.3 | 2038833.23 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,726.806 ms |  2.74 | 583 |   338.1 | 1726.8 |  266831.77 KB |        1.00 |
