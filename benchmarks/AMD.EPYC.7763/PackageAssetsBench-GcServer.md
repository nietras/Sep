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
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  33.02 ms |  1.00 |  29 |  883.8 |  660.3 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  17.17 ms |  0.52 |  29 | 1699.2 |  343.5 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |  42.29 ms |  1.28 |  29 |  689.9 |  845.9 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  42.48 ms |  1.29 |  29 |  686.9 |  849.7 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 682.71 ms |  1.00 | 583 |  855.1 |  682.7 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 348.21 ms |  0.51 | 583 | 1676.5 |  348.2 | 271.82 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 867.24 ms |  1.27 | 583 |  673.1 |  867.2 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 810.63 ms |  1.19 | 583 |  720.2 |  810.6 | 261.35 MB |        1.00 |
