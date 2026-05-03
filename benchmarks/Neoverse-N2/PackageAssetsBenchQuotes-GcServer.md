```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    38.24 ms |  1.00 |  33 |  870.3 |  764.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.18 ms |  0.40 |  33 | 2192.2 |  303.6 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    57.50 ms |  1.50 |  33 |  578.8 | 1150.0 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.28 ms |  1.58 |  33 |  552.1 | 1205.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   107.66 ms |  2.82 |  33 |  309.1 | 2153.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   773.15 ms |  1.00 | 665 |  861.2 |  773.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   251.77 ms |  0.33 | 665 | 2644.4 |  251.8 |  263.86 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,152.65 ms |  1.49 | 665 |  577.6 | 1152.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,295.12 ms |  1.68 | 665 |  514.1 | 1295.1 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,264.86 ms |  2.93 | 665 |  294.0 | 2264.9 |  260.58 MB |        1.00 |
