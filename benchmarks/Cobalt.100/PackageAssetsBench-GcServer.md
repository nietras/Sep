```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.40 ms |  1.00 |  29 |  900.7 |  648.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.38 ms |  0.35 |  29 | 2564.0 |  227.6 |   13.55 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    54.86 ms |  1.69 |  29 |  531.9 | 1097.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    51.10 ms |  1.58 |  29 |  571.0 | 1022.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    99.01 ms |  3.06 |  29 |  294.7 | 1980.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   644.64 ms |  1.00 | 583 |  905.6 |  644.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   200.36 ms |  0.31 | 583 | 2913.7 |  200.4 |   267.4 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,103.44 ms |  1.71 | 583 |  529.1 | 1103.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,020.39 ms |  1.58 | 583 |  572.1 | 1020.4 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,991.24 ms |  3.09 | 583 |  293.2 | 1991.2 |  260.58 MB |        1.00 |
