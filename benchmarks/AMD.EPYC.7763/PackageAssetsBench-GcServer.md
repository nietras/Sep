```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.61 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.59 ms |  1.00 |  29 |  868.8 |  671.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.41 ms |  0.52 |  29 | 1676.1 |  348.2 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    41.66 ms |  1.24 |  29 |  700.5 |  833.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    53.31 ms |  1.59 |  29 |  547.3 | 1066.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   119.11 ms |  3.55 |  29 |  245.0 | 2382.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   684.30 ms |  1.00 | 583 |  853.1 |  684.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   352.53 ms |  0.52 | 583 | 1656.0 |  352.5 |  272.73 MB |        1.05 |
| Sylvan___ | Asset | 1000000 |   844.88 ms |  1.24 | 583 |  691.0 |  844.9 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,138.81 ms |  1.66 | 583 |  512.6 | 1138.8 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,334.39 ms |  3.41 | 583 |  250.1 | 2334.4 |  260.58 MB |        1.00 |
