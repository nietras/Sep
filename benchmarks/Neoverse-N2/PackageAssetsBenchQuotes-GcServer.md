```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.36 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    38.08 ms |  1.00 |  33 |  874.0 |  761.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.67 ms |  0.41 |  33 | 2123.4 |  313.5 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.62 ms |  1.49 |  33 |  587.8 | 1132.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.26 ms |  1.58 |  33 |  552.3 | 1205.2 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   107.77 ms |  2.83 |  33 |  308.8 | 2155.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   748.33 ms |  1.00 | 665 |  889.7 |  748.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   249.49 ms |  0.33 | 665 | 2668.6 |  249.5 |  262.76 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,109.32 ms |  1.48 | 665 |  600.2 | 1109.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,281.14 ms |  1.71 | 665 |  519.7 | 1281.1 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,118.59 ms |  2.83 | 665 |  314.3 | 2118.6 |  260.58 MB |        1.00 |
