```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
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
| Sep______ | Row    | 25000 |   2.951 ms |  1.00 | 20 | 6868.6 |  118.1 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.484 ms |  1.18 | 20 | 5818.6 |  139.4 |     12.5 KB |        9.94 |
| ReadLine_ | Row    | 25000 |  18.943 ms |  6.42 | 20 | 1070.2 |  757.7 | 73489.62 KB |   58,426.53 |
| CsvHelper | Row    | 25000 |  40.406 ms | 13.69 | 20 |  501.7 | 1616.2 |    19.95 KB |       15.86 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.122 ms |  1.00 | 20 | 4917.5 |  164.9 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.357 ms |  1.54 | 20 | 3188.8 |  254.3 |     12.5 KB |        9.94 |
| ReadLine_ | Cols   | 25000 |  18.925 ms |  4.59 | 20 | 1071.2 |  757.0 | 73489.62 KB |   58,426.53 |
| CsvHelper | Cols   | 25000 |  43.326 ms | 10.51 | 20 |  467.9 | 1733.0 | 21340.16 KB |   16,966.09 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  34.049 ms |  1.00 | 20 |  595.4 | 1362.0 |     7.91 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  14.631 ms |  0.43 | 20 | 1385.5 |  585.3 |    75.55 KB |        9.56 |
| Sylvan___ | Floats | 25000 |  78.832 ms |  2.32 | 20 |  257.2 | 3153.3 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 106.297 ms |  3.12 | 20 |  190.7 | 4251.9 | 73495.95 KB |    9,295.93 |
| CsvHelper | Floats | 25000 | 154.429 ms |  4.54 | 20 |  131.3 | 6177.2 | 22061.51 KB |    2,790.39 |
