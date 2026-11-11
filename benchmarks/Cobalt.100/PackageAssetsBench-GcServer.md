```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26200.9725/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.96 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    33.39 ms |  1.00 |  29 |  874.0 |  667.8 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.61 ms |  0.38 |  29 | 2313.7 |  252.2 |  13.52 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.41 ms |  1.63 |  29 |  536.3 | 1088.2 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    42.91 ms |  1.29 |  29 |  680.1 |  858.1 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   680.29 ms |  1.00 | 583 |  858.1 |  680.3 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   206.23 ms |  0.30 | 583 | 2830.7 |  206.2 |  268.3 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,110.63 ms |  1.63 | 583 |  525.6 | 1110.6 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   858.77 ms |  1.26 | 583 |  679.8 |  858.8 | 261.35 MB |        1.00 |
