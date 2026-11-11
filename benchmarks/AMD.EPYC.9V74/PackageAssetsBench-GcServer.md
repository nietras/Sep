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
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  26.76 ms |  1.00 |  29 | 1086.9 |  535.2 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  14.23 ms |  0.53 |  29 | 2044.4 |  284.5 |  13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |  34.66 ms |  1.30 |  29 |  839.2 |  693.2 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  29.16 ms |  1.09 |  29 |  997.3 |  583.3 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 537.15 ms |  1.00 | 581 | 1083.3 |  537.1 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 305.70 ms |  0.57 | 581 | 1903.4 |  305.7 | 268.94 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 698.99 ms |  1.30 | 581 |  832.5 |  699.0 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 602.85 ms |  1.12 | 581 |  965.2 |  602.8 | 261.35 MB |        1.00 |
