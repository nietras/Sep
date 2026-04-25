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
| Sep______    | Row   | 50000   |     7.063 ms |  1.00 |  33 | 4712.5 |  141.3 |        960 B |        1.00 |
| Sep_Async    | Row   | 50000   |     7.049 ms |  1.00 |  33 | 4721.6 |  141.0 |        960 B |        1.00 |
| Sep_Unescape | Row   | 50000   |     7.173 ms |  1.02 |  33 | 4640.0 |  143.5 |        960 B |        1.00 |
| Sylvan___    | Row   | 50000   |    20.460 ms |  2.90 |  33 | 1626.7 |  409.2 |       7516 B |        7.83 |
| ReadLine_    | Row   | 50000   |    19.699 ms |  2.79 |  33 | 1689.5 |  394.0 |  111389416 B |  116,030.64 |
| CsvHelper    | Row   | 50000   |    46.922 ms |  6.64 |  33 |  709.3 |  938.4 |      20496 B |       21.35 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Cols  | 50000   |     8.268 ms |  1.00 |  33 | 4025.4 |  165.4 |        960 B |        1.00 |
| Sep_Unescape | Cols  | 50000   |     9.084 ms |  1.10 |  33 | 3663.8 |  181.7 |        961 B |        1.00 |
| Sylvan___    | Cols  | 50000   |    23.584 ms |  2.85 |  33 | 1411.2 |  471.7 |       7516 B |        7.83 |
| ReadLine_    | Cols  | 50000   |    19.731 ms |  2.39 |  33 | 1686.8 |  394.6 |  111389416 B |  116,030.64 |
| CsvHelper    | Cols  | 50000   |    72.376 ms |  8.75 |  33 |  459.8 | 1447.5 |     456296 B |      475.31 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 50000   |    35.076 ms |  1.00 |  33 |  948.8 |  701.5 |   14133372 B |        1.00 |
| Sep_MT___    | Asset | 50000   |    21.356 ms |  0.61 |  33 | 1558.4 |  427.1 |   14273964 B |        1.01 |
| Sylvan___    | Asset | 50000   |    51.378 ms |  1.47 |  33 |  647.8 | 1027.6 |   14295882 B |        1.01 |
| ReadLine_    | Asset | 50000   |   117.825 ms |  3.37 |  33 |  282.5 | 2356.5 |  125240038 B |        8.86 |
| CsvHelper    | Asset | 50000   |    79.105 ms |  2.26 |  33 |  420.7 | 1582.1 |   14305304 B |        1.01 |
|              |       |         |              |       |     |        |        |              |             |
| Sep______    | Asset | 1000000 |   712.793 ms |  1.00 | 665 |  934.1 |  712.8 |  273067136 B |        1.00 |
| Sep_MT___    | Asset | 1000000 |   480.678 ms |  0.67 | 665 | 1385.1 |  480.7 |  283217688 B |        1.04 |
| Sylvan___    | Asset | 1000000 | 1,120.908 ms |  1.57 | 665 |  594.0 | 1120.9 |  273226744 B |        1.00 |
| ReadLine_    | Asset | 1000000 | 2,250.766 ms |  3.16 | 665 |  295.8 | 2250.8 | 2500933480 B |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,737.967 ms |  2.44 | 665 |  383.1 | 1738.0 |  273241080 B |        1.00 |
