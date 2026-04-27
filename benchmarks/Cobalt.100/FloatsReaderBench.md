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
| Sep______ | Row    | 25000 |   3.256 ms |  1.00 | 20 | 6240.7 |  130.2 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.476 ms |  7.21 | 20 |  865.6 |  939.0 |    12.12 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  16.670 ms |  5.12 | 20 | 1218.9 |  666.8 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  32.893 ms | 10.10 | 20 |  617.7 | 1315.7 |    20.02 KB |       17.08 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.660 ms |  1.00 | 20 | 4360.7 |  186.4 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.591 ms |  5.71 | 20 |  764.2 | 1063.6 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  17.372 ms |  3.73 | 20 | 1169.7 |  694.9 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  36.720 ms |  7.88 | 20 |  553.4 | 1468.8 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.543 ms |  1.00 | 20 |  644.2 | 1261.7 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.061 ms |  0.29 | 20 | 2242.6 |  362.4 |    69.05 KB |        8.83 |
| Sylvan___ | Floats | 25000 |  87.301 ms |  2.77 | 20 |  232.8 | 3492.0 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  94.134 ms |  2.98 | 20 |  215.9 | 3765.4 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 132.317 ms |  4.19 | 20 |  153.6 | 5292.7 | 22060.98 KB |    2,820.98 |
