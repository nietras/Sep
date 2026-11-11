```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.67 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     6.660 ms |  1.00 |  33 | 4997.7 |  133.2 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     6.752 ms |  1.01 |  33 | 4928.9 |  135.0 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     8.818 ms |  1.32 |  33 | 3774.5 |  176.4 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.074 ms |  3.16 |  33 | 1579.3 |  421.5 |      7477 B |       25.96 |
| CsvHelper    | Row   | 50000   |    18.532 ms |  2.78 |  33 | 1796.0 |  370.6 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     8.089 ms |  1.00 |  33 | 4114.4 |  161.8 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    11.735 ms |  1.45 |  33 | 2836.2 |  234.7 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.274 ms |  3.12 |  33 | 1316.8 |  505.5 |      7479 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    42.700 ms |  5.28 |  33 |  779.4 |  854.0 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    39.145 ms |  1.00 |  33 |  850.2 |  782.9 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.497 ms |  0.52 |  33 | 1623.8 |  409.9 |  14192982 B |        1.00 |
| Sylvan___    | Asset | 50000   |    57.904 ms |  1.48 |  33 |  574.8 | 1158.1 |  14295507 B |        1.01 |
| CsvHelper    | Asset | 50000   |    58.051 ms |  1.48 |  33 |  573.3 | 1161.0 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   869.430 ms |  1.00 | 665 |  765.8 |  869.4 | 273063176 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   378.659 ms |  0.44 | 665 | 1758.3 |  378.7 | 281121960 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,265.994 ms |  1.46 | 665 |  525.9 | 1266.0 | 273225488 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,287.738 ms |  1.48 | 665 |  517.0 | 1287.7 | 274041944 B |        1.00 |
