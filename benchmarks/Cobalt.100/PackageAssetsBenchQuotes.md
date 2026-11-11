```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9725/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 12.13 GB Available
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
| Sep______    | Row   | 50000   |     7.122 ms |  1.00 |  33 | 4686.8 |  142.4 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.339 ms |  1.03 |  33 | 4548.1 |  146.8 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     9.229 ms |  1.30 |  33 | 3616.4 |  184.6 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.802 ms |  3.06 |  33 | 1530.9 |  436.0 |      7477 B |       25.96 |
| CsvHelper    | Row   | 50000   |    19.703 ms |  2.77 |  33 | 1694.0 |  394.1 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     9.247 ms |  1.00 |  33 | 3609.7 |  184.9 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.607 ms |  1.36 |  33 | 2647.5 |  252.1 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.689 ms |  2.78 |  33 | 1299.3 |  513.8 |      7479 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    43.738 ms |  4.73 |  33 |  763.1 |  874.8 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    39.137 ms |  1.00 |  33 |  852.8 |  782.7 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.713 ms |  0.58 |  33 | 1469.5 |  454.3 |  14187634 B |        1.00 |
| Sylvan___    | Asset | 50000   |    57.543 ms |  1.47 |  33 |  580.0 | 1150.9 |  14295507 B |        1.01 |
| CsvHelper    | Asset | 50000   |    58.763 ms |  1.50 |  33 |  568.0 | 1175.3 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   880.973 ms |  1.00 | 667 |  757.9 |  881.0 | 273063200 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   373.785 ms |  0.42 | 667 | 1786.3 |  373.8 | 278984496 B |        1.02 |
| Sylvan___    | Asset | 1000000 | 1,270.955 ms |  1.44 | 667 |  525.4 | 1271.0 | 273225560 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,284.670 ms |  1.46 | 667 |  519.7 | 1284.7 | 274041936 B |        1.00 |
