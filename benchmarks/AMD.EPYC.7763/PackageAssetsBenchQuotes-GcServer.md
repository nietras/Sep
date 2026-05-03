```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.71GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Asset | 50000   |    37.25 ms |  1.00 |  33 |  893.5 |  745.0 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    22.04 ms |  0.59 |  33 | 1510.2 |  440.8 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    60.90 ms |  1.63 |  33 |  546.5 | 1217.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    65.88 ms |  1.77 |  33 |  505.2 | 1317.7 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   118.52 ms |  3.18 |  33 |  280.8 | 2370.3 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   771.76 ms |  1.00 | 665 |  862.7 |  771.8 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   432.75 ms |  0.56 | 665 | 1538.5 |  432.7 |  262.96 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,228.04 ms |  1.59 | 665 |  542.2 | 1228.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,370.39 ms |  1.78 | 665 |  485.8 | 1370.4 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,354.18 ms |  3.05 | 665 |  282.8 | 2354.2 |  260.58 MB |        1.00 |
