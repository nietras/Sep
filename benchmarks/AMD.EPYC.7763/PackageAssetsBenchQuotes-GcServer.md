```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 3.08GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.38 GB Available
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
| Sep______ | Asset | 50000   |    38.88 ms |  1.00 |  33 |  856.0 |  777.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.32 ms |  0.58 |  33 | 1490.9 |  446.5 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    61.87 ms |  1.59 |  33 |  538.0 | 1237.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.34 ms |  1.53 |  33 |  560.8 | 1186.9 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.85 ms |  3.06 |  33 |  280.0 | 2377.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   784.00 ms |  1.00 | 665 |  849.2 |  784.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   425.13 ms |  0.54 | 665 | 1566.1 |  425.1 |   263.7 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,249.05 ms |  1.59 | 665 |  533.0 | 1249.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,364.59 ms |  1.74 | 665 |  487.9 | 1364.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,397.41 ms |  3.06 | 665 |  277.7 | 2397.4 |  260.58 MB |        1.00 |
