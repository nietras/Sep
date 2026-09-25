```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.44 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.79 ms |  1.00 |  29 |  887.0 |  655.8 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    12.16 ms |  0.37 |  29 | 2391.7 |  243.2 |   13.56 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    53.91 ms |  1.64 |  29 |  539.5 | 1078.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    51.92 ms |  1.58 |  29 |  560.2 | 1038.4 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   100.22 ms |  3.06 |  29 |  290.2 | 2004.5 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   644.08 ms |  1.00 | 581 |  903.4 |  644.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   217.49 ms |  0.34 | 581 | 2675.4 |  217.5 |  267.86 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,060.21 ms |  1.65 | 581 |  548.8 | 1060.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,072.36 ms |  1.66 | 581 |  542.6 | 1072.4 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,995.68 ms |  3.10 | 581 |  291.6 | 1995.7 |  260.58 MB |        1.00 |
