```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26100.33674/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.55 GB Available
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
| Sep______ | Asset | 50000   |    38.94 ms |  1.00 |  33 |  857.2 |  778.8 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.29 ms |  0.57 |  33 | 1497.2 |  445.9 |  13.56 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    73.45 ms |  1.89 |  33 |  454.4 | 1469.0 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    48.14 ms |  1.24 |  33 |  693.4 |  962.8 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   803.93 ms |  1.00 | 667 |  830.6 |  803.9 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   395.50 ms |  0.49 | 667 | 1688.2 |  395.5 | 265.97 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,310.02 ms |  1.63 | 667 |  509.7 | 1310.0 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   965.68 ms |  1.20 | 667 |  691.4 |  965.7 | 261.35 MB |        1.00 |
