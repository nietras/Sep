```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.202
  [Host]     : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  Job-OCZSUI : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2

Job=Job-OCZSUI  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     2.326 ms |  1.00 |  29 | 12544.5 |   46.5 |       1.01 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.387 ms |  1.03 |  29 | 12226.3 |   47.7 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     3.014 ms |  1.30 |  29 |  9682.9 |   60.3 |       7.21 KB |        7.10 |
| ReadLine_    | Row   | 50000   |    12.914 ms |  5.57 |  29 |  2259.6 |  258.3 |   88608.24 KB |   87,329.01 |
| CsvHelper    | Row   | 50000   |    46.935 ms | 20.17 |  29 |   621.7 |  938.7 |         20 KB |       19.71 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     3.151 ms |  1.00 |  29 |  9262.0 |   63.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     3.773 ms |  1.20 |  29 |  7734.6 |   75.5 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     5.166 ms |  1.64 |  29 |  5648.5 |  103.3 |       7.21 KB |        7.09 |
| ReadLine_    | Cols  | 50000   |    13.021 ms |  4.12 |  29 |  2241.0 |  260.4 |   88608.24 KB |   87,077.58 |
| CsvHelper    | Cols  | 50000   |    72.451 ms | 22.99 |  29 |   402.8 | 1449.0 |     445.76 KB |      438.06 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    37.506 ms |  1.00 |  29 |   778.0 |  750.1 |    13803.3 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.617 ms |  0.60 |  29 |  1290.2 |  452.3 |   13992.22 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    39.622 ms |  1.06 |  29 |   736.5 |  792.4 |   13962.44 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   105.490 ms |  2.98 |  29 |   276.6 | 2109.8 |  102133.28 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    87.642 ms |  2.35 |  29 |   333.0 | 1752.8 |   13971.76 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   622.282 ms |  1.00 | 583 |   938.1 |  622.3 |  266667.45 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   249.164 ms |  0.40 | 583 |  2343.0 |  249.2 |  268111.22 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   771.816 ms |  1.23 | 583 |   756.4 |  771.8 |  266826.86 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,554.977 ms |  2.43 | 583 |   375.4 | 1555.0 | 2038833.85 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,701.078 ms |  2.67 | 583 |   343.2 | 1701.1 |   266838.3 KB |        1.00 |
