```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  Job-QSXOQW : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2

Job=Job-QSXOQW  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    26.56 ms |  1.00 |  33 | 1256.9 |  531.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.72 ms |  0.40 |  33 | 3112.1 |  214.5 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    45.31 ms |  1.70 |  33 |  736.6 |  906.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    39.91 ms |  1.53 |  33 |  836.3 |  798.3 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    92.48 ms |  3.48 |  33 |  360.9 | 1849.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   536.22 ms |  1.00 | 667 | 1245.2 |  536.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   203.42 ms |  0.38 | 667 | 3282.3 |  203.4 |  261.45 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   906.15 ms |  1.69 | 667 |  736.9 |  906.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   658.41 ms |  1.23 | 667 | 1014.1 |  658.4 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,842.77 ms |  3.44 | 667 |  362.3 | 1842.8 |  260.58 MB |        1.00 |
