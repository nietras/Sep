```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.13GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    31.73 ms |  1.00 |  29 |  916.5 |  634.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.42 ms |  0.52 |  29 | 1771.0 |  328.5 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    41.41 ms |  1.31 |  29 |  702.3 |  828.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    58.93 ms |  1.86 |  29 |  493.5 | 1178.7 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   115.57 ms |  3.64 |  29 |  251.7 | 2311.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   656.52 ms |  1.00 | 581 |  886.3 |  656.5 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   347.61 ms |  0.53 | 581 | 1673.9 |  347.6 |     270 MB |        1.04 |
| Sylvan___ | Asset | 1000000 |   842.44 ms |  1.28 | 581 |  690.7 |  842.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,222.33 ms |  1.86 | 581 |  476.0 | 1222.3 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,358.67 ms |  3.59 | 581 |  246.7 | 2358.7 |  260.58 MB |        1.00 |
