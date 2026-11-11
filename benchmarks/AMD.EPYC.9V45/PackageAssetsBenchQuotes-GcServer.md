```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 26.04.1 LTS (Resolute Raccoon)
AMD EPYC 9V45 4.06GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.5 GB Available
.NET SDK 11.0.100
  [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Server=True  
Runtime=.NET 11.0  Toolchain=CsProjCore  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  22.20 ms |  1.00 |  33 | 1499.3 |  444.0 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  11.58 ms |  0.52 |  33 | 2874.0 |  231.6 |  13.56 MB |        1.01 |
| Sylvan___ | Asset | 50000   |  39.75 ms |  1.80 |  33 |  837.3 |  795.0 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  26.86 ms |  1.22 |  33 | 1239.2 |  537.2 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 455.12 ms |  1.00 | 665 | 1462.9 |  455.1 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 240.79 ms |  0.53 | 665 | 2765.1 |  240.8 | 269.25 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 771.07 ms |  1.70 | 665 |  863.5 |  771.1 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 517.63 ms |  1.14 | 665 | 1286.2 |  517.6 | 261.35 MB |        1.00 |
