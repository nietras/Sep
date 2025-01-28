```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-KNSVKR : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

Job=Job-KNSVKR  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.041 ms |  1.00 |  29 | 7196.8 |   80.8 |       1033 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.385 ms |  1.09 |  29 | 6633.5 |   87.7 |        990 B |        0.96 |
| Sep_Unescape | Row   | 50000   |     4.449 ms |  1.10 |  29 | 6537.7 |   89.0 |        990 B |        0.96 |
| Sylvan___    | Row   | 50000   |    21.045 ms |  5.22 |  29 | 1382.1 |  420.9 |       6958 B |        6.74 |
| ReadLine_    | Row   | 50000   |    21.449 ms |  5.32 |  29 | 1356.1 |  429.0 |   90734895 B |   87,836.30 |
| CsvHelper    | Row   | 50000   |    46.465 ms | 11.52 |  29 |  626.0 |  929.3 |      20692 B |       20.03 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.001 ms |  1.00 |  29 | 5816.4 |  100.0 |        994 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.269 ms |  1.25 |  29 | 4639.4 |  125.4 |        999 B |        1.01 |
| Sylvan___    | Cols  | 50000   |    23.746 ms |  4.75 |  29 | 1224.9 |  474.9 |       6958 B |        7.00 |
| ReadLine_    | Cols  | 50000   |    21.710 ms |  4.34 |  29 | 1339.7 |  434.2 |   90734901 B |   91,282.60 |
| CsvHelper    | Cols  | 50000   |    66.705 ms | 13.34 |  29 |  436.0 | 1334.1 |     457440 B |      460.20 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    33.390 ms |  1.00 |  29 |  871.1 |  667.8 |   14134046 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.413 ms |  0.67 |  29 | 1297.7 |  448.3 |   14280628 B |        1.01 |
| Sylvan___    | Asset | 50000   |    53.205 ms |  1.60 |  29 |  546.7 | 1064.1 |   14296832 B |        1.01 |
| ReadLine_    | Asset | 50000   |   109.717 ms |  3.30 |  29 |  265.1 | 2194.3 |  104585674 B |        7.40 |
| CsvHelper    | Asset | 50000   |   102.502 ms |  3.08 |  29 |  283.8 | 2050.0 |   14305752 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   657.056 ms |  1.00 | 581 |  885.6 |  657.1 |  273070256 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   572.779 ms |  0.87 | 581 | 1015.9 |  572.8 |  284492848 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,177.217 ms |  1.80 | 581 |  494.3 | 1177.2 |  273228824 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,052.148 ms |  3.13 | 581 |  283.5 | 2052.1 | 2087769848 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,733.243 ms |  2.65 | 581 |  335.7 | 1733.2 |  273238320 B |        1.00 |
