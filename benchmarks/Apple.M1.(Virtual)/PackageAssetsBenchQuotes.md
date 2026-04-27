```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |     7.079 ms |  1.00 |  33 | 4701.4 |  141.6 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.291 ms |  1.03 |  33 | 4565.1 |  145.8 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.234 ms |  1.02 |  33 | 4601.0 |  144.7 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.311 ms |  2.87 |  33 | 1638.6 |  406.2 |       7516 B |        7.83 |
| ReadLine_    | Row   | 50000   |    20.958 ms |  2.96 |  33 | 1588.0 |  419.2 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    47.550 ms |  6.72 |  33 |  699.9 |  951.0 |      20424 B |       21.27 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.075 ms |  1.00 |  33 | 4121.4 |  161.5 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.070 ms |  1.12 |  33 | 3669.5 |  181.4 |        960 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    24.071 ms |  2.98 |  33 | 1382.6 |  481.4 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    21.187 ms |  2.62 |  33 | 1570.9 |  423.7 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    72.641 ms |  9.00 |  33 |  458.2 | 1452.8 |     456368 B |      475.38 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.568 ms |  1.00 |  33 |  935.7 |  711.4 |   14133366 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    22.788 ms |  0.64 |  33 | 1460.5 |  455.8 |   14200160 B |        1.00 |
| Sylvan___    | Asset | 50000   |    52.618 ms |  1.48 |  33 |  632.5 | 1052.4 |   14296248 B |        1.01 |
| ReadLine_    | Asset | 50000   |   122.089 ms |  3.44 |  33 |  272.6 | 2441.8 |  125240004 B |        8.86 |
| CsvHelper    | Asset | 50000   |    81.503 ms |  2.30 |  33 |  408.4 | 1630.1 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   779.554 ms |  1.00 | 665 |  854.1 |  779.6 |  273066976 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   542.628 ms |  0.70 | 665 | 1227.0 |  542.6 |  281650392 B |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,271.602 ms |  1.64 | 665 |  523.6 | 1271.6 |  273227040 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,107.842 ms |  5.29 | 665 |  162.1 | 4107.8 | 2500933472 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,236.050 ms |  2.88 | 665 |  297.8 | 2236.1 |  273236344 B |        1.00 |
