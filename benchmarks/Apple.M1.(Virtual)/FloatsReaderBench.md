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
| Sep______ | Row    | 25000 |   2.390 ms |  1.00 | 20 | 8480.3 |   95.6 |     1.18 KB |        1.00 |
| Sylvan___ | Row    | 25000 |  18.346 ms |  7.67 | 20 | 1105.0 |  733.8 |    12.12 KB |       10.27 |
| ReadLine_ | Row    | 25000 |  13.517 ms |  5.65 | 20 | 1499.7 |  540.7 | 73489.62 KB |   62,295.83 |
| CsvHelper | Row    | 25000 |  28.011 ms | 11.72 | 20 |  723.7 | 1120.4 |    20.02 KB |       16.97 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   3.267 ms |  1.00 | 20 | 6204.4 |  130.7 |     1.18 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |  21.048 ms |  6.44 | 20 |  963.1 |  841.9 |    12.16 KB |       10.31 |
| ReadLine_ | Cols   | 25000 |  13.942 ms |  4.27 | 20 | 1454.0 |  557.7 | 73489.62 KB |   62,295.83 |
| CsvHelper | Cols   | 25000 |  29.603 ms |  9.06 | 20 |  684.8 | 1184.1 | 21340.23 KB |   18,089.74 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  24.392 ms |  1.00 | 20 |  831.1 |  975.7 |     7.83 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |   9.148 ms |  0.38 | 20 | 2216.0 |  365.9 |    81.05 KB |       10.35 |
| Sylvan___ | Floats | 25000 |  66.190 ms |  2.71 | 20 |  306.3 | 2647.6 |    18.24 KB |        2.33 |
| ReadLine_ | Floats | 25000 |  75.316 ms |  3.09 | 20 |  269.2 | 3012.6 | 73492.94 KB |    9,388.32 |
| CsvHelper | Floats | 25000 | 100.775 ms |  4.13 | 20 |  201.2 | 4031.0 | 22061.63 KB |    2,818.25 |
