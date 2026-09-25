```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.4 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    26.53 ms |  1.00 |  29 | 1096.4 |  530.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    13.78 ms |  0.52 |  29 | 2111.3 |  275.5 |   13.55 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    34.01 ms |  1.29 |  29 |  855.3 |  680.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    46.88 ms |  1.78 |  29 |  620.4 |  937.6 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    94.36 ms |  3.57 |  29 |  308.2 | 1887.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   523.78 ms |  1.00 | 581 | 1110.9 |  523.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   303.46 ms |  0.58 | 581 | 1917.5 |  303.5 |  269.85 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   680.82 ms |  1.30 | 581 |  854.7 |  680.8 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,049.81 ms |  2.01 | 581 |  554.3 | 1049.8 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,905.24 ms |  3.64 | 581 |  305.4 | 1905.2 |  260.58 MB |        1.00 |
