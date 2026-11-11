```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.73 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     4.885 ms |  1.00 |  29 | 5954.7 |   97.7 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.895 ms |  1.00 |  29 | 5941.7 |   97.9 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.011 ms |  1.03 |  29 | 5804.5 |  100.2 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.804 ms |  3.85 |  29 | 1546.8 |  376.1 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    12.018 ms |  2.46 |  29 | 2420.2 |  240.4 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.221 ms |  1.00 |  29 | 4675.1 |  124.4 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.239 ms |  1.16 |  29 | 4017.8 |  144.8 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    22.540 ms |  3.62 |  29 | 1290.4 |  450.8 |      7477 B |       25.96 |
| CsvHelper    | Cols  | 50000   |    27.927 ms |  4.49 |  29 | 1041.5 |  558.5 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    34.720 ms |  1.00 |  29 |  837.7 |  694.4 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    18.415 ms |  0.53 |  29 | 1579.5 |  368.3 |  14192637 B |        1.00 |
| Sylvan___    | Asset | 50000   |    56.713 ms |  1.63 |  29 |  512.9 | 1134.3 |  14295531 B |        1.01 |
| CsvHelper    | Asset | 50000   |    43.539 ms |  1.25 |  29 |  668.0 |  870.8 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   788.188 ms |  1.00 | 581 |  738.2 |  788.2 | 273063064 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   376.044 ms |  0.48 | 581 | 1547.4 |  376.0 | 281926056 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,220.539 ms |  1.55 | 581 |  476.7 | 1220.5 | 273225384 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   948.649 ms |  1.20 | 581 |  613.4 |  948.6 | 274042136 B |        1.00 |
