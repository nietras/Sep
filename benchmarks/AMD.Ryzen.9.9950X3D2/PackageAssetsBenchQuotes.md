```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 44.57 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   2.435 ms |  1.00 |  33 | 13705.3 |   48.7 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   2.473 ms |  1.02 |  33 | 13498.4 |   49.5 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   2.906 ms |  1.19 |  33 | 11484.6 |   58.1 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |  10.896 ms |  4.47 |  33 |  3063.2 |  217.9 |      8660 B |       30.07 |
| CsvHelper    | Row   | 50000   |   2.855 ms |  1.17 |  33 | 11691.3 |   57.1 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   3.000 ms |  1.00 |  33 | 11124.9 |   60.0 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   4.180 ms |  1.39 |  33 |  7984.2 |   83.6 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |  12.006 ms |  4.00 |  33 |  2780.0 |  240.1 |      8662 B |       30.08 |
| CsvHelper    | Cols  | 50000   |  13.064 ms |  4.35 |  33 |  2555.0 |  261.3 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  23.251 ms |  1.00 |  33 |  1435.5 |  465.0 |  14132390 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  11.218 ms |  0.48 |  33 |  2975.4 |  224.4 |  14305943 B |        1.01 |
| Sylvan___    | Asset | 50000   |  34.266 ms |  1.48 |  33 |   974.1 |  685.3 |  14296726 B |        1.01 |
| CsvHelper    | Asset | 50000   |  26.690 ms |  1.15 |  33 |  1250.5 |  533.8 |  15111782 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 447.979 ms |  1.00 | 667 |  1490.5 |  448.0 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 201.641 ms |  0.45 | 667 |  3311.4 |  201.6 | 276105320 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 672.549 ms |  1.50 | 667 |   992.8 |  672.5 | 273226592 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 517.429 ms |  1.16 | 667 |  1290.4 |  517.4 | 274041952 B |        1.00 |
