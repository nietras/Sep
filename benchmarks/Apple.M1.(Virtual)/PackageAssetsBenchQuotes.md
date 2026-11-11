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
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |     5.406 ms |  1.00 |  33 | 6156.6 |  108.1 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     5.339 ms |  0.99 |  33 | 6234.1 |  106.8 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.126 ms |  1.32 |  33 | 4670.5 |  142.5 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.727 ms |  3.84 |  33 | 1605.7 |  414.5 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    12.234 ms |  2.26 |  33 | 2720.6 |  244.7 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     8.815 ms |  1.00 |  33 | 3775.4 |  176.3 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.435 ms |  1.53 |  33 | 2477.2 |  268.7 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    28.958 ms |  3.30 |  33 | 1149.3 |  579.2 |      7481 B |       25.98 |
| CsvHelper    | Cols  | 50000   |    31.228 ms |  3.56 |  33 | 1065.8 |  624.6 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    48.148 ms |  1.00 |  33 |  691.2 |  963.0 |  14133158 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    43.507 ms |  0.91 |  33 |  765.0 |  870.1 |  14217291 B |        1.01 |
| Sylvan___    | Asset | 50000   |    65.317 ms |  1.36 |  33 |  509.5 | 1306.3 |  14296126 B |        1.01 |
| CsvHelper    | Asset | 50000   |    70.284 ms |  1.46 |  33 |  473.5 | 1405.7 |  15112294 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 | 1,044.496 ms |  1.00 | 665 |  637.4 | 1044.5 | 273069336 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   604.362 ms |  0.59 | 665 | 1101.7 |  604.4 | 283647504 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,373.792 ms |  1.35 | 665 |  484.6 | 1373.8 | 273233280 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,144.441 ms |  1.12 | 665 |  581.8 | 1144.4 | 274048496 B |        1.00 |
