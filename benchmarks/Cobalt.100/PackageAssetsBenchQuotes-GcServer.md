```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.6899) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    38.23 ms |  1.00 |  33 |  873.2 |  764.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    14.94 ms |  0.39 |  33 | 2234.7 |  298.7 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    56.75 ms |  1.48 |  33 |  588.1 | 1135.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    56.10 ms |  1.47 |  33 |  595.0 | 1122.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   105.98 ms |  2.77 |  33 |  314.9 | 2119.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   761.89 ms |  1.00 | 667 |  876.4 |  761.9 |  260.42 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   244.95 ms |  0.32 | 667 | 2725.9 |  244.9 |  262.42 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,139.57 ms |  1.50 | 667 |  585.9 | 1139.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,236.30 ms |  1.62 | 667 |  540.1 | 1236.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,151.47 ms |  2.82 | 667 |  310.3 | 2151.5 |  260.58 MB |        1.00 |
