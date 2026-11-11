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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    30.34 ms |  1.00 |  33 | 1097.0 |  606.8 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.79 ms |  0.62 |  33 | 1771.4 |  375.8 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    58.28 ms |  1.94 |  33 |  571.1 | 1165.5 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    40.57 ms |  1.35 |  33 |  820.4 |  811.3 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   750.59 ms |  1.00 | 665 |  887.0 |  750.6 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   372.38 ms |  0.50 | 665 | 1787.9 |  372.4 | 265.55 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,261.99 ms |  1.70 | 665 |  527.6 | 1262.0 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   919.25 ms |  1.24 | 665 |  724.3 |  919.2 | 261.35 MB |        1.00 |
