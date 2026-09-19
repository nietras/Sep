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
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s    | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|--------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    14.645 ms |  1.00 |  29 |  1992.6 |  292.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     3.216 ms |  0.22 |  29 |  9074.6 |   64.3 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    18.847 ms |  1.29 |  29 |  1548.3 |  376.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    18.680 ms |  1.28 |  29 |  1562.2 |  373.6 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    50.322 ms |  3.44 |  29 |   579.9 | 1006.4 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |         |        |            |             |
| Sep______ | Asset | 1000000 |   294.100 ms |  1.00 | 583 |  1985.0 |  294.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |    52.370 ms |  0.18 | 583 | 11147.3 |   52.4 |  261.64 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   379.257 ms |  1.29 | 583 |  1539.3 |  379.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   463.493 ms |  1.58 | 583 |  1259.5 |  463.5 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,026.722 ms |  3.49 | 583 |   568.6 | 1026.7 |  260.58 MB |        1.00 |
