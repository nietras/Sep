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
| Sep______ | Asset | 50000   |    32.17 ms |  1.00 |  29 |  907.1 |  643.4 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.47 ms |  0.36 |  29 | 2545.0 |  229.3 |   13.55 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    54.31 ms |  1.69 |  29 |  537.3 | 1086.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    48.36 ms |  1.50 |  29 |  603.5 |  967.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    98.88 ms |  3.07 |  29 |  295.1 | 1977.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   645.40 ms |  1.00 | 583 |  904.5 |  645.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   199.15 ms |  0.31 | 583 | 2931.3 |  199.2 |  267.03 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,096.08 ms |  1.70 | 583 |  532.6 | 1096.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   976.99 ms |  1.51 | 583 |  597.5 |  977.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,981.37 ms |  3.07 | 583 |  294.6 | 1981.4 |  260.58 MB |        1.00 |
