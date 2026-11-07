```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 40.79 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  16.294 ms |  1.00 |  33 | 2048.5 |  325.9 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   4.971 ms |  0.31 |  33 | 6714.0 |   99.4 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  27.415 ms |  1.68 |  33 | 1217.5 |  548.3 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  19.885 ms |  1.22 |  33 | 1678.6 |  397.7 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 328.104 ms |  1.00 | 667 | 2035.0 |  328.1 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  75.723 ms |  0.23 | 667 | 8817.8 |   75.7 | 261.57 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 562.611 ms |  1.71 | 667 | 1186.8 |  562.6 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 399.479 ms |  1.22 | 667 | 1671.4 |  399.5 | 261.35 MB |        1.00 |
