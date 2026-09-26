```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.33 GB Available
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
| Sep______ | Asset | 50000   |    39.92 ms |  1.00 |  33 |  833.6 |  798.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.47 ms |  0.56 |  33 | 1481.3 |  449.4 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.80 ms |  1.55 |  33 |  538.5 | 1236.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    65.13 ms |  1.63 |  33 |  511.0 | 1302.6 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   114.66 ms |  2.88 |  33 |  290.3 | 2293.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   798.02 ms |  1.00 | 665 |  834.3 |  798.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   439.11 ms |  0.55 | 665 | 1516.2 |  439.1 |  260.98 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,249.16 ms |  1.57 | 665 |  533.0 | 1249.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,374.26 ms |  1.72 | 665 |  484.5 | 1374.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,312.85 ms |  2.90 | 665 |  287.9 | 2312.9 |  260.58 MB |        1.00 |
