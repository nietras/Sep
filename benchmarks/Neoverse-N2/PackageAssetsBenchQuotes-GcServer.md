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
| Sep______ | Asset | 50000   |    40.24 ms |  1.00 |  33 |  827.0 |  804.9 |   13.48 MB |        1.00 |
| Sep_MT___ | Asset | 50000   |    15.23 ms |  0.38 |  33 | 2185.6 |  304.6 |   13.53 MB |        1.00 |
| Sylvan___ | Asset | 50000   |    57.24 ms |  1.42 |  33 |  581.5 | 1144.7 |   13.63 MB |        1.01 |
| ReadLine_ | Asset | 50000   |    59.64 ms |  1.48 |  33 |  558.1 | 1192.8 |  119.44 MB |        8.86 |
| CsvHelper | Asset | 50000   |   109.02 ms |  2.71 |  33 |  305.3 | 2180.4 |   13.64 MB |        1.01 |
|           |       |         |             |       |     |        |        |            |             |
| Sep______ | Asset | 1000000 |   778.99 ms |  1.00 | 665 |  854.7 |  779.0 |  260.41 MB |        1.00 |
| Sep_MT___ | Asset | 1000000 |   247.76 ms |  0.32 | 665 | 2687.3 |  247.8 |  261.63 MB |        1.00 |
| Sylvan___ | Asset | 1000000 | 1,150.39 ms |  1.48 | 665 |  578.8 | 1150.4 |  260.57 MB |        1.00 |
| ReadLine_ | Asset | 1000000 | 1,284.60 ms |  1.65 | 665 |  518.3 | 1284.6 | 2385.07 MB |        9.16 |
| CsvHelper | Asset | 1000000 | 2,117.89 ms |  2.72 | 665 |  314.4 | 2117.9 |  260.58 MB |        1.00 |
