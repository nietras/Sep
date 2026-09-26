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
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.81 ms |  1.00 |  29 |  886.5 |  656.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.31 ms |  0.53 |  29 | 1679.9 |  346.3 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    44.23 ms |  1.35 |  29 |  657.6 |  884.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    57.96 ms |  1.77 |  29 |  501.8 | 1159.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   120.04 ms |  3.67 |  29 |  242.3 | 2400.8 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   668.77 ms |  1.00 | 581 |  870.1 |  668.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   351.26 ms |  0.53 | 581 | 1656.5 |  351.3 |     270 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   860.09 ms |  1.29 | 581 |  676.5 |  860.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,239.41 ms |  1.85 | 581 |  469.5 | 1239.4 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,409.73 ms |  3.61 | 581 |  241.5 | 2409.7 |  260.58 MB |        1.00 |
