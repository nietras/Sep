```

BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100-rc.2.23502.2
  [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
  Job-AFDVVY : .NET 7.0.12 (7.0.1223.47720), X64 RyuJIT AVX2
  Job-RGAYEX : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2

InvocationCount=Default  IterationTime=300.0000 ms  MaxIterationCount=Default  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method    | Runtime  | Scope | Rows  | Mean       | Ratio | MB | MB/s    | ns/row | Allocated    | Alloc Ratio |
|---------- |--------- |------ |------ |-----------:|------:|---:|--------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 | Row   | 50000 |   2.676 ms |  1.00 | 29 | 10906.3 |   53.5 |      1.15 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Row   | 50000 |   3.312 ms |  1.23 | 29 |  8810.4 |   66.2 |      7.17 KB |        6.25 |
| ReadLine_ | .NET 7.0 | Row   | 50000 |  13.984 ms |  5.22 | 29 |  2086.8 |  279.7 |  88608.25 KB |   77,221.15 |
| CsvHelper | .NET 7.0 | Row   | 50000 |  53.964 ms | 20.15 | 29 |   540.8 | 1079.3 |     20.65 KB |       18.00 |
| Sep______ | .NET 8.0 | Row   | 50000 |   2.548 ms |  0.95 | 29 | 11453.1 |   51.0 |      1.15 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Row   | 50000 |   3.131 ms |  1.16 | 29 |  9318.7 |   62.6 |      7.17 KB |        6.25 |
| ReadLine_ | .NET 8.0 | Row   | 50000 |  12.868 ms |  4.83 | 29 |  2267.7 |  257.4 |  88608.24 KB |   77,221.14 |
| CsvHelper | .NET 8.0 | Row   | 50000 |  45.224 ms | 17.00 | 29 |   645.3 |  904.5 |     20.59 KB |       17.94 |
|           |          |       |       |            |       |    |         |        |              |             |
| Sep______ | .NET 7.0 | Cols  | 50000 |   3.409 ms |  1.00 | 29 |  8558.9 |   68.2 |      1.15 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Cols  | 50000 |   5.724 ms |  1.68 | 29 |  5098.1 |  114.5 |      7.18 KB |        6.24 |
| ReadLine_ | .NET 7.0 | Cols  | 50000 |  14.118 ms |  4.14 | 29 |  2066.9 |  282.4 |  88608.25 KB |   77,024.49 |
| CsvHelper | .NET 7.0 | Cols  | 50000 |  79.537 ms | 23.21 | 29 |   366.9 | 1590.7 |    446.31 KB |      387.96 |
| Sep______ | .NET 8.0 | Cols  | 50000 |   3.308 ms |  0.97 | 29 |  8821.8 |   66.2 |      1.15 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Cols  | 50000 |   5.514 ms |  1.61 | 29 |  5292.0 |  110.3 |      7.17 KB |        6.24 |
| ReadLine_ | .NET 8.0 | Cols  | 50000 |  13.312 ms |  3.91 | 29 |  2192.1 |  266.2 |  88608.24 KB |   77,024.48 |
| CsvHelper | .NET 8.0 | Cols  | 50000 |  72.497 ms | 21.09 | 29 |   402.5 | 1449.9 |    446.35 KB |      388.00 |
|           |          |       |       |            |       |    |         |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  34.356 ms |  1.00 | 29 |   849.4 |  687.1 |  13801.08 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 |  42.055 ms |  1.21 | 29 |   693.9 |  841.1 |  14025.03 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 114.735 ms |  3.36 | 29 |   254.3 | 2294.7 | 102133.99 KB |        7.40 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 106.649 ms |  3.12 | 29 |   273.6 | 2133.0 |  13972.27 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  30.449 ms |  0.89 | 29 |   958.4 |  609.0 |  13799.68 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  38.350 ms |  1.12 | 29 |   760.9 |  767.0 |  14025.03 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 114.011 ms |  3.33 | 29 |   256.0 | 2280.2 | 102133.28 KB |        7.40 |
| CsvHelper | .NET 8.0 | Asset | 50000 |  87.295 ms |  2.58 | 29 |   334.3 | 1745.9 |  13970.99 KB |        1.01 |
