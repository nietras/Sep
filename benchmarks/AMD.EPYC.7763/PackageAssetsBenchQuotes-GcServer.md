```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.13GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    37.00 ms |  1.00 |  33 |  899.4 |  740.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    21.90 ms |  0.59 |  33 | 1519.7 |  438.0 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    60.20 ms |  1.63 |  33 |  552.8 | 1204.1 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    64.90 ms |  1.75 |  33 |  512.8 | 1298.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.31 ms |  3.20 |  33 |  281.3 | 2366.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   772.12 ms |  1.00 | 665 |  862.3 |  772.1 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   433.98 ms |  0.56 | 665 | 1534.2 |  434.0 |  263.02 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,228.29 ms |  1.59 | 665 |  542.1 | 1228.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,385.33 ms |  1.79 | 665 |  480.6 | 1385.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,388.20 ms |  3.09 | 665 |  278.8 | 2388.2 |  260.58 MB |        1.00 |
