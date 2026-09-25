```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.99 GB Total, 12.59 GB Available
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
| Sep______ | Asset | 50000   |    33.50 ms |  1.00 |  29 |  871.1 |  670.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.43 ms |  0.52 |  29 | 1673.9 |  348.7 |   13.57 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    42.40 ms |  1.27 |  29 |  688.3 |  847.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    50.90 ms |  1.52 |  29 |  573.3 | 1018.0 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   120.87 ms |  3.62 |  29 |  241.4 | 2417.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   700.43 ms |  1.00 | 583 |  833.5 |  700.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   364.37 ms |  0.52 | 583 | 1602.2 |  364.4 |  271.08 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   852.09 ms |  1.22 | 583 |  685.1 |  852.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,150.07 ms |  1.64 | 583 |  507.6 | 1150.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,387.32 ms |  3.41 | 583 |  244.5 | 2387.3 |  260.58 MB |        1.00 |
