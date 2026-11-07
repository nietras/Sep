```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 40.79 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   1.168 ms |  1.00 |  29 | 24973.3 |   23.4 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   1.193 ms |  1.02 |  29 | 24465.8 |   23.9 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   1.344 ms |  1.15 |  29 | 21713.8 |   26.9 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |   1.857 ms |  1.59 |  29 | 15713.5 |   37.1 |      8657 B |       30.06 |
| CsvHelper    | Row   | 50000   |   1.704 ms |  1.46 |  29 | 17128.4 |   34.1 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   1.713 ms |  1.00 |  29 | 17030.9 |   34.3 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   2.288 ms |  1.34 |  29 | 12752.8 |   45.8 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |   3.206 ms |  1.87 |  29 |  9103.2 |   64.1 |      8658 B |       30.06 |
| CsvHelper    | Cols  | 50000   |  10.206 ms |  5.96 |  29 |  2859.3 |  204.1 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  20.046 ms |  1.00 |  29 |  1455.7 |  400.9 |  14132379 B |        1.00 |
| Sep_MT___    | Asset | 50000   |   8.134 ms |  0.41 |  29 |  3587.6 |  162.7 |  14303295 B |        1.01 |
| Sylvan___    | Asset | 50000   |  25.750 ms |  1.29 |  29 |  1133.3 |  515.0 |  14296781 B |        1.01 |
| CsvHelper    | Asset | 50000   |  22.902 ms |  1.14 |  29 |  1274.2 |  458.0 |  15111746 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 394.589 ms |  1.00 | 583 |  1479.5 |  394.6 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 174.356 ms |  0.44 | 583 |  3348.2 |  174.4 | 274445256 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 492.392 ms |  1.25 | 583 |  1185.6 |  492.4 | 273226592 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 440.128 ms |  1.12 | 583 |  1326.4 |  440.1 | 274041976 B |        1.00 |
