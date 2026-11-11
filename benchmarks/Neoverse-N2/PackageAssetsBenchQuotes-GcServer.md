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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    37.21 ms |  1.00 |  33 |  894.4 |  744.3 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.02 ms |  0.40 |  33 | 2215.7 |  300.4 |  13.52 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.39 ms |  1.52 |  33 |  590.2 | 1127.9 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    57.64 ms |  1.55 |  33 |  577.4 | 1152.8 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   747.45 ms |  1.00 | 665 |  890.8 |  747.5 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   237.41 ms |  0.32 | 665 | 2804.4 |  237.4 | 262.24 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,125.41 ms |  1.51 | 665 |  591.6 | 1125.4 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 1,140.24 ms |  1.53 | 665 |  583.9 | 1140.2 | 261.35 MB |        1.00 |
