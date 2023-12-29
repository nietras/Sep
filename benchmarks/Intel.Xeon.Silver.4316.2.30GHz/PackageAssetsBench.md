```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-QHZJOP : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-QHZJOP  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     5.118 ms |  1.00 |  29 | 5701.4 |  102.4 |       1.18 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.146 ms |  1.01 |  29 | 5670.6 |  102.9 |       1.18 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     5.953 ms |  1.16 |  29 | 4901.9 |  119.1 |       7.21 KB |        6.12 |
| ReadLine_    | Row   | 50000   |    25.973 ms |  5.07 |  29 | 1123.5 |  519.5 |   88608.26 KB |   75,111.64 |
| CsvHelper    | Row   | 50000   |    90.063 ms | 17.60 |  29 |  324.0 | 1801.3 |      20.69 KB |       17.54 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     7.123 ms |  1.00 |  29 | 4096.5 |  142.5 |       1.19 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.943 ms |  1.12 |  29 | 3673.9 |  158.9 |       1.19 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    10.124 ms |  1.42 |  29 | 2882.3 |  202.5 |       7.22 KB |        6.09 |
| ReadLine_    | Cols  | 50000   |    25.928 ms |  3.64 |  29 | 1125.5 |  518.6 |   88608.24 KB |   74,678.88 |
| CsvHelper    | Cols  | 50000   |   140.614 ms | 19.74 |  29 |  207.5 | 2812.3 |     446.45 KB |      376.26 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    53.901 ms |  1.00 |  29 |  541.4 | 1078.0 |   13802.75 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    30.484 ms |  0.57 |  29 |  957.3 |  609.7 |   14030.67 KB |        1.02 |
| Sylvan___    | Asset | 50000   |    67.354 ms |  1.25 |  29 |  433.2 | 1347.1 |   13961.77 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   149.924 ms |  2.78 |  29 |  194.6 | 2998.5 |  102133.61 KB |        7.40 |
| CsvHelper    | Asset | 50000   |   158.310 ms |  2.94 |  29 |  184.3 | 3166.2 |    13970.8 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,122.212 ms |  1.00 | 583 |  520.2 | 1122.2 |  266672.93 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   378.388 ms |  0.34 | 583 | 1542.8 |  378.4 |  267505.55 KB |        1.00 |
| Sylvan___    | Asset | 1000000 | 1,408.440 ms |  1.26 | 583 |  414.5 | 1408.4 |  266826.38 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,962.035 ms |  2.63 | 583 |  197.1 | 2962.0 | 2038832.76 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 3,379.135 ms |  3.00 | 583 |  172.8 | 3379.1 |  266833.95 KB |        1.00 |
