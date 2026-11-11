```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V45 4.06GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   3.912 ms |  1.00 |  33 | 8506.6 |   78.2 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   3.956 ms |  1.01 |  33 | 8412.7 |   79.1 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   4.408 ms |  1.13 |  33 | 7551.1 |   88.2 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |  15.965 ms |  4.08 |  33 | 2084.7 |  319.3 |      8662 B |       30.08 |
| CsvHelper    | Row   | 50000   |   4.661 ms |  1.19 |  33 | 7141.2 |   93.2 |      1264 B |        4.39 |
|              |       |         |            |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |   4.589 ms |  1.00 |  33 | 7252.2 |   91.8 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   6.144 ms |  1.34 |  33 | 5417.4 |  122.9 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |  16.873 ms |  3.68 |  33 | 1972.6 |  337.5 |      8662 B |       30.08 |
| CsvHelper    | Cols  | 50000   |  16.449 ms |  3.58 |  33 | 2023.3 |  329.0 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |  33.906 ms |  1.00 |  33 |  981.6 |  678.1 |  14132958 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  20.875 ms |  0.62 |  33 | 1594.3 |  417.5 |  14240436 B |        1.01 |
| Sylvan___    | Asset | 50000   |  48.567 ms |  1.43 |  33 |  685.3 |  971.3 |  14297119 B |        1.01 |
| CsvHelper    | Asset | 50000   |  35.958 ms |  1.06 |  33 |  925.6 |  719.2 |  15112103 B |        1.07 |
|              |       |         |            |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 | 643.560 ms |  1.00 | 665 | 1034.6 |  643.6 | 273067512 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 433.440 ms |  0.67 | 665 | 1536.1 |  433.4 | 282026976 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 945.606 ms |  1.47 | 665 |  704.1 |  945.6 | 273228840 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 698.821 ms |  1.09 | 665 |  952.7 |  698.8 | 274045760 B |        1.00 |
