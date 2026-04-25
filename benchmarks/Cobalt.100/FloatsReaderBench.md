```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
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
| Sep______ | Row    | 25000 |   3.433 ms |  1.00 | 20 | 5918.5 |  137.3 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.827 ms |  6.94 | 20 |  852.8 |  953.1 |    12.12 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  17.340 ms |  5.05 | 20 | 1171.8 |  693.6 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  32.932 ms |  9.60 | 20 |  617.0 | 1317.3 |    20.02 KB |       17.08 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   5.204 ms |  1.00 | 20 | 3904.3 |  208.2 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.777 ms |  5.15 | 20 |  758.9 | 1071.1 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  17.761 ms |  3.41 | 20 | 1144.1 |  710.4 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  36.659 ms |  7.05 | 20 |  554.3 | 1466.4 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.772 ms |  1.00 | 20 |  639.5 | 1270.9 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.296 ms |  0.29 | 20 | 2185.9 |  371.8 |    67.88 KB |        8.68 |
| Sylvan___ | Floats | 25000 |  89.112 ms |  2.80 | 20 |  228.0 | 3564.5 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  93.970 ms |  2.96 | 20 |  216.2 | 3758.8 | 73492.94 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 128.584 ms |  4.05 | 20 |  158.0 | 5143.4 | 22060.98 KB |    2,820.98 |
