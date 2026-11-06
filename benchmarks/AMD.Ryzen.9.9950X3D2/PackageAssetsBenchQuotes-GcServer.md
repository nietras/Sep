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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  16.908 ms |  1.00 |  33 | 1974.1 |  338.2 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   5.241 ms |  0.31 |  33 | 6368.2 |  104.8 |  13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  28.714 ms |  1.70 |  33 | 1162.4 |  574.3 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  19.892 ms |  1.18 |  33 | 1677.9 |  397.8 |  14.41 MB |        1.07 |
|           |       |         |            |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 336.347 ms |  1.00 | 667 | 1985.2 |  336.3 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |  82.670 ms |  0.25 | 667 | 8076.8 |   82.7 |  261.8 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 575.754 ms |  1.71 | 667 | 1159.7 |  575.8 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 406.483 ms |  1.21 | 667 | 1642.6 |  406.5 | 261.35 MB |        1.00 |
