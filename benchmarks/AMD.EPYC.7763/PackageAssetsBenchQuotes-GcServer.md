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
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    41.21 ms |  1.00 |  33 |  810.0 |  824.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.59 ms |  0.55 |  33 | 1477.6 |  451.8 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    60.61 ms |  1.47 |  33 |  550.7 | 1212.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    61.48 ms |  1.49 |  33 |  542.9 | 1229.5 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   121.22 ms |  2.95 |  33 |  275.3 | 2424.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   818.30 ms |  1.00 | 667 |  816.0 |  818.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   422.87 ms |  0.52 | 667 | 1579.0 |  422.9 |  262.63 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,301.18 ms |  1.59 | 667 |  513.2 | 1301.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,334.24 ms |  1.63 | 667 |  500.4 | 1334.2 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,428.04 ms |  2.97 | 667 |  275.0 | 2428.0 |  260.58 MB |        1.00 |
