```

BenchmarkDotNet v0.13.11-nightly.20231116.103, Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-CEECDD : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.698 ms |  1.00 | 20 | 5494.3 |  147.9 |     1.31 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   4.570 ms |  1.24 | 20 | 4446.5 |  182.8 |    10.03 KB |        7.67 |
| ReadLine_ | Row    | 25000 |  20.949 ms |  5.66 | 20 |  970.0 |  837.9 | 73489.67 KB |   56,201.21 |
| CsvHelper | Row    | 25000 |  56.975 ms | 15.41 | 20 |  356.6 | 2279.0 |    20.77 KB |       15.89 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   5.087 ms |  1.00 | 20 | 3994.7 |  203.5 |     1.31 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   7.370 ms |  1.45 | 20 | 2757.1 |  294.8 |    10.04 KB |        7.68 |
| ReadLine_ | Cols   | 25000 |  21.340 ms |  4.20 | 20 |  952.2 |  853.6 | 73489.67 KB |   56,243.22 |
| CsvHelper | Cols   | 25000 |  60.316 ms | 11.86 | 20 |  336.9 | 2412.6 | 21340.99 KB |   16,332.72 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  41.747 ms |  1.00 | 20 |  486.7 | 1669.9 |     8.12 KB |        1.00 |
| Sylvan___ | Floats | 25000 | 138.798 ms |  3.33 | 20 |  146.4 | 5551.9 |    18.43 KB |        2.27 |
| ReadLine_ | Floats | 25000 | 148.648 ms |  3.56 | 20 |  136.7 | 5945.9 |  73493.3 KB |    9,054.03 |
| CsvHelper | Floats | 25000 | 212.156 ms |  5.08 | 20 |   95.8 | 8486.2 | 22062.78 KB |    2,718.03 |
