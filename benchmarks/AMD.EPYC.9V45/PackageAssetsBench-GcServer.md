```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V45 4.06GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  20.11 ms |  1.00 |  29 | 1446.3 |  402.2 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  10.37 ms |  0.52 |  29 | 2804.2 |  207.4 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |  27.14 ms |  1.35 |  29 | 1071.8 |  542.8 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  22.60 ms |  1.12 |  29 | 1286.8 |  452.1 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 396.28 ms |  1.00 | 581 | 1468.3 |  396.3 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 214.04 ms |  0.54 | 581 | 2718.5 |  214.0 | 270.21 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 550.83 ms |  1.39 | 581 | 1056.4 |  550.8 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 447.11 ms |  1.13 | 581 | 1301.4 |  447.1 | 261.35 MB |        1.00 |
