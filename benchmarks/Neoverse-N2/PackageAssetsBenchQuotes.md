```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.73 GB Available
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
| Sep______    | Row   | 50000   |     6.765 ms |  1.00 |  33 | 4920.0 |  135.3 |       288 B |        1.00 |
| Sep_Async    | Row   | 50000   |     6.792 ms |  1.00 |  33 | 4900.1 |  135.8 |       288 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     8.603 ms |  1.27 |  33 | 3868.6 |  172.1 |       288 B |        1.00 |
| Sylvan___    | Row   | 50000   |    21.167 ms |  3.13 |  33 | 1572.3 |  423.3 |      7470 B |       25.94 |
| CsvHelper    | Row   | 50000   |    18.485 ms |  2.73 |  33 | 1800.4 |  369.7 |      1264 B |        4.39 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Cols  | 50000   |     8.070 ms |  1.00 |  33 | 4124.1 |  161.4 |       288 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    11.362 ms |  1.41 |  33 | 2929.3 |  227.2 |       288 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    25.106 ms |  3.11 |  33 | 1325.6 |  502.1 |      7479 B |       25.97 |
| CsvHelper    | Cols  | 50000   |    43.007 ms |  5.33 |  33 |  773.9 |  860.1 |   1262624 B |    4,384.11 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 50000   |    39.271 ms |  1.00 |  33 |  847.5 |  785.4 |  14132256 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    20.833 ms |  0.53 |  33 | 1597.5 |  416.7 |  14188366 B |        1.00 |
| Sylvan___    | Asset | 50000   |    58.050 ms |  1.48 |  33 |  573.3 | 1161.0 |  14295507 B |        1.01 |
| CsvHelper    | Asset | 50000   |    59.094 ms |  1.50 |  33 |  563.2 | 1181.9 |  15111560 B |        1.07 |
|              |       |         |              |       |     |        |        |             |             |
| Sep______    | Asset | 1000000 |   870.732 ms |  1.00 | 665 |  764.6 |  870.7 | 273063096 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   383.072 ms |  0.44 | 665 | 1738.1 |  383.1 | 276771600 B |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,265.204 ms |  1.45 | 665 |  526.2 | 1265.2 | 273225488 B |        1.00 |
| CsvHelper    | Asset | 1000000 | 1,279.991 ms |  1.47 | 665 |  520.2 | 1280.0 | 274042040 B |        1.00 |
