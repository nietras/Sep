``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK=8.0.100-preview.7.23376.3
  [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
  Job-UHOMSO : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2
  Job-SRTOSO : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2

InvocationCount=Default  IterationTime=300.0000 ms  MaxIterationCount=Default  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
|    Method |  Runtime | Scope |  Rows |      Mean | Ratio | MB |   MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |  15.13 ms |  1.00 | 33 | 2206.5 |  302.5 |      1.17 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |  42.25 ms |  2.80 | 33 |  790.0 |  845.0 |      7.26 KB |        6.22 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  32.95 ms |  2.18 | 33 | 1013.0 |  659.0 | 108778.78 KB |   93,135.01 |
| CsvHelper | .NET 7.0 |   Row | 50000 | 121.55 ms |  8.03 | 33 |  274.6 | 2431.1 |     20.84 KB |       17.84 |
| Sep______ | .NET 8.0 |   Row | 50000 |  12.34 ms |  0.82 | 33 | 2705.0 |  246.8 |      1.31 KB |        1.12 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |  35.42 ms |  2.34 | 33 |  942.4 |  708.4 |      7.24 KB |        6.20 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  32.15 ms |  2.13 | 33 | 1038.3 |  642.9 | 108778.77 KB |   93,135.00 |
| CsvHelper | .NET 8.0 |   Row | 50000 | 104.02 ms |  6.88 | 33 |  320.9 | 2080.4 |     20.69 KB |       17.72 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |  15.80 ms |  1.00 | 33 | 2112.6 |  316.0 |      1.18 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |  45.83 ms |  2.91 | 33 |  728.3 |  916.6 |      7.33 KB |        6.24 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  33.98 ms |  2.15 | 33 |  982.2 |  679.7 | 108778.78 KB |   92,516.17 |
| CsvHelper | .NET 7.0 |  Cols | 50000 | 177.61 ms | 11.23 | 33 |  187.9 | 3552.2 |    446.43 KB |      379.69 |
| Sep______ | .NET 8.0 |  Cols | 50000 |  15.59 ms |  0.99 | 33 | 2140.6 |  311.8 |       1.5 KB |        1.28 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |  38.85 ms |  2.46 | 33 |  859.2 |  777.0 |      7.25 KB |        6.16 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  32.64 ms |  2.06 | 33 | 1022.6 |  652.8 | 108778.77 KB |   92,516.16 |
| CsvHelper | .NET 8.0 |  Cols | 50000 | 161.84 ms | 10.23 | 33 |  206.2 | 3236.9 |    446.45 KB |      379.70 |
|           |          |       |       |           |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  71.19 ms |  1.00 | 33 |  468.9 | 1423.8 |  13808.03 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 | 106.82 ms |  1.50 | 33 |  312.5 | 2136.5 |  14025.33 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 205.79 ms |  2.89 | 33 |  162.2 | 4115.9 | 122304.11 KB |        8.86 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 192.54 ms |  2.71 | 33 |  173.3 | 3850.9 |  13970.85 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  52.18 ms |  0.74 | 33 |  639.6 | 1043.7 |  13808.73 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  96.05 ms |  1.35 | 33 |  347.5 | 1921.0 |  14026.75 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 202.80 ms |  2.82 | 33 |  164.6 | 4056.0 | 122304.21 KB |        8.86 |
| CsvHelper | .NET 8.0 | Asset | 50000 | 179.61 ms |  2.52 | 33 |  185.8 | 3592.1 |   13970.8 KB |        1.01 |
