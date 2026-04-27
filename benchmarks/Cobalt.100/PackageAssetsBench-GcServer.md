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
| Sep______ | Asset | 50000   |    33.38 ms |  1.00 |  29 |  874.3 |  667.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.73 ms |  0.35 |  29 | 2486.7 |  234.7 |   13.55 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    54.51 ms |  1.63 |  29 |  535.4 | 1090.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    50.29 ms |  1.51 |  29 |  580.2 | 1005.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    99.47 ms |  2.98 |  29 |  293.4 | 1989.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   645.97 ms |  1.00 | 583 |  903.7 |  646.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   199.20 ms |  0.31 | 583 | 2930.6 |  199.2 |  266.53 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,097.74 ms |  1.70 | 583 |  531.8 | 1097.7 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,010.49 ms |  1.56 | 583 |  577.7 | 1010.5 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,089.33 ms |  3.23 | 583 |  279.4 | 2089.3 |  260.58 MB |        1.00 |
