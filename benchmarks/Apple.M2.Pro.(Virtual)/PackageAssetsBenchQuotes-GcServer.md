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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    28.05 ms |  1.00 |  33 | 1186.6 |  560.9 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.29 ms |  0.44 |  33 | 2707.7 |  245.8 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    49.64 ms |  1.77 |  33 |  670.5 |  992.8 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    33.95 ms |  1.21 |  33 |  980.3 |  679.0 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   563.44 ms |  1.00 | 665 | 1181.7 |  563.4 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   221.98 ms |  0.39 | 665 | 2999.3 |  222.0 | 261.07 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,131.37 ms |  2.01 | 665 |  588.5 | 1131.4 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   795.53 ms |  1.41 | 665 |  836.9 |  795.5 | 261.35 MB |        1.00 |
