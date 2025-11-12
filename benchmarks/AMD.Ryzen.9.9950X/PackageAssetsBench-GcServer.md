```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.6575/22H2/2022Update)
AMD Ryzen 9 9950X 4.30GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.100
  [Host]    : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  .NET 10.0 : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean         | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |-------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    14.407 ms |  1.00 |  29 | 2025.5 |  288.1 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |     3.452 ms |  0.24 |  29 | 8452.2 |   69.0 |   13.65 MB |        1.01 |
| Sylvan___ | Asset | 50000   |    18.671 ms |  1.30 |  29 | 1562.9 |  373.4 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    19.166 ms |  1.33 |  29 | 1522.5 |  383.3 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |    50.898 ms |  3.53 |  29 |  573.3 | 1018.0 |   13.64 MB |        1.01 |
|           |       |         |              |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   293.391 ms |  1.00 | 583 | 1989.8 |  293.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |    58.678 ms |  0.20 | 583 | 9949.0 |   58.7 |  261.63 MB |        1.00 |
| Sylvan___ | Asset | 1000000 |   377.446 ms |  1.29 | 583 | 1546.7 |  377.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 |   367.319 ms |  1.25 | 583 | 1589.3 |  367.3 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,012.274 ms |  3.45 | 583 |  576.7 | 1012.3 |  260.58 MB |        1.00 |
