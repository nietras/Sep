```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.4 (23H420) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD
  Job-HYWXRS : .NET 9.0.4 (9.0.425.16305), Arm64 RyuJIT AdvSIMD

Job=Job-HYWXRS  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.172 ms |  1.00 |  29 | 6971.8 |   83.4 |        989 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.875 ms |  0.93 |  29 | 7505.5 |   77.5 |       1030 B |        1.04 |
| Sep_Unescape | Row   | 50000   |     3.776 ms |  0.91 |  29 | 7702.2 |   75.5 |        987 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.368 ms |  4.64 |  29 | 1501.8 |  387.4 |       6958 B |        7.04 |
| ReadLine_    | Row   | 50000   |    18.163 ms |  4.35 |  29 | 1601.4 |  363.3 |   90734887 B |   91,744.07 |
| CsvHelper    | Row   | 50000   |    42.467 ms | 10.18 |  29 |  684.9 |  849.3 |      20764 B |       20.99 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.296 ms |  1.00 |  29 | 5491.7 |  105.9 |        994 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.612 ms |  1.06 |  29 | 5182.3 |  112.2 |        995 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    22.012 ms |  4.16 |  29 | 1321.3 |  440.2 |       6725 B |        6.77 |
| ReadLine_    | Cols  | 50000   |    20.804 ms |  3.93 |  29 | 1398.1 |  416.1 |   90734891 B |   91,282.59 |
| CsvHelper    | Cols  | 50000   |    66.289 ms | 12.52 |  29 |  438.8 | 1325.8 |     457440 B |      460.20 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.140 ms |  1.01 |  29 |  852.0 |  682.8 |   14134048 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.616 ms |  0.64 |  29 | 1345.6 |  432.3 |   14225818 B |        1.01 |
| Sylvan___    | Asset | 50000   |    53.141 ms |  1.56 |  29 |  547.3 | 1062.8 |   14296896 B |        1.01 |
| ReadLine_    | Asset | 50000   |    86.283 ms |  2.54 |  29 |  337.1 | 1725.7 |  104585846 B |        7.40 |
| CsvHelper    | Asset | 50000   |    75.267 ms |  2.22 |  29 |  386.4 | 1505.3 |   14306376 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   593.732 ms |  1.00 | 581 |  980.0 |  593.7 |  273070232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   440.534 ms |  0.74 | 581 | 1320.8 |  440.5 |  282222496 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,122.696 ms |  1.89 | 581 |  518.3 | 1122.7 |  273232584 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,988.487 ms |  3.35 | 581 |  292.6 | 1988.5 | 2087769648 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,589.624 ms |  2.68 | 581 |  366.0 | 1589.6 |  273239384 B |        1.00 |
