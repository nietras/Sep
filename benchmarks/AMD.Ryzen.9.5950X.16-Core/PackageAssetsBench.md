``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=8.0.100-preview.7.23376.3
  [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
  Job-ZCBCZS : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2
  Job-KHRFRP : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2

InvocationCount=Default  IterationTime=300.0000 ms  MaxIterationCount=Default  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
|    Method |  Runtime | Scope |  Rows |       Mean | Ratio | MB |    MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |-----------:|------:|---:|--------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |   2.565 ms |  1.00 | 29 | 11375.3 |   51.3 |      1.13 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |   3.301 ms |  1.29 | 29 |  8840.1 |   66.0 |      7.17 KB |        6.34 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  13.676 ms |  5.26 | 29 |  2133.7 |  273.5 |  88608.25 KB |   78,287.18 |
| CsvHelper | .NET 7.0 |   Row | 50000 |  63.120 ms | 24.57 | 29 |   462.3 | 1262.4 |     20.65 KB |       18.25 |
| Sep______ | .NET 8.0 |   Row | 50000 |   2.458 ms |  0.96 | 29 | 11872.7 |   49.2 |      1.13 KB |        1.00 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |   3.053 ms |  1.19 | 29 |  9558.2 |   61.1 |      7.17 KB |        6.33 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  12.642 ms |  4.96 | 29 |  2308.3 |  252.8 |  88608.24 KB |   78,287.18 |
| CsvHelper | .NET 8.0 |   Row | 50000 |  44.515 ms | 17.37 | 29 |   655.5 |  890.3 |     20.59 KB |       18.19 |
|           |          |       |       |            |       |    |         |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |   3.328 ms |  1.00 | 29 |  8769.4 |   66.6 |      1.13 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |   5.773 ms |  1.74 | 29 |  5054.4 |  115.5 |      7.18 KB |        6.33 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  13.962 ms |  4.11 | 29 |  2090.0 |  279.2 |  88608.25 KB |   78,152.32 |
| CsvHelper | .NET 7.0 |  Cols | 50000 |  79.138 ms | 23.81 | 29 |   368.7 | 1582.8 |    446.31 KB |      393.65 |
| Sep______ | .NET 8.0 |  Cols | 50000 |   3.188 ms |  0.96 | 29 |  9153.5 |   63.8 |      1.13 KB |        1.00 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |   5.598 ms |  1.68 | 29 |  5213.0 |  112.0 |      7.17 KB |        6.33 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  12.944 ms |  3.86 | 29 |  2254.4 |  258.9 |  88608.24 KB |   78,152.32 |
| CsvHelper | .NET 8.0 |  Cols | 50000 |  71.520 ms | 21.27 | 29 |   408.0 | 1430.4 |    446.29 KB |      393.62 |
|           |          |       |       |            |       |    |         |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  30.729 ms |  1.00 | 29 |   949.6 |  614.6 |  13799.67 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 |  42.129 ms |  1.37 | 29 |   692.7 |  842.6 |  14025.03 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 115.299 ms |  3.75 | 29 |   253.1 | 2306.0 | 102133.39 KB |        7.40 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 104.802 ms |  3.41 | 29 |   278.4 | 2096.0 |  13972.08 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  30.395 ms |  0.99 | 29 |   960.1 |  607.9 |  13799.62 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  38.198 ms |  1.24 | 29 |   764.0 |  764.0 |  14026.64 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 109.821 ms |  3.53 | 29 |   265.7 | 2196.4 | 102133.38 KB |        7.40 |
| CsvHelper | .NET 8.0 | Asset | 50000 |  85.305 ms |  2.78 | 29 |   342.1 | 1706.1 |  13972.36 KB |        1.01 |
