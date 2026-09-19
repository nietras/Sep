```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=False  Reader=String  

```
| Method       | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |-------------:|------:|----:|--------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |     1.298 ms |  1.00 |  29 | 22476.3 |   26.0 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |     1.456 ms |  1.12 |  29 | 20046.3 |   29.1 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |     1.329 ms |  1.02 |  29 | 21952.7 |   26.6 |       1.07 KB |        1.04 |
| Sylvan___    | Row   | 50000   |     1.851 ms |  1.43 |  29 | 15765.6 |   37.0 |       8.45 KB |        8.26 |
| ReadLine_    | Row   | 50000   |     8.018 ms |  6.18 |  29 |  3639.3 |  160.4 |   88608.23 KB |   86,579.03 |
| CsvHelper    | Row   | 50000   |    24.731 ms | 19.05 |  29 |  1180.0 |  494.6 |      19.95 KB |       19.49 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Cols  | 50000   |     2.196 ms |  1.00 |  29 | 13287.4 |   43.9 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |     2.332 ms |  1.06 |  29 | 12513.6 |   46.6 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |     3.264 ms |  1.49 |  29 |  8940.1 |   65.3 |       8.46 KB |        8.26 |
| ReadLine_    | Cols  | 50000   |     8.487 ms |  3.86 |  29 |  3438.5 |  169.7 |   88608.23 KB |   86,579.03 |
| CsvHelper    | Cols  | 50000   |    44.994 ms | 20.49 |  29 |   648.6 |  899.9 |     445.61 KB |      435.40 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 50000   |    21.694 ms |  1.00 |  29 |  1345.1 |  433.9 |   13801.88 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |     8.825 ms |  0.41 |  29 |  3306.6 |  176.5 |    13978.1 KB |        1.01 |
| Sylvan___    | Asset | 50000   |    24.907 ms |  1.15 |  29 |  1171.6 |  498.1 |   13961.71 KB |        1.01 |
| ReadLine_    | Asset | 50000   |    55.089 ms |  2.54 |  29 |   529.7 | 1101.8 |  102133.19 KB |        7.40 |
| CsvHelper    | Asset | 50000   |    54.737 ms |  2.53 |  29 |   533.1 | 1094.7 |   13969.95 KB |        1.01 |
|              |       |         |              |       |     |         |        |               |             |
| Sep______    | Asset | 1000000 |   416.144 ms |  1.00 | 583 |  1402.8 |  416.1 |   266663.7 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   173.966 ms |  0.42 | 583 |  3355.7 |  174.0 |  268360.57 KB |        1.01 |
| Sylvan___    | Asset | 1000000 |   490.000 ms |  1.18 | 583 |  1191.4 |  490.0 |  266822.58 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 1,597.672 ms |  3.85 | 583 |   365.4 | 1597.7 | 2038834.29 KB |        7.65 |
| CsvHelper    | Asset | 1000000 | 1,153.871 ms |  2.78 | 583 |   505.9 | 1153.9 |  266830.88 KB |        1.00 |
