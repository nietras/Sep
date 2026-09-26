```

BenchmarkDotNet v0.16.0-preview.1, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
Memory: 15.57 GB Total, 11.36 GB Available
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
| Sep______ | Row    | 25000 |   3.264 ms |  1.00 | 20 | 6211.6 |  130.5 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  22.929 ms |  7.03 | 20 |  884.1 |  917.1 |    12.12 KB |       10.34 |
| ReadLine_ | Row    | 25000 |  19.137 ms |  5.86 | 20 | 1059.3 |  765.5 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  33.162 ms | 10.16 | 20 |  611.3 | 1326.5 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.223 ms |  1.00 | 20 | 4799.9 |  168.9 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.468 ms |  6.27 | 20 |  765.9 | 1058.7 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  19.268 ms |  4.56 | 20 | 1052.1 |  770.7 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  35.943 ms |  8.51 | 20 |  564.0 | 1437.7 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.343 ms |  1.00 | 20 |  646.8 | 1253.7 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.373 ms |  0.30 | 20 | 2162.9 |  374.9 |    78.54 KB |       10.04 |
| Sylvan___ | Floats | 25000 |  84.094 ms |  2.68 | 20 |  241.1 | 3363.7 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  96.642 ms |  3.08 | 20 |  209.8 | 3865.7 | 73492.93 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 135.964 ms |  4.34 | 20 |  149.1 | 5438.5 |  22061.3 KB |    2,821.03 |
