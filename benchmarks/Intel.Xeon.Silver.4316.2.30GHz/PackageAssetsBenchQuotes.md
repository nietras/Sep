```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-QHZJOP : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-QHZJOP  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    13.60 ms |  1.00 |  33 | 2453.9 |  272.0 |       1.21 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    12.79 ms |  0.94 |  33 | 2610.7 |  255.7 |        1.2 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    33.79 ms |  2.49 |  33 |  987.8 |  675.8 |       7.26 KB |        6.01 |
| ReadLine_    | Row   | 50000   |    30.72 ms |  2.26 |  33 | 1086.5 |  614.4 |  108778.76 KB |   90,048.06 |
| CsvHelper    | Row   | 50000   |   102.72 ms |  7.55 |  33 |  324.9 | 2054.3 |      20.69 KB |       17.13 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    15.89 ms |  1.00 |  33 | 2100.0 |  317.9 |       1.21 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    16.99 ms |  1.07 |  33 | 1964.6 |  339.8 |       1.22 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    39.57 ms |  2.53 |  33 |  843.6 |  791.3 |       7.27 KB |        5.99 |
| ReadLine_    | Cols  | 50000   |    31.03 ms |  1.95 |  33 | 1075.5 |  620.7 |  108778.74 KB |   89,613.38 |
| CsvHelper    | Cols  | 50000   |   160.67 ms | 10.11 |  33 |  207.7 | 3213.4 |     446.45 KB |      367.79 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    64.27 ms |  1.00 |  33 |  519.4 | 1285.3 |   13804.23 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    36.01 ms |  0.56 |  33 |  926.9 |  720.2 |   14020.14 KB |        1.02 |
| Sylvan___    | Asset | 50000   |    96.43 ms |  1.50 |  33 |  346.1 | 1928.5 |   13962.36 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   198.74 ms |  3.10 |  33 |  167.9 | 3974.7 |  122304.04 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   179.60 ms |  2.79 |  33 |  185.8 | 3591.9 |   13970.63 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,325.26 ms |  1.00 | 667 |  503.8 | 1325.3 |  266667.79 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   573.47 ms |  0.45 | 667 | 1164.3 |  573.5 |  267685.55 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,983.93 ms |  1.50 | 667 |  336.6 | 1983.9 |  266834.56 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 3,804.46 ms |  2.87 | 667 |  175.5 | 3804.5 | 2442323.66 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 3,767.71 ms |  2.84 | 667 |  177.2 | 3767.7 |  266840.59 KB |        1.00 |
