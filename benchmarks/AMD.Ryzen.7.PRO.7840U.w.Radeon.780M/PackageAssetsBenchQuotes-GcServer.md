```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26100.6899/24H2/2024Update/HudsonValley)
AMD Ryzen 7 PRO 7840U w/ Radeon 780M Graphics 3.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    39.09 ms |  1.00 |  33 |  854.0 |  781.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    25.90 ms |  0.66 |  33 | 1288.9 |  517.9 |   13.58 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    59.92 ms |  1.53 |  33 |  557.0 | 1198.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.27 ms |  1.54 |  33 |  553.8 | 1205.4 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   111.54 ms |  2.85 |  33 |  299.2 | 2230.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   810.33 ms |  1.00 | 667 |  824.0 |  810.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   433.80 ms |  0.54 | 667 | 1539.2 |  433.8 |  261.43 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,216.78 ms |  1.50 | 667 |  548.8 | 1216.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,079.99 ms |  1.33 | 667 |  618.3 | 1080.0 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,275.20 ms |  2.81 | 667 |  293.5 | 2275.2 |  260.58 MB |        1.00 |
