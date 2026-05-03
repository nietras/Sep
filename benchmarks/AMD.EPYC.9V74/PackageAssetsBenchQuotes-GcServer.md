```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    37.86 ms |  1.00 |  33 |  879.1 |  757.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.75 ms |  0.60 |  33 | 1462.8 |  455.0 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    60.76 ms |  1.60 |  33 |  547.8 | 1215.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    68.13 ms |  1.80 |  33 |  488.5 | 1362.6 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   115.01 ms |  3.04 |  33 |  289.4 | 2300.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   786.63 ms |  1.00 | 665 |  846.4 |  786.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   455.86 ms |  0.58 | 665 | 1460.5 |  455.9 |  263.41 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,239.99 ms |  1.58 | 665 |  536.9 | 1240.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,439.31 ms |  1.83 | 665 |  462.6 | 1439.3 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,309.28 ms |  2.94 | 665 |  288.3 | 2309.3 |  260.58 MB |        1.00 |
