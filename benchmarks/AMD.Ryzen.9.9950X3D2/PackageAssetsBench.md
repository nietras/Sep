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
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     1.300 ms |  1.00 |  29 | 22451.9 |   26.0 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     1.450 ms |  1.12 |  29 | 20131.5 |   29.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     1.313 ms |  1.01 |  29 | 22220.0 |   26.3 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |     1.858 ms |  1.43 |  29 | 15708.8 |   37.2 |       8.45 KB |        8.32 |
| ReadLine_    | Row   | 50000   |     7.627 ms |  5.87 |  29 |  3826.1 |  152.5 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Row   | 50000   |    24.656 ms | 18.97 |  29 |  1183.5 |  493.1 |      19.95 KB |       19.64 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     1.851 ms |  1.00 |  29 | 15766.8 |   37.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     2.252 ms |  1.22 |  29 | 12958.3 |   45.0 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     3.199 ms |  1.73 |  29 |  9122.8 |   64.0 |       8.46 KB |        8.32 |
| ReadLine_    | Cols  | 50000   |     8.249 ms |  4.46 |  29 |  3537.6 |  165.0 |   88608.23 KB |   87,245.02 |
| CsvHelper    | Cols  | 50000   |    44.812 ms | 24.21 |  29 |   651.2 |  896.2 |      445.6 KB |      438.75 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    20.932 ms |  1.00 |  29 |  1394.1 |  418.6 |   13801.89 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |     9.588 ms |  0.46 |  29 |  3043.5 |  191.8 |   13976.54 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    25.103 ms |  1.20 |  29 |  1162.5 |  502.1 |   13961.71 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    57.269 ms |  2.74 |  29 |   509.5 | 1145.4 |  102133.38 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    53.148 ms |  2.55 |  29 |   549.1 | 1063.0 |   13969.95 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   406.995 ms |  1.00 | 583 |  1434.4 |  407.0 |  266664.02 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   183.364 ms |  0.45 | 583 |  3183.7 |  183.4 |   268786.5 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   476.032 ms |  1.17 | 583 |  1226.4 |  476.0 |  266822.84 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,408.037 ms |  3.47 | 583 |   414.6 | 1408.0 | 2038835.55 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,146.483 ms |  2.82 | 583 |   509.2 | 1146.5 |  266831.14 KB |        1.00 |
