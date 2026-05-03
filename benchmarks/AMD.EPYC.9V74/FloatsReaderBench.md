```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.203
  [Host]    : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.7 (10.0.7, 10.0.726.21808), X64 RyuJIT x86-64-v3

Job=.NET 10.0  EnvironmentVariables=DOTNET_GCDynamicAdaptationMode=0  Runtime=.NET 10.0  
Toolchain=net10.0  InvocationCount=Default  IterationTime=350ms  
MaxIterationCount=15  MinIterationCount=5  WarmupCount=6  
Reader=String  

```
| Method    | Scope  | Rows  | Mean       | Ratio | MB | MB/s   | ns/row | Allocated   | Alloc Ratio |
|---------- |------- |------ |-----------:|------:|---:|-------:|-------:|------------:|------------:|
| Sep______ | Row    | 25000 |   2.870 ms |  1.00 | 20 | 7080.3 |  114.8 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.602 ms |  1.26 | 20 | 5641.0 |  144.1 |     12.5 KB |        9.94 |
| ReadLine_ | Row    | 25000 |  15.049 ms |  5.24 | 20 | 1350.2 |  602.0 | 73489.62 KB |   58,426.53 |
| CsvHelper | Row    | 25000 |  40.598 ms | 14.15 | 20 |  500.5 | 1623.9 |    20.03 KB |       15.92 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.068 ms |  1.00 | 20 | 4994.9 |  162.7 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.508 ms |  1.60 | 20 | 3122.5 |  260.3 |     12.5 KB |        9.94 |
| ReadLine_ | Cols   | 25000 |  15.529 ms |  3.82 | 20 | 1308.5 |  621.1 | 73489.62 KB |   58,426.53 |
| CsvHelper | Cols   | 25000 |  43.826 ms | 10.77 | 20 |  463.6 | 1753.0 | 21340.16 KB |   16,966.09 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  33.448 ms |  1.00 | 20 |  607.5 | 1337.9 |     7.91 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.884 ms |  0.44 | 20 | 1365.2 |  595.3 |    71.21 KB |        9.01 |
| Sylvan___ | Floats | 25000 |  80.979 ms |  2.42 | 20 |  250.9 | 3239.2 |    21.64 KB |        2.74 |
| ReadLine_ | Floats | 25000 | 105.904 ms |  3.17 | 20 |  191.9 | 4236.2 | 73492.94 KB |    9,295.55 |
| CsvHelper | Floats | 25000 | 156.834 ms |  4.69 | 20 |  129.6 | 6273.3 | 22061.22 KB |    2,790.35 |
