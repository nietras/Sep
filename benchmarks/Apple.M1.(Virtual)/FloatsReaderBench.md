```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
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
| Sep______ | Row    | 25000 |   2.502 ms |  1.00 | 20 | 8103.4 |  100.1 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  18.315 ms |  7.33 | 20 | 1106.9 |  732.6 |    12.12 KB |       10.34 |
| ReadLine_ | Row    | 25000 |  13.819 ms |  5.53 | 20 | 1467.0 |  552.8 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  28.323 ms | 11.33 | 20 |  715.8 | 1132.9 |    20.02 KB |       17.08 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.262 ms |  1.00 | 20 | 6214.1 |  130.5 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  20.969 ms |  6.43 | 20 |  966.8 |  838.8 |    12.16 KB |       10.38 |
| ReadLine_ | Cols   | 25000 |  14.096 ms |  4.32 | 20 | 1438.1 |  563.8 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  29.694 ms |  9.10 | 20 |  682.7 | 1187.7 | 21340.23 KB |   18,210.33 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  24.545 ms |  1.00 | 20 |  825.9 |  981.8 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.975 ms |  0.61 | 20 | 1353.7 |  599.0 |    54.87 KB |        7.02 |
| Sylvan___ | Floats | 25000 |  66.458 ms |  2.71 | 20 |  305.0 | 2658.3 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  75.236 ms |  3.07 | 20 |  269.4 | 3009.4 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 105.073 ms |  4.28 | 20 |  192.9 | 4202.9 | 22061.55 KB |    2,821.06 |
