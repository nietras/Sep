```

BenchmarkDotNet v0.16.0-preview.1, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.09 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     4.115 ms |  1.00 |  29 | 7069.0 |   82.3 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.158 ms |  1.02 |  29 | 6995.1 |   83.2 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.104 ms |  1.00 |  29 | 7086.6 |   82.1 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    25.123 ms |  6.14 |  29 | 1157.7 |  502.5 |       7477 B |        7.79 |
| ReadLine_    | Row   | 50000   |    25.265 ms |  6.17 |  29 | 1151.2 |  505.3 |   90734824 B |   94,515.44 |
| CsvHelper    | Row   | 50000   |    45.047 ms | 11.00 |  29 |  645.7 |  900.9 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     5.678 ms |  1.00 |  29 | 5122.5 |  113.6 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.000 ms |  1.24 |  29 | 4155.3 |  140.0 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    32.073 ms |  5.67 |  29 |  906.9 |  641.5 |       7481 B |        7.79 |
| ReadLine_    | Cols  | 50000   |    25.526 ms |  4.51 |  29 | 1139.5 |  510.5 |   90734824 B |   94,515.44 |
| CsvHelper    | Cols  | 50000   |    81.777 ms | 14.46 |  29 |  355.7 | 1635.5 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    45.060 ms |  1.00 |  29 |  645.5 |  901.2 |   14133966 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    40.312 ms |  0.90 |  29 |  721.5 |  806.2 |   14215927 B |        1.01 |
| Sylvan___    | Asset | 50000   |    72.394 ms |  1.61 |  29 |  401.8 | 1447.9 |   14296210 B |        1.01 |
| ReadLine_    | Asset | 50000   |   126.710 ms |  2.82 |  29 |  229.5 | 2534.2 |  104585460 B |        7.40 |
| CsvHelper    | Asset | 50000   |   100.603 ms |  2.24 |  29 |  289.1 | 2012.1 |   14306132 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   847.915 ms |  1.00 | 581 |  686.2 |  847.9 |  273070136 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   468.377 ms |  0.56 | 581 | 1242.3 |  468.4 |  283444368 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,478.192 ms |  1.76 | 581 |  393.6 | 1478.2 |  273228544 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,336.417 ms |  2.79 | 581 |  249.0 | 2336.4 | 2087768840 B |        7.65 |
| CsvHelper    | Asset | 1000000 | 2,202.858 ms |  2.63 | 581 |  264.1 | 2202.9 |  273238328 B |        1.00 |
