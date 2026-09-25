```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
Memory: 61.61 GB Total, 52.15 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean       | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-----------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |  18.010 ms |  1.00 |  33 | 1853.3 |  360.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |   6.742 ms |  0.37 |  33 | 4950.8 |  134.8 |   13.64 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  28.010 ms |  1.56 |  33 | 1191.6 |  560.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |  24.633 ms |  1.37 |  33 | 1355.0 |  492.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |  48.301 ms |  2.68 |  33 |  691.0 |  966.0 |   13.64 MB |        1.01 |
|           |       |         |            |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 | 365.281 ms |  1.00 | 667 | 1827.9 |  365.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 107.768 ms |  0.30 | 667 | 6195.8 |  107.8 |  261.79 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 571.951 ms |  1.57 | 667 | 1167.4 |  572.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 512.824 ms |  1.40 | 667 | 1302.0 |  512.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 970.257 ms |  2.66 | 667 |  688.2 |  970.3 |  260.58 MB |        1.00 |
