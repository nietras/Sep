```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.202
  [Host]     : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  Job-OCZSUI : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2

Job=Job-OCZSUI  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     6.401 ms |  1.00 |  33 | 5214.3 |  128.0 |       1.03 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     6.785 ms |  1.06 |  33 | 4919.3 |  135.7 |       1.03 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    18.566 ms |  2.91 |  33 | 1797.8 |  371.3 |       7.23 KB |        7.04 |
| ReadLine_    | Row   | 50000   |    14.481 ms |  2.26 |  33 | 2304.8 |  289.6 |  108778.74 KB |  105,883.49 |
| CsvHelper    | Row   | 50000   |    52.862 ms |  8.26 |  33 |  631.4 | 1057.2 |         20 KB |       19.47 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     7.238 ms |  1.00 |  33 | 4611.2 |  144.8 |       1.03 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.971 ms |  1.23 |  33 | 3720.6 |  179.4 |       1.03 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.286 ms |  2.94 |  33 | 1568.0 |  425.7 |       7.24 KB |        7.02 |
| ReadLine_    | Cols  | 50000   |    14.900 ms |  2.06 |  33 | 2240.1 |  298.0 |  108778.74 KB |  105,482.42 |
| CsvHelper    | Cols  | 50000   |    83.563 ms | 11.54 |  33 |  399.4 | 1671.3 |     445.76 KB |      432.25 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    39.372 ms |  1.00 |  33 |  847.8 |  787.4 |    13802.4 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    24.035 ms |  0.61 |  33 | 1388.7 |  480.7 |   13985.88 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    50.022 ms |  1.26 |  33 |  667.2 | 1000.4 |   13962.17 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   120.676 ms |  3.07 |  33 |  276.6 | 2413.5 |  122304.18 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    96.420 ms |  2.44 |  33 |  346.2 | 1928.4 |   13971.94 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   703.318 ms |  1.00 | 667 |  949.4 |  703.3 |  266667.29 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   332.798 ms |  0.47 | 667 | 2006.3 |  332.8 |  267969.09 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,075.648 ms |  1.53 | 667 |  620.7 | 1075.6 |  266824.34 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,409.387 ms |  3.34 | 667 |  277.1 | 2409.4 | 2442315.91 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,087.482 ms |  2.96 | 667 |  319.9 | 2087.5 |  266832.87 KB |        1.00 |
