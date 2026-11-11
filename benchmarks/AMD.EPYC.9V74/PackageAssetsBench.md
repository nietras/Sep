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
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   3.007 ms |  1.00 |  29 | 9673.4 |   60.1 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   3.075 ms |  1.02 |  29 | 9457.7 |   61.5 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   3.157 ms |  1.05 |  29 | 9214.6 |   63.1 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |   3.712 ms |  1.23 |  29 | 7836.2 |   74.2 |      8658 B |       30.06 |
| CsvHelper    | Row   | 50000   |   4.207 ms |  1.40 |  29 | 6913.1 |   84.1 |      1264 B |        4.39 |
|              |       |         |            |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |   4.172 ms |  1.00 |  29 | 6971.1 |   83.4 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   5.133 ms |  1.23 |  29 | 5666.3 |  102.7 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |   6.536 ms |  1.57 |  29 | 4449.9 |  130.7 |      8659 B |       30.07 |
| CsvHelper    | Cols  | 50000   |  16.940 ms |  4.06 |  29 | 1716.9 |  338.8 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |  38.590 ms |  1.00 |  29 |  753.7 |  771.8 |  14132798 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  24.838 ms |  0.64 |  29 | 1171.0 |  496.8 |  14184442 B |        1.00 |
| Sylvan___    | Asset | 50000   |  43.539 ms |  1.13 |  29 |  668.0 |  870.8 |  14297155 B |        1.01 |
| CsvHelper    | Asset | 50000   |  39.660 ms |  1.03 |  29 |  733.4 |  793.2 |  15112851 B |        1.07 |
|              |       |         |            |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 | 741.663 ms |  1.00 | 581 |  784.6 |  741.7 | 273067608 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 466.786 ms |  0.63 | 581 | 1246.6 |  466.8 | 282383360 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 903.258 ms |  1.22 | 581 |  644.2 |  903.3 | 273228664 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 758.718 ms |  1.02 | 581 |  766.9 |  758.7 | 274045728 B |        1.00 |
