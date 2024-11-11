```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-ZCGTWP : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

Job=Job-ZCGTWP  Runtime=.NET 8.0  Server=True  
Toolchain=net80  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    22.116 ms |  1.00 |  29 | 1319.5 |  442.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     5.550 ms |  0.25 |  29 | 5257.8 |  111.0 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    29.640 ms |  1.34 |  29 |  984.5 |  592.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    34.290 ms |  1.55 |  29 |  851.0 |  685.8 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    78.540 ms |  3.55 |  29 |  371.5 | 1570.8 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   449.588 ms |  1.00 | 583 | 1298.5 |  449.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   109.711 ms |  0.24 | 583 | 5321.1 |  109.7 |  261.42 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   596.673 ms |  1.33 | 583 |  978.4 |  596.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   583.206 ms |  1.30 | 583 | 1001.0 |  583.2 | 1991.05 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,560.941 ms |  3.47 | 583 |  374.0 | 1560.9 |  260.58 MB |        1.00 |
