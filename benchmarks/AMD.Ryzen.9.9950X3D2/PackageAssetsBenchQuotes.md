```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 52.15 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     4.252 ms |  1.00 |  33 | 7850.6 |   85.0 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.180 ms |  0.98 |  33 | 7984.6 |   83.6 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     3.989 ms |  0.94 |  33 | 8367.3 |   79.8 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    10.795 ms |  2.54 |  33 | 3091.8 |  215.9 |       8.46 KB |        8.33 |
| ReadLine_    | Row   | 50000   |     9.123 ms |  2.15 |  33 | 3658.5 |  182.5 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    28.260 ms |  6.65 |  33 | 1181.1 |  565.2 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.014 ms |  1.00 |  33 | 6656.3 |  100.3 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.367 ms |  1.07 |  33 | 6219.5 |  107.3 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    12.688 ms |  2.53 |  33 | 2630.6 |  253.8 |       8.46 KB |        8.33 |
| ReadLine_    | Cols  | 50000   |     9.400 ms |  1.87 |  33 | 3550.9 |  188.0 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |    41.837 ms |  8.34 |  33 |  797.8 |  836.7 |      445.6 KB |      438.75 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    25.479 ms |  1.00 |  33 | 1310.0 |  509.6 |    13801.9 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    12.111 ms |  0.48 |  33 | 2755.9 |  242.2 |   13975.38 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    31.375 ms |  1.23 |  33 | 1063.8 |  627.5 |   13961.65 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    68.645 ms |  2.70 |  33 |  486.2 | 1372.9 |  122304.07 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    50.970 ms |  2.00 |  33 |  654.8 | 1019.4 |   13969.95 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   470.303 ms |  1.00 | 667 | 1419.7 |  470.3 |  266663.86 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   226.690 ms |  0.48 | 667 | 2945.5 |  226.7 |  268001.88 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   667.054 ms |  1.42 | 667 | 1001.0 |  667.1 |  266822.84 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,738.841 ms |  3.70 | 667 |  384.0 | 1738.8 | 2442318.52 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,084.758 ms |  2.31 | 667 |  615.5 | 1084.8 |  266831.14 KB |        1.00 |
