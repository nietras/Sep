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
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    32.64 ms |  1.00 |  29 |  891.2 |  652.7 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.24 ms |  0.38 |  29 | 2376.5 |  244.8 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.02 ms |  1.66 |  29 |  538.5 | 1080.3 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    41.61 ms |  1.28 |  29 |  699.0 |  832.2 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   655.38 ms |  1.00 | 581 |  887.8 |  655.4 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   215.22 ms |  0.33 | 581 | 2703.7 |  215.2 | 266.99 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,092.67 ms |  1.67 | 581 |  532.5 | 1092.7 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   824.72 ms |  1.26 | 581 |  705.5 |  824.7 | 261.35 MB |        1.00 |
