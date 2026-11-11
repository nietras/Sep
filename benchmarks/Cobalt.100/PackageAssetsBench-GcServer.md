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
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    31.73 ms |  1.00 |  29 |  919.7 |  634.6 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    13.36 ms |  0.42 |  29 | 2183.5 |  267.3 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    53.40 ms |  1.68 |  29 |  546.5 | 1068.0 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    40.93 ms |  1.29 |  29 |  713.0 |  818.5 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   637.89 ms |  1.00 | 583 |  915.2 |  637.9 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   208.12 ms |  0.33 | 583 | 2805.0 |  208.1 | 269.06 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,061.57 ms |  1.66 | 583 |  549.9 | 1061.6 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   816.24 ms |  1.28 | 583 |  715.2 |  816.2 | 261.35 MB |        1.00 |
