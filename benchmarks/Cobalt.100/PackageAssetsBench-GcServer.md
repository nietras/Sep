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
| Sep______ | Asset | 50000   |    32.30 ms |  1.00 |  29 |  903.3 |  646.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.63 ms |  0.36 |  29 | 2509.7 |  232.5 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.58 ms |  1.69 |  29 |  534.6 | 1091.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    48.26 ms |  1.49 |  29 |  604.6 |  965.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    98.61 ms |  3.05 |  29 |  295.9 | 1972.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   645.55 ms |  1.00 | 583 |  904.3 |  645.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   199.49 ms |  0.31 | 583 | 2926.4 |  199.5 |  266.18 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,090.59 ms |  1.69 | 583 |  535.3 | 1090.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,001.06 ms |  1.55 | 583 |  583.2 | 1001.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,986.34 ms |  3.08 | 583 |  293.9 | 1986.3 |  260.58 MB |        1.00 |
