```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     4.348 ms |  1.00 |  33 | 7676.3 |   87.0 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     4.358 ms |  1.00 |  33 | 7658.3 |   87.2 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     4.352 ms |  1.00 |  33 | 7669.3 |   87.0 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    10.362 ms |  2.38 |  33 | 3221.0 |  207.2 |       8.46 KB |        8.27 |
| ReadLine_    | Row   | 50000   |     9.818 ms |  2.26 |  33 | 3399.6 |  196.4 |  108778.73 KB |  106,287.61 |
| CsvHelper    | Row   | 50000   |    28.150 ms |  6.47 |  33 | 1185.7 |  563.0 |      19.95 KB |       19.49 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |     5.128 ms |  1.00 |  33 | 6508.7 |  102.6 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     5.609 ms |  1.09 |  33 | 5950.4 |  112.2 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    12.678 ms |  2.47 |  33 | 2632.7 |  253.6 |       8.46 KB |        8.27 |
| ReadLine_    | Cols  | 50000   |    10.319 ms |  2.01 |  33 | 3234.6 |  206.4 |  108778.73 KB |  106,287.61 |
| CsvHelper    | Cols  | 50000   |    41.924 ms |  8.18 |  33 |  796.1 |  838.5 |     445.61 KB |      435.40 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    24.151 ms |  1.00 |  33 | 1382.0 |  483.0 |    13801.9 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    12.633 ms |  0.52 |  33 | 2642.0 |  252.7 |   13976.56 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    31.142 ms |  1.29 |  33 | 1071.8 |  622.8 |   13961.67 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    62.129 ms |  2.58 |  33 |  537.2 | 1242.6 |  122303.92 KB |        8.86 |
| CsvHelper    | Asset | 50000   |    49.645 ms |  2.06 |  33 |  672.3 |  992.9 |   13973.02 KB |        1.01 |
|              |       |         |              |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 |   483.001 ms |  1.00 | 667 | 1382.4 |  483.0 |   266663.6 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   229.912 ms |  0.48 | 667 | 2904.2 |  229.9 |  267929.57 KB |        1.00 |
| Sylvan___    | Asset | 1000000 |   672.455 ms |  1.40 | 667 |  992.9 |  672.5 |  266822.58 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,762.645 ms |  3.66 | 667 |  378.8 | 1762.6 | 2442316.97 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 1,081.094 ms |  2.24 | 667 |  617.6 | 1081.1 |  266834.35 KB |        1.00 |
