```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.82GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    30.87 ms |  1.00 |  29 |  942.2 |  617.4 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.57 ms |  0.57 |  29 | 1655.0 |  351.5 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.93 ms |  1.36 |  29 |  693.6 |  838.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.24 ms |  1.92 |  29 |  491.0 | 1184.8 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   114.76 ms |  3.72 |  29 |  253.5 | 2295.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   640.46 ms |  1.00 | 581 |  908.5 |  640.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   348.59 ms |  0.54 | 581 | 1669.2 |  348.6 |  269.83 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   816.17 ms |  1.27 | 581 |  712.9 |  816.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,226.32 ms |  1.91 | 581 |  474.5 | 1226.3 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,299.13 ms |  3.59 | 581 |  253.1 | 2299.1 |  260.58 MB |        1.00 |
