```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.72 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.344 ms |  1.00 | 20 | 6076.2 |  133.8 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.128 ms |  6.92 | 20 |  878.6 |  925.1 |    12.12 KB |       10.34 |
| ReadLine_ | Row    | 25000 |  17.099 ms |  5.12 | 20 | 1188.3 |  684.0 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  34.571 ms | 10.34 | 20 |  587.8 | 1382.8 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.482 ms |  1.00 | 20 | 4533.6 |  179.3 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.091 ms |  5.82 | 20 |  778.8 | 1043.6 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  17.487 ms |  3.90 | 20 | 1162.0 |  699.5 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  36.442 ms |  8.13 | 20 |  557.6 | 1457.7 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.418 ms |  1.00 | 20 |  646.7 | 1256.7 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.283 ms |  0.30 | 20 | 2189.0 |  371.3 |     73.4 KB |        9.39 |
| Sylvan___ | Floats | 25000 |  85.405 ms |  2.72 | 20 |  237.9 | 3416.2 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  89.176 ms |  2.84 | 20 |  227.9 | 3567.0 | 73492.93 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 128.405 ms |  4.09 | 20 |  158.2 | 5136.2 | 22061.02 KB |    2,820.99 |
