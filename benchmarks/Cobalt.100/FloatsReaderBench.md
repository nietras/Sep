```

BenchmarkDotNet v0.16.0-preview.1, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2) (Hyper-V)
Cobalt 100 3.40GHz, 1 CPU, 4 logical and 4 physical cores
Memory: 15.99 GB Total, 12.01 GB Available
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
| Sep______ | Row    | 25000 |   3.427 ms |  1.00 | 20 | 5929.7 |  137.1 |     1.17 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  23.382 ms |  6.82 | 20 |  869.0 |  935.3 |    12.12 KB |       10.35 |
| ReadLine_ | Row    | 25000 |  18.343 ms |  5.35 | 20 | 1107.7 |  733.7 | 73489.62 KB |   62,711.14 |
| CsvHelper | Row    | 25000 |  33.257 ms |  9.71 | 20 |  611.0 | 1330.3 |    19.95 KB |       17.02 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.426 ms |  1.00 | 20 | 4591.5 |  177.0 |     1.17 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  26.064 ms |  5.89 | 20 |  779.6 | 1042.6 |    12.13 KB |       10.35 |
| ReadLine_ | Cols   | 25000 |  18.402 ms |  4.16 | 20 | 1104.2 |  736.1 | 73489.62 KB |   62,711.14 |
| CsvHelper | Cols   | 25000 |  37.497 ms |  8.48 | 20 |  541.9 | 1499.9 | 21340.16 KB |   18,210.27 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.296 ms |  1.00 | 20 |  649.3 | 1251.8 |     7.82 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.248 ms |  0.30 | 20 | 2197.2 |  369.9 |    74.99 KB |        9.59 |
| Sylvan___ | Floats | 25000 |  83.488 ms |  2.67 | 20 |  243.4 | 3339.5 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  91.164 ms |  2.91 | 20 |  222.9 | 3646.6 | 73492.93 KB |    9,397.70 |
| CsvHelper | Floats | 25000 | 128.266 ms |  4.10 | 20 |  158.4 | 5130.6 | 22061.26 KB |    2,821.02 |
