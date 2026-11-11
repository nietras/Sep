```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 10.67 GB Available
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
| Sep______ | Asset | 50000   |    32.62 ms |  1.00 |  29 |  891.5 |  652.5 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.10 ms |  0.37 |  29 | 2404.6 |  241.9 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.06 ms |  1.66 |  29 |  538.0 | 1081.2 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    41.76 ms |  1.28 |  29 |  696.4 |  835.3 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   651.02 ms |  1.00 | 581 |  893.8 |  651.0 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   218.10 ms |  0.34 | 581 | 2668.0 |  218.1 | 266.98 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,076.77 ms |  1.65 | 581 |  540.4 | 1076.8 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   821.60 ms |  1.26 | 581 |  708.2 |  821.6 | 261.35 MB |        1.00 |
