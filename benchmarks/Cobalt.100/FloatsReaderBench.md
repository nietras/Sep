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
| Sep______ | Row    | 25000 |   3.303 ms |  1.00 | 20 | 6151.0 |  132.1 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.603 ms |  7.15 | 20 |  860.9 |  944.1 |    12.12 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  16.944 ms |  5.13 | 20 | 1199.2 |  677.8 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  33.148 ms | 10.04 | 20 |  613.0 | 1325.9 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.534 ms |  1.00 | 20 | 4482.0 |  181.3 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.678 ms |  5.89 | 20 |  761.7 | 1067.1 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  17.290 ms |  3.81 | 20 | 1175.2 |  691.6 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  35.857 ms |  7.91 | 20 |  566.7 | 1434.3 | 21340.17 KB |   18,210.28 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.593 ms |  1.00 | 20 |  643.2 | 1263.7 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   8.976 ms |  0.28 | 20 | 2263.8 |  359.0 |    68.46 KB |        8.75 |
| Sylvan___ | Floats | 25000 |  89.617 ms |  2.84 | 20 |  226.7 | 3584.7 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  92.788 ms |  2.94 | 20 |  219.0 | 3711.5 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 131.115 ms |  4.15 | 20 |  155.0 | 5244.6 | 22060.98 KB |    2,820.98 |
