```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.44 GB Available
.NET SDK 11.0.100-rc.1.26425.128
  [Host]    : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.1.26425.128, 11.0.26.42628), Arm64 RyuJIT armv8.0-a

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=net11.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   3.565 ms |  1.00 | 20 | 5685.7 |  142.6 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.033 ms |  6.48 | 20 |  880.1 |  921.3 |    12.12 KB |       10.34 |
| ReadLine_ | Row    | 25000 |  19.192 ms |  5.40 | 20 | 1056.3 |  767.7 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  34.141 ms |  9.60 | 20 |  593.8 | 1365.6 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.920 ms |  1.00 | 20 | 4120.3 |  196.8 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.062 ms |  5.30 | 20 |  777.8 | 1042.5 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  19.374 ms |  3.94 | 20 | 1046.4 |  775.0 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  35.932 ms |  7.30 | 20 |  564.2 | 1437.3 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.534 ms |  1.00 | 20 |  642.9 | 1261.4 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.283 ms |  0.29 | 20 | 2183.9 |  371.3 |    77.89 KB |        9.96 |
| Sylvan___ | Floats | 25000 |  83.616 ms |  2.65 | 20 |  242.4 | 3344.6 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  96.363 ms |  3.06 | 20 |  210.4 | 3854.5 | 73492.93 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 136.319 ms |  4.32 | 20 |  148.7 | 5452.8 |  22061.3 KB |    2,821.03 |
