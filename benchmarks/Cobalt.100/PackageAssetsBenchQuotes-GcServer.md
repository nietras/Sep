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
| Sep______ | Asset | 50000   |    38.03 ms |  1.00 |  33 |  877.7 |  760.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.83 ms |  0.39 |  33 | 2250.1 |  296.7 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.77 ms |  1.49 |  33 |  588.0 | 1135.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    56.26 ms |  1.48 |  33 |  593.3 | 1125.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   107.47 ms |  2.83 |  33 |  310.6 | 2149.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   765.42 ms |  1.00 | 667 |  872.3 |  765.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   240.62 ms |  0.31 | 667 | 2774.9 |  240.6 |  263.88 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,136.62 ms |  1.48 | 667 |  587.4 | 1136.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,185.66 ms |  1.55 | 667 |  563.2 | 1185.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,228.35 ms |  2.91 | 667 |  299.6 | 2228.4 |  260.58 MB |        1.00 |
