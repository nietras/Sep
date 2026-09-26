```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 12.01 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.20 ms |  1.00 |  29 |  879.0 |  663.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.55 ms |  0.35 |  29 | 2525.8 |  231.1 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    54.22 ms |  1.64 |  29 |  538.2 | 1084.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    46.69 ms |  1.41 |  29 |  625.0 |  933.8 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   102.02 ms |  3.08 |  29 |  286.0 | 2040.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   672.56 ms |  1.00 | 583 |  868.0 |  672.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   200.18 ms |  0.30 | 583 | 2916.3 |  200.2 |  268.61 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,090.37 ms |  1.62 | 583 |  535.4 | 1090.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   949.98 ms |  1.41 | 583 |  614.5 |  950.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,036.60 ms |  3.03 | 583 |  286.6 | 2036.6 |  260.58 MB |        1.00 |
