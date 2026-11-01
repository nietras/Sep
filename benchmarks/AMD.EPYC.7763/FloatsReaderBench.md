```

BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.61GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 11.83 GB Available
.NET SDK 11.0.100-rc.3.26522.148
  [Host]    : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3
  .NET 11.0 : .NET 11.0.0 (11.0.0-rc.3.26522.148, 11.0.26.52348), X64 RyuJIT x86-64-v3

Job=.NET 11.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 11.0  
Toolchain=CsProjCore  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated  | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|-----------:|------------:|
| Sep______ | Row    | 25000 |   2.938 ms |  1.00 | 20 | 6899.4 |  117.5 |     1280 B |        1.00 |
| Sylvan___ | Row    | 25000 |   3.544 ms |  1.21 | 20 | 5720.2 |  141.8 |    12802 B |       10.00 |
| ReadLine_ | Row    | 25000 |  17.176 ms |  5.85 | 20 | 1180.3 |  687.0 | 75253368 B |   58,791.69 |
| CsvHelper | Row    | 25000 |   2.128 ms |  0.72 | 20 | 9528.5 |   85.1 |      776 B |        0.61 |
|           |        |       |            |       |    |        |        |            |             |
| Sep______ | Cols   | 25000 |   4.011 ms |  1.00 | 20 | 5054.1 |  160.4 |     1280 B |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.035 ms |  1.50 | 20 | 3358.9 |  241.4 |    12803 B |       10.00 |
| ReadLine_ | Cols   | 25000 |  18.278 ms |  4.56 | 20 | 1109.1 |  731.1 | 75253368 B |   58,791.69 |
| CsvHelper | Cols   | 25000 |   2.151 ms |  0.54 | 20 | 9424.2 |   86.0 |      776 B |        0.61 |
|           |        |       |            |       |    |        |        |            |             |
| Sep______ | Floats | 25000 |  33.805 ms |  1.00 | 20 |  599.7 | 1352.2 |     8088 B |        1.00 |
| Sep_MT___ | Floats | 25000 |  15.645 ms |  0.46 | 20 | 1295.7 |  625.8 |    73662 B |        9.11 |
| Sylvan___ | Floats | 25000 |  84.540 ms |  2.50 | 20 |  239.8 | 3381.6 |    19076 B |        2.36 |
| ReadLine_ | Floats | 25000 | 122.291 ms |  3.62 | 20 |  165.8 | 4891.6 | 75256680 B |    9,304.73 |
| CsvHelper | Floats | 25000 |  32.976 ms |  0.98 | 20 |  614.7 | 1319.1 |     7800 B |        0.96 |
