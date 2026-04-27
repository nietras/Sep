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
| Sep______    | Row   | 50000   |    12.02 ms |  1.00 |  33 | 2776.4 |  240.4 |       1.02 KB |        1.00 |
| Sep_Async    | Row   | 50000   |    11.94 ms |  0.99 |  33 | 2796.4 |  238.7 |       1.02 KB |        1.00 |
| Sep_Unescape | Row   | 50000   |    11.63 ms |  0.97 |  33 | 2869.2 |  232.7 |       1.02 KB |        1.00 |
| Sylvan___    | Row   | 50000   |    25.23 ms |  2.10 |  33 | 1322.8 |  504.7 |       8.47 KB |        8.34 |
| ReadLine_    | Row   | 50000   |    25.20 ms |  2.10 |  33 | 1324.5 |  504.0 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Row   | 50000   |    82.06 ms |  6.83 |  33 |  406.7 | 1641.3 |      19.95 KB |       19.64 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Cols  | 50000   |    13.62 ms |  1.00 |  33 | 2451.2 |  272.3 |       1.02 KB |        1.00 |
| Sep_Unescape | Cols  | 50000   |    14.46 ms |  1.06 |  33 | 2309.0 |  289.1 |       1.02 KB |        1.00 |
| Sylvan___    | Cols  | 50000   |    31.73 ms |  2.33 |  33 | 1051.8 |  634.7 |       8.47 KB |        8.34 |
| ReadLine_    | Cols  | 50000   |    28.01 ms |  2.06 |  33 | 1191.5 |  560.2 |  108778.73 KB |  107,105.21 |
| CsvHelper    | Cols  | 50000   |   120.13 ms |  8.83 |  33 |  277.8 | 2402.6 |      445.6 KB |      438.75 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 50000   |    56.85 ms |  1.00 |  33 |  587.1 | 1137.1 |   13802.29 KB |        1.00 |
| Sep_MT___    | Asset | 50000   |    37.04 ms |  0.65 |  33 |  901.1 |  740.8 |   13859.82 KB |        1.00 |
| Sylvan___    | Asset | 50000   |    79.07 ms |  1.39 |  33 |  422.1 | 1581.5 |   13961.93 KB |        1.01 |
| ReadLine_    | Asset | 50000   |   207.09 ms |  3.64 |  33 |  161.2 | 4141.9 |  122304.22 KB |        8.86 |
| CsvHelper    | Asset | 50000   |   133.17 ms |  2.34 |  33 |  250.6 | 2663.4 |   13970.13 KB |        1.01 |
|              |       |         |             |       |     |        |        |               |             |
| Sep______    | Asset | 1000000 | 1,060.59 ms |  1.00 | 667 |  629.6 | 1060.6 |   266667.3 KB |        1.00 |
| Sep_MT___    | Asset | 1000000 |   627.97 ms |  0.59 | 667 | 1063.3 |  628.0 |   270194.4 KB |        1.01 |
| Sylvan___    | Asset | 1000000 | 1,494.94 ms |  1.41 | 667 |  446.6 | 1494.9 |  266825.35 KB |        1.00 |
| ReadLine_    | Asset | 1000000 | 4,448.97 ms |  4.19 | 667 |  150.1 | 4449.0 | 2442318.81 KB |        9.16 |
| CsvHelper    | Asset | 1000000 | 2,693.69 ms |  2.54 | 667 |  247.9 | 2693.7 |  266843.22 KB |        1.00 |
