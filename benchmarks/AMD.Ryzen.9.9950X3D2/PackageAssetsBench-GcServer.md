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
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|--------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  14.352 ms |  1.00 |  29 |  2033.3 |  287.0 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   2.929 ms |  0.20 |  29 |  9962.4 |   58.6 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  18.996 ms |  1.32 |  29 |  1536.2 |  379.9 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  16.613 ms |  1.16 |  29 |  1756.5 |  332.3 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |         |        |           |             |
| Sep______ | Asset | 1000000 | 288.949 ms |  1.00 | 583 |  2020.4 |  288.9 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  53.260 ms |  0.18 | 583 | 10961.1 |   53.3 |  261.6 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 384.564 ms |  1.33 | 583 |  1518.0 |  384.6 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 341.865 ms |  1.18 | 583 |  1707.6 |  341.9 | 261.35 MB |        1.00 |
