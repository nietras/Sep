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
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean      | Ratio | MB  | MB/s   | ns/row | Allocated | Alloc Ratio |
|---------- |------ |-------- |----------:|------:|----:|-------:|-------:|----------:|------------:|
| Sep______ | Asset | 50000   |  26.08 ms |  1.00 |  29 | 1115.5 |  521.5 |  13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |  14.03 ms |  0.54 |  29 | 2073.8 |  280.5 |  13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |  34.02 ms |  1.31 |  29 |  855.0 |  680.4 |  13.63 MB |        1.01 |
| CsvHelper | Asset | 50000   |  28.51 ms |  1.10 |  29 | 1020.3 |  570.1 |  14.41 MB |        1.07 |
|           |       |         |           |       |     |        |        |           |             |
| Sep______ | Asset | 1000000 | 533.80 ms |  1.00 | 581 | 1090.1 |  533.8 | 260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 | 304.09 ms |  0.57 | 581 | 1913.5 |  304.1 | 268.94 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 690.23 ms |  1.29 | 581 |  843.0 |  690.2 | 260.57 MB |        1.00 |
| CsvHelper | Asset | 1000000 | 559.87 ms |  1.05 | 581 | 1039.3 |  559.9 | 261.35 MB |        1.00 |
