``` ini

BenchmarkDotNet=v0.13.5, OS=ubuntu 22.04
Unknown processor
.NET SDK=8.0.100-preview.7.23376.3
  [Host]     : .NET 8.0.0 (8.0.23.37506), Arm64 RyuJIT AdvSIMD
  Job-GDSTLJ : .NET 7.0.10 (7.0.1023.36801), Arm64 RyuJIT AdvSIMD
  Job-WFSLTV : .NET 8.0.0 (8.0.23.37506), Arm64 RyuJIT AdvSIMD

InvocationCount=Default  IterationTime=300.0000 ms  MaxIterationCount=Default  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
|    Method |  Runtime | Scope |  Rows |      Mean | Ratio | MB |   MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |  25.47 ms |  1.00 | 33 | 1306.5 |  509.5 |      1.39 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |  46.21 ms |  1.81 | 33 |  720.2 |  924.2 |      6.25 KB |        4.50 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  49.46 ms |  1.94 | 33 |  673.0 |  989.1 | 108778.86 KB |   78,333.02 |
| CsvHelper | .NET 7.0 |   Row | 50000 | 121.28 ms |  4.76 | 33 |  274.4 | 2425.6 |     20.74 KB |       14.93 |
| Sep______ | .NET 8.0 |   Row | 50000 |  23.43 ms |  0.92 | 33 | 1420.5 |  468.6 |      1.36 KB |        0.98 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |  39.42 ms |  1.55 | 33 |  844.2 |  788.5 |      6.11 KB |        4.40 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  47.68 ms |  1.87 | 33 |  698.1 |  953.5 | 108778.91 KB |   78,333.05 |
| CsvHelper | .NET 8.0 |   Row | 50000 | 107.00 ms |  4.20 | 33 |  311.0 | 2140.1 |     20.77 KB |       14.96 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |  28.30 ms |  1.00 | 33 | 1175.9 |  566.1 |      1.39 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |  53.39 ms |  1.89 | 33 |  623.4 | 1067.8 |      6.25 KB |        4.50 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  50.71 ms |  1.79 | 33 |  656.3 | 1014.2 | 108778.93 KB |   78,333.07 |
| CsvHelper | .NET 7.0 |  Cols | 50000 | 180.08 ms |  6.36 | 33 |  184.8 | 3601.5 |    446.66 KB |      321.65 |
| Sep______ | .NET 8.0 |  Cols | 50000 |  25.68 ms |  0.91 | 33 | 1295.8 |  513.7 |      1.36 KB |        0.98 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |  45.33 ms |  1.61 | 33 |  734.2 |  906.6 |      6.15 KB |        4.43 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  44.96 ms |  1.59 | 33 |  740.3 |  899.2 | 108778.91 KB |   78,333.05 |
| CsvHelper | .NET 8.0 |  Cols | 50000 | 153.04 ms |  5.41 | 33 |  217.5 | 3060.7 |    446.61 KB |      321.61 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  88.20 ms |  1.00 | 33 |  377.4 | 1764.0 |  13808.62 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 | 115.22 ms |  1.31 | 33 |  288.9 | 2304.4 |  14024.52 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 200.14 ms |  2.28 | 33 |  166.3 | 4002.9 | 122305.55 KB |        8.86 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 208.19 ms |  2.35 | 33 |  159.9 | 4163.8 |  13971.42 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  84.89 ms |  0.96 | 33 |  392.1 | 1697.7 |  13808.47 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 | 109.42 ms |  1.24 | 33 |  304.2 | 2188.4 |   14025.4 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 190.83 ms |  2.19 | 33 |  174.4 | 3816.6 | 122305.06 KB |        8.86 |
| CsvHelper | .NET 8.0 | Asset | 50000 | 185.03 ms |  2.09 | 33 |  179.9 | 3700.5 |  13972.06 KB |        1.01 |
