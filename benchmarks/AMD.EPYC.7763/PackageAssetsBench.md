```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.55 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     3.903 ms |  1.00 |  29 | 7475.8 |   78.1 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.995 ms |  1.02 |  29 | 7304.6 |   79.9 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.010 ms |  1.03 |  29 | 7276.7 |   80.2 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |     5.060 ms |  1.30 |  29 | 5767.5 |  101.2 |      8659 B |       30.07 |
| CsvHelper    | Row   | 50000   |     6.610 ms |  1.69 |  29 | 4414.6 |  132.2 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     5.554 ms |  1.00 |  29 | 5254.3 |  111.1 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     6.836 ms |  1.23 |  29 | 4268.6 |  136.7 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |     9.422 ms |  1.70 |  29 | 3097.2 |  188.4 |      8660 B |       30.07 |
| CsvHelper    | Cols  | 50000   |    26.081 ms |  4.70 |  29 | 1118.9 |  521.6 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    45.401 ms |  1.00 |  29 |  642.8 |  908.0 |  14132846 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    29.942 ms |  0.66 |  29 |  974.6 |  598.8 |  14214779 B |        1.01 |
| Sylvan___    | Asset | 50000   |    49.163 ms |  1.08 |  29 |  593.6 |  983.3 |  14297036 B |        1.01 |
| CsvHelper    | Asset | 50000   |    48.842 ms |  1.08 |  29 |  597.5 |  976.8 |  15112600 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   935.982 ms |  1.00 | 583 |  623.7 |  936.0 | 273067232 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   572.694 ms |  0.61 | 583 | 1019.4 |  572.7 | 283863728 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,095.227 ms |  1.17 | 583 |  533.0 | 1095.2 | 273228904 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,042.429 ms |  1.11 | 583 |  560.0 | 1042.4 | 274045576 B |        1.00 |
