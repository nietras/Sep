```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.100-rc.2.24474.11
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-YVJTZC : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-ZDJCYM : .NET 9.0.0 (9.0.24.47305), X64 RyuJIT AVX2

Server=True  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method    | Runtime  | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |--------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | .NET 8.0 | Asset | 50000   |    26.13 ms |  1.00 |  33 | 1277.4 |  522.6 |   13.48 MB |        1.00 |
| Sep_MT___ | .NET 8.0 | Asset | 50000   |    11.04 ms |  0.42 |  33 | 3023.1 |  220.8 |   13.64 MB |        1.01 |
| Sylvan___ | .NET 8.0 | Asset | 50000   |    44.04 ms |  1.69 |  33 |  757.9 |  880.8 |   13.63 MB |        1.01 |
| ReadLine_ | .NET 8.0 | Asset | 50000   |    39.90 ms |  1.53 |  33 |  836.6 |  798.0 |  119.44 MB |        8.86 |
| CsvHelper | .NET 8.0 | Asset | 50000   |    90.83 ms |  3.48 |  33 |  367.5 | 1816.5 |   13.64 MB |        1.01 |
| Sep______ | .NET 9.0 | Asset | 50000   |    27.51 ms |  1.05 |  33 | 1213.4 |  550.1 |   13.48 MB |        1.00 |
| Sep_MT___ | .NET 9.0 | Asset | 50000   |    14.61 ms |  0.56 |  33 | 2285.1 |  292.1 |   13.64 MB |        1.01 |
| Sylvan___ | .NET 9.0 | Asset | 50000   |    45.19 ms |  1.73 |  33 |  738.6 |  903.8 |   13.63 MB |        1.01 |
| ReadLine_ | .NET 9.0 | Asset | 50000   |    61.84 ms |  2.37 |  33 |  539.8 | 1236.8 |  119.44 MB |        8.86 |
| CsvHelper | .NET 9.0 | Asset | 50000   |    77.37 ms |  2.96 |  33 |  431.4 | 1547.3 |   13.64 MB |        1.01 |
|           |          |       |         |             |       |     |        |        |            |             |
| Sep______ | .NET 8.0 | Asset | 1000000 |   530.94 ms |  1.00 | 667 | 1257.6 |  530.9 |  260.41 MB |        1.00 |
| Sep_MT___ | .NET 8.0 | Asset | 1000000 |   209.02 ms |  0.39 | 667 | 3194.4 |  209.0 |  261.46 MB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 1000000 |   916.99 ms |  1.73 | 667 |  728.1 |  917.0 |  260.57 MB |        1.00 |
| ReadLine_ | .NET 8.0 | Asset | 1000000 |   675.93 ms |  1.27 | 667 |  987.8 |  675.9 | 2385.07 MB |        9.16 |
| CsvHelper | .NET 8.0 | Asset | 1000000 | 1,816.04 ms |  3.42 | 667 |  367.7 | 1816.0 |  260.58 MB |        1.00 |
| Sep______ | .NET 9.0 | Asset | 1000000 |   600.04 ms |  1.13 | 667 | 1112.8 |  600.0 |  260.41 MB |        1.00 |
| Sep_MT___ | .NET 9.0 | Asset | 1000000 |   244.07 ms |  0.46 | 667 | 2735.7 |  244.1 |  261.54 MB |        1.00 |
| Sylvan___ | .NET 9.0 | Asset | 1000000 |   923.90 ms |  1.74 | 667 |  722.7 |  923.9 |  260.57 MB |        1.00 |
| ReadLine_ | .NET 9.0 | Asset | 1000000 | 1,187.71 ms |  2.24 | 667 |  562.2 | 1187.7 | 2385.07 MB |        9.16 |
| CsvHelper | .NET 9.0 | Asset | 1000000 | 1,652.88 ms |  3.11 | 667 |  404.0 | 1652.9 |  260.58 MB |        1.00 |
