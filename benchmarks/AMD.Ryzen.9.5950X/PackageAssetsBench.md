```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-PQAZLF : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-PQAZLF  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     2.319 ms |  1.00 |  29 | 12581.3 |   46.4 |        967 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.335 ms |  1.01 |  29 | 12497.4 |   46.7 |        967 B |        1.00 |
| Sylvan___    | Row   | 50000   |     2.985 ms |  1.29 |  29 |  9776.4 |   59.7 |       7381 B |        7.63 |
| ReadLine_    | Row   | 50000   |    12.779 ms |  5.55 |  29 |  2283.6 |  255.6 |   90734838 B |   93,831.27 |
| CsvHelper    | Row   | 50000   |    43.702 ms | 18.84 |  29 |   667.7 |  874.0 |      21074 B |       21.79 |
|              |       |         |              |       |     |         |        |              |             |
| Sep______    | Cols  | 50000   |     3.127 ms |  1.00 |  29 |  9331.2 |   62.5 |        970 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     3.757 ms |  1.20 |  29 |  7766.9 |   75.1 |        972 B |        1.00 |
| Sylvan___    | Cols  | 50000   |     5.353 ms |  1.71 |  29 |  5451.1 |  107.1 |       7385 B |        7.61 |
| ReadLine_    | Cols  | 50000   |    12.753 ms |  4.07 |  29 |  2288.1 |  255.1 |   90734839 B |   93,541.07 |
| CsvHelper    | Cols  | 50000   |    69.377 ms | 22.20 |  29 |   420.6 | 1387.5 |     457060 B |      471.20 |
|              |       |         |              |       |     |         |        |              |             |
| Sep______    | Asset | 50000   |    35.112 ms |  1.00 |  29 |   831.1 |  702.2 |   14133938 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.928 ms |  0.66 |  29 |  1272.8 |  458.6 |   14327404 B |        1.01 |
| Sylvan___    | Asset | 50000   |    39.254 ms |  1.11 |  29 |   743.4 |  785.1 |   14297119 B |        1.01 |
| ReadLine_    | Asset | 50000   |   106.311 ms |  3.04 |  29 |   274.5 | 2126.2 |  104584704 B |        7.40 |
| CsvHelper    | Asset | 50000   |    83.844 ms |  2.39 |  29 |   348.0 | 1676.9 |   14306424 B |        1.01 |
|              |       |         |              |       |     |         |        |              |             |
| Sep______    | Asset | 1000000 |   658.125 ms |  1.00 | 583 |   887.0 |  658.1 |  273067600 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   305.092 ms |  0.46 | 583 |  1913.5 |  305.1 |  274288576 B |        1.00 |
| Sylvan___    | Asset | 1000000 |   794.169 ms |  1.21 | 583 |   735.1 |  794.2 |  273227616 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,842.849 ms |  2.80 | 583 |   316.8 | 1842.8 | 2087764672 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,740.008 ms |  2.64 | 583 |   335.5 | 1740.0 |  273238000 B |        1.00 |
