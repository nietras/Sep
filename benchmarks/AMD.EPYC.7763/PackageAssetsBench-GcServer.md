```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    30.61 ms |  1.00 |  29 |  950.3 |  612.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.77 ms |  0.58 |  29 | 1636.8 |  355.4 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.63 ms |  1.36 |  29 |  698.8 |  832.5 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    58.05 ms |  1.90 |  29 |  501.0 | 1161.1 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   115.79 ms |  3.78 |  29 |  251.2 | 2315.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   643.24 ms |  1.00 | 581 |  904.6 |  643.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   342.13 ms |  0.53 | 581 | 1700.8 |  342.1 |  269.11 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   816.23 ms |  1.27 | 581 |  712.9 |  816.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,219.10 ms |  1.90 | 581 |  477.3 | 1219.1 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,304.30 ms |  3.58 | 581 |  252.5 | 2304.3 |  260.58 MB |        1.00 |
