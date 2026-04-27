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
| Sep______ | Row    | 25000 |   2.387 ms |  1.00 | 20 | 8493.6 |   95.5 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  18.501 ms |  7.75 | 20 | 1095.7 |  740.0 |    12.16 KB |       10.38 |
| ReadLine_ | Row    | 25000 |  13.505 ms |  5.66 | 20 | 1501.0 |  540.2 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  29.167 ms | 12.22 | 20 |  695.0 | 1166.7 |    20.02 KB |       17.08 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.232 ms |  1.00 | 20 | 6272.8 |  129.3 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  21.010 ms |  6.50 | 20 |  964.9 |  840.4 |    12.16 KB |       10.38 |
| ReadLine_ | Cols   | 25000 |  13.866 ms |  4.29 | 20 | 1462.0 |  554.6 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  30.371 ms |  9.40 | 20 |  667.5 | 1214.9 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  23.220 ms |  1.00 | 20 |  873.0 |  928.8 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.228 ms |  0.40 | 20 | 2196.8 |  369.1 |    97.67 KB |       12.49 |
| Sylvan___ | Floats | 25000 |  66.094 ms |  2.85 | 20 |  306.7 | 2643.8 |    18.39 KB |        2.35 |
| ReadLine_ | Floats | 25000 |  75.214 ms |  3.24 | 20 |  269.5 | 3008.5 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 101.694 ms |  4.38 | 20 |  199.3 | 4067.8 | 22061.63 KB |    2,821.07 |
