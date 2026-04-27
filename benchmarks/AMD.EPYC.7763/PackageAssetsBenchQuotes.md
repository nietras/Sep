```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Quotes=True  Reader=String  

```
| Method       | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated     | Alloc Ratio |
|------------- |------ |-------- |------------:|------:|----:|-------:|-------:|--------------:|------------:|
| Sep______    | Row   | 50000   |    11.93 ms |  1.00 |  33 | 2797.1 |  238.7 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    12.30 ms |  1.03 |  33 | 2714.0 |  246.0 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.51 ms |  0.96 |  33 | 2900.0 |  230.2 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    24.60 ms |  2.06 |  33 | 1357.1 |  491.9 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    23.73 ms |  1.99 |  33 | 1406.5 |  474.6 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    80.39 ms |  6.74 |  33 |  415.2 | 1607.8 |      19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    13.22 ms |  1.00 |  33 | 2524.8 |  264.4 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    13.88 ms |  1.05 |  33 | 2404.1 |  277.7 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    29.27 ms |  2.21 |  33 | 1140.3 |  585.4 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    25.88 ms |  1.96 |  33 | 1289.8 |  517.6 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   108.54 ms |  8.21 |  33 |  307.5 | 2170.7 |      445.6 KB |      438.75 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    50.07 ms |  1.00 |  33 |  666.6 | 1001.5 |   13802.29 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    41.12 ms |  0.82 |  33 |  811.6 |  822.5 |   13865.11 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    70.42 ms |  1.41 |  33 |  474.0 | 1408.4 |   13961.98 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   211.16 ms |  4.23 |  33 |  158.1 | 4223.2 |  122304.52 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   126.70 ms |  2.54 |  33 |  263.4 | 2534.0 |   13970.02 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,062.79 ms |  1.00 | 667 |  628.3 | 1062.8 |  266667.16 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   617.62 ms |  0.58 | 667 | 1081.1 |  617.6 |  274249.07 KB |        1.03 |
| Sylvan___    | Asset | 1000000 | 1,478.51 ms |  1.39 | 667 |  451.6 | 1478.5 |  266824.18 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,076.57 ms |  3.84 | 667 |  163.8 | 4076.6 | 2442318.37 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,642.60 ms |  2.49 | 667 |  252.7 | 2642.6 |  266839.88 KB |        1.00 |
