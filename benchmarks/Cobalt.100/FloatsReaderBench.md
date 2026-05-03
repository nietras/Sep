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
| Sep______ | Row    | 25000 |   3.286 ms |  1.00 | 20 | 6183.0 |  131.5 |     1.18 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.213 ms |  7.06 | 20 |  875.4 |  928.5 |    12.12 KB |       10.28 |
| ReadLine_ | Row    | 25000 |  16.411 ms |  4.99 | 20 | 1238.2 |  656.4 | 73489.62 KB |   62,295.83 |
| CsvHelper | Row    | 25000 |  33.035 ms | 10.05 | 20 |  615.1 | 1321.4 |    20.02 KB |       16.97 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.368 ms |  1.00 | 20 | 4651.7 |  174.7 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.315 ms |  6.03 | 20 |  772.2 | 1052.6 |    12.13 KB |       10.28 |
| ReadLine_ | Cols   | 25000 |  16.860 ms |  3.86 | 20 | 1205.2 |  674.4 | 73489.62 KB |   62,295.83 |
| CsvHelper | Cols   | 25000 |  36.011 ms |  8.25 | 20 |  564.3 | 1440.4 | 21340.23 KB |   18,089.74 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.338 ms |  1.00 | 20 |  648.4 | 1253.5 |     7.83 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  10.827 ms |  0.35 | 20 | 1876.8 |  433.1 |    68.47 KB |        8.75 |
| Sylvan___ | Floats | 25000 |  87.814 ms |  2.80 | 20 |  231.4 | 3512.6 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  91.864 ms |  2.93 | 20 |  221.2 | 3674.6 | 73492.94 KB |    9,388.32 |
| CsvHelper | Floats | 25000 | 129.036 ms |  4.12 | 20 |  157.5 | 5161.4 | 22060.98 KB |    2,818.17 |
