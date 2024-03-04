```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.201
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
  Job-QSXOQW : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2

Job=Job-QSXOQW  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350.0000 ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    21.674 ms |  1.00 |  29 | 1346.4 |  433.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     6.017 ms |  0.28 |  29 | 4850.0 |  120.3 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    29.966 ms |  1.38 |  29 |  973.8 |  599.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    34.051 ms |  1.53 |  29 |  857.0 |  681.0 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    78.578 ms |  3.62 |  29 |  371.4 | 1571.6 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   442.652 ms |  1.00 | 583 | 1318.8 |  442.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   110.797 ms |  0.25 | 583 | 5268.9 |  110.8 |  261.26 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   603.402 ms |  1.36 | 583 |  967.5 |  603.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   584.228 ms |  1.32 | 583 |  999.2 |  584.2 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,572.055 ms |  3.55 | 583 |  371.4 | 1572.1 |  260.58 MB |        1.00 |
