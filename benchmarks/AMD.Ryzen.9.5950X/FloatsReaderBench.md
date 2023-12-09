```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-HCXKGU : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Runtime=.NET 8.0  Toolchain=net80  InvocationCount=Default  
IterationTime=350.0000 ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|--------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   1.893 ms |  1.00 | 20 | 10733.4 |   75.7 |     1.15 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   2.212 ms |  1.17 | 20 |  9187.9 |   88.5 |    10.02 KB |        8.74 |
| ReadLine_ | Row    | 25000 |  10.913 ms |  5.76 | 20 |  1862.0 |  436.5 | 73489.63 KB |   64,099.99 |
| CsvHelper | Row    | 25000 |  25.079 ms | 13.25 | 20 |   810.2 | 1003.2 |    20.58 KB |       17.95 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Cols   | 25000 |   2.591 ms |  1.00 | 20 |  7843.3 |  103.6 |     1.15 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   3.738 ms |  1.44 | 20 |  5436.2 |  149.5 |    10.03 KB |        8.74 |
| ReadLine_ | Cols   | 25000 |  11.010 ms |  4.25 | 20 |  1845.6 |  440.4 | 73489.64 KB |   64,045.44 |
| CsvHelper | Cols   | 25000 |  27.201 ms | 10.43 | 20 |   747.0 | 1088.0 | 21340.99 KB |   18,598.45 |
|           |        |       |            |       |    |         |        |             |             |
| Sep______ | Floats | 25000 |  21.397 ms |  1.00 | 20 |   949.7 |  855.9 |     7.87 KB |        1.00 |
| Sylvan___ | Floats | 25000 |  65.771 ms |  3.08 | 20 |   308.9 | 2630.8 |     18.2 KB |        2.31 |
| ReadLine_ | Floats | 25000 |  72.513 ms |  3.39 | 20 |   280.2 | 2900.5 | 73493.12 KB |    9,338.25 |
| CsvHelper | Floats | 25000 | 106.198 ms |  4.96 | 20 |   191.3 | 4247.9 | 22062.55 KB |    2,803.33 |
