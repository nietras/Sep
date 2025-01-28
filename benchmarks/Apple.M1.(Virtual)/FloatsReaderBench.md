```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.2 (23H311) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.102
  [Host]     : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD
  Job-KNSVKR : .NET 9.0.1 (9.0.124.61010), Arm64 RyuJIT AdvSIMD

EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 9.0  Toolchain=net90  
InvocationCount=Default  IterationTime=350ms  MaxIterationCount=15  
MinIterationCount=5  WarmupCount=6  Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   4.209 ms |  1.00 | 20 | 4815.8 |  168.4 |      1.2 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  19.401 ms |  4.61 | 20 | 1044.9 |  776.0 |    10.62 KB |        8.87 |
| ReadLine_ | Row    | 25000 |  15.132 ms |  3.60 | 20 | 1339.7 |  605.3 | 73489.65 KB |   61,381.24 |
| CsvHelper | Row    | 25000 |  30.200 ms |  7.18 | 20 |  671.3 | 1208.0 |    20.21 KB |       16.88 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   5.070 ms |  1.00 | 20 | 3998.5 |  202.8 |     1.21 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  23.742 ms |  4.68 | 20 |  853.9 |  949.7 |    10.62 KB |        8.74 |
| ReadLine_ | Cols   | 25000 |  17.569 ms |  3.47 | 20 | 1153.9 |  702.7 | 73489.65 KB |   60,493.09 |
| CsvHelper | Cols   | 25000 |  34.182 ms |  6.74 | 20 |  593.1 | 1367.3 | 21340.43 KB |   17,566.40 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  27.363 ms |  1.00 | 20 |  740.8 | 1094.5 |     8.08 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  12.814 ms |  0.47 | 20 | 1582.0 |  512.6 |    67.85 KB |        8.40 |
| Sylvan___ | Floats | 25000 |  78.840 ms |  2.88 | 20 |  257.1 | 3153.6 |    18.57 KB |        2.30 |
| ReadLine_ | Floats | 25000 |  89.458 ms |  3.27 | 20 |  226.6 | 3578.3 |  73493.2 KB |    9,093.41 |
| CsvHelper | Floats | 25000 | 130.793 ms |  4.78 | 20 |  155.0 | 5231.7 | 22061.99 KB |    2,729.76 |
