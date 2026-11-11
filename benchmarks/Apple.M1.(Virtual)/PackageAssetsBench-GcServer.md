```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.08 GB Available
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
| Sep______ | Asset | 50000   |    26.64 ms |  1.00 |  29 | 1091.8 |  532.8 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    13.39 ms |  0.51 |  29 | 2172.9 |  267.7 |  13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    59.11 ms |  2.23 |  29 |  492.1 | 1182.1 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    32.02 ms |  1.21 |  29 |  908.4 |  640.4 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   540.30 ms |  1.00 | 581 | 1076.9 |  540.3 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   320.41 ms |  0.61 | 581 | 1816.1 |  320.4 | 270.58 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,952.48 ms |  3.69 | 581 |  298.0 | 1952.5 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   736.86 ms |  1.39 | 581 |  789.7 |  736.9 | 261.35 MB |        1.00 |
