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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  16.308 ms |  1.00 |  33 | 2046.7 |  326.2 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   4.948 ms |  0.30 |  33 | 6745.9 |   99.0 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  27.510 ms |  1.69 |  33 | 1213.3 |  550.2 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  19.692 ms |  1.21 |  33 | 1695.0 |  393.8 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 329.766 ms |  1.00 | 667 | 2024.8 |  329.8 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  81.742 ms |  0.25 | 667 | 8168.5 |   81.7 |  261.8 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 578.468 ms |  1.75 | 667 | 1154.3 |  578.5 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 393.603 ms |  1.19 | 667 | 1696.4 |  393.6 | 261.35 MB |        1.00 |
