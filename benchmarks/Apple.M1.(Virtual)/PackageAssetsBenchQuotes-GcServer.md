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
| Sep______ | Asset | 50000   |    25.99 ms |  1.00 |  33 | 1280.6 |  519.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.90 ms |  0.46 |  33 | 2797.0 |  238.0 |    13.6 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    45.83 ms |  1.77 |  33 |  726.2 |  916.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    35.97 ms |  1.39 |  33 |  925.3 |  719.4 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    76.69 ms |  2.95 |  33 |  434.0 | 1533.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   542.50 ms |  1.00 | 665 | 1227.3 |  542.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   239.81 ms |  0.44 | 665 | 2776.4 |  239.8 |  266.31 MB |        1.02 |
| Sylvan___ | Asset | 1000000 |   961.86 ms |  1.77 | 665 |  692.2 |  961.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   929.89 ms |  1.71 | 665 |  716.0 |  929.9 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,584.75 ms |  2.92 | 665 |  420.1 | 1584.8 |  260.58 MB |        1.00 |
