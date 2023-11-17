```

BenchmarkDotNet v0.13.11-nightly.20231116.103, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-CEECDD : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-CEECDD  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |   5.253 ms |  1.00 | 29 | 5554.8 |  105.1 |     1.07 KB |        1.00 |
| Sep_Unescape | Row   | 50000 |   5.287 ms |  1.01 | 29 | 5519.5 |  105.7 |     1.07 KB |        1.00 |
| Sylvan___    | Row   | 50000 |   5.925 ms |  1.13 | 29 | 4925.4 |  118.5 |     7.21 KB |        6.72 |
| ReadLine_    | Row   | 50000 |  25.585 ms |  4.87 | 29 | 1140.5 |  511.7 | 88608.26 KB |   82,561.29 |
| CsvHelper    | Row   | 50000 |  89.448 ms | 17.02 | 29 |  326.2 | 1789.0 |    20.69 KB |       19.28 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Cols  | 50000 |   7.123 ms |  1.00 | 29 | 4097.0 |  142.5 |     1.08 KB |        1.00 |
| Sep_Unescape | Cols  | 50000 |   7.953 ms |  1.12 | 29 | 3669.4 |  159.1 |     1.08 KB |        1.00 |
| Sylvan___    | Cols  | 50000 |  10.047 ms |  1.41 | 29 | 2904.6 |  200.9 |     7.22 KB |        6.71 |
| ReadLine_    | Cols  | 50000 |  25.720 ms |  3.61 | 29 | 1134.6 |  514.4 | 88608.26 KB |   82,261.89 |
| CsvHelper    | Cols  | 50000 | 138.163 ms | 19.40 | 29 |  211.2 | 2763.3 |   446.28 KB |      414.31 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Asset | 50000 |  53.365 ms |  1.00 | 29 |  546.8 | 1067.3 | 13799.86 KB |        1.00 |
| Sep_Unescape | Asset | 50000 |  53.388 ms |  1.00 | 29 |  546.6 | 1067.8 | 13800.99 KB |        1.00 |
| Sylvan___    | Asset | 50000 |  67.865 ms |  1.27 | 29 |  430.0 | 1357.3 | 13962.46 KB |        1.01 |
| ReadLine_    | Asset | 50000 | 153.589 ms |  2.88 | 29 |  190.0 | 3071.8 | 102133.4 KB |        7.40 |
| CsvHelper    | Asset | 50000 | 159.288 ms |  2.99 | 29 |  183.2 | 3185.8 |  13970.8 KB |        1.01 |
