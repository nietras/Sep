```

BenchmarkDotNet v0.16.0-preview.2, Windows 11 (10.0.26300.9711)
AMD Ryzen 9 9950X3D2 4.29GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 53.15 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|--------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  14.044 ms |  1.00 |  29 |  2077.8 |  280.9 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   2.919 ms |  0.21 |  29 |  9997.3 |   58.4 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  18.849 ms |  1.34 |  29 |  1548.1 |  377.0 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  16.296 ms |  1.16 |  29 |  1790.6 |  325.9 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |         |        |           |             |
| Sep______ | Asset | 1000000 | 281.728 ms |  1.00 | 583 |  2072.2 |  281.7 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  50.774 ms |  0.18 | 583 | 11497.6 |   50.8 |  261.6 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 376.077 ms |  1.33 | 583 |  1552.3 |  376.1 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 333.274 ms |  1.18 | 583 |  1751.7 |  333.3 | 261.35 MB |        1.00 |
