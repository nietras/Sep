```

BenchmarkDotNet v0.13.10, Ubuntu 22.04.2 LTS (Jammy Jellyfish)
Unknown processor
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  Job-HCCPYN : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD

Job=Job-HCCPYN  Runtime=.NET 8.0  Toolchain=net80  
InvocationCount=Default  IterationTime=350.0000 ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Quotes=False  
Reader=String  

```
| Method       | Scope | Rows  | Mean      | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |------ |----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000 |  12.56 ms |  1.00 | 29 | 2315.4 |  251.2 |       904 B |        1.00 |
| Sep_Unescape | Row   | 50000 |  12.46 ms |  0.99 | 29 | 2334.2 |  249.2 |       902 B |        1.00 |
| Sylvan___    | Row   | 50000 |  31.26 ms |  2.49 | 29 |  930.4 |  625.2 |      6269 B |        6.93 |
| ReadLine_    | Row   | 50000 |  38.28 ms |  3.05 | 29 |  759.8 |  765.6 |  90734916 B |  100,370.48 |
| CsvHelper    | Row   | 50000 |  91.28 ms |  7.27 | 29 |  318.6 | 1825.6 |     21272 B |       23.53 |
|              |       |       |           |       |    |        |        |             |             |
| Sep______    | Cols  | 50000 |  14.96 ms |  1.00 | 29 | 1944.2 |  299.2 |       913 B |        1.00 |
| Sep_Unescape | Cols  | 50000 |  15.70 ms |  1.05 | 29 | 1853.0 |  313.9 |       915 B |        1.00 |
| Sylvan___    | Cols  | 50000 |  35.36 ms |  2.36 | 29 |  822.6 |  707.2 |      6288 B |        6.89 |
| ReadLine_    | Cols  | 50000 |  40.74 ms |  2.72 | 29 |  713.9 |  814.9 |  90734929 B |   99,381.08 |
| CsvHelper    | Cols  | 50000 | 136.92 ms |  9.15 | 29 |  212.4 | 2738.3 |    457144 B |      500.71 |
|              |       |       |           |       |    |        |        |             |             |
| Sep______    | Asset | 50000 |  67.27 ms |  1.00 | 29 |  432.4 | 1345.4 |  14131346 B |        1.00 |
| Sep_Unescape | Asset | 50000 |  67.01 ms |  1.00 | 29 |  434.1 | 1340.2 |  14131792 B |        1.00 |
| Sylvan___    | Asset | 50000 | 100.28 ms |  1.49 | 29 |  290.0 | 2005.7 |  14295848 B |        1.01 |
| ReadLine_    | Asset | 50000 | 152.69 ms |  2.27 | 29 |  190.5 | 3053.8 | 104585572 B |        7.40 |
| CsvHelper    | Asset | 50000 | 166.75 ms |  2.46 | 29 |  174.4 | 3334.9 |  14306628 B |        1.01 |
