```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 53.15 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   1.147 ms |  1.00 |  29 | 25444.6 |   22.9 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   1.170 ms |  1.02 |  29 | 24930.9 |   23.4 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   1.339 ms |  1.17 |  29 | 21789.1 |   26.8 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |   1.830 ms |  1.60 |  29 | 15946.1 |   36.6 |      8657 B |       30.06 |
| CsvHelper    | Row   | 50000   |   1.693 ms |  1.48 |  29 | 17232.1 |   33.9 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   1.683 ms |  1.00 |  29 | 17334.4 |   33.7 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   2.265 ms |  1.35 |  29 | 12885.6 |   45.3 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |   3.152 ms |  1.87 |  29 |  9258.7 |   63.0 |      8658 B |       30.06 |
| CsvHelper    | Cols  | 50000   |  10.025 ms |  5.96 |  29 |  2910.8 |  200.5 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  20.367 ms |  1.00 |  29 |  1432.8 |  407.3 |  14132455 B |        1.00 |
| Sep_MT___    | Asset | 50000   |   8.433 ms |  0.41 |  29 |  3460.4 |  168.7 |  14303020 B |        1.01 |
| Sylvan___    | Asset | 50000   |  26.342 ms |  1.29 |  29 |  1107.8 |  526.8 |  14296784 B |        1.01 |
| CsvHelper    | Asset | 50000   |  23.147 ms |  1.14 |  29 |  1260.7 |  462.9 |  15111674 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 385.651 ms |  1.00 | 583 |  1513.8 |  385.7 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 181.312 ms |  0.47 | 583 |  3219.8 |  181.3 | 274851412 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 499.948 ms |  1.30 | 583 |  1167.7 |  499.9 | 273226752 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 428.180 ms |  1.11 | 583 |  1363.4 |  428.2 | 274041952 B |        1.00 |
