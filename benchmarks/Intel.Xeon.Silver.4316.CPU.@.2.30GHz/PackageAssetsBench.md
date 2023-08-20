``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.17763.3287/1809/October2018Update/Redstone5)
Intel Xeon Silver 4316 CPU 2.30GHz, 1 CPU, 40 logical and 20 physical cores
.NET SDK=8.0.100-preview.7.23376.3
  [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
  Job-UHOMSO : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2
  Job-SRTOSO : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2

InvocationCount=Default  IterationTime=300.0000 ms  MaxIterationCount=Default  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
|    Method |  Runtime | Scope |  Rows |       Mean | Ratio | MB |   MB/s | ns/row |    Allocated | Alloc Ratio |
|---------- |--------- |------ |------ |-----------:|------:|---:|-------:|-------:|-------------:|------------:|
| Sep______ | .NET 7.0 |   Row | 50000 |   5.948 ms |  1.00 | 29 | 4905.8 |  119.0 |      1.14 KB |        1.00 |
| Sylvan___ | .NET 7.0 |   Row | 50000 |   6.733 ms |  1.13 | 29 | 4334.0 |  134.7 |      7.18 KB |        6.29 |
| ReadLine_ | .NET 7.0 |   Row | 50000 |  27.988 ms |  4.70 | 29 | 1042.6 |  559.8 |  88608.29 KB |   77,617.53 |
| CsvHelper | .NET 7.0 |   Row | 50000 | 114.902 ms | 19.31 | 29 |  254.0 | 2298.0 |     20.65 KB |       18.09 |
| Sep______ | .NET 8.0 |   Row | 50000 |   5.440 ms |  0.91 | 29 | 5363.8 |  108.8 |      1.29 KB |        1.13 |
| Sylvan___ | .NET 8.0 |   Row | 50000 |   6.383 ms |  1.07 | 29 | 4571.5 |  127.7 |      7.18 KB |        6.29 |
| ReadLine_ | .NET 8.0 |   Row | 50000 |  26.933 ms |  4.53 | 29 | 1083.5 |  538.7 |  88608.26 KB |   77,617.50 |
| CsvHelper | .NET 8.0 |   Row | 50000 |  90.332 ms | 15.18 | 29 |  323.0 | 1806.6 |     20.69 KB |       18.12 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 |  Cols | 50000 |   7.401 ms |  1.00 | 29 | 3942.7 |  148.0 |      1.15 KB |        1.00 |
| Sylvan___ | .NET 7.0 |  Cols | 50000 |  11.545 ms |  1.56 | 29 | 2527.5 |  230.9 |      7.19 KB |        6.28 |
| ReadLine_ | .NET 7.0 |  Cols | 50000 |  28.892 ms |  3.90 | 29 | 1010.0 |  577.8 |  88608.29 KB |   77,352.84 |
| CsvHelper | .NET 7.0 |  Cols | 50000 | 157.705 ms | 21.31 | 29 |  185.0 | 3154.1 |     446.5 KB |      389.78 |
| Sep______ | .NET 8.0 |  Cols | 50000 |   6.683 ms |  0.90 | 29 | 4366.8 |  133.7 |       1.3 KB |        1.13 |
| Sylvan___ | .NET 8.0 |  Cols | 50000 |  10.417 ms |  1.41 | 29 | 2801.2 |  208.3 |      7.19 KB |        6.28 |
| ReadLine_ | .NET 8.0 |  Cols | 50000 |  27.292 ms |  3.69 | 29 | 1069.2 |  545.8 |  88608.26 KB |   77,352.82 |
| CsvHelper | .NET 8.0 |  Cols | 50000 | 141.559 ms | 19.11 | 29 |  206.1 | 2831.2 |    446.45 KB |      389.74 |
|           |          |       |       |            |       |    |        |        |              |             |
| Sep______ | .NET 7.0 | Asset | 50000 |  58.708 ms |  1.00 | 29 |  497.1 | 1174.2 |  13799.65 KB |        1.00 |
| Sylvan___ | .NET 7.0 | Asset | 50000 |  76.436 ms |  1.31 | 29 |  381.8 | 1528.7 |  14025.71 KB |        1.02 |
| ReadLine_ | .NET 7.0 | Asset | 50000 | 174.574 ms |  3.00 | 29 |  167.2 | 3491.5 | 102133.35 KB |        7.40 |
| CsvHelper | .NET 7.0 | Asset | 50000 | 172.747 ms |  2.95 | 29 |  168.9 | 3454.9 |  13970.85 KB |        1.01 |
| Sep______ | .NET 8.0 | Asset | 50000 |  55.672 ms |  0.95 | 29 |  524.2 | 1113.4 |  13800.53 KB |        1.00 |
| Sylvan___ | .NET 8.0 | Asset | 50000 |  71.428 ms |  1.22 | 29 |  408.5 | 1428.6 |  14026.38 KB |        1.02 |
| ReadLine_ | .NET 8.0 | Asset | 50000 | 162.203 ms |  2.76 | 29 |  179.9 | 3244.1 | 102133.69 KB |        7.40 |
| CsvHelper | .NET 8.0 | Asset | 50000 | 161.805 ms |  2.76 | 29 |  180.3 | 3236.1 |   13970.8 KB |        1.01 |
