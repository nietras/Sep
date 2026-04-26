```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.254 ms |  1.01 |  29 | 6836.6 |   85.1 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.518 ms |  1.07 |  29 | 6438.5 |   90.4 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.793 ms |  0.90 |  29 | 7668.1 |   75.9 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.582 ms |  4.88 |  29 | 1413.1 |  411.6 |       7516 B |        7.83 |
| ReadLine_    | Row   | 50000   |    22.994 ms |  5.45 |  29 | 1264.9 |  459.9 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    55.349 ms | 13.13 |  29 |  525.5 | 1107.0 |      20496 B |       21.35 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.664 ms |  1.01 |  29 | 5135.6 |  113.3 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.272 ms |  0.94 |  29 | 5517.0 |  105.4 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.871 ms |  4.62 |  29 | 1124.2 |  517.4 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    18.917 ms |  3.38 |  29 | 1537.5 |  378.3 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    68.904 ms | 12.32 |  29 |  422.1 | 1378.1 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    33.683 ms |  1.02 |  29 |  863.5 |  673.7 |   14133390 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.641 ms |  0.65 |  29 | 1344.0 |  432.8 |   14286657 B |        1.01 |
| Sylvan___    | Asset | 50000   |    55.903 ms |  1.69 |  29 |  520.3 | 1118.1 |   14295906 B |        1.01 |
| ReadLine_    | Asset | 50000   |    84.918 ms |  2.57 |  29 |  342.5 | 1698.4 |  104585026 B |        7.40 |
| CsvHelper    | Asset | 50000   |    88.583 ms |  2.68 |  29 |  328.3 | 1771.7 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   976.719 ms |  1.01 | 581 |  595.7 |  976.7 |  273067096 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   631.827 ms |  0.65 | 581 |  920.9 |  631.8 |  282013592 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,056.070 ms |  1.09 | 581 |  551.0 | 1056.1 |  273226880 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,814.144 ms |  1.87 | 581 |  320.7 | 1814.1 | 2087766600 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,689.581 ms |  1.74 | 581 |  344.4 | 1689.6 |  273238328 B |        1.00 |
