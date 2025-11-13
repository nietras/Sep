```

BenchmarkDotNet v0.15.6, macOS Sequoia 15.7.1 (24G231) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated    | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-------------:|------------:|
| Sep______    | Row   | 50000   |    10.004 ms |  1.01 |  33 | 3326.8 |  200.1 |        952 B |        1.00 |
| Sep_Async    | Row   | 50000   |     8.591 ms |  0.86 |  33 | 3874.3 |  171.8 |        952 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.763 ms |  0.78 |  33 | 4287.5 |  155.3 |        952 B |        1.00 |
| Sylvan___    | Row   | 50000   |    23.401 ms |  2.36 |  33 | 1422.2 |  468.0 |       6692 B |        7.03 |
| ReadLine_    | Row   | 50000   |    27.778 ms |  2.80 |  33 | 1198.1 |  555.6 |  111389416 B |  117,005.69 |
| CsvHelper    | Row   | 50000   |    53.094 ms |  5.34 |  33 |  626.9 | 1061.9 |      20424 B |       21.45 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |    11.885 ms |  1.01 |  33 | 2800.2 |  237.7 |        952 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |    12.821 ms |  1.09 |  33 | 2596.0 |  256.4 |        952 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    27.213 ms |  2.31 |  33 | 1223.0 |  544.3 |       6692 B |        7.03 |
| ReadLine_    | Cols  | 50000   |    23.611 ms |  2.00 |  33 | 1409.6 |  472.2 |  111389416 B |  117,005.69 |
| CsvHelper    | Cols  | 50000   |    92.689 ms |  7.86 |  33 |  359.1 | 1853.8 |     456296 B |      479.30 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    40.423 ms |  1.00 |  33 |  823.3 |  808.5 |   14134213 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    36.395 ms |  0.90 |  33 |  914.5 |  727.9 |   14211028 B |        1.01 |
| Sylvan___    | Asset | 50000   |    87.081 ms |  2.16 |  33 |  382.2 | 1741.6 |   14295952 B |        1.01 |
| ReadLine_    | Asset | 50000   |   135.098 ms |  3.35 |  33 |  246.4 | 2702.0 |  125239776 B |        8.86 |
| CsvHelper    | Asset | 50000   |    89.705 ms |  2.23 |  33 |  371.0 | 1794.1 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 | 1,023.915 ms |  1.01 | 665 |  650.2 | 1023.9 |  273066968 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   901.079 ms |  0.89 | 665 |  738.9 |  901.1 |  285049448 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,455.283 ms |  1.44 | 665 |  457.5 | 1455.3 |  273232840 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,628.378 ms |  4.58 | 665 |  143.9 | 4628.4 | 2500933560 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,299.950 ms |  2.28 | 665 |  289.5 | 2299.9 |  273242120 B |        1.00 |
