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
| Sep______ | Asset | 50000   |    39.23 ms |  1.00 |  33 |  850.7 |  784.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.62 ms |  0.37 |  33 | 2283.8 |  292.3 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.71 ms |  1.45 |  33 |  588.5 | 1134.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    55.29 ms |  1.41 |  33 |  603.7 | 1105.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   107.15 ms |  2.73 |  33 |  311.5 | 2143.0 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   761.44 ms |  1.00 | 667 |  876.9 |  761.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   243.95 ms |  0.32 | 667 | 2737.1 |  243.9 |  264.16 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,139.74 ms |  1.50 | 667 |  585.8 | 1139.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,186.75 ms |  1.56 | 667 |  562.6 | 1186.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,150.80 ms |  2.82 | 667 |  310.4 | 2150.8 |  260.58 MB |        1.00 |
