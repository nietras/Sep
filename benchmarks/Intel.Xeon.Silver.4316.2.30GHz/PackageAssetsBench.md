```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-ETZYVC : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-ETZYVC  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |   5.358 ms |  1.00 | 29 | 5446.4 |  107.2 |     1.07 KB |        1.00 |
| Sep_Unescape | Row   | 50000 |   5.468 ms |  1.02 | 29 | 5337.0 |  109.4 |     1.07 KB |        1.00 |
| Sylvan___    | Row   | 50000 |   6.092 ms |  1.14 | 29 | 4789.9 |  121.8 |     7.21 KB |        6.72 |
| ReadLine_    | Row   | 50000 |  26.195 ms |  4.89 | 29 | 1114.0 |  523.9 | 88608.26 KB |   82,486.23 |
| CsvHelper    | Row   | 50000 |  90.481 ms | 16.93 | 29 |  322.5 | 1809.6 |    20.69 KB |       19.26 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Cols  | 50000 |   7.351 ms |  1.00 | 29 | 3969.6 |  147.0 |     1.08 KB |        1.00 |
| Sep_Unescape | Cols  | 50000 |   8.211 ms |  1.12 | 29 | 3554.1 |  164.2 |     1.08 KB |        1.00 |
| Sylvan___    | Cols  | 50000 |  10.365 ms |  1.41 | 29 | 2815.4 |  207.3 |     7.22 KB |        6.70 |
| ReadLine_    | Cols  | 50000 |  26.844 ms |  3.65 | 29 | 1087.1 |  536.9 | 88608.27 KB |   82,187.38 |
| CsvHelper    | Cols  | 50000 | 140.804 ms | 19.15 | 29 |  207.2 | 2816.1 |   446.45 KB |      414.09 |
|              |       |       |            |       |    |        |        |             |             |
| Sep______    | Asset | 50000 |  56.102 ms |  1.00 | 29 |  520.1 | 1122.0 |  13800.5 KB |        1.00 |
| Sep_Unescape | Asset | 50000 |  56.119 ms |  1.00 | 29 |  520.0 | 1122.4 |  13800.8 KB |        1.00 |
| Sylvan___    | Asset | 50000 |  69.936 ms |  1.25 | 29 |  417.3 | 1398.7 | 13962.15 KB |        1.01 |
| ReadLine_    | Asset | 50000 | 160.963 ms |  2.85 | 29 |  181.3 | 3219.3 | 102133.4 KB |        7.40 |
| CsvHelper    | Asset | 50000 | 161.915 ms |  2.89 | 29 |  180.2 | 3238.3 |  13970.8 KB |        1.01 |
