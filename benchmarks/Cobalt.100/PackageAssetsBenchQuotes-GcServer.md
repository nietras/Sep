```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 11.72 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    36.83 ms |  1.00 |  33 |  906.2 |  736.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.13 ms |  0.41 |  33 | 2205.6 |  302.7 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    55.85 ms |  1.52 |  33 |  597.7 | 1116.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    57.20 ms |  1.55 |  33 |  583.5 | 1144.1 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   107.93 ms |  2.93 |  33 |  309.2 | 2158.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   742.58 ms |  1.00 | 667 |  899.2 |  742.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   233.53 ms |  0.31 | 667 | 2859.2 |  233.5 |  261.44 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,103.22 ms |  1.49 | 667 |  605.2 | 1103.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,194.98 ms |  1.61 | 667 |  558.8 | 1195.0 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,137.60 ms |  2.88 | 667 |  312.4 | 2137.6 |  260.58 MB |        1.00 |
