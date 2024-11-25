```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  Job-HKRCZO : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=Job-HKRCZO  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  
Toolchain=net90  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     3.761 ms |  1.00 |  29 | 7732.9 |   75.2 |        988 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.202 ms |  1.12 |  29 | 6921.5 |   84.0 |       1033 B |        1.05 |
| Sylvan___    | Row   | 50000   |    19.616 ms |  5.22 |  29 | 1482.7 |  392.3 |       6958 B |        7.04 |
| ReadLine_    | Row   | 50000   |    17.984 ms |  4.78 |  29 | 1617.3 |  359.7 |   90734887 B |   91,836.93 |
| CsvHelper    | Row   | 50000   |    42.806 ms | 11.38 |  29 |  679.5 |  856.1 |      20764 B |       21.02 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.242 ms |  1.00 |  29 | 5548.3 |  104.8 |        995 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.306 ms |  1.01 |  29 | 5481.3 |  106.1 |        994 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    22.184 ms |  4.23 |  29 | 1311.1 |  443.7 |       6958 B |        6.99 |
| ReadLine_    | Cols  | 50000   |    19.444 ms |  3.71 |  29 | 1495.9 |  388.9 |   90734891 B |   91,190.85 |
| CsvHelper    | Cols  | 50000   |    66.750 ms | 12.73 |  29 |  435.7 | 1335.0 |     456636 B |      458.93 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    32.905 ms |  1.00 |  29 |  883.9 |  658.1 |   14134044 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.284 ms |  0.65 |  29 | 1366.5 |  425.7 |   14236153 B |        1.01 |
| Sylvan___    | Asset | 50000   |    52.459 ms |  1.60 |  29 |  554.4 | 1049.2 |   14296308 B |        1.01 |
| ReadLine_    | Asset | 50000   |    90.785 ms |  2.77 |  29 |  320.4 | 1815.7 |  104586002 B |        7.40 |
| CsvHelper    | Asset | 50000   |    79.577 ms |  2.43 |  29 |  365.5 | 1591.5 |   14305850 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   586.057 ms |  1.00 | 581 |  992.9 |  586.1 |  273070336 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   450.524 ms |  0.77 | 581 | 1291.6 |  450.5 |  283959880 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,054.994 ms |  1.80 | 581 |  551.5 | 1055.0 |  273242248 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,834.688 ms |  3.13 | 581 |  317.2 | 1834.7 | 2087769376 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,561.048 ms |  2.66 | 581 |  372.7 | 1561.0 |  273242536 B |        1.00 |
