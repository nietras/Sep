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
| Sep______ | Asset | 50000   |    31.62 ms |  1.00 |  29 |  923.0 |  632.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    10.47 ms |  0.33 |  29 | 2785.9 |  209.5 |   13.55 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    54.09 ms |  1.71 |  29 |  539.5 | 1081.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    47.75 ms |  1.51 |  29 |  611.1 |  955.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    98.54 ms |  3.12 |  29 |  296.1 | 1970.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   646.20 ms |  1.00 | 583 |  903.4 |  646.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   201.86 ms |  0.31 | 583 | 2892.1 |  201.9 |  266.81 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,069.24 ms |  1.65 | 583 |  546.0 | 1069.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   975.61 ms |  1.51 | 583 |  598.4 |  975.6 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,968.24 ms |  3.05 | 583 |  296.6 | 1968.2 |  260.58 MB |        1.00 |
