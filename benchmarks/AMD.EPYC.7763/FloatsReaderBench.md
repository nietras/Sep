```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32522/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Row    | 25000 |   3.279 ms |  1.00 | 20 | 6197.1 |  131.2 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.809 ms |  1.16 | 20 | 5335.3 |  152.3 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  15.605 ms |  4.76 | 20 | 1302.2 |  624.2 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  38.219 ms | 11.66 | 20 |  531.7 | 1528.7 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.395 ms |  1.00 | 20 | 4623.5 |  175.8 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.275 ms |  1.43 | 20 | 3238.2 |  251.0 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  16.599 ms |  3.78 | 20 | 1224.1 |  664.0 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  41.138 ms |  9.36 | 20 |  493.9 | 1645.5 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.896 ms |  1.00 | 20 |  637.1 | 1275.8 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  17.609 ms |  0.55 | 20 | 1153.9 |  704.4 |    80.33 KB |       10.17 |
| Sylvan___ | Floats | 25000 |  76.958 ms |  2.41 | 20 |  264.0 | 3078.3 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 103.266 ms |  3.24 | 20 |  196.8 | 4130.6 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 147.106 ms |  4.61 | 20 |  138.1 | 5884.3 | 22061.22 KB |    2,793.11 |
