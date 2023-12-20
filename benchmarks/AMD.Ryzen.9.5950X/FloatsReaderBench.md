```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-SVHPFG : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.011 ms |  1.00 | 20 | 10105.3 |   80.4 |     1.18 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.325 ms |  1.16 | 20 |  8738.1 |   93.0 |    10.02 KB |        8.52 |
| ReadLine_ | Row    | 25000 |  11.574 ms |  5.77 | 20 |  1755.6 |  463.0 | 73489.64 KB |   62,450.95 |
| CsvHelper | Row    | 25000 |  25.509 ms | 12.69 | 20 |   796.6 | 1020.4 |    20.59 KB |       17.49 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |   2.605 ms |  1.00 | 20 |  7799.5 |  104.2 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   3.815 ms |  1.46 | 20 |  5326.0 |  152.6 |    10.03 KB |        8.48 |
| ReadLine_ | Cols   | 25000 |  11.729 ms |  4.60 | 20 |  1732.5 |  469.2 | 73489.64 KB |   62,141.53 |
| CsvHelper | Cols   | 25000 |  27.727 ms | 10.64 | 20 |   732.8 | 1109.1 | 21340.81 KB |   18,045.40 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Floats | 25000 |  22.110 ms |  1.00 | 20 |   919.0 |  884.4 |     7.93 KB |        1.00 |
| Sep__MT__ | Floats | 25000 |   4.084 ms |  0.18 | 20 |  4975.1 |  163.4 |   176.38 KB |       22.25 |
| Sylvan___ | Floats | 25000 |  69.014 ms |  3.12 | 20 |   294.4 | 2760.6 |     18.2 KB |        2.30 |
| ReadLine_ | Floats | 25000 |  73.638 ms |  3.33 | 20 |   275.9 | 2945.5 | 73493.12 KB |    9,271.52 |
| CsvHelper | Floats | 25000 | 108.419 ms |  4.90 | 20 |   187.4 | 4336.8 | 22062.55 KB |    2,783.30 |
