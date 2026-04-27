```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), Arm64 RyuJIT armv8.0-a

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.648 ms |  1.00 | 20 | 5557.2 |  145.9 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.802 ms |  6.53 | 20 |  851.7 |  952.1 |    12.12 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  19.538 ms |  5.36 | 20 | 1037.6 |  781.5 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  33.482 ms |  9.19 | 20 |  605.5 | 1339.3 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.980 ms |  1.00 | 20 | 4070.6 |  199.2 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.877 ms |  5.40 | 20 |  754.3 | 1075.1 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  20.073 ms |  4.03 | 20 | 1009.9 |  802.9 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  36.993 ms |  7.43 | 20 |  548.0 | 1479.7 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.860 ms |  1.00 | 20 |  636.3 | 1274.4 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.477 ms |  0.30 | 20 | 2139.1 |  379.1 |    70.26 KB |        8.98 |
| Sylvan___ | Floats | 25000 |  87.292 ms |  2.74 | 20 |  232.2 | 3491.7 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  99.291 ms |  3.12 | 20 |  204.2 | 3971.6 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 136.433 ms |  4.28 | 20 |  148.6 | 5457.3 | 22061.27 KB |    2,821.02 |
