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
| Sep______ | Asset | 50000   |    29.20 ms |  1.01 |  29 |  996.2 |  584.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.74 ms |  0.65 |  29 | 1551.8 |  374.9 |   13.61 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    71.80 ms |  2.48 |  29 |  405.1 | 1436.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    52.02 ms |  1.79 |  29 |  559.1 | 1040.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   104.88 ms |  3.62 |  29 |  277.3 | 2097.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   950.62 ms |  1.02 | 581 |  612.1 |  950.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   287.80 ms |  0.31 | 581 | 2021.8 |  287.8 |  269.01 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,205.79 ms |  1.29 | 581 |  482.6 | 1205.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,095.19 ms |  1.17 | 581 |  531.3 | 1095.2 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,858.72 ms |  1.99 | 581 |  313.1 | 1858.7 |  260.58 MB |        1.00 |
