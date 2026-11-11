```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.68GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.45 GB Available
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
| Sep______ | Asset | 50000   |    30.15 ms |  1.00 |  33 | 1103.8 |  603.0 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.65 ms |  0.56 |  33 | 1999.0 |  333.0 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    50.68 ms |  1.69 |  33 |  656.7 | 1013.6 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |    32.66 ms |  1.09 |  33 | 1018.9 |  653.3 |  14.41 MB |        1.07 |
|           |       |         |             |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 |   613.83 ms |  1.00 | 665 | 1084.7 |  613.8 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   332.38 ms |  0.54 | 665 | 2003.1 |  332.4 | 268.26 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,009.96 ms |  1.65 | 665 |  659.2 | 1010.0 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 |   656.36 ms |  1.07 | 665 | 1014.4 |  656.4 | 261.35 MB |        1.00 |
