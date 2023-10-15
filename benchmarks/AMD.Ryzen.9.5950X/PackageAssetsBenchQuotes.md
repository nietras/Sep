```

BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100-rc.2.23502.2
  [Host]     : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2
  Job-AFDVVY : .NET 7.0.12 (7.0.1223.47720), X64 RyuJIT AVX2
  Job-RGAYEX : .NET 8.0.0 (8.0.23.47906), X64 RyuJIT AVX2

InvocationCount=Default  IterationTime=300.0000 ms  MaxIterationCount=Default  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method    | Runtime  | Scope | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated    | Alloc Ratio |
|---------- |--------- |------ |------ |-----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 | Row   | 50000 |   7.610 ms |  1.00 | 33 | 4386.3 |  152.2 |      1.16 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Row   | 50000 |  21.789 ms |  2.86 | 33 | 1531.8 |  435.8 |      7.33 KB |        6.32 |
| ReadLine_ | .NET 7.0 | Row   | 50000 |  16.414 ms |  2.17 | 33 | 2033.5 |  328.3 | 108778.76 KB |   93,762.16 |
| CsvHelper | .NET 7.0 | Row   | 50000 |  69.513 ms |  9.08 | 33 |  480.2 | 1390.3 |     20.65 KB |       17.80 |
| Sep______ | .NET 8.0 | Row   | 50000 |   6.902 ms |  0.91 | 33 | 4835.9 |  138.0 |      1.16 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Row   | 50000 |  18.241 ms |  2.40 | 33 | 1829.8 |  364.8 |       7.2 KB |        6.21 |
| ReadLine_ | .NET 8.0 | Row   | 50000 |  15.438 ms |  2.04 | 33 | 2162.0 |  308.8 | 108778.75 KB |   93,762.16 |
| CsvHelper | .NET 8.0 | Row   | 50000 |  55.032 ms |  7.23 | 33 |  606.5 | 1100.6 |      20.6 KB |       17.75 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Cols  | 50000 |   8.236 ms |  1.00 | 33 | 4052.7 |  164.7 |      1.16 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Cols  | 50000 |  25.537 ms |  3.11 | 33 | 1307.0 |  510.7 |      7.33 KB |        6.33 |
| ReadLine_ | .NET 7.0 | Cols  | 50000 |  17.572 ms |  2.08 | 33 | 1899.5 |  351.4 | 108778.76 KB |   93,920.28 |
| CsvHelper | .NET 7.0 | Cols  | 50000 |  97.839 ms | 11.89 | 33 |  341.1 | 1956.8 |    446.31 KB |      385.35 |
| Sep______ | .NET 8.0 | Cols  | 50000 |   7.391 ms |  0.90 | 33 | 4516.1 |  147.8 |      1.16 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Cols  | 50000 |  20.925 ms |  2.54 | 33 | 1595.1 |  418.5 |      7.21 KB |        6.22 |
| ReadLine_ | .NET 8.0 | Cols  | 50000 |  15.597 ms |  1.90 | 33 | 2140.0 |  311.9 | 108778.75 KB |   93,920.27 |
| CsvHelper | .NET 8.0 | Cols  | 50000 |  84.788 ms | 10.31 | 33 |  393.7 | 1695.8 |    446.35 KB |      385.38 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  39.583 ms |  1.00 | 33 |  843.2 |  791.7 |  13808.04 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 |  61.288 ms |  1.55 | 33 |  544.6 | 1225.8 |  14024.99 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 137.987 ms |  3.47 | 33 |  241.9 | 2759.7 | 122304.23 KB |        8.86 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 108.892 ms |  2.78 | 33 |  306.5 | 2177.8 |  13973.81 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  36.736 ms |  0.95 | 33 |  908.6 |  734.7 |  13808.03 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  54.075 ms |  1.37 | 33 |  617.2 | 1081.5 |  14026.13 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 132.839 ms |  3.35 | 33 |  251.3 | 2656.8 | 122303.78 KB |        8.86 |
| CsvHelper | .NET 8.0 | Asset | 50000 |  99.443 ms |  2.54 | 33 |  335.6 | 1988.9 |  13972.48 KB |        1.01 |
