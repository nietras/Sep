```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-ILBOFO : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-ILBOFO  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.782 ms |  1.00 |  29 | 7689.6 |   75.6 |        987 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.813 ms |  1.27 |  29 | 6042.7 |   96.3 |       1041 B |        1.05 |
| Sep_Unescape | Row   | 50000   |     4.160 ms |  1.10 |  29 | 6991.5 |   83.2 |        989 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.366 ms |  5.12 |  29 | 1501.9 |  387.3 |       6958 B |        7.05 |
| ReadLine_    | Row   | 50000   |    18.281 ms |  4.83 |  29 | 1591.0 |  365.6 |   90734887 B |   91,929.98 |
| CsvHelper    | Row   | 50000   |    42.667 ms | 11.28 |  29 |  681.7 |  853.3 |      20764 B |       21.04 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.945 ms |  1.00 |  29 | 5882.5 |   98.9 |        992 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.853 ms |  1.18 |  29 | 4969.3 |  117.1 |        998 B |        1.01 |
| Sylvan___    | Cols  | 50000   |    22.830 ms |  4.62 |  29 | 1274.0 |  456.6 |       6958 B |        7.01 |
| ReadLine_    | Cols  | 50000   |    20.969 ms |  4.24 |  29 | 1387.1 |  419.4 |   90734895 B |   91,466.63 |
| CsvHelper    | Cols  | 50000   |    69.199 ms | 14.00 |  29 |  420.3 | 1384.0 |     457440 B |      461.13 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    34.958 ms |  1.01 |  29 |  832.0 |  699.2 |   14134090 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.794 ms |  0.63 |  29 | 1334.6 |  435.9 |   14220783 B |        1.01 |
| Sylvan___    | Asset | 50000   |    54.412 ms |  1.56 |  29 |  534.6 | 1088.2 |   14296672 B |        1.01 |
| ReadLine_    | Asset | 50000   |    92.796 ms |  2.67 |  29 |  313.4 | 1855.9 |  104585906 B |        7.40 |
| CsvHelper    | Asset | 50000   |    82.754 ms |  2.38 |  29 |  351.5 | 1655.1 |   14306376 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   609.259 ms |  1.00 | 581 |  955.1 |  609.3 |  273070232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   457.152 ms |  0.75 | 581 | 1272.8 |  457.2 |  281202328 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,075.979 ms |  1.77 | 581 |  540.8 | 1076.0 |  273228808 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,758.357 ms |  2.89 | 581 |  330.9 | 1758.4 | 2087769472 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,584.297 ms |  2.60 | 581 |  367.3 | 1584.3 |  273242312 B |        1.00 |
