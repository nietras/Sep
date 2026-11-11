```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.68GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.45 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     5.733 ms |  1.00 |  33 | 5805.7 |  114.7 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.755 ms |  1.00 |  33 | 5783.0 |  115.1 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     6.284 ms |  1.10 |  33 | 5296.4 |  125.7 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.594 ms |  3.42 |  33 | 1698.6 |  391.9 |      8662 B |       30.08 |
| CsvHelper    | Row   | 50000   |     6.214 ms |  1.08 |  33 | 5355.9 |  124.3 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.823 ms |  1.00 |  33 | 4877.9 |  136.5 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.831 ms |  1.29 |  33 | 3768.9 |  176.6 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    22.082 ms |  3.24 |  33 | 1507.2 |  441.6 |      8669 B |       30.10 |
| CsvHelper    | Cols  | 50000   |    20.831 ms |  3.05 |  33 | 1597.7 |  416.6 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    41.151 ms |  1.00 |  33 |  808.8 |  823.0 |  14132797 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    25.751 ms |  0.63 |  33 | 1292.5 |  515.0 |  14198482 B |        1.00 |
| Sylvan___    | Asset | 50000   |    59.927 ms |  1.46 |  33 |  555.4 | 1198.5 |  14297129 B |        1.01 |
| CsvHelper    | Asset | 50000   |    42.240 ms |  1.03 |  33 |  787.9 |  844.8 |  15111978 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   818.123 ms |  1.00 | 665 |  813.8 |  818.1 | 273067408 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   508.269 ms |  0.62 | 665 | 1309.9 |  508.3 | 280234680 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,203.021 ms |  1.47 | 665 |  553.4 | 1203.0 | 273228840 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   863.482 ms |  1.06 | 665 |  771.1 |  863.5 | 274045640 B |        1.00 |
