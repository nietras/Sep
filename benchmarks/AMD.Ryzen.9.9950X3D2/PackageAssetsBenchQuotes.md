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
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated   | Alloc Ratio |
|------------- |------ |-------- |-----------:|------:|----:|--------:|-------:|------------:|------------:|
| Sep______    | Row   | 50000   |   2.225 ms |  1.00 |  33 | 15003.5 |   44.5 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |   2.259 ms |  1.02 |  33 | 14778.2 |   45.2 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |   2.664 ms |  1.20 |  33 | 12530.4 |   53.3 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |  10.617 ms |  4.77 |  33 |  3143.9 |  212.3 |      8660 B |       30.07 |
| CsvHelper    | Row   | 50000   |   2.799 ms |  1.26 |  33 | 11925.0 |   56.0 |      1264 B |        4.39 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Cols  | 50000   |   2.749 ms |  1.00 |  33 | 12143.5 |   55.0 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |   3.896 ms |  1.42 |  33 |  8567.7 |   77.9 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |  12.104 ms |  4.40 |  33 |  2757.6 |  242.1 |      8662 B |       30.08 |
| CsvHelper    | Cols  | 50000   |  12.778 ms |  4.65 |  33 |  2612.0 |  255.6 |   1262624 B |    4,384.11 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 50000   |  21.302 ms |  1.00 |  33 |  1566.8 |  426.0 |  14132407 B |        1.00 |
| Sep_MT___    | Asset | 50000   |  10.640 ms |  0.50 |  33 |  3137.1 |  212.8 |  14304828 B |        1.01 |
| Sylvan___    | Asset | 50000   |  32.367 ms |  1.52 |  33 |  1031.2 |  647.3 |  14296721 B |        1.01 |
| CsvHelper    | Asset | 50000   |  26.121 ms |  1.23 |  33 |  1277.8 |  522.4 |  15111668 B |        1.07 |
|              |       |         |            |       |     |         |        |             |             |
| Sep______    | Asset | 1000000 | 432.455 ms |  1.00 | 667 |  1544.0 |  432.5 | 273063040 B |        1.00 |
| Sep_MT___    | Asset | 1000000 | 198.416 ms |  0.46 | 667 |  3365.2 |  198.4 | 275809996 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 677.688 ms |  1.57 | 667 |   985.3 |  677.7 | 273226752 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 521.251 ms |  1.21 | 667 |  1281.0 |  521.3 | 274042112 B |        1.00 |
