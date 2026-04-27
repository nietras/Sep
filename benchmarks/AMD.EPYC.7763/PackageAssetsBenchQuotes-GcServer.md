```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.99 ms |  1.00 |  33 |  834.7 |  799.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    23.79 ms |  0.59 |  33 | 1403.0 |  475.8 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.82 ms |  1.55 |  33 |  539.9 | 1236.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    62.37 ms |  1.56 |  33 |  535.2 | 1247.3 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   121.17 ms |  3.03 |  33 |  275.5 | 2423.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   850.50 ms |  1.00 | 667 |  785.1 |  850.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   438.40 ms |  0.52 | 667 | 1523.1 |  438.4 |  262.76 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,283.49 ms |  1.51 | 667 |  520.2 | 1283.5 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,362.04 ms |  1.60 | 667 |  490.2 | 1362.0 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,487.45 ms |  2.93 | 667 |  268.4 | 2487.4 |  260.59 MB |        1.00 |
