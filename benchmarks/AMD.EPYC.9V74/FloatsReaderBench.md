```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.46 GB Available
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
| Sep______ | Row    | 25000 |  1.822 ms |  1.00 | 20 | 11128.2 |   72.9 |   3.81 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  2.751 ms |  1.51 | 20 |  7368.3 |  110.0 |  16.94 KB |        4.44 |
| CsvHelper | Row    | 25000 |  2.288 ms |  1.26 | 20 |  8861.3 |   91.5 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Cols   | 25000 |  2.685 ms |  1.00 | 20 |  7549.3 |  107.4 |   3.81 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  4.803 ms |  1.79 | 20 |  4221.1 |  192.1 |  16.94 KB |        4.44 |
| CsvHelper | Cols   | 25000 |  3.527 ms |  1.31 | 20 |  5746.9 |  141.1 |    4.3 KB |        1.13 |
|           |        |       |           |       |    |         |        |           |             |
| Sep______ | Floats | 25000 | 26.820 ms |  1.00 | 20 |   755.9 | 1072.8 |   5.85 KB |        1.00 |
| Sep_MT___ | Floats | 25000 | 11.756 ms |  0.44 | 20 |  1724.4 |  470.3 |  50.75 KB |        8.67 |
| Sylvan___ | Floats | 25000 | 59.334 ms |  2.21 | 20 |   341.7 | 2373.4 |  18.61 KB |        3.18 |
| CsvHelper | Floats | 25000 | 26.966 ms |  1.01 | 20 |   751.8 | 1078.6 |   6.09 KB |        1.04 |
