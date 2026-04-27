```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
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
| Sep______ | Asset | 50000   |    32.41 ms |  1.00 |  29 |  900.4 |  648.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.78 ms |  0.33 |  29 | 2708.2 |  215.5 |   13.56 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    54.55 ms |  1.68 |  29 |  534.9 | 1091.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    48.64 ms |  1.50 |  29 |  599.9 |  972.8 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    98.87 ms |  3.05 |  29 |  295.2 | 1977.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   651.78 ms |  1.00 | 583 |  895.7 |  651.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   198.68 ms |  0.30 | 583 | 2938.3 |  198.7 |  267.48 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,093.20 ms |  1.68 | 583 |  534.0 | 1093.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,015.27 ms |  1.56 | 583 |  575.0 | 1015.3 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,982.90 ms |  3.04 | 583 |  294.4 | 1982.9 |  260.58 MB |        1.00 |
