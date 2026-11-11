```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.08 GB Available
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
| Sep______    | Row   | 50000   |     3.925 ms |  1.00 |  29 | 7409.9 |   78.5 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     3.572 ms |  0.95 |  29 | 8143.0 |   71.4 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.872 ms |  1.03 |  29 | 7512.6 |   77.4 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.242 ms |  5.64 |  29 | 1369.3 |  424.8 |      7477 B |       25.96 |
| CsvHelper    | Row   | 50000   |     7.734 ms |  2.05 |  29 | 3761.0 |  154.7 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     4.867 ms |  1.00 |  29 | 5976.4 |   97.3 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.476 ms |  1.13 |  29 | 5312.0 |  109.5 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.896 ms |  4.92 |  29 | 1217.2 |  477.9 |      7480 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    18.767 ms |  3.87 |  29 | 1549.9 |  375.3 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    44.815 ms |  1.00 |  29 |  649.0 |  896.3 |  14133186 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    32.957 ms |  0.74 |  29 |  882.5 |  659.1 |  14227586 B |        1.01 |
| Sylvan___    | Asset | 50000   |    61.671 ms |  1.39 |  29 |  471.6 | 1233.4 |  14296215 B |        1.01 |
| CsvHelper    | Asset | 50000   |    46.784 ms |  1.06 |  29 |  621.7 |  935.7 |  15113249 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   836.731 ms |  1.00 | 581 |  695.4 |  836.7 | 273069336 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   462.578 ms |  0.56 | 581 | 1257.9 |  462.6 | 283460136 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,216.486 ms |  1.46 | 581 |  478.3 | 1216.5 | 273235408 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   782.140 ms |  0.94 | 581 |  744.0 |  782.1 | 274047240 B |        1.00 |
