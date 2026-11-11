```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9725/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.96 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     5.296 ms |  1.00 |  29 | 5510.1 |  105.9 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.310 ms |  1.00 |  29 | 5496.0 |  106.2 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     5.063 ms |  0.96 |  29 | 5763.4 |  101.3 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    19.836 ms |  3.75 |  29 | 1471.1 |  396.7 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    12.799 ms |  2.42 |  29 | 2279.9 |  256.0 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     7.179 ms |  1.00 |  29 | 4064.5 |  143.6 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     7.707 ms |  1.07 |  29 | 3786.4 |  154.1 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.446 ms |  3.27 |  29 | 1244.6 |  468.9 |      7478 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    28.083 ms |  3.92 |  29 | 1039.1 |  561.7 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    48.547 ms |  1.00 |  29 |  601.1 |  970.9 |  14133277 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    31.758 ms |  0.65 |  29 |  918.9 |  635.2 |  14233827 B |        1.01 |
| Sylvan___    | Asset | 50000   |    63.568 ms |  1.31 |  29 |  459.1 | 1271.4 |  14296432 B |        1.01 |
| CsvHelper    | Asset | 50000   |    52.492 ms |  1.08 |  29 |  555.9 | 1049.8 |  15112272 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   894.859 ms |  1.00 | 583 |  652.4 |  894.9 | 273069464 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   443.358 ms |  0.50 | 583 | 1316.7 |  443.4 | 284399200 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,316.804 ms |  1.47 | 583 |  443.3 | 1316.8 | 273228680 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,079.332 ms |  1.21 | 583 |  540.9 | 1079.3 | 274047376 B |        1.00 |
