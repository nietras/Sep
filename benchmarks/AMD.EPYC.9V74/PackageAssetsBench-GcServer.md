```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    32.10 ms |  1.00 |  29 |  909.1 |  642.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    19.00 ms |  0.59 |  29 | 1536.0 |  380.0 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    43.26 ms |  1.35 |  29 |  674.6 |  865.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    54.23 ms |  1.69 |  29 |  538.1 | 1084.6 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   120.75 ms |  3.76 |  29 |  241.7 | 2415.0 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   690.78 ms |  1.00 | 583 |  845.1 |  690.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   379.86 ms |  0.55 | 583 | 1536.9 |  379.9 |  270.21 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   878.13 ms |  1.27 | 583 |  664.8 |  878.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,178.67 ms |  1.71 | 583 |  495.3 | 1178.7 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,469.23 ms |  3.57 | 583 |  236.4 | 2469.2 |  260.58 MB |        1.00 |
