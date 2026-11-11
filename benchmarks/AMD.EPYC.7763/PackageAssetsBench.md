```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.57 GB Available
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
| Sep______    | Row   | 50000   |     4.035 ms |  1.00 |  29 | 7232.4 |   80.7 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     4.099 ms |  1.02 |  29 | 7119.7 |   82.0 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.367 ms |  1.08 |  29 | 6682.0 |   87.3 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |     5.057 ms |  1.26 |  29 | 5770.7 |  101.1 |      8659 B |       30.07 |
| CsvHelper    | Row   | 50000   |     6.221 ms |  1.54 |  29 | 4690.9 |  124.4 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     5.083 ms |  1.00 |  29 | 5741.0 |  101.7 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.907 ms |  1.16 |  29 | 4940.0 |  118.1 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |     8.309 ms |  1.64 |  29 | 3511.9 |  166.2 |      8660 B |       30.07 |
| CsvHelper    | Cols  | 50000   |    24.215 ms |  4.76 |  29 | 1205.1 |  484.3 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    42.552 ms |  1.00 |  29 |  685.8 |  851.0 |  14133917 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    28.246 ms |  0.66 |  29 | 1033.1 |  564.9 |  14220810 B |        1.01 |
| Sylvan___    | Asset | 50000   |    50.678 ms |  1.19 |  29 |  575.8 | 1013.6 |  14297074 B |        1.01 |
| CsvHelper    | Asset | 50000   |    47.163 ms |  1.11 |  29 |  618.7 |  943.3 |  15112520 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   922.401 ms |  1.00 | 583 |  632.9 |  922.4 | 273067296 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   539.278 ms |  0.58 | 583 | 1082.5 |  539.3 | 283816640 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,087.523 ms |  1.18 | 583 |  536.8 | 1087.5 | 273228840 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,029.218 ms |  1.12 | 583 |  567.2 | 1029.2 | 274048008 B |        1.00 |
