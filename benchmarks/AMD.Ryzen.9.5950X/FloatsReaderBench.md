```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.202
  [Host]     : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2
  Job-OCZSUI : .NET 8.0.3 (8.0.324.11423), X64 RyuJIT AVX2

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.061 ms |  1.00 | 20 | 9857.0 |   82.5 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.448 ms |  1.19 | 20 | 8299.1 |   97.9 |    10.02 KB |        8.02 |
| ReadLine_ | Row    | 25000 |  11.663 ms |  5.66 | 20 | 1742.2 |  466.5 | 73489.64 KB |   58,791.71 |
| CsvHelper | Row    | 25000 |  26.409 ms | 12.82 | 20 |  769.4 | 1056.3 |       20 KB |       16.00 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   2.696 ms |  1.00 | 20 | 7537.1 |  107.8 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   4.017 ms |  1.49 | 20 | 5058.7 |  160.7 |    10.02 KB |        8.00 |
| ReadLine_ | Cols   | 25000 |  11.638 ms |  4.33 | 20 | 1746.0 |  465.5 | 73489.64 KB |   58,654.24 |
| CsvHelper | Cols   | 25000 |  27.323 ms | 10.14 | 20 |  743.7 | 1092.9 | 21340.22 KB |   17,032.26 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  22.268 ms |  1.00 | 20 |  912.5 |  890.7 |        8 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   3.396 ms |  0.15 | 20 | 5983.3 |  135.8 |   182.55 KB |       22.83 |
| Sylvan___ | Floats | 25000 |  66.977 ms |  3.01 | 20 |  303.4 | 2679.1 |     18.2 KB |        2.28 |
| ReadLine_ | Floats | 25000 |  72.970 ms |  3.27 | 20 |  278.5 | 2918.8 | 73493.12 KB |    9,190.01 |
| CsvHelper | Floats | 25000 | 105.666 ms |  4.75 | 20 |  192.3 | 4226.6 | 22061.92 KB |    2,758.75 |
