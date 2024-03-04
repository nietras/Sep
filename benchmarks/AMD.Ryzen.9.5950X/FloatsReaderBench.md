```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  Job-XZYDSN : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   1.959 ms |  1.00 | 20 | 10370.4 |   78.4 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.306 ms |  1.18 | 20 |  8810.9 |   92.2 |    10.02 KB |        8.02 |
| ReadLine_ | Row    | 25000 |  10.875 ms |  5.55 | 20 |  1868.4 |  435.0 | 73489.63 KB |   58,791.71 |
| CsvHelper | Row    | 25000 |  25.222 ms | 12.88 | 20 |   805.6 | 1008.9 |       20 KB |       16.00 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |   2.631 ms |  1.00 | 20 |  7723.4 |  105.2 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   3.772 ms |  1.43 | 20 |  5387.7 |  150.9 |    10.03 KB |        8.00 |
| ReadLine_ | Cols   | 25000 |  11.067 ms |  4.20 | 20 |  1836.0 |  442.7 | 73489.64 KB |   58,654.24 |
| CsvHelper | Cols   | 25000 |  26.921 ms | 10.23 | 20 |   754.8 | 1076.8 | 21340.22 KB |   17,032.26 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Floats | 25000 |  22.155 ms |  1.00 | 20 |   917.2 |  886.2 |        8 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   3.405 ms |  0.16 | 20 |  5967.9 |  136.2 |   182.29 KB |       22.78 |
| Sylvan___ | Floats | 25000 |  67.351 ms |  3.04 | 20 |   301.7 | 2694.1 |     18.2 KB |        2.27 |
| ReadLine_ | Floats | 25000 |  71.910 ms |  3.25 | 20 |   282.6 | 2876.4 | 73493.12 KB |    9,182.16 |
| CsvHelper | Floats | 25000 | 104.141 ms |  4.70 | 20 |   195.1 | 4165.7 | 22061.92 KB |    2,756.39 |
