```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.71GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    31.32 ms |  1.00 |  29 |  928.5 |  626.5 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    17.09 ms |  0.55 |  29 | 1702.0 |  341.8 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.95 ms |  1.34 |  29 |  693.4 |  838.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.51 ms |  1.90 |  29 |  488.7 | 1190.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   115.00 ms |  3.67 |  29 |  252.9 | 2299.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   648.38 ms |  1.00 | 581 |  897.4 |  648.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   350.60 ms |  0.54 | 581 | 1659.7 |  350.6 |  269.59 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   835.17 ms |  1.29 | 581 |  696.7 |  835.2 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,237.22 ms |  1.91 | 581 |  470.3 | 1237.2 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,312.89 ms |  3.57 | 581 |  251.6 | 2312.9 |  260.58 MB |        1.00 |
