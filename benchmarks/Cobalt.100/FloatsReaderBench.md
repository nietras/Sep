```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.282 ms |  1.00 | 20 | 6191.7 |  131.3 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.198 ms |  7.07 | 20 |  875.9 |  927.9 |    12.12 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  16.428 ms |  5.01 | 20 | 1236.9 |  657.1 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  32.597 ms |  9.93 | 20 |  623.4 | 1303.9 |    20.02 KB |       17.08 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.549 ms |  1.00 | 20 | 4466.9 |  182.0 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.387 ms |  5.80 | 20 |  770.1 | 1055.5 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  16.858 ms |  3.71 | 20 | 1205.4 |  674.3 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  35.752 ms |  7.86 | 20 |  568.3 | 1430.1 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.525 ms |  1.00 | 20 |  644.6 | 1261.0 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.971 ms |  0.28 | 20 | 2265.0 |  358.8 |    68.46 KB |        8.75 |
| Sylvan___ | Floats | 25000 |  90.318 ms |  2.86 | 20 |  225.0 | 3612.7 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  93.840 ms |  2.98 | 20 |  216.5 | 3753.6 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 127.440 ms |  4.04 | 20 |  159.4 | 5097.6 | 22060.98 KB |    2,820.98 |
