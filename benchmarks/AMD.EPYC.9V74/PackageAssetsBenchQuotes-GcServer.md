```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 10.4 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), X64 RyuJIT x86-64-v4

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Server=True  Toolchain=net11.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.15 ms |  1.00 |  33 | 1035.4 |  642.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    18.37 ms |  0.57 |  33 | 1812.1 |  367.3 |   13.54 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    49.67 ms |  1.55 |  33 |  670.1 |  993.3 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    52.64 ms |  1.64 |  33 |  632.3 | 1052.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |    89.80 ms |  2.80 |  33 |  370.6 | 1796.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   637.63 ms |  1.00 | 665 | 1044.2 |  637.6 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   356.33 ms |  0.56 | 665 | 1868.5 |  356.3 |  263.08 MB |        1.01 |
| Sylvan___ | Asset | 1000000 |   982.27 ms |  1.54 | 665 |  677.8 |  982.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,181.95 ms |  1.85 | 665 |  563.3 | 1181.9 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 1,812.09 ms |  2.84 | 665 |  367.4 | 1812.1 |  260.58 MB |        1.00 |
