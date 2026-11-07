```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 40.79 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   2.228 ms |  1.00 |  33 | 14979.0 |   44.6 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   2.268 ms |  1.02 |  33 | 14718.9 |   45.4 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   2.664 ms |  1.20 |  33 | 12529.9 |   53.3 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |  10.387 ms |  4.66 |  33 |  3213.3 |  207.7 |      8660 B |       30.07 |
| CsvHelper    | Row   | 50000   |   2.845 ms |  1.28 |  33 | 11732.5 |   56.9 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   2.784 ms |  1.00 |  33 | 11989.8 |   55.7 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   3.917 ms |  1.41 |  33 |  8521.7 |   78.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |  12.463 ms |  4.48 |  33 |  2678.1 |  249.3 |      8662 B |       30.08 |
| CsvHelper    | Cols  | 50000   |  12.938 ms |  4.65 |  33 |  2579.9 |  258.8 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  22.900 ms |  1.00 |  33 |  1457.5 |  458.0 |  14132383 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  10.176 ms |  0.45 |  33 |  3280.0 |  203.5 |  14303629 B |        1.01 |
| Sylvan___    | Asset | 50000   |  33.197 ms |  1.45 |  33 |  1005.4 |  663.9 |  14296726 B |        1.01 |
| CsvHelper    | Asset | 50000   |  26.150 ms |  1.15 |  33 |  1276.4 |  523.0 |  15111672 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 451.302 ms |  1.00 | 667 |  1479.5 |  451.3 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 198.953 ms |  0.44 | 667 |  3356.1 |  199.0 | 275454304 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 679.649 ms |  1.51 | 667 |   982.4 |  679.6 | 273226592 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 501.184 ms |  1.11 | 667 |  1332.3 |  501.2 | 274041952 B |        1.00 |
