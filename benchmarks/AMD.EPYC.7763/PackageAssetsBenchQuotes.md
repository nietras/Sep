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
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     7.569 ms |  1.00 |  33 | 4409.5 |  151.4 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.746 ms |  1.02 |  33 | 4308.8 |  154.9 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     8.345 ms |  1.10 |  33 | 3999.8 |  166.9 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    24.970 ms |  3.30 |  33 | 1336.7 |  499.4 |      8671 B |       30.11 |
| CsvHelper    | Row   | 50000   |    10.438 ms |  1.38 |  33 | 3197.6 |  208.8 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     9.093 ms |  1.00 |  33 | 3670.9 |  181.9 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    11.457 ms |  1.26 |  33 | 2913.4 |  229.1 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.789 ms |  3.17 |  33 | 1159.4 |  575.8 |      8672 B |       30.11 |
| CsvHelper    | Cols  | 50000   |    31.702 ms |  3.49 |  33 | 1052.8 |  634.0 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    56.259 ms |  1.00 |  33 |  593.3 | 1125.2 |  14132794 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.287 ms |  0.58 |  33 | 1033.8 |  645.7 |  14211768 B |        1.01 |
| Sylvan___    | Asset | 50000   |    69.748 ms |  1.25 |  33 |  478.5 | 1395.0 |  14297658 B |        1.01 |
| CsvHelper    | Asset | 50000   |    57.934 ms |  1.04 |  33 |  576.1 | 1158.7 |  15112738 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 | 1,049.880 ms |  1.00 | 667 |  636.0 | 1049.9 | 273071432 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   587.111 ms |  0.56 | 667 | 1137.3 |  587.1 | 281350848 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,518.652 ms |  1.45 | 667 |  439.7 | 1518.7 | 273228848 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,194.737 ms |  1.14 | 667 |  558.9 | 1194.7 | 274045608 B |        1.00 |
