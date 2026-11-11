```

BenchmarkDotNet v0.16.0-preview.2, macOS Tahoe 26.7.2 (25G332) [Darwin 25.6.0]
Apple M2 Pro (Virtual), 1 CPU, 5 logical and 5 physical cores
Memory: 14 GB Total, 0.8 GB Available
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
| Sep______ | Asset | 50000   |    25.65 ms |  1.00 |  29 | 1134.0 |  513.0 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.23 ms |  0.40 |  29 | 2842.2 |  204.7 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.09 ms |  2.11 |  29 |  537.7 | 1081.8 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    30.12 ms |  1.17 |  29 |  965.7 |  602.4 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   522.54 ms |  1.00 | 581 | 1113.6 |  522.5 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   129.54 ms |  0.25 | 581 | 4491.9 |  129.5 | 267.01 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,057.15 ms |  2.03 | 581 |  550.4 | 1057.1 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   536.69 ms |  1.03 | 581 | 1084.2 |  536.7 | 261.35 MB |        1.00 |
