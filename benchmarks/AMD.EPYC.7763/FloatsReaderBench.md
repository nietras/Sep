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
| Sep______ | Row    | 25000 |   2.945 ms |  1.00 | 20 | 6900.4 |  117.8 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.480 ms |  1.18 | 20 | 5839.1 |  139.2 |     12.5 KB |       10.00 |
| ReadLine_ | Row    | 25000 |  14.569 ms |  4.95 | 20 | 1394.7 |  582.7 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  37.734 ms | 12.81 | 20 |  538.5 | 1509.4 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.004 ms |  1.00 | 20 | 5074.8 |  160.2 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.007 ms |  1.50 | 20 | 3382.4 |  240.3 |     12.5 KB |       10.00 |
| ReadLine_ | Cols   | 25000 |  15.175 ms |  3.79 | 20 | 1339.1 |  607.0 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  40.186 ms | 10.04 | 20 |  505.6 | 1607.4 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  31.600 ms |  1.00 | 20 |  643.0 | 1264.0 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.282 ms |  0.45 | 20 | 1422.7 |  571.3 |    69.43 KB |        8.79 |
| Sylvan___ | Floats | 25000 |  79.631 ms |  2.52 | 20 |  255.2 | 3185.3 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 100.560 ms |  3.18 | 20 |  202.1 | 4022.4 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 149.210 ms |  4.72 | 20 |  136.2 | 5968.4 | 22061.22 KB |    2,793.11 |
