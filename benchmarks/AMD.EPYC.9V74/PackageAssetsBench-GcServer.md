```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    31.08 ms |  1.00 |  29 |  935.7 |  621.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.29 ms |  0.56 |  29 | 1682.2 |  345.8 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    43.18 ms |  1.39 |  29 |  673.5 |  863.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    60.19 ms |  1.94 |  29 |  483.2 | 1203.9 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   119.97 ms |  3.86 |  29 |  242.4 | 2399.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   650.60 ms |  1.00 | 581 |  894.4 |  650.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   342.94 ms |  0.53 | 581 | 1696.7 |  342.9 |  269.46 MB |        1.03 |
| Sylvan___ | Asset | 1000000 |   847.37 ms |  1.30 | 581 |  686.7 |  847.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,289.37 ms |  1.98 | 581 |  451.3 | 1289.4 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,420.82 ms |  3.72 | 581 |  240.4 | 2420.8 |  260.58 MB |        1.00 |
