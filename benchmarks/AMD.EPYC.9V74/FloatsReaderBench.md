```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.68GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.45 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean      | Ratio | MB | MB/s    | ns/row | Allocated | Alloc Ratio |
|---------- |------- |------ |----------:|------:|---:|--------:|-------:|----------:|------------:|
| Sep______ | Row    | 25000 |  1.973 ms |  1.00 | 20 | 10272.8 |   78.9 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  3.078 ms |  1.56 | 20 |  6585.2 |  123.1 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  3.003 ms |  1.52 | 20 |  6751.1 |  120.1 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  2.932 ms |  1.00 | 20 |  6914.1 |  117.3 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  5.097 ms |  1.74 | 20 |  3977.1 |  203.9 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  3.713 ms |  1.27 | 20 |  5460.4 |  148.5 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 25.534 ms |  1.00 | 20 |   793.9 | 1021.4 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 | 11.840 ms |  0.46 | 20 |  1712.2 |  473.6 |  55.53 KB |        9.49 |
| Sylvan___ | Floats | 25000 | 59.778 ms |  2.34 | 20 |   339.1 | 2391.1 |  18.61 KB |        3.18 |
| CsvHelper | Floats | 25000 | 26.900 ms |  1.05 | 20 |   753.6 | 1076.0 |   6.09 KB |        1.04 |
