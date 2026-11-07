```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 52.22 GB Available
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
| Sep______    | Row   | 50000   |   2.401 ms |  1.00 |  33 | 13901.8 |   48.0 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   2.441 ms |  1.02 |  33 | 13675.5 |   48.8 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   2.866 ms |  1.19 |  33 | 11646.1 |   57.3 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |  10.331 ms |  4.30 |  33 |  3230.7 |  206.6 |      8660 B |       30.07 |
| CsvHelper    | Row   | 50000   |   2.814 ms |  1.17 |  33 | 11859.5 |   56.3 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   2.947 ms |  1.00 |  33 | 11326.0 |   58.9 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   4.107 ms |  1.39 |  33 |  8126.4 |   82.1 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |  12.109 ms |  4.11 |  33 |  2756.5 |  242.2 |      8662 B |       30.08 |
| CsvHelper    | Cols  | 50000   |  12.877 ms |  4.37 |  33 |  2592.1 |  257.5 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  23.179 ms |  1.00 |  33 |  1440.0 |  463.6 |  14132545 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  10.753 ms |  0.46 |  33 |  3103.9 |  215.1 |  14304500 B |        1.01 |
| Sylvan___    | Asset | 50000   |  33.059 ms |  1.43 |  33 |  1009.6 |  661.2 |  14296721 B |        1.01 |
| CsvHelper    | Asset | 50000   |  26.746 ms |  1.16 |  33 |  1247.9 |  534.9 |  15111676 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 437.517 ms |  1.00 | 667 |  1526.1 |  437.5 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 207.588 ms |  0.47 | 667 |  3216.5 |  207.6 | 276063488 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 654.848 ms |  1.50 | 667 |  1019.6 |  654.8 | 273226752 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 504.598 ms |  1.15 | 667 |  1323.2 |  504.6 | 274041952 B |        1.00 |
