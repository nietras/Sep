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
| Sep______ | Asset | 50000   |    34.55 ms |  1.00 |  29 |  842.0 |  690.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    11.74 ms |  0.34 |  29 | 2478.4 |  234.7 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    55.33 ms |  1.60 |  29 |  525.7 | 1106.6 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    52.83 ms |  1.53 |  29 |  550.6 | 1056.6 |   99.74 MB |        7.40 |
| CsvHelper | Asset | 50000   |   101.16 ms |  2.93 |  29 |  287.5 | 2023.2 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   664.91 ms |  1.00 | 581 |  875.1 |  664.9 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   212.67 ms |  0.32 | 581 | 2736.1 |  212.7 |  266.94 MB |        1.03 |
| Sylvan___ | Asset | 1000000 | 1,103.32 ms |  1.66 | 581 |  527.4 | 1103.3 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,105.67 ms |  1.66 | 581 |  526.3 | 1105.7 | 1991.04 MB |        7.65 |
| CsvHelper | Asset | 1000000 | 1,999.10 ms |  3.01 | 581 |  291.1 | 1999.1 |  260.58 MB |        1.00 |
