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
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    21.28 ms |  1.00 |  29 | 1367.0 |  425.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.89 ms |  0.51 |  29 | 2671.8 |  217.7 |   13.61 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    44.55 ms |  2.10 |  29 |  652.9 |  891.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    31.85 ms |  1.50 |  29 |  913.1 |  637.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    70.45 ms |  3.31 |  29 |  412.8 | 1409.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   457.17 ms |  1.00 | 581 | 1272.8 |  457.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   236.47 ms |  0.52 | 581 | 2460.6 |  236.5 |  270.01 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   902.79 ms |  1.97 | 581 |  644.5 |  902.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   807.28 ms |  1.77 | 581 |  720.8 |  807.3 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,481.92 ms |  3.24 | 581 |  392.6 | 1481.9 |  260.58 MB |        1.00 |
