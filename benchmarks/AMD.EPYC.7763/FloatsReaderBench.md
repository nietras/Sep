```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763 2.71GHz, 1 CPU, 4 logical and 2 physical cores
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
| Sep______ | Row    | 25000 |   3.212 ms |  1.00 | 20 | 6311.1 |  128.5 |     1.26 KB |        1.00 |
| Sylvan___ | Row    | 25000 |   3.637 ms |  1.13 | 20 | 5574.5 |  145.5 |     12.5 KB |        9.94 |
| ReadLine_ | Row    | 25000 |  19.213 ms |  5.98 | 20 | 1055.1 |  768.5 | 73489.62 KB |   58,426.53 |
| CsvHelper | Row    | 25000 |  37.859 ms | 11.79 | 20 |  535.5 | 1514.4 |    19.95 KB |       15.86 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Cols   | 25000 |   4.274 ms |  1.00 | 20 | 4743.2 |  171.0 |     1.26 KB |        1.00 |
| Sylvan___ | Cols   | 25000 |   6.248 ms |  1.46 | 20 | 3244.3 |  249.9 |     12.5 KB |        9.94 |
| ReadLine_ | Cols   | 25000 |  19.165 ms |  4.48 | 20 | 1057.8 |  766.6 | 73489.62 KB |   58,426.53 |
| CsvHelper | Cols   | 25000 |  40.495 ms |  9.48 | 20 |  500.6 | 1619.8 | 21340.16 KB |   16,966.09 |
|           |        |       |            |       |    |        |        |             |             |
| Sep______ | Floats | 25000 |  33.825 ms |  1.00 | 20 |  599.3 | 1353.0 |     7.91 KB |        1.00 |
| Sep_MT___ | Floats | 25000 |  16.383 ms |  0.48 | 20 | 1237.4 |  655.3 |    70.26 KB |        8.89 |
| Sylvan___ | Floats | 25000 |  81.779 ms |  2.42 | 20 |  247.9 | 3271.2 |    18.63 KB |        2.36 |
| ReadLine_ | Floats | 25000 | 113.972 ms |  3.37 | 20 |  177.9 | 4558.9 | 73492.94 KB |    9,295.55 |
| CsvHelper | Floats | 25000 | 155.461 ms |  4.60 | 20 |  130.4 | 6218.5 | 22061.51 KB |    2,790.39 |
