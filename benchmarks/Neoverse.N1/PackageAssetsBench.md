``` ini

BenchmarkDotNet=v0.13.5, OS=ubuntu 22.04
Unknown processor
.NET SDK=8.0.100-preview.7.23376.3
  [Host]     : .NET 8.0.0 (8.0.23.37506), Arm64 RyuJIT AdvSIMD
  Job-GDSTLJ : .NET 7.0.10 (7.0.1023.36801), Arm64 RyuJIT AdvSIMD
  Job-WFSLTV : .NET 8.0.0 (8.0.23.37506), Arm64 RyuJIT AdvSIMD

InvocationCount=Default  IterationTime=300.0000 ms  MaxIterationCount=Default  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
|    Method |  Runtime | Scope |  Rows |      Mean | Ratio | MB |   MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |  12.15 ms |  1.00 | 29 | 2393.9 |  243.0 |      1.11 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |  34.31 ms |  2.82 | 29 |  847.7 |  686.2 |      6.25 KB |        5.64 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  40.99 ms |  3.37 | 29 |  709.6 |  819.8 |  88608.34 KB |   79,942.68 |
| CsvHelper | .NET 7.0 |   Row | 50000 | 107.97 ms |  8.89 | 29 |  269.4 | 2159.4 |     20.74 KB |       18.71 |
| Sep______ | .NET 8.0 |   Row | 50000 |  12.01 ms |  0.99 | 29 | 2421.2 |  240.3 |       1.1 KB |        1.00 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |  30.14 ms |  2.48 | 29 |  965.0 |  602.8 |      6.09 KB |        5.50 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  39.05 ms |  3.21 | 29 |  744.9 |  780.9 |  88608.41 KB |   79,942.74 |
| CsvHelper | .NET 8.0 |   Row | 50000 |  91.57 ms |  7.54 | 29 |  317.6 | 1831.5 |     20.77 KB |       18.74 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |  14.52 ms |  1.00 | 29 | 2002.8 |  290.5 |      1.12 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |  40.36 ms |  2.78 | 29 |  720.6 |  807.2 |      6.25 KB |        5.57 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  41.92 ms |  2.89 | 29 |  693.8 |  838.5 |  88608.36 KB |   78,899.97 |
| CsvHelper | .NET 7.0 |  Cols | 50000 | 156.03 ms | 10.74 | 29 |  186.4 | 3120.6 |    446.66 KB |      397.72 |
| Sep______ | .NET 8.0 |  Cols | 50000 |  14.24 ms |  0.98 | 29 | 2042.8 |  284.8 |      1.11 KB |        0.99 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |  35.23 ms |  2.43 | 29 |  825.6 |  704.6 |      6.11 KB |        5.44 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  37.10 ms |  2.55 | 29 |  784.0 |  742.0 |  88608.32 KB |   78,899.93 |
| CsvHelper | .NET 8.0 |  Cols | 50000 | 138.20 ms |  9.52 | 29 |  210.5 | 2763.9 |    446.61 KB |      397.68 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  70.16 ms |  1.00 | 29 |  414.5 | 1403.3 |  13800.75 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 | 107.28 ms |  1.53 | 29 |  271.1 | 2145.6 |  14025.09 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 159.87 ms |  2.27 | 29 |  181.9 | 3197.4 | 102134.11 KB |        7.40 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 189.44 ms |  2.69 | 29 |  153.5 | 3788.8 |  13973.66 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  67.72 ms |  0.96 | 29 |  429.5 | 1354.3 |  13800.73 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 | 100.09 ms |  1.43 | 29 |  290.6 | 2001.8 |  14024.23 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 144.19 ms |  2.06 | 29 |  201.7 | 2883.8 | 102134.74 KB |        7.40 |
| CsvHelper | .NET 8.0 | Asset | 50000 | 170.81 ms |  2.43 | 29 |  170.3 | 3416.1 |  13971.32 KB |        1.01 |
