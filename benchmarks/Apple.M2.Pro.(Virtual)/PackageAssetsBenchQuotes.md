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
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     5.408 ms |  1.00 |  33 | 6154.4 |  108.2 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.395 ms |  1.00 |  33 | 6168.8 |  107.9 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.379 ms |  1.36 |  33 | 4510.6 |  147.6 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.198 ms |  3.92 |  33 | 1570.0 |  424.0 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    11.246 ms |  2.08 |  33 | 2959.4 |  224.9 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     6.727 ms |  1.00 |  33 | 4947.4 |  134.5 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.266 ms |  1.38 |  33 | 3592.0 |  185.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.624 ms |  3.52 |  33 | 1408.8 |  472.5 |      7478 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    26.937 ms |  4.01 |  33 | 1235.5 |  538.7 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    44.260 ms |  1.00 |  33 |  752.0 |  885.2 |  14133151 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    35.951 ms |  0.81 |  33 |  925.8 |  719.0 |  14248068 B |        1.01 |
| Sylvan___    | Asset | 50000   |    61.259 ms |  1.39 |  33 |  543.3 | 1225.2 |  14296224 B |        1.01 |
| CsvHelper    | Asset | 50000   |    50.410 ms |  1.14 |  33 |  660.2 | 1008.2 |  15112468 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   799.532 ms |  1.00 | 665 |  832.7 |  799.5 | 273069304 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   460.625 ms |  0.58 | 665 | 1445.4 |  460.6 | 282657704 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,204.471 ms |  1.51 | 665 |  552.8 | 1204.5 | 273228544 B |        1.00 |
| CsvHelper    | Asset | 1000000 |   968.121 ms |  1.21 | 665 |  687.7 |  968.1 | 274047248 B |        1.00 |
