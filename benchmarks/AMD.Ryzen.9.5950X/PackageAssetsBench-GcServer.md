```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 9.0.100-rc.2.24474.11
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-YVJTZC : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-ZDJCYM : .NET 9.0.0 (9.0.24.47305), X64 RyuJIT AVX2

Server=True  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method    | Runtime  | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|---------- |--------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 8.0 | Asset | 50000   |    21.402 ms |  1.00 |  29 | 1363.5 |  428.0 |   14133102 B |        1.00 |
| Sep_MT___ | .NET 8.0 | Asset | 50000   |     5.576 ms |  0.26 |  29 | 5233.7 |  111.5 |   14308501 B |        1.01 |
| Sylvan___ | .NET 8.0 | Asset | 50000   |    29.353 ms |  1.37 |  29 |  994.2 |  587.1 |   14296807 B |        1.01 |
| ReadLine_ | .NET 8.0 | Asset | 50000   |    34.506 ms |  1.61 |  29 |  845.7 |  690.1 |  104583817 B |        7.40 |
| CsvHelper | .NET 8.0 | Asset | 50000   |    76.909 ms |  3.59 |  29 |  379.4 | 1538.2 |   14305396 B |        1.01 |
| Sep______ | .NET 9.0 | Asset | 50000   |    24.444 ms |  1.14 |  29 | 1193.8 |  488.9 |   14133077 B |        1.00 |
| Sep_MT___ | .NET 9.0 | Asset | 50000   |     8.965 ms |  0.42 |  29 | 3255.0 |  179.3 |   14310332 B |        1.01 |
| Sylvan___ | .NET 9.0 | Asset | 50000   |    29.814 ms |  1.39 |  29 |  978.8 |  596.3 |            - |        0.00 |
| ReadLine_ | .NET 9.0 | Asset | 50000   |    51.641 ms |  2.41 |  29 |  565.1 | 1032.8 |  104583864 B |        7.40 |
| CsvHelper | .NET 9.0 | Asset | 50000   |    75.502 ms |  3.53 |  29 |  386.5 | 1510.0 |            - |        0.00 |
|           |          |       |         |              |       |     |        |        |              |             |
| Sep______ | .NET 8.0 | Asset | 1000000 |   429.654 ms |  1.00 | 583 | 1358.7 |  429.7 |  273063216 B |        1.00 |
| Sep_MT___ | .NET 8.0 | Asset | 1000000 |   102.979 ms |  0.24 | 583 | 5668.9 |  103.0 |  274049328 B |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 1000000 |   588.747 ms |  1.37 | 583 |  991.6 |  588.7 |  273226088 B |        1.00 |
| ReadLine_ | .NET 8.0 | Asset | 1000000 |   578.663 ms |  1.35 | 583 | 1008.8 |  578.7 | 2087762184 B |        7.65 |
| CsvHelper | .NET 8.0 | Asset | 1000000 | 1,541.094 ms |  3.59 | 583 |  378.8 | 1541.1 |  273234432 B |        1.00 |
| Sep______ | .NET 9.0 | Asset | 1000000 |   500.250 ms |  1.16 | 583 | 1167.0 |  500.3 |  273062592 B |        1.00 |
| Sep_MT___ | .NET 9.0 | Asset | 1000000 |   174.802 ms |  0.41 | 583 | 3339.7 |  174.8 |  273973628 B |        1.00 |
| Sylvan___ | .NET 9.0 | Asset | 1000000 |   631.960 ms |  1.47 | 583 |  923.8 |  632.0 |  273225752 B |        1.00 |
| ReadLine_ | .NET 9.0 | Asset | 1000000 | 1,064.981 ms |  2.48 | 583 |  548.2 | 1065.0 | 2087764680 B |        7.65 |
| CsvHelper | .NET 9.0 | Asset | 1000000 | 1,658.890 ms |  3.86 | 583 |  351.9 | 1658.9 |  273234104 B |        1.00 |
