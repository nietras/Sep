```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.73 GB Available
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
| Sep______ | Asset | 50000   |    37.62 ms |  1.00 |  33 |  884.7 |  752.4 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.17 ms |  0.40 |  33 | 2194.2 |  303.4 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.80 ms |  1.51 |  33 |  586.0 | 1135.9 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    58.15 ms |  1.55 |  33 |  572.3 | 1163.1 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   752.13 ms |  1.00 | 665 |  885.2 |  752.1 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   238.72 ms |  0.32 | 665 | 2789.1 |  238.7 | 263.49 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,134.68 ms |  1.51 | 665 |  586.8 | 1134.7 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 1,147.00 ms |  1.53 | 665 |  580.5 | 1147.0 | 261.35 MB |        1.00 |
