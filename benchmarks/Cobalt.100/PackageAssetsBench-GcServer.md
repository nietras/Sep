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
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    31.88 ms |  1.00 |  29 |  915.4 |  637.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.73 ms |  0.37 |  29 | 2487.6 |  234.6 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    52.96 ms |  1.66 |  29 |  551.1 | 1059.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    47.33 ms |  1.48 |  29 |  616.6 |  946.6 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    99.38 ms |  3.12 |  29 |  293.6 | 1987.6 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   632.55 ms |  1.00 | 583 |  922.9 |  632.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   207.67 ms |  0.33 | 583 | 2811.1 |  207.7 |  269.91 MB |        1.04 |
| Sylvan___ | Asset | 1000000 | 1,049.21 ms |  1.66 | 583 |  556.4 | 1049.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,013.09 ms |  1.60 | 583 |  576.2 | 1013.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,979.59 ms |  3.13 | 583 |  294.9 | 1979.6 |  260.58 MB |        1.00 |
