```

BenchmarkDotNet v0.15.1, macOS Sonoma 14.7.6 (23H626) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD
  Job-AKIMDM : .NET 9.0.5 (9.0.525.21509), Arm64 RyuJIT AdvSIMD

Job=Job-AKIMDM  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     9.290 ms |  1.00 |  33 | 3582.6 |  185.8 |        997 B |        1.00 |
| Sep_Async    | Row   | 50000   |     9.339 ms |  1.01 |  33 | 3563.8 |  186.8 |       1012 B |        1.02 |
| Sep_Unescape | Row   | 50000   |     9.897 ms |  1.07 |  33 | 3363.0 |  197.9 |       1201 B |        1.20 |
| Sylvan___    | Row   | 50000   |    23.249 ms |  2.51 |  33 | 1431.6 |  465.0 |       6958 B |        6.98 |
| ReadLine_    | Row   | 50000   |    22.531 ms |  2.44 |  33 | 1477.1 |  450.6 |  111389493 B |  111,724.67 |
| CsvHelper    | Row   | 50000   |    46.522 ms |  5.03 |  33 |  715.4 |  930.4 |      20764 B |       20.83 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.664 ms |  1.00 |  33 | 3841.6 |  173.3 |        999 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.746 ms |  1.12 |  33 | 3415.0 |  194.9 |       1170 B |        1.17 |
| Sylvan___    | Cols  | 50000   |    22.858 ms |  2.64 |  33 | 1456.0 |  457.2 |       6725 B |        6.73 |
| ReadLine_    | Cols  | 50000   |    22.199 ms |  2.56 |  33 | 1499.2 |  444.0 |  111389493 B |  111,500.99 |
| CsvHelper    | Cols  | 50000   |    71.417 ms |  8.24 |  33 |  466.0 | 1428.3 |     456636 B |      457.09 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    36.004 ms |  1.00 |  33 |  924.4 |  720.1 |   14134024 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.068 ms |  0.61 |  33 | 1508.1 |  441.4 |   14233705 B |        1.01 |
| Sylvan___    | Asset | 50000   |    51.940 ms |  1.45 |  33 |  640.8 | 1038.8 |   14296832 B |        1.01 |
| ReadLine_    | Asset | 50000   |   136.529 ms |  3.80 |  33 |  243.8 | 2730.6 |  125241136 B |        8.86 |
| CsvHelper    | Asset | 50000   |    88.008 ms |  2.45 |  33 |  378.2 | 1760.2 |   14307374 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   733.901 ms |  1.00 | 665 |  907.2 |  733.9 |  273075208 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   469.473 ms |  0.64 | 665 | 1418.2 |  469.5 |  281562856 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,154.378 ms |  1.57 | 665 |  576.8 | 1154.4 |  273228848 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,877.672 ms |  3.92 | 665 |  231.4 | 2877.7 | 2500938080 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,962.262 ms |  2.68 | 665 |  339.3 | 1962.3 |  273242672 B |        1.00 |
