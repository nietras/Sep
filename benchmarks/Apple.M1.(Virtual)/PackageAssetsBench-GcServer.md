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
| Sep______ | Asset | 50000   |    23.75 ms |  1.00 |  29 | 1224.8 |  474.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.72 ms |  0.66 |  29 | 1849.8 |  314.5 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    46.67 ms |  1.97 |  29 |  623.2 |  933.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    39.71 ms |  1.68 |  29 |  732.5 |  794.2 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    75.59 ms |  3.19 |  29 |  384.8 | 1511.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   504.36 ms |  1.00 | 581 | 1153.7 |  504.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   218.29 ms |  0.43 | 581 | 2665.6 |  218.3 |  269.57 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   953.96 ms |  1.90 | 581 |  610.0 |  954.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,141.58 ms |  2.27 | 581 |  509.7 | 1141.6 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,261.53 ms |  4.49 | 581 |  257.3 | 2261.5 |  260.58 MB |        1.00 |
