```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    29.69 ms |  1.01 |  33 | 1121.0 |  593.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    23.10 ms |  0.78 |  33 | 1440.6 |  462.0 |   13.56 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    53.79 ms |  1.82 |  33 |  618.7 | 1075.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    41.70 ms |  1.41 |  33 |  798.0 |  834.1 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    86.49 ms |  2.93 |  33 |  384.8 | 1729.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   594.91 ms |  1.00 | 665 | 1119.2 |  594.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   278.37 ms |  0.47 | 665 | 2391.8 |  278.4 |  266.16 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,071.14 ms |  1.81 | 665 |  621.6 | 1071.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,694.60 ms |  2.86 | 665 |  392.9 | 1694.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,648.18 ms |  2.78 | 665 |  404.0 | 1648.2 |  260.58 MB |        1.00 |
