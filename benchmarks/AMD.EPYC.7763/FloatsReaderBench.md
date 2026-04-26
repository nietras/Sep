```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.82GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Row    | 25000 |   3.004 ms |  1.00 | 20 | 6749.0 |  120.1 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.383 ms |  1.13 | 20 | 5991.9 |  135.3 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  18.427 ms |  6.13 | 20 | 1100.1 |  737.1 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  37.445 ms | 12.47 | 20 |  541.4 | 1497.8 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.087 ms |  1.00 | 20 | 4960.5 |  163.5 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   5.957 ms |  1.46 | 20 | 3403.2 |  238.3 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  19.086 ms |  4.67 | 20 | 1062.1 |  763.4 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  40.168 ms |  9.83 | 20 |  504.7 | 1606.7 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  33.152 ms |  1.00 | 20 |  611.5 | 1326.1 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.426 ms |  0.44 | 20 | 1405.2 |  577.0 |    71.06 KB |        9.00 |
| Sylvan___ | Floats | 25000 |  84.156 ms |  2.54 | 20 |  240.9 | 3366.2 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 110.771 ms |  3.34 | 20 |  183.0 | 4430.8 | 73495.95 KB |    9,305.13 |
| CsvHelper | Floats | 25000 | 156.838 ms |  4.73 | 20 |  129.3 | 6273.5 | 22061.51 KB |    2,793.15 |
