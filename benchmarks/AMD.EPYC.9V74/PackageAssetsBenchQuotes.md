```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.46 GB Available
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
| Sep______    | Row   | 50000   |     5.370 ms |  1.00 |  33 | 6198.3 |  107.4 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.482 ms |  1.02 |  33 | 6071.6 |  109.6 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.876 ms |  1.09 |  33 | 5664.5 |  117.5 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.745 ms |  3.49 |  33 | 1775.6 |  374.9 |      8662 B |       30.08 |
| CsvHelper    | Row   | 50000   |     5.758 ms |  1.07 |  33 | 5780.4 |  115.2 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.506 ms |  1.00 |  33 | 5115.3 |  130.1 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     8.463 ms |  1.30 |  33 | 3932.8 |  169.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    22.174 ms |  3.41 |  33 | 1501.0 |  443.5 |      8669 B |       30.10 |
| CsvHelper    | Cols  | 50000   |    20.367 ms |  3.13 |  33 | 1634.1 |  407.3 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    39.602 ms |  1.00 |  33 |  840.4 |  792.0 |  14132773 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    24.926 ms |  0.63 |  33 | 1335.2 |  498.5 |  14188467 B |        1.00 |
| Sylvan___    | Asset | 50000   |    57.519 ms |  1.45 |  33 |  578.6 | 1150.4 |  14297016 B |        1.01 |
| CsvHelper    | Asset | 50000   |    40.936 ms |  1.03 |  33 |  813.0 |  818.7 |  15112840 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   810.909 ms |  1.00 | 665 |  821.1 |  810.9 | 273067392 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   497.675 ms |  0.61 | 665 | 1337.8 |  497.7 | 282301840 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,175.901 ms |  1.45 | 665 |  566.2 | 1175.9 | 273228752 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   841.747 ms |  1.04 | 665 |  791.0 |  841.7 | 274046416 B |        1.00 |
