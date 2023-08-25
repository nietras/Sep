``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=8.0.100-preview.7.23376.3
  [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
  Job-ZCBCZS : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2
  Job-KHRFRP : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2

InvocationCount=Default  IterationTime=300.0000 ms  MaxIterationCount=Default  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
|    Method |  Runtime | Scope |  Rows |       Mean | Ratio | MB |   MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |-----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |   7.414 ms |  1.00 | 33 | 4501.8 |  148.3 |      1.15 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |  21.528 ms |  2.91 | 33 | 1550.4 |  430.6 |      7.33 KB |        6.40 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  15.944 ms |  2.13 | 33 | 2093.4 |  318.9 | 108778.76 KB |   94,961.17 |
| CsvHelper | .NET 7.0 |   Row | 50000 |  68.558 ms |  9.26 | 33 |  486.8 | 1371.2 |     20.65 KB |       18.03 |
| Sep______ | .NET 8.0 |   Row | 50000 |   6.773 ms |  0.91 | 33 | 4928.3 |  135.5 |      1.14 KB |        1.00 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |  18.409 ms |  2.47 | 33 | 1813.1 |  368.2 |       7.2 KB |        6.28 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  14.955 ms |  2.03 | 33 | 2231.9 |  299.1 | 108778.75 KB |   94,961.16 |
| CsvHelper | .NET 8.0 |   Row | 50000 |  53.158 ms |  7.17 | 33 |  627.9 | 1063.2 |      20.6 KB |       17.98 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |   7.723 ms |  1.00 | 33 | 4321.6 |  154.5 |      1.15 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |  23.941 ms |  3.10 | 33 | 1394.2 |  478.8 |      7.33 KB |        6.40 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  16.261 ms |  2.11 | 33 | 2052.6 |  325.2 | 108778.75 KB |   94,880.28 |
| CsvHelper | .NET 7.0 |  Cols | 50000 |  90.538 ms | 11.65 | 33 |  368.7 | 1810.8 |    446.31 KB |      389.29 |
| Sep______ | .NET 8.0 |  Cols | 50000 |   7.323 ms |  0.95 | 33 | 4557.6 |  146.5 |      1.14 KB |        1.00 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |  21.064 ms |  2.73 | 33 | 1584.6 |  421.3 |      7.31 KB |        6.38 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  15.523 ms |  2.02 | 33 | 2150.2 |  310.5 | 108778.75 KB |   94,880.27 |
| CsvHelper | .NET 8.0 |  Cols | 50000 |  84.033 ms | 10.81 | 33 |  397.2 | 1680.7 |    446.35 KB |      389.32 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  38.130 ms |  1.00 | 33 |  875.4 |  762.6 |  13808.03 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 |  61.626 ms |  1.62 | 33 |  541.6 | 1232.5 |  14025.04 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 125.779 ms |  3.33 | 33 |  265.4 | 2515.6 | 122304.12 KB |        8.86 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 114.214 ms |  3.00 | 33 |  292.2 | 2284.3 |  13971.43 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  34.585 ms |  0.91 | 33 |  965.1 |  691.7 |  13808.01 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  53.850 ms |  1.41 | 33 |  619.8 | 1077.0 |  14025.15 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 115.295 ms |  3.02 | 33 |  289.5 | 2305.9 | 122304.01 KB |        8.86 |
| CsvHelper | .NET 8.0 | Asset | 50000 |  99.115 ms |  2.60 | 33 |  336.8 | 1982.3 |  13970.79 KB |        1.01 |
