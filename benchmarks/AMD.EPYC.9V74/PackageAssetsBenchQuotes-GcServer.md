```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.46 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |    29.64 ms |  1.00 |  33 | 1123.1 |  592.7 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.79 ms |  0.57 |  33 | 1982.1 |  335.8 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    50.20 ms |  1.70 |  33 |  663.0 | 1004.0 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    32.36 ms |  1.10 |  33 | 1028.4 |  647.2 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   607.02 ms |  1.00 | 665 | 1096.8 |  607.0 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   332.58 ms |  0.55 | 665 | 2001.9 |  332.6 | 266.13 MB |        1.02 |
| Sylvan___ | Asset | 1000000 | 1,017.32 ms |  1.68 | 665 |  654.5 | 1017.3 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   646.31 ms |  1.07 | 665 | 1030.2 |  646.3 | 261.35 MB |        1.00 |
