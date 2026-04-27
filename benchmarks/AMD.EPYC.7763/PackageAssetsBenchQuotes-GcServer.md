```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.24GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    36.95 ms |  1.00 |  33 |  900.8 |  738.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.04 ms |  0.60 |  33 | 1510.2 |  440.8 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    60.86 ms |  1.65 |  33 |  546.8 | 1217.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    64.87 ms |  1.76 |  33 |  513.1 | 1297.4 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.69 ms |  3.21 |  33 |  280.4 | 2373.7 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   763.29 ms |  1.00 | 665 |  872.3 |  763.3 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   429.16 ms |  0.56 | 665 | 1551.4 |  429.2 |  261.76 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,223.33 ms |  1.60 | 665 |  544.3 | 1223.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,393.90 ms |  1.83 | 665 |  477.7 | 1393.9 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,365.35 ms |  3.10 | 665 |  281.5 | 2365.4 |  260.58 MB |        1.00 |
