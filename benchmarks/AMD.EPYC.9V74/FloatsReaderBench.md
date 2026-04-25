```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74 2.86GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Row    | 25000 |   3.232 ms |  1.00 | 20 | 6271.7 |  129.3 |     1.25 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.764 ms |  1.16 | 20 | 5386.1 |  150.6 |    12.51 KB |       10.01 |
| ReadLine_ | Row    | 25000 |  18.883 ms |  5.84 | 20 | 1073.6 |  755.3 | 73489.62 KB |   58,791.69 |
| CsvHelper | Row    | 25000 |  40.518 ms | 12.54 | 20 |  500.3 | 1620.7 |    19.95 KB |       15.96 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.352 ms |  1.00 | 20 | 4657.7 |  174.1 |     1.25 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.654 ms |  1.53 | 20 | 3046.5 |  266.2 |    12.51 KB |       10.01 |
| ReadLine_ | Cols   | 25000 |  20.067 ms |  4.61 | 20 | 1010.2 |  802.7 | 73489.62 KB |   58,791.69 |
| CsvHelper | Cols   | 25000 |  43.506 ms | 10.00 | 20 |  466.0 | 1740.2 | 21340.16 KB |   17,072.13 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  34.426 ms |  1.00 | 20 |  588.9 | 1377.0 |      7.9 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.946 ms |  0.43 | 20 | 1356.4 |  597.8 |    70.25 KB |        8.89 |
| Sylvan___ | Floats | 25000 |  79.040 ms |  2.30 | 20 |  256.5 | 3161.6 |    21.64 KB |        2.74 |
| ReadLine_ | Floats | 25000 | 108.804 ms |  3.16 | 20 |  186.3 | 4352.2 | 73492.94 KB |    9,304.74 |
| CsvHelper | Floats | 25000 | 154.614 ms |  4.49 | 20 |  131.1 | 6184.6 | 22061.51 KB |    2,793.15 |
