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
| Sep______    | Row   | 50000   |     2.971 ms |  1.00 |  29 | 9790.9 |   59.4 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.025 ms |  1.02 |  29 | 9614.8 |   60.5 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     2.981 ms |  1.00 |  29 | 9755.5 |   59.6 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.006 ms |  6.06 |  29 | 1615.3 |  360.1 |       7475 B |        7.79 |
| ReadLine_    | Row   | 50000   |    16.749 ms |  5.64 |  29 | 1736.6 |  335.0 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    43.180 ms | 14.54 |  29 |  673.6 |  863.6 |      20496 B |       21.35 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     4.085 ms |  1.00 |  29 | 7120.0 |   81.7 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.628 ms |  1.13 |  29 | 6284.5 |   92.6 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.259 ms |  5.21 |  29 | 1368.2 |  425.2 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    16.801 ms |  4.11 |  29 | 1731.2 |  336.0 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    65.949 ms | 16.15 |  29 |  441.0 | 1319.0 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    30.072 ms |  1.00 |  29 |  967.2 |  601.4 |   14133394 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.703 ms |  0.72 |  29 | 1340.2 |  434.1 |   14271705 B |        1.01 |
| Sylvan___    | Asset | 50000   |    49.570 ms |  1.65 |  29 |  586.8 |  991.4 |   14295922 B |        1.01 |
| ReadLine_    | Asset | 50000   |    79.663 ms |  2.66 |  29 |  365.1 | 1593.3 |  104585174 B |        7.40 |
| CsvHelper    | Asset | 50000   |    73.557 ms |  2.45 |  29 |  395.4 | 1471.1 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   590.295 ms |  1.00 | 581 |  985.7 |  590.3 |  273067136 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   429.687 ms |  0.73 | 581 | 1354.2 |  429.7 |  284090744 B |        1.04 |
| Sylvan___    | Asset | 1000000 |   997.005 ms |  1.69 | 581 |  583.6 |  997.0 |  273230816 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,645.187 ms |  2.79 | 581 |  353.7 | 1645.2 | 2087766816 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,534.770 ms |  2.60 | 581 |  379.1 | 1534.8 |  273236224 B |        1.00 |
