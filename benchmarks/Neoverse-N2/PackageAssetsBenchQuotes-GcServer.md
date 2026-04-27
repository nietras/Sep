```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=True  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    40.48 ms |  1.00 |  33 |  822.1 |  809.7 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    16.31 ms |  0.40 |  33 | 2040.5 |  326.2 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    57.69 ms |  1.43 |  33 |  576.9 | 1153.9 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    61.90 ms |  1.53 |  33 |  537.7 | 1238.0 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   108.25 ms |  2.67 |  33 |  307.5 | 2164.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   779.44 ms |  1.00 | 665 |  854.2 |  779.4 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   252.41 ms |  0.32 | 665 | 2637.8 |  252.4 |  261.87 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,149.00 ms |  1.47 | 665 |  579.5 | 1149.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,304.88 ms |  1.67 | 665 |  510.2 | 1304.9 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,188.07 ms |  2.81 | 665 |  304.3 | 2188.1 |  260.58 MB |        1.00 |
