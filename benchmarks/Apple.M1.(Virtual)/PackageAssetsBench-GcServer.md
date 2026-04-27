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
| Sep______ | Asset | 50000   |    23.94 ms |  1.00 |  29 | 1215.1 |  478.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.31 ms |  0.64 |  29 | 1899.6 |  306.2 |   13.63 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    48.74 ms |  2.04 |  29 |  596.8 |  974.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    36.36 ms |  1.52 |  29 |  799.9 |  727.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    77.56 ms |  3.25 |  29 |  375.0 | 1551.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   505.54 ms |  1.00 | 581 | 1151.0 |  505.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   205.23 ms |  0.41 | 581 | 2835.2 |  205.2 |   268.2 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   994.27 ms |  1.97 | 581 |  585.2 |  994.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,054.39 ms |  2.09 | 581 |  551.9 | 1054.4 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,628.92 ms |  3.23 | 581 |  357.2 | 1628.9 |  260.58 MB |        1.00 |
