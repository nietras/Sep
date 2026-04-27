```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.99 ms |  1.00 |  29 |  884.5 |  659.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.91 ms |  0.54 |  29 | 1629.4 |  358.2 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.23 ms |  1.25 |  29 |  707.8 |  824.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    54.45 ms |  1.65 |  29 |  536.0 | 1088.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   115.53 ms |  3.51 |  29 |  252.6 | 2310.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   676.92 ms |  1.00 | 583 |  862.4 |  676.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   351.35 ms |  0.52 | 583 | 1661.5 |  351.4 |  270.46 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   847.65 ms |  1.25 | 583 |  688.7 |  847.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,137.48 ms |  1.68 | 583 |  513.2 | 1137.5 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,412.68 ms |  3.56 | 583 |  242.0 | 2412.7 |  260.58 MB |        1.00 |
