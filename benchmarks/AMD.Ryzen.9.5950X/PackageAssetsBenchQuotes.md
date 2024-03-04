```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  Job-XZYDSN : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2

Job=Job-XZYDSN  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     6.801 ms |  1.00 |  33 | 4907.6 |  136.0 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     6.539 ms |  0.96 |  33 | 5104.6 |  130.8 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    18.719 ms |  2.76 |  33 | 1783.1 |  374.4 |       7.23 KB |        7.03 |
| ReadLine_    | Row   | 50000   |    14.711 ms |  2.16 |  33 | 2268.9 |  294.2 |  108778.74 KB |  105,682.57 |
| CsvHelper    | Row   | 50000   |    53.126 ms |  7.82 |  33 |  628.3 | 1062.5 |         20 KB |       19.43 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     7.529 ms |  1.00 |  33 | 4433.4 |  150.6 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.619 ms |  1.14 |  33 | 3872.7 |  172.4 |       1.03 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.147 ms |  2.81 |  33 | 1578.4 |  422.9 |       7.24 KB |        7.02 |
| ReadLine_    | Cols  | 50000   |    15.110 ms |  2.00 |  33 | 2209.0 |  302.2 |  108778.74 KB |  105,482.42 |
| CsvHelper    | Cols  | 50000   |    83.857 ms | 11.14 |  33 |  398.0 | 1677.1 |     445.76 KB |      432.25 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.139 ms |  1.00 |  33 |  852.8 |  782.8 |   13802.84 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    25.626 ms |  0.66 |  33 | 1302.5 |  512.5 |   13985.74 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    50.782 ms |  1.28 |  33 |  657.3 | 1015.6 |   13961.97 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   118.157 ms |  2.98 |  33 |  282.5 | 2363.1 |  122304.33 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    97.615 ms |  2.45 |  33 |  341.9 | 1952.3 |   13970.69 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   757.197 ms |  1.00 | 667 |  881.8 |  757.2 |  266667.66 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   356.181 ms |  0.47 | 667 | 1874.6 |  356.2 |  267910.07 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,088.059 ms |  1.45 | 667 |  613.7 | 1088.1 |  266827.67 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,224.329 ms |  2.96 | 667 |  300.2 | 2224.3 | 2442316.13 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,011.872 ms |  2.68 | 667 |  331.9 | 2011.9 |  266835.48 KB |        1.00 |
