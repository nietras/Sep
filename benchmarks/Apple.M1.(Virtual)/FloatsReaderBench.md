```

BenchmarkDotNet v0.16.0-preview.1, macOS Tahoe 26.6.2 (25G83) [Darwin 25.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
Memory: 7 GB Total, 0.09 GB Available
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
| Sep______ | Row    | 25000 |   3.988 ms |  1.00 | 20 | 5082.7 |  159.5 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  28.667 ms |  7.23 | 20 |  707.2 | 1146.7 |    12.13 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  26.063 ms |  6.57 | 20 |  777.8 | 1042.5 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  42.096 ms | 10.61 | 20 |  481.6 | 1683.8 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   5.201 ms |  1.00 | 20 | 3897.6 |  208.0 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  31.599 ms |  6.09 | 20 |  641.5 | 1264.0 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  24.527 ms |  4.73 | 20 |  826.5 |  981.1 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  44.300 ms |  8.54 | 20 |  457.6 | 1772.0 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  36.631 ms |  1.00 | 20 |  553.4 | 1465.2 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.634 ms |  0.43 | 20 | 1296.6 |  625.4 |    80.08 KB |       10.24 |
| Sylvan___ | Floats | 25000 | 101.495 ms |  2.78 | 20 |  199.7 | 4059.8 |    18.29 KB |        2.34 |
| ReadLine_ | Floats | 25000 |  99.565 ms |  2.73 | 20 |  203.6 | 3982.6 | 73492.93 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 124.786 ms |  3.42 | 20 |  162.5 | 4991.4 | 22061.55 KB |    2,821.06 |
