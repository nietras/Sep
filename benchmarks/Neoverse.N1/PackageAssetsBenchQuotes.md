```

BenchmarkDotNet v0.13.10, Ubuntu 22.04.2 LTS (Jammy Jellyfish)
Unknown processor
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  Job-HCCPYN : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Job=Job-HCCPYN  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=True  
Reader=String  

```
| Method       | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |  24.35 ms |  1.00 | 33 | 1366.9 |  487.0 |       941 B |        1.00 |
| Sep_Unescape | Row   | 50000 |  24.21 ms |  0.99 | 33 | 1374.7 |  484.2 |       941 B |        1.00 |
| Sylvan___    | Row   | 50000 |  39.92 ms |  1.64 | 33 |  833.6 |  798.5 |      6288 B |        6.68 |
| ReadLine_    | Row   | 50000 |  45.96 ms |  1.89 | 33 |  724.1 |  919.3 | 111389508 B |  118,373.55 |
| CsvHelper    | Row   | 50000 | 106.99 ms |  4.40 | 33 |  311.1 | 2139.8 |     21272 B |       22.61 |
|              |       |       |           |       |    |        |        |             |             |
| Sep______    | Cols  | 50000 |  26.96 ms |  1.00 | 33 | 1234.3 |  539.3 |       869 B |        1.00 |
| Sep_Unescape | Cols  | 50000 |  28.60 ms |  1.06 | 33 | 1163.6 |  572.1 |       953 B |        1.10 |
| Sylvan___    | Cols  | 50000 |  45.48 ms |  1.69 | 33 |  731.8 |  909.6 |      6318 B |        7.27 |
| ReadLine_    | Cols  | 50000 |  45.95 ms |  1.70 | 33 |  724.4 |  918.9 | 111389521 B |  128,181.27 |
| CsvHelper    | Cols  | 50000 | 152.46 ms |  5.65 | 33 |  218.3 | 3049.1 |    457328 B |      526.27 |
|              |       |       |           |       |    |        |        |             |             |
| Sep______    | Asset | 50000 |  83.86 ms |  1.00 | 33 |  396.9 | 1677.2 |  14139912 B |        1.00 |
| Sep_Unescape | Asset | 50000 |  79.99 ms |  0.96 | 33 |  416.1 | 1599.9 |  14132628 B |        1.00 |
| Sylvan___    | Asset | 50000 | 110.29 ms |  1.31 | 33 |  301.8 | 2205.9 |  14298320 B |        1.01 |
| ReadLine_    | Asset | 50000 | 178.89 ms |  2.13 | 33 |  186.1 | 3577.7 | 125240480 B |        8.86 |
| CsvHelper    | Asset | 50000 | 182.59 ms |  2.18 | 33 |  182.3 | 3651.7 |  14308800 B |        1.01 |
