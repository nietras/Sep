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
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   2.740 ms |  1.00 |  29 | 10615.4 |   54.8 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   2.790 ms |  1.02 |  29 | 10423.6 |   55.8 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   2.829 ms |  1.03 |  29 | 10282.1 |   56.6 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |   3.450 ms |  1.26 |  29 |  8431.8 |   69.0 |      8658 B |       30.06 |
| CsvHelper    | Row   | 50000   |   3.764 ms |  1.37 |  29 |  7727.2 |   75.3 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   3.794 ms |  1.00 |  29 |  7665.7 |   75.9 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   4.617 ms |  1.22 |  29 |  6299.3 |   92.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |   6.085 ms |  1.60 |  29 |  4779.8 |  121.7 |      8659 B |       30.07 |
| CsvHelper    | Cols  | 50000   |  16.641 ms |  4.39 |  29 |  1747.8 |  332.8 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  37.342 ms |  1.00 |  29 |   778.9 |  746.8 |  14132935 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  22.560 ms |  0.60 |  29 |  1289.3 |  451.2 |  14206100 B |        1.01 |
| Sylvan___    | Asset | 50000   |  42.456 ms |  1.14 |  29 |   685.1 |  849.1 |  14297093 B |        1.01 |
| CsvHelper    | Asset | 50000   |  36.585 ms |  0.98 |  29 |   795.0 |  731.7 |  15112858 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 725.338 ms |  1.00 | 581 |   802.2 |  725.3 | 273075232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 446.824 ms |  0.62 | 581 |  1302.2 |  446.8 | 281356608 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 860.685 ms |  1.19 | 581 |   676.1 |  860.7 | 273228816 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 747.509 ms |  1.03 | 581 |   778.4 |  747.5 | 274045416 B |        1.00 |
