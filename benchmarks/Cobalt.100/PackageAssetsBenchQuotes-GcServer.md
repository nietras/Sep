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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    38.09 ms |  1.00 |  33 |  876.3 |  761.7 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.42 ms |  0.38 |  33 | 2314.1 |  288.5 |  13.52 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.59 ms |  1.49 |  33 |  589.8 | 1131.9 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    58.64 ms |  1.54 |  33 |  569.2 | 1172.8 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   779.35 ms |  1.00 | 667 |  856.7 |  779.4 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   256.69 ms |  0.33 | 667 | 2601.2 |  256.7 |    264 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,150.86 ms |  1.48 | 667 |  580.2 | 1150.9 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 1,176.91 ms |  1.51 | 667 |  567.3 | 1176.9 | 261.35 MB |        1.00 |
