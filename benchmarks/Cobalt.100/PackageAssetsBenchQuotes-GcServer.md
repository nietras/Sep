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
| Sep______ | Asset | 50000   |    37.59 ms |  1.00 |  33 |  888.1 |  751.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.27 ms |  0.38 |  33 | 2339.4 |  285.4 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.65 ms |  1.51 |  33 |  589.2 | 1132.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    53.90 ms |  1.43 |  33 |  619.2 | 1078.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   107.11 ms |  2.85 |  33 |  311.6 | 2142.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   756.61 ms |  1.00 | 667 |  882.5 |  756.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   239.47 ms |  0.32 | 667 | 2788.2 |  239.5 |  261.15 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,138.14 ms |  1.50 | 667 |  586.7 | 1138.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,170.43 ms |  1.55 | 667 |  570.5 | 1170.4 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,150.05 ms |  2.84 | 667 |  310.6 | 2150.0 |  260.58 MB |        1.00 |
