```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 52.22 GB Available
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
| Sep______    | Row   | 50000   |   1.234 ms |  1.00 |  29 | 23648.6 |   24.7 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   1.261 ms |  1.02 |  29 | 23147.0 |   25.2 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   1.295 ms |  1.05 |  29 | 22529.3 |   25.9 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |   1.837 ms |  1.49 |  29 | 15881.5 |   36.7 |      8657 B |       30.06 |
| CsvHelper    | Row   | 50000   |   1.687 ms |  1.37 |  29 | 17296.4 |   33.7 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   1.794 ms |  1.00 |  29 | 16265.3 |   35.9 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   2.209 ms |  1.23 |  29 | 13208.8 |   44.2 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |   3.188 ms |  1.78 |  29 |  9152.9 |   63.8 |      8658 B |       30.06 |
| CsvHelper    | Cols  | 50000   |  10.298 ms |  5.74 |  29 |  2833.7 |  206.0 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  20.842 ms |  1.00 |  29 |  1400.1 |  416.8 |  14132478 B |        1.00 |
| Sep_MT___    | Asset | 50000   |   9.195 ms |  0.44 |  29 |  3173.4 |  183.9 |  14301946 B |        1.01 |
| Sylvan___    | Asset | 50000   |  25.646 ms |  1.23 |  29 |  1137.9 |  512.9 |  14296780 B |        1.01 |
| CsvHelper    | Asset | 50000   |  23.038 ms |  1.11 |  29 |  1266.6 |  460.8 |  15111676 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 417.386 ms |  1.00 | 583 |  1398.7 |  417.4 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 174.034 ms |  0.42 | 583 |  3354.4 |  174.0 | 274336376 B |        1.00 |
| Sylvan___    | Asset | 1000000 | 491.018 ms |  1.18 | 583 |  1188.9 |  491.0 | 273226592 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 437.386 ms |  1.05 | 583 |  1334.7 |  437.4 | 274041952 B |        1.00 |
