```

BenchmarkDotNet v0.13.11, Ubuntu 22.04.2 LTS (Jammy Jellyfish)
Unknown processor
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  Job-EEMDRF : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Job=Job-EEMDRF  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    25.14 ms |  1.00 |  33 | 1323.9 |  502.8 |       1.04 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    24.79 ms |  0.99 |  33 | 1342.6 |  495.8 |       1.04 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    39.92 ms |  1.59 |  33 |  833.7 |  798.4 |       6.14 KB |        5.88 |
| ReadLine_    | Row   | 50000   |    44.29 ms |  1.76 |  33 |  751.5 |  885.8 |  108778.83 KB |  104,199.74 |
| CsvHelper    | Row   | 50000   |   106.65 ms |  4.24 |  33 |  312.1 | 2133.0 |      20.77 KB |       19.90 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    27.56 ms |  1.00 |  33 | 1207.5 |  551.3 |       1.06 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    29.35 ms |  1.06 |  33 | 1133.8 |  587.1 |       1.06 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    45.55 ms |  1.65 |  33 |  730.7 |  911.0 |       6.17 KB |        5.82 |
| ReadLine_    | Cols  | 50000   |    45.78 ms |  1.66 |  33 |  727.1 |  915.5 |  108778.83 KB |  102,663.15 |
| CsvHelper    | Cols  | 50000   |   152.23 ms |  5.52 |  33 |  218.6 | 3044.5 |     446.61 KB |      421.50 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    80.29 ms |  1.00 |  33 |  414.5 | 1605.8 |   13804.89 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    49.12 ms |  0.61 |  33 |  677.5 |  982.4 |   13858.29 KB |        1.00 |
| Sylvan___    | Asset | 50000   |   110.68 ms |  1.38 |  33 |  300.7 | 2213.6 |   13961.37 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   196.34 ms |  2.44 |  33 |  169.5 | 3926.7 |  122305.09 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   180.62 ms |  2.24 |  33 |  184.3 | 3612.4 |   13974.57 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,619.19 ms |  1.00 | 665 |  411.2 | 1619.2 |  266671.09 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   830.09 ms |  0.52 | 665 |  802.1 |  830.1 |  269668.23 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 2,185.17 ms |  1.35 | 665 |  304.7 | 2185.2 |  266828.83 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,986.74 ms |  2.46 | 665 |  167.0 | 3986.7 | 2442318.74 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 3,682.98 ms |  2.27 | 665 |  180.8 | 3683.0 |  266841.98 KB |        1.00 |
