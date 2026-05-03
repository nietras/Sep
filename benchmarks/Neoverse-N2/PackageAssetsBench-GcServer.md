```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Server=True  Toolchain=net10.0  InvocationCount=Default  
IterationTime=350ms  MaxIterationCount=15  MinIterationCount=5  
WarmupCount=6  Quotes=False  Reader=String  

```
| Method    | Scope | Rows    | Mean        | Ratio | MB  | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------ |-------- |------------:|------:|----:|-------:|-------:|-----------:|------------:|
| Sep______ | Asset | 50000   |    32.22 ms |  1.00 |  29 |  902.9 |  644.3 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.91 ms |  0.37 |  29 | 2441.7 |  238.2 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    55.44 ms |  1.72 |  29 |  524.6 | 1108.8 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    54.33 ms |  1.69 |  29 |  535.4 | 1086.6 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   101.15 ms |  3.14 |  29 |  287.5 | 2023.1 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   662.25 ms |  1.00 | 581 |  878.6 |  662.2 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   231.12 ms |  0.35 | 581 | 2517.7 |  231.1 |  267.88 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,098.12 ms |  1.66 | 581 |  529.9 | 1098.1 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,103.00 ms |  1.67 | 581 |  527.5 | 1103.0 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 2,008.06 ms |  3.03 | 581 |  289.8 | 2008.1 |  260.58 MB |        1.00 |
