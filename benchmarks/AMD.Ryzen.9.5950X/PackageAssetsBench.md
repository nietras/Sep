```

BenchmarkDotNet v0.13.11, Windows 10 (10.0.19044.3086/21H2/November2021Update)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  Job-SVHPFG : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=Job-SVHPFG  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method           | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated    | Alloc Ratio |
|----------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|-------------:|------------:|
| Sep______        | Row   | 50000   |     2.305 ms |  1.00 |  29 | 12658.6 |   46.1 |        967 B |        1.00 |
| Sep_Unescape     | Row   | 50000   |     2.345 ms |  1.02 |  29 | 12444.9 |   46.9 |        967 B |        1.00 |
| Sylvan___        | Row   | 50000   |     2.939 ms |  1.27 |  29 |  9929.2 |   58.8 |       7381 B |        7.63 |
| ReadLine_        | Row   | 50000   |    12.693 ms |  5.55 |  29 |  2298.9 |  253.9 |   90734838 B |   93,831.27 |
| CsvHelper        | Row   | 50000   |    42.865 ms | 18.59 |  29 |   680.8 |  857.3 |      21074 B |       21.79 |
|                  |       |         |              |       |     |         |        |              |             |
| Sep______        | Cols  | 50000   |     3.139 ms |  1.00 |  29 |  9295.1 |   62.8 |        970 B |        1.00 |
| Sep_Unescape     | Cols  | 50000   |     3.746 ms |  1.19 |  29 |  7789.7 |   74.9 |        972 B |        1.00 |
| Sylvan___        | Cols  | 50000   |     5.206 ms |  1.66 |  29 |  5605.1 |  104.1 |       7385 B |        7.61 |
| ReadLine_        | Cols  | 50000   |    12.748 ms |  4.04 |  29 |  2289.1 |  255.0 |   90734839 B |   93,541.07 |
| CsvHelper        | Cols  | 50000   |    69.417 ms | 22.11 |  29 |   420.4 | 1388.3 |     457060 B |      471.20 |
|                  |       |         |              |       |     |         |        |              |             |
| Sep______        | Asset | 1000000 |   654.674 ms |  1.00 | 583 |   891.7 |  654.7 |  273067448 B |        1.00 |
| Sep_Unescape     | Asset | 1000000 |   654.754 ms |  1.00 | 583 |   891.6 |  654.8 |  273067376 B |        1.00 |
| Sep__MT__        | Asset | 1000000 |   304.340 ms |  0.47 | 583 |  1918.2 |  304.3 |  276207048 B |        1.01 |
| Sep__MT_Unescape | Asset | 1000000 |   320.516 ms |  0.49 | 583 |  1821.4 |  320.5 |  274088768 B |        1.00 |
| Sylvan___        | Asset | 1000000 |   794.811 ms |  1.23 | 583 |   734.5 |  794.8 |  273227024 B |        1.00 |
| ReadLine_        | Asset | 1000000 | 1,907.330 ms |  2.91 | 583 |   306.1 | 1907.3 | 2087765152 B |        7.65 |
| CsvHelper        | Asset | 1000000 | 1,716.730 ms |  2.62 | 583 |   340.1 | 1716.7 |  273236328 B |        1.00 |
