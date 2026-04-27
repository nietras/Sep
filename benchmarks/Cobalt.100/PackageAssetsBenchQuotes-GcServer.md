```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.30 ms |  1.00 |  33 |  849.4 |  785.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.84 ms |  0.38 |  33 | 2249.8 |  296.7 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.87 ms |  1.45 |  33 |  586.9 | 1137.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    55.58 ms |  1.41 |  33 |  600.6 | 1111.6 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   106.27 ms |  2.70 |  33 |  314.1 | 2125.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   761.88 ms |  1.00 | 667 |  876.4 |  761.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   242.18 ms |  0.32 | 667 | 2757.1 |  242.2 |  263.78 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,136.14 ms |  1.49 | 667 |  587.7 | 1136.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,182.92 ms |  1.55 | 667 |  564.5 | 1182.9 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,155.27 ms |  2.83 | 667 |  309.8 | 2155.3 |  260.58 MB |        1.00 |
