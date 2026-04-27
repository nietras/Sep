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
| Sep______ | Row    | 25000 |   2.693 ms |  1.00 | 20 | 7527.2 |  107.7 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  19.233 ms |  7.15 | 20 | 1054.0 |  769.3 |    12.16 KB |       10.38 |
| ReadLine_ | Row    | 25000 |  16.777 ms |  6.23 | 20 | 1208.4 |  671.1 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  33.612 ms | 12.49 | 20 |  603.1 | 1344.5 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.637 ms |  1.00 | 20 | 5573.4 |  145.5 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  21.571 ms |  5.95 | 20 |  939.8 |  862.8 |    12.16 KB |       10.38 |
| ReadLine_ | Cols   | 25000 |  14.283 ms |  3.94 | 20 | 1419.3 |  571.3 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  31.331 ms |  8.64 | 20 |  647.0 | 1253.2 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  25.764 ms |  1.00 | 20 |  786.8 | 1030.6 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  16.588 ms |  0.65 | 20 | 1222.1 |  663.5 |    92.33 KB |       11.81 |
| Sylvan___ | Floats | 25000 |  67.293 ms |  2.62 | 20 |  301.3 | 2691.7 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  77.332 ms |  3.02 | 20 |  262.1 | 3093.3 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 103.738 ms |  4.05 | 20 |  195.4 | 4149.5 | 22061.68 KB |    2,821.07 |
