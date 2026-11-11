```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
Apple M2 Pro (Virtual), 1 CPU, 5 logical and 5 physical cores
Memory: 14 GB Total, 0.8 GB Available
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
| Sep______    | Row   | 50000   |     3.011 ms |  1.00 |  29 | 9661.4 |   60.2 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.057 ms |  1.02 |  29 | 9513.5 |   61.1 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.054 ms |  1.01 |  29 | 9523.3 |   61.1 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    18.353 ms |  6.10 |  29 | 1584.8 |  367.1 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |     6.899 ms |  2.29 |  29 | 4215.9 |  138.0 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     4.266 ms |  1.00 |  29 | 6817.4 |   85.3 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     4.885 ms |  1.15 |  29 | 5954.3 |   97.7 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    21.860 ms |  5.13 |  29 | 1330.6 |  437.2 |      7477 B |       25.96 |
| CsvHelper    | Cols  | 50000   |    18.161 ms |  4.26 |  29 | 1601.6 |  363.2 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    40.105 ms |  1.00 |  29 |  725.2 |  802.1 |  14133184 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    33.864 ms |  0.85 |  29 |  858.9 |  677.3 |  14270578 B |        1.01 |
| Sylvan___    | Asset | 50000   |    61.812 ms |  1.55 |  29 |  470.6 | 1236.2 |  14296227 B |        1.01 |
| CsvHelper    | Asset | 50000   |    43.298 ms |  1.09 |  29 |  671.8 |  866.0 |  15112562 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   721.960 ms |  1.00 | 581 |  806.0 |  722.0 | 273069384 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   425.894 ms |  0.59 | 581 | 1366.2 |  425.9 | 284521584 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,148.618 ms |  1.59 | 581 |  506.6 | 1148.6 | 273228680 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   727.604 ms |  1.01 | 581 |  799.7 |  727.6 | 274047120 B |        1.00 |
