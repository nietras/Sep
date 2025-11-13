```

BenchmarkDotNet v0.15.6, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763 2.90GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    33.24 ms |  1.00 |  29 |  874.9 |  664.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.93 ms |  0.51 |  29 | 1718.1 |  338.6 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.02 ms |  1.23 |  29 |  709.1 |  820.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.34 ms |  1.79 |  29 |  490.1 | 1186.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   114.45 ms |  3.44 |  29 |  254.1 | 2289.0 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   656.08 ms |  1.00 | 581 |  886.9 |  656.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   333.94 ms |  0.51 | 581 | 1742.5 |  333.9 |   268.8 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   824.37 ms |  1.26 | 581 |  705.8 |  824.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,231.05 ms |  1.88 | 581 |  472.7 | 1231.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,342.51 ms |  3.57 | 581 |  248.4 | 2342.5 |  260.58 MB |        1.00 |
