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
| Sep______ | Asset | 50000   |    40.16 ms |  1.00 |  33 |  828.7 |  803.2 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.65 ms |  0.39 |  33 | 2126.5 |  313.0 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    57.51 ms |  1.43 |  33 |  578.7 | 1150.2 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    61.13 ms |  1.52 |  33 |  544.5 | 1222.6 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   108.40 ms |  2.70 |  33 |  307.0 | 2167.9 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   771.02 ms |  1.00 | 665 |  863.5 |  771.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   249.77 ms |  0.32 | 665 | 2665.7 |  249.8 |  262.13 MB |        1.01 |
| Sylvan___ | Asset | 1000000 | 1,146.98 ms |  1.49 | 665 |  580.5 | 1147.0 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,324.76 ms |  1.72 | 665 |  502.6 | 1324.8 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,239.86 ms |  2.91 | 665 |  297.3 | 2239.9 |  260.58 MB |        1.00 |
