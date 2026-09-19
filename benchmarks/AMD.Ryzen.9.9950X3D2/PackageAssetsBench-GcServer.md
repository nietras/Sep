```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9445/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 9950X3D2 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.401
  [Host]    : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|--------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    14.432 ms |  1.00 |  29 |  2022.0 |  288.6 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     3.236 ms |  0.22 |  29 |  9016.6 |   64.7 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    18.741 ms |  1.30 |  29 |  1557.0 |  374.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    15.347 ms |  1.06 |  29 |  1901.4 |  306.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    50.694 ms |  3.51 |  29 |   575.6 | 1013.9 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |         |        |            |             |
| Sep______ | Asset | 1000000 |   293.346 ms |  1.00 | 583 |  1990.1 |  293.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |    57.195 ms |  0.19 | 583 | 10206.9 |   57.2 |  262.03 MB |        1.01 |
| Sylvan___ | Asset | 1000000 |   374.570 ms |  1.28 | 583 |  1558.5 |  374.6 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   461.250 ms |  1.57 | 583 |  1265.7 |  461.3 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,020.732 ms |  3.48 | 583 |   571.9 | 1020.7 |  260.58 MB |        1.00 |
