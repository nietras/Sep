```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.57 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    38.69 ms |  1.00 |  33 |  862.7 |  773.8 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    19.66 ms |  0.51 |  33 | 1697.7 |  393.2 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.44 ms |  1.59 |  33 |  543.2 | 1228.8 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    47.35 ms |  1.23 |  33 |  704.9 |  947.0 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   785.66 ms |  1.00 | 667 |  849.9 |  785.7 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   395.36 ms |  0.50 | 667 | 1688.9 |  395.4 | 266.83 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,274.77 ms |  1.62 | 667 |  523.8 | 1274.8 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   992.33 ms |  1.26 | 667 |  672.9 |  992.3 | 261.35 MB |        1.00 |
