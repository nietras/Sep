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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.36 ms |  1.00 |  33 |  826.9 |  807.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    23.52 ms |  0.58 |  33 | 1418.9 |  470.5 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.89 ms |  1.54 |  33 |  539.3 | 1237.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    62.68 ms |  1.56 |  33 |  532.5 | 1253.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   124.47 ms |  3.09 |  33 |  268.2 | 2489.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   881.30 ms |  1.00 | 667 |  757.6 |  881.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   444.24 ms |  0.50 | 667 | 1503.0 |  444.2 |  262.43 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,495.49 ms |  1.70 | 667 |  446.5 | 1495.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,301.09 ms |  1.48 | 667 |  513.2 | 1301.1 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,450.31 ms |  2.79 | 667 |  272.5 | 2450.3 |  260.58 MB |        1.00 |
