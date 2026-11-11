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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  16.014 ms |  1.00 |  33 | 2084.2 |  320.3 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   4.886 ms |  0.31 |  33 | 6831.7 |   97.7 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  26.843 ms |  1.68 |  33 | 1243.4 |  536.9 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  19.488 ms |  1.22 |  33 | 1712.7 |  389.8 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 329.845 ms |  1.00 | 667 | 2024.3 |  329.8 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  74.398 ms |  0.23 | 667 | 8974.8 |   74.4 | 261.54 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 564.077 ms |  1.71 | 667 | 1183.7 |  564.1 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 394.538 ms |  1.20 | 667 | 1692.4 |  394.5 | 261.35 MB |        1.00 |
