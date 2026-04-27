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
| Sep______ | Asset | 50000   |    32.97 ms |  1.00 |  29 |  885.2 |  659.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.31 ms |  0.56 |  29 | 1593.5 |  366.3 |   13.55 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    42.17 ms |  1.28 |  29 |  692.0 |  843.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    55.78 ms |  1.69 |  29 |  523.2 | 1115.5 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   116.93 ms |  3.55 |  29 |  249.6 | 2338.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   703.12 ms |  1.00 | 583 |  830.3 |  703.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   358.22 ms |  0.51 | 583 | 1629.7 |  358.2 |  271.44 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   867.57 ms |  1.23 | 583 |  672.9 |  867.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,193.79 ms |  1.70 | 583 |  489.0 | 1193.8 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,565.85 ms |  3.65 | 583 |  227.5 | 2565.8 |  260.58 MB |        1.00 |
