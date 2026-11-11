```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9725/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 12.13 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    36.50 ms |  1.00 |  33 |  914.4 |  730.1 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.61 ms |  0.43 |  33 | 2138.5 |  312.2 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    55.83 ms |  1.53 |  33 |  597.8 | 1116.6 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    57.10 ms |  1.56 |  33 |  584.5 | 1142.1 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   736.25 ms |  1.00 | 667 |  906.9 |  736.2 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   239.01 ms |  0.32 | 667 | 2793.7 |  239.0 | 262.19 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,111.87 ms |  1.51 | 667 |  600.5 | 1111.9 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 1,131.49 ms |  1.54 | 667 |  590.1 | 1131.5 | 261.35 MB |        1.00 |
