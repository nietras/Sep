```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-NGWSCM : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

Job=Job-NGWSCM  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     6.822 ms |  1.00 |  33 | 4892.9 |  136.4 |      1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     6.653 ms |  0.98 |  33 | 5016.7 |  133.1 |      1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    18.141 ms |  2.66 |  33 | 1839.9 |  362.8 |      7.68 KB |        7.47 |
| ReadLine_    | Row   | 50000   |    14.819 ms |  2.17 |  33 | 2252.3 |  296.4 | 108778.74 KB |  105,782.94 |
| CsvHelper    | Row   | 50000   |    52.704 ms |  7.73 |  33 |  633.3 | 1054.1 |        20 KB |       19.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.091 ms |  1.00 |  33 | 4125.1 |  161.8 |      1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.823 ms |  1.09 |  33 | 3783.2 |  176.5 |      1.04 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    20.992 ms |  2.59 |  33 | 1590.0 |  419.8 |      7.68 KB |        7.44 |
| ReadLine_    | Cols  | 50000   |    15.053 ms |  1.86 |  33 | 2217.3 |  301.1 | 108778.74 KB |  105,283.02 |
| CsvHelper    | Cols  | 50000   |    83.601 ms | 10.33 |  33 |  399.2 | 1672.0 |    445.76 KB |      431.44 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    41.186 ms |  1.00 |  33 |  810.4 |  823.7 |  13803.74 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    25.087 ms |  0.61 |  33 | 1330.4 |  501.7 |  13990.89 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    50.979 ms |  1.24 |  33 |  654.7 | 1019.6 |  13962.02 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   119.898 ms |  2.91 |  33 |  278.4 | 2398.0 | 122304.13 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    97.741 ms |  2.37 |  33 |  341.5 | 1954.8 |  13970.31 KB |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   720.696 ms |  1.00 | 667 |  926.5 |  720.7 | 266667.68 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   335.456 ms |  0.47 | 667 | 1990.4 |  335.5 | 268181.72 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,081.252 ms |  1.50 | 667 |  617.5 | 1081.3 |    266829 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,252.495 ms |  3.13 | 667 |  296.4 | 2252.5 | 2442315.7 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,006.176 ms |  2.78 | 667 |  332.8 | 2006.2 | 266838.77 KB |        1.00 |
