```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    21.545 ms |  1.00 |  29 | 1350.0 |  430.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     9.889 ms |  0.46 |  29 | 2941.4 |  197.8 |   13.59 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    43.137 ms |  2.00 |  29 |  674.3 |  862.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    28.729 ms |  1.33 |  29 | 1012.4 |  574.6 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    75.601 ms |  3.51 |  29 |  384.7 | 1512.0 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   457.098 ms |  1.00 | 581 | 1273.0 |  457.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   224.967 ms |  0.49 | 581 | 2586.5 |  225.0 |  268.14 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   894.656 ms |  1.96 | 581 |  650.4 |  894.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   792.175 ms |  1.73 | 581 |  734.5 |  792.2 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,462.667 ms |  3.20 | 581 |  397.8 | 1462.7 |  260.58 MB |        1.00 |
