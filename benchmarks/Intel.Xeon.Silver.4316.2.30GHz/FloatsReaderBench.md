```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-QHZJOP : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   4.012 ms |  1.00 | 20 | 5064.2 |  160.5 |     1.41 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   4.607 ms |  1.15 | 20 | 4410.6 |  184.3 |    10.02 KB |        7.11 |
| ReadLine_ | Row    | 25000 |  21.873 ms |  5.46 | 20 |  929.0 |  874.9 | 73489.67 KB |   52,114.56 |
| CsvHelper | Row    | 25000 |  57.624 ms | 14.36 | 20 |  352.6 | 2305.0 |    20.77 KB |       14.73 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   5.841 ms |  1.00 | 20 | 3478.9 |  233.6 |     1.42 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   7.398 ms |  1.27 | 20 | 2746.5 |  295.9 |    10.03 KB |        7.06 |
| ReadLine_ | Cols   | 25000 |  21.529 ms |  3.69 | 20 |  943.8 |  861.2 | 73489.67 KB |   51,756.14 |
| CsvHelper | Cols   | 25000 |  60.610 ms | 10.38 | 20 |  335.3 | 2424.4 | 21340.82 KB |   15,029.57 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  43.612 ms |  1.00 | 20 |  465.9 | 1744.5 |     8.22 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   6.159 ms |  0.14 | 20 | 3299.0 |  246.4 |   213.54 KB |       25.96 |
| Sylvan___ | Floats | 25000 | 142.246 ms |  3.26 | 20 |  142.8 | 5689.8 |    18.43 KB |        2.24 |
| ReadLine_ | Floats | 25000 | 155.347 ms |  3.56 | 20 |  130.8 | 6213.9 |  73493.3 KB |    8,935.78 |
| CsvHelper | Floats | 25000 | 215.336 ms |  4.94 | 20 |   94.4 | 8613.4 | 22062.78 KB |    2,682.53 |
