```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 44.57 GB Available
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
| Sep______ | Asset | 50000   |  14.467 ms |  1.00 |  29 |  2017.1 |  289.3 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   3.252 ms |  0.22 |  29 |  8972.6 |   65.0 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  19.041 ms |  1.32 |  29 |  1532.5 |  380.8 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  16.711 ms |  1.16 |  29 |  1746.2 |  334.2 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |         |        |           |             |
| Sep______ | Asset | 1000000 | 290.182 ms |  1.00 | 583 |  2011.8 |  290.2 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  55.126 ms |  0.19 | 583 | 10590.0 |   55.1 | 261.59 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 382.092 ms |  1.32 | 583 |  1527.9 |  382.1 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 337.641 ms |  1.16 | 583 |  1729.0 |  337.6 | 261.35 MB |        1.00 |
