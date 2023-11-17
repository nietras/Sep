```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-ETZYVC : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.694 ms |  1.00 | 20 | 5500.0 |  147.8 |     1.31 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   4.414 ms |  1.19 | 20 | 4603.7 |  176.6 |    10.03 KB |        7.67 |
| ReadLine_ | Row    | 25000 |  20.829 ms |  5.64 | 20 |  975.5 |  833.2 | 73489.67 KB |   56,201.21 |
| CsvHelper | Row    | 25000 |  57.181 ms | 15.48 | 20 |  355.4 | 2287.3 |    20.65 KB |       15.79 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   5.083 ms |  1.00 | 20 | 3997.4 |  203.3 |     1.31 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   7.463 ms |  1.47 | 20 | 2722.7 |  298.5 |    10.04 KB |        7.66 |
| ReadLine_ | Cols   | 25000 |  21.766 ms |  4.28 | 20 |  933.6 |  870.6 | 73489.66 KB |   56,117.39 |
| CsvHelper | Cols   | 25000 |  60.254 ms | 11.87 | 20 |  337.2 | 2410.2 | 21340.99 KB |   16,296.18 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  41.634 ms |  1.00 | 20 |  488.1 | 1665.4 |     8.12 KB |        1.00 |
| Sylvan___ | Floats | 25000 | 137.558 ms |  3.30 | 20 |  147.7 | 5502.3 |    18.13 KB |        2.23 |
| ReadLine_ | Floats | 25000 | 152.711 ms |  3.67 | 20 |  133.1 | 6108.5 |  73493.3 KB |    9,054.03 |
| CsvHelper | Floats | 25000 | 213.789 ms |  5.14 | 20 |   95.0 | 8551.6 | 22062.95 KB |    2,718.05 |
