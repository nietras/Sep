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
| Sep______ | Asset | 50000   |    32.74 ms |  1.00 |  29 |  891.2 |  654.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.58 ms |  0.54 |  29 | 1659.5 |  351.7 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.35 ms |  1.26 |  29 |  705.6 |  827.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    53.95 ms |  1.65 |  29 |  540.9 | 1079.0 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   115.72 ms |  3.53 |  29 |  252.2 | 2314.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   686.74 ms |  1.00 | 583 |  850.1 |  686.7 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   354.50 ms |  0.52 | 583 | 1646.8 |  354.5 |  270.33 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   855.48 ms |  1.25 | 583 |  682.4 |  855.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,179.90 ms |  1.72 | 583 |  494.8 | 1179.9 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,495.53 ms |  3.63 | 583 |  233.9 | 2495.5 |  260.58 MB |        1.00 |
