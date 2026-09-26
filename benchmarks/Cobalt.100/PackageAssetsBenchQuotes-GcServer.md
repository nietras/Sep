```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 12.01 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    38.97 ms |  1.00 |  33 |  856.5 |  779.4 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.52 ms |  0.37 |  33 | 2298.1 |  290.5 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.74 ms |  1.46 |  33 |  588.2 | 1134.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    56.94 ms |  1.46 |  33 |  586.2 | 1138.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   107.35 ms |  2.76 |  33 |  310.9 | 2146.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   779.05 ms |  1.00 | 667 |  857.1 |  779.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   259.68 ms |  0.33 | 667 | 2571.2 |  259.7 |  262.33 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,135.53 ms |  1.46 | 667 |  588.0 | 1135.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,159.73 ms |  1.49 | 667 |  575.7 | 1159.7 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,145.29 ms |  2.75 | 667 |  311.2 | 2145.3 |  260.58 MB |        1.00 |
