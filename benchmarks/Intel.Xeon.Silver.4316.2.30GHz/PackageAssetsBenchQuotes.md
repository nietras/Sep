```

BenchmarkDotNet v0.13.11-nightly.20231116.103, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-CEECDD : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Job-CEECDD  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method       | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |------ |----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000 |  13.72 ms |  1.00 | 33 | 2433.1 |  274.4 |      1.09 KB |        1.00 |
| Sep_Unescape | Row   | 50000 |  14.36 ms |  1.05 | 33 | 2325.0 |  287.1 |      1.09 KB |        1.00 |
| Sylvan___    | Row   | 50000 |  34.97 ms |  2.55 | 33 |  954.3 |  699.5 |      7.26 KB |        6.66 |
| ReadLine_    | Row   | 50000 |  29.99 ms |  2.18 | 33 | 1113.0 |  599.8 | 108778.77 KB |   99,721.98 |
| CsvHelper    | Row   | 50000 | 102.27 ms |  7.45 | 33 |  326.4 | 2045.5 |     20.69 KB |       18.97 |
|              |       |       |           |       |    |        |        |              |             |
| Sep______    | Cols  | 50000 |  15.38 ms |  1.00 | 33 | 2169.7 |  307.7 |      1.09 KB |        1.00 |
| Sep_Unescape | Cols  | 50000 |  17.71 ms |  1.15 | 33 | 1884.8 |  354.2 |       1.1 KB |        1.01 |
| Sylvan___    | Cols  | 50000 |  40.92 ms |  2.71 | 33 |  815.6 |  818.5 |      7.27 KB |        6.64 |
| ReadLine_    | Cols  | 50000 |  30.22 ms |  1.96 | 33 | 1104.5 |  604.4 | 108778.77 KB |   99,366.16 |
| CsvHelper    | Cols  | 50000 | 159.90 ms | 10.39 | 33 |  208.7 | 3198.0 |    446.45 KB |      407.81 |
|              |       |       |           |       |    |        |        |              |             |
| Sep______    | Asset | 50000 |  66.17 ms |  1.00 | 33 |  504.4 | 1323.4 |  13809.96 KB |        1.00 |
| Sep_Unescape | Asset | 50000 |  62.28 ms |  0.95 | 33 |  536.0 | 1245.5 |  13799.79 KB |        1.00 |
| Sylvan___    | Asset | 50000 |  95.92 ms |  1.45 | 33 |  348.0 | 1918.4 |  13962.73 KB |        1.01 |
| ReadLine_    | Asset | 50000 | 193.85 ms |  2.96 | 33 |  172.2 | 3876.9 |  122304.1 KB |        8.86 |
| CsvHelper    | Asset | 50000 | 178.36 ms |  2.70 | 33 |  187.1 | 3567.2 |   13970.8 KB |        1.01 |
