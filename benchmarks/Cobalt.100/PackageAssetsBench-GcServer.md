```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.6899) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.59 ms |  1.00 |  29 |  895.3 |  651.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.62 ms |  0.36 |  29 | 2510.8 |  232.4 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.88 ms |  1.68 |  29 |  531.8 | 1097.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    49.51 ms |  1.52 |  29 |  589.4 |  990.2 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    99.62 ms |  3.06 |  29 |  292.9 | 1992.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   650.92 ms |  1.00 | 583 |  896.9 |  650.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   200.74 ms |  0.31 | 583 | 2908.2 |  200.7 |  266.53 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,104.81 ms |  1.70 | 583 |  528.4 | 1104.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,025.26 ms |  1.58 | 583 |  569.4 | 1025.3 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,089.05 ms |  3.21 | 583 |  279.4 | 2089.1 |  260.58 MB |        1.00 |
