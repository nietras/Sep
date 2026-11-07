```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 52.22 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|--------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  14.279 ms |  1.00 |  29 |  2043.6 |  285.6 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   3.225 ms |  0.23 |  29 |  9048.1 |   64.5 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  18.796 ms |  1.32 |  29 |  1552.5 |  375.9 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  16.415 ms |  1.15 |  29 |  1777.7 |  328.3 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |         |        |           |             |
| Sep______ | Asset | 1000000 | 288.099 ms |  1.00 | 583 |  2026.3 |  288.1 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  52.582 ms |  0.18 | 583 | 11102.4 |   52.6 | 261.54 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 378.647 ms |  1.31 | 583 |  1541.8 |  378.6 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 336.604 ms |  1.17 | 583 |  1734.3 |  336.6 | 261.35 MB |        1.00 |
