```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 3.13GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Row    | 25000 |   3.157 ms |  1.00 | 20 | 6421.4 |  126.3 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.557 ms |  1.13 | 20 | 5699.4 |  142.3 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  18.801 ms |  5.96 | 20 | 1078.2 |  752.0 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  37.887 ms | 12.01 | 20 |  535.1 | 1515.5 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.259 ms |  1.00 | 20 | 4759.8 |  170.4 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.162 ms |  1.45 | 20 | 3289.7 |  246.5 |    12.71 KB |       10.17 |
| ReadLine_ | Cols   | 25000 |  19.713 ms |  4.63 | 20 | 1028.3 |  788.5 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  41.051 ms |  9.64 | 20 |  493.8 | 1642.1 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  34.004 ms |  1.00 | 20 |  596.2 | 1360.1 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.289 ms |  0.42 | 20 | 1418.7 |  571.6 |    70.25 KB |        8.89 |
| Sylvan___ | Floats | 25000 |  84.845 ms |  2.50 | 20 |  238.9 | 3393.8 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 115.330 ms |  3.39 | 20 |  175.8 | 4613.2 | 73498.96 KB |    9,305.51 |
| CsvHelper | Floats | 25000 | 158.733 ms |  4.67 | 20 |  127.7 | 6349.3 | 22061.51 KB |    2,793.15 |
